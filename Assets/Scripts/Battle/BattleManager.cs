using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PixelWarriors
{
    public class BattleManager : MonoBehaviour
    {
        private BattleState _currentState;
        private List<BattleCharacter> _players;
        private List<BattleCharacter> _enemies;
        private BattleScreenUI _battleScreen;
        private Queue<BattleCharacter> _turnQueue;
        private BattleCharacter _activeCharacter;
        private int _roundNumber;

        private PlayerInputHandler _playerInput;
        private BattleVisualController _visuals;

        public BattleResult Result { get; private set; } = BattleResult.None;
        public bool IsFinished => Result != BattleResult.None;
        public List<BattleCharacter> Players => _players;
        public List<BattleCharacter> Enemies => _enemies;

        public void StartBattle(List<BattleCharacter> players, List<BattleCharacter> enemies,
            BattleScreenUI battleScreen)
        {
            _players = players;
            _enemies = enemies;
            _battleScreen = battleScreen;

            _visuals = new BattleVisualController(battleScreen);
            _playerInput = new PlayerInputHandler(battleScreen, _visuals);
            _playerInput.SubscribeEvents();
            SubscribePassiveEvents();

            StartCoroutine(BattleLoop());
        }

        private void OnDestroy()
        {
            _playerInput?.UnsubscribeEvents();
            UnsubscribePassiveEvents();
        }

        private void SubscribePassiveEvents()
        {
            GameEvents.OnDamageDealt += HandleDamageForPassives;
            GameEvents.OnCharacterDefeated += HandleDefeatedForPassives;
            GameEvents.OnPositionSwapped += HandlePositionSwapForCaltrops;
        }

        private void UnsubscribePassiveEvents()
        {
            GameEvents.OnDamageDealt -= HandleDamageForPassives;
            GameEvents.OnCharacterDefeated -= HandleDefeatedForPassives;
            GameEvents.OnPositionSwapped -= HandlePositionSwapForCaltrops;
        }

        private void HandleDamageForPassives(BattleCharacter target, int damage, DamageType type)
        {
            PassiveProcessor.OnDamageTaken(target, damage, GetAllCharacters());
        }

        private bool _processingDefeat;

        private void HandleDefeatedForPassives(BattleCharacter defeated)
        {
            if (_processingDefeat) return;
            _processingDefeat = true;
            PassiveProcessor.OnCharacterDefeated(defeated, GetAllCharacters());
            _processingDefeat = false;
        }

        private void HandlePositionSwapForCaltrops(BattleCharacter a, BattleCharacter b)
        {
            ProcessCaltrops(a);
            if (a != b) ProcessCaltrops(b);
        }

        private void ProcessCaltrops(BattleCharacter movedCharacter)
        {
            if (!movedCharacter.IsAlive) return;

            List<BattleCharacter> opponents = movedCharacter.Side == TeamSide.Player ? _enemies : _players;
            foreach (BattleCharacter opponent in opponents)
            {
                if (!opponent.IsAlive) continue;
                if (!opponent.HasEffect(StatusEffect.Caltrops)) continue;

                int damage = GameplayConfig.CaltropsDamage;
                movedCharacter.CurrentHP = Mathf.Max(0, movedCharacter.CurrentHP - damage);
                GameEvents.RaiseDamageDealt(movedCharacter, damage, DamageType.Physical);
                Log($"{movedCharacter.Data.Name} steps on caltrops! {damage} damage!");
                ActionExecutor.CheckDefeated(movedCharacter);
                break;
            }
        }

        private IEnumerator BattleLoop()
        {
            SetState(BattleState.Setup);
            GameEvents.RaiseBattleStarted();
            WarriorAbilityHandler.ResetBladedanceCounts();
            Log("Battle begins!");

            _battleScreen.BattleGrid.SetPlayers(_players);
            _battleScreen.BattleGrid.SetEnemies(_enemies);
            _battleScreen.BattleGrid.RefreshAll();

            // Fire passive battle-start hooks
            foreach (BattleCharacter character in GetAllCharacters())
                PassiveProcessor.OnBattleStart(character);
            _battleScreen.BattleGrid.RefreshAll();

            yield return new WaitForSeconds(GameplayConfig.BattleStartDelay);

            while (true)
            {
                _roundNumber++;
                List<BattleCharacter> allCharacters = GetAllCharacters();
                _turnQueue = TurnOrderCalculator.CalculateTurnOrder(allCharacters);

                while (_turnQueue.Count > 0)
                {
                    _activeCharacter = _turnQueue.Dequeue();

                    if (!_activeCharacter.IsAlive) continue;

                    // --- TURN START ---
                    SetState(BattleState.TurnStart);
                    _activeCharacter.StartTurn();
                    StatusEffectProcessor.ProcessTurnStart(_activeCharacter);
                    PassiveProcessor.OnTurnStart(_activeCharacter);
                    GameEvents.RaiseTurnStarted(_activeCharacter);
                    GameEvents.RaiseTurnOrderUpdated(_roundNumber, _activeCharacter, _turnQueue.ToList());
                    _battleScreen.BattleGrid.ClearAllHighlights();
                    _battleScreen.BattleGrid.SetHighlight(_activeCharacter, true);
                    Log($"{_activeCharacter.Data.Name}'s turn!");

                    yield return new WaitForSeconds(GameplayConfig.TurnStartDelay);

                    // --- ACTION LOOP ---
                    while (_activeCharacter.IsAlive && _activeCharacter.HasActionsRemaining())
                    {
                        if (_activeCharacter.Side == TeamSide.Player)
                        {
                            yield return PlayerTurn();
                        }
                        else
                        {
                            yield return EnemyTurn();
                        }

                        _battleScreen.BattleGrid.RefreshAll();
                        yield return new WaitForSeconds(GameplayConfig.PostActionDelay);

                        if (CheckVictory())
                        {
                            Result = BattleResult.Victory;
                            SetState(BattleState.Victory);
                            _battleScreen.BattleGrid.ClearAllHighlights();
                            Log("Victory!");
                            GameEvents.RaiseBattleEnded();
                            yield break;
                        }

                        if (CheckDefeat())
                        {
                            Result = BattleResult.Defeat;
                            SetState(BattleState.Defeat);
                            _battleScreen.BattleGrid.ClearAllHighlights();
                            Log("Defeat...");
                            GameEvents.RaiseBattleEnded();
                            yield break;
                        }
                    }

                    // --- TURN END ---
                    SetState(BattleState.TurnEnd);
                    StatusEffectProcessor.ProcessTurnEnd(_activeCharacter);

                    // Only reset priority if no Anticipate/React was applied this turn
                    if (!_activeCharacter.HasEffect(StatusEffect.Anticipate) &&
                        !_activeCharacter.HasEffect(StatusEffect.React))
                    {
                        _activeCharacter.Priority = Priority.Normal;
                    }

                    // Check for deaths from DoT effects
                    _battleScreen.BattleGrid.RefreshAll();

                    if (CheckVictory())
                    {
                        SetState(BattleState.Victory);
                        _battleScreen.BattleGrid.ClearAllHighlights();
                        Log("Victory!");
                        GameEvents.RaiseBattleEnded();
                        yield break;
                    }

                    if (CheckDefeat())
                    {
                        SetState(BattleState.Defeat);
                        _battleScreen.BattleGrid.ClearAllHighlights();
                        Log("Defeat...");
                        GameEvents.RaiseBattleEnded();
                        yield break;
                    }

                    GameEvents.RaiseTurnEnded(_activeCharacter);
                }
            }
        }

        private IEnumerator PlayerTurn()
        {
            SetState(BattleState.AwaitingInput);
            yield return _playerInput.WaitForAction(_activeCharacter, _players, _enemies);

            SetState(BattleState.ExecutingAction);
            ActionExecutor.ExecuteAbility(_activeCharacter, _playerInput.ChosenAbility, _playerInput.ChosenTargets);
        }

        private IEnumerator EnemyTurn()
        {
            SetState(BattleState.ExecutingAction);
            yield return new WaitForSeconds(GameplayConfig.EnemyThinkDelay);

            EnemyAI.DecideAction(_activeCharacter, _players, _enemies,
                out AbilityData ability, out List<BattleCharacter> targets);

            if (ability != null)
            {
                ActionExecutor.ExecuteAbility(_activeCharacter, ability, targets);
            }
            else
            {
                Log($"{_activeCharacter.Data.Name} passes!");
                _activeCharacter.LongActionsRemaining = 0;
                _activeCharacter.ShortActionsRemaining = 0;
            }
        }

        private void SetState(BattleState newState)
        {
            _currentState = newState;
            GameEvents.RaiseBattleStateChanged(newState);
        }

        private List<BattleCharacter> GetAllCharacters()
        {
            return _players.Concat(_enemies).ToList();
        }

        private bool CheckVictory() => _enemies.TrueForAll(e => !e.IsAlive);
        private bool CheckDefeat() => _players.TrueForAll(p => !p.IsAlive);

        private void Log(string message) => GameEvents.RaiseCombatLogMessage(message);
    }
}

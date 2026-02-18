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

        public void StartBattle(List<BattleCharacter> players, List<BattleCharacter> enemies,
            BattleScreenUI battleScreen)
        {
            _players = players;
            _enemies = enemies;
            _battleScreen = battleScreen;

            _visuals = new BattleVisualController(battleScreen);
            _playerInput = new PlayerInputHandler(battleScreen, _visuals);
            _playerInput.SubscribeEvents();

            StartCoroutine(BattleLoop());
        }

        private void OnDestroy()
        {
            _playerInput?.UnsubscribeEvents();
        }

        private IEnumerator BattleLoop()
        {
            SetState(BattleState.Setup);
            GameEvents.RaiseBattleStarted();
            Log("Battle begins!");

            _battleScreen.BattleGrid.SetPlayers(_players);
            _battleScreen.BattleGrid.SetEnemies(_enemies);
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
                    }

                    // --- TURN END ---
                    SetState(BattleState.TurnEnd);
                    StatusEffectProcessor.ProcessTurnEnd(_activeCharacter);

                    // Only reset priority if no Anticipate/Prepare was applied this turn
                    if (!_activeCharacter.HasEffect(StatusEffect.Anticipate) &&
                        !_activeCharacter.HasEffect(StatusEffect.Prepare))
                    {
                        _activeCharacter.Priority = Priority.Normal;
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

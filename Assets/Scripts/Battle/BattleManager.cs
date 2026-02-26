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
        private BattleAnimationController _animController;
        private RunData _runData;

        // Reinforcement system
        private List<ReinforcementWave> _pendingWaves = new();

        public BattleResult Result { get; private set; } = BattleResult.None;
        public bool IsFinished => Result != BattleResult.None;
        public List<BattleCharacter> Players => _players;
        public List<BattleCharacter> Enemies => _enemies;

        public void StartBattle(List<BattleCharacter> players, List<BattleCharacter> initialEnemies,
            EncounterData encounterData, BattleScreenUI battleScreen, RunData runData = null)
        {
            _players = players;
            _enemies = initialEnemies;
            _battleScreen = battleScreen;
            _runData = runData;

            // Copy waves so we can mutate the list as they trigger
            _pendingWaves = new List<ReinforcementWave>(encounterData.Waves);

            _visuals = new BattleVisualController(battleScreen);
            _playerInput = new PlayerInputHandler(battleScreen, _visuals);
            _playerInput.SubscribeEvents();
            SubscribePassiveEvents();
            ActionExecutor.SetBattleContext(_players, _enemies);

            _animController = gameObject.AddComponent<BattleAnimationController>();
            _animController.Initialize(_battleScreen);

            // Pass RunData to ability panel for consumable items
            if (_runData != null)
                _battleScreen.AbilityPanel.SetRunData(_runData);

            StartCoroutine(BattleLoop());
        }

        private void OnDestroy()
        {
            _playerInput?.UnsubscribeEvents();
            UnsubscribePassiveEvents();

            if (_animController != null)
                Destroy(_animController);
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
            ProcessTrap(a);
            if (a != b) ProcessTrap(b);
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

        private void ProcessTrap(BattleCharacter movedCharacter)
        {
            if (!movedCharacter.IsAlive) return;

            List<BattleCharacter> opponents = movedCharacter.Side == TeamSide.Player ? _enemies : _players;
            foreach (BattleCharacter opponent in opponents)
            {
                if (!opponent.IsAlive) continue;
                if (!opponent.HasEffect(StatusEffect.Trap)) continue;

                // Trap triggers: damage + stun, then remove
                int damage = GameplayConfig.TrapDamage;
                movedCharacter.CurrentHP = Mathf.Max(0, movedCharacter.CurrentHP - damage);
                GameEvents.RaiseDamageDealt(movedCharacter, damage, DamageType.Physical);
                Log($"{movedCharacter.Data.Name} triggers a trap! {damage} damage!");

                if (movedCharacter.IsAlive)
                {
                    var stun = new StatusEffectInstance(StatusEffect.Stun, 1, 0, opponent);
                    movedCharacter.AddEffect(stun);
                    GameEvents.RaiseStatusEffectApplied(movedCharacter, StatusEffect.Stun, 0);
                    Log($"{movedCharacter.Data.Name} is stunned!");
                }

                opponent.RemoveEffect(StatusEffect.Trap);
                GameEvents.RaiseStatusEffectRemoved(opponent, StatusEffect.Trap);
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

            // Notify UI of initial wave count
            if (_pendingWaves.Count > 0)
            {
                GameEvents.RaiseReinforcementWaveCountChanged(_pendingWaves.Count);
                Log($"Warning: {_pendingWaves.Count} reinforcement wave(s) detected!");
            }

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

                        // Check reinforcements after each action
                        yield return CheckAndSpawnReinforcements();

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

                    // Check reinforcements after turn end (DoT kills may trigger)
                    yield return CheckAndSpawnReinforcements();

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
            AbilityData chosenAbility = _playerInput.ChosenAbility;
            ActionExecutor.ExecuteAbility(_activeCharacter, chosenAbility, _playerInput.ChosenTargets);

            // Decrement consumable if one was used
            if (_runData != null)
            {
                string consumableId = _battleScreen.AbilityPanel.GetConsumableId(chosenAbility);
                if (consumableId != null)
                {
                    _runData.RemoveConsumable(consumableId);
                    _battleScreen.AbilityPanel.RefreshAbilities();
                }
            }
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

        // --- Reinforcement System ---

        private IEnumerator CheckAndSpawnReinforcements()
        {
            // Check waves in reverse so we can remove triggered ones
            for (int i = _pendingWaves.Count - 1; i >= 0; i--)
            {
                if (IsWaveTriggered(_pendingWaves[i]))
                {
                    ReinforcementWave wave = _pendingWaves[i];
                    _pendingWaves.RemoveAt(i);
                    yield return SpawnReinforcementWave(wave);
                }
            }
        }

        private bool IsWaveTriggered(ReinforcementWave wave)
        {
            switch (wave.Trigger)
            {
                case ReinforcementTrigger.OnEnemyCount:
                    int aliveCount = _enemies.Count(e => e.IsAlive);
                    return aliveCount <= wave.TriggerValue;

                case ReinforcementTrigger.OnRoundNumber:
                    return _roundNumber >= wave.TriggerValue;

                case ReinforcementTrigger.OnBossHPPercent:
                    bool anyBossExists = false;
                    foreach (BattleCharacter enemy in _enemies)
                    {
                        if (!enemy.Data.IsBoss) continue;
                        anyBossExists = true;

                        // Dead boss = 0% HP, triggers any threshold
                        if (!enemy.IsAlive)
                            return true;

                        float hpPercent = (float)enemy.CurrentHP / enemy.MaxHP * 100f;
                        if (hpPercent <= wave.TriggerValue)
                            return true;
                    }
                    // No boss found at all — discard wave (shouldn't happen)
                    return !anyBossExists;

                default:
                    return false;
            }
        }

        private IEnumerator SpawnReinforcementWave(ReinforcementWave wave)
        {
            var emptySlots = GridSlotUtil.GetEmptySlots(_enemies);
            if (emptySlots.Count == 0) yield break;

            // Place reinforcements into available slots
            int spawnCount = Mathf.Min(wave.Enemies.Count, emptySlots.Count);
            List<BattleCharacter> spawned = new();

            for (int i = 0; i < spawnCount; i++)
            {
                BattleCharacter bc = new BattleCharacter(
                    wave.Enemies[i], TeamSide.Enemy, emptySlots[i].Row, emptySlots[i].Col);
                _enemies.Add(bc);
                _battleScreen.BattleGrid.AddEnemy(bc);
                spawned.Add(bc);
            }

            // Fire passive battle-start hooks for new enemies
            foreach (BattleCharacter bc in spawned)
                PassiveProcessor.OnBattleStart(bc);

            // Announce
            Log(wave.AnnouncementText);
            GameEvents.RaiseReinforcementsSpawned(wave.AnnouncementText, spawnCount);
            GameEvents.RaiseReinforcementWaveCountChanged(_pendingWaves.Count);

            _battleScreen.BattleGrid.RefreshAll();

            yield return new WaitForSeconds(GameplayConfig.ReinforcementSpawnDelay);
        }

        // --- State Management ---

        private void SetState(BattleState newState)
        {
            _currentState = newState;
            GameEvents.RaiseBattleStateChanged(newState);
        }

        private List<BattleCharacter> GetAllCharacters()
        {
            return _players.Concat(_enemies).ToList();
        }

        private bool CheckVictory() => _enemies.TrueForAll(e => !e.IsAlive) && _pendingWaves.Count == 0;
        private bool CheckDefeat() => _players.TrueForAll(p => !p.IsAlive);

        private void Log(string message) => GameEvents.RaiseCombatLogMessage(message);
    }
}

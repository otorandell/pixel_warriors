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
        private AbilityData _selectedAbility;
        private BattleCharacter _selectedTarget;
        private bool _waitingForAbilitySelection;
        private bool _waitingForTargetSelection;
        private int _roundNumber;

        public void StartBattle(List<BattleCharacter> players, List<BattleCharacter> enemies,
            BattleScreenUI battleScreen)
        {
            _players = players;
            _enemies = enemies;
            _battleScreen = battleScreen;

            GameEvents.OnAbilitySelected += HandleAbilitySelected;
            GameEvents.OnTargetSelected += HandleTargetSelected;

            StartCoroutine(BattleLoop());
        }

        private void OnDestroy()
        {
            GameEvents.OnAbilitySelected -= HandleAbilitySelected;
            GameEvents.OnTargetSelected -= HandleTargetSelected;
        }

        private IEnumerator BattleLoop()
        {
            SetState(BattleState.Setup);
            GameEvents.RaiseBattleStarted();
            Log("Battle begins!");

            _battleScreen.BattleGrid.SetPlayers(_players);
            _battleScreen.BattleGrid.SetEnemies(_enemies);

            yield return new WaitForSeconds(0.5f);

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
                    GameEvents.RaiseTurnStarted(_activeCharacter);
                    _battleScreen.BattleGrid.ClearAllHighlights();
                    _battleScreen.BattleGrid.SetHighlight(_activeCharacter, true);
                    Log($"{_activeCharacter.Data.Name}'s turn!");

                    yield return new WaitForSeconds(0.3f);

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

                        // Refresh UI
                        _battleScreen.BattleGrid.RefreshAll();
                        yield return new WaitForSeconds(0.3f);

                        // Check end conditions
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
                    _activeCharacter.Priority = Priority.Normal;
                    GameEvents.RaiseTurnEnded(_activeCharacter);
                }
            }
        }

        private IEnumerator PlayerTurn()
        {
            SetState(BattleState.AwaitingInput);
            _battleScreen.AbilityPanel.SetCharacter(_activeCharacter);

            // Wait for ability selection
            _selectedAbility = null;
            _waitingForAbilitySelection = true;

            while (_waitingForAbilitySelection)
            {
                yield return null;
            }

            // Determine targets
            List<BattleCharacter> targets;

            if (TargetSelector.RequiresManualTargetSelection(_selectedAbility.TargetType))
            {
                List<BattleCharacter> validTargets = TargetSelector.GetValidTargets(
                    _activeCharacter, _selectedAbility, _players, _enemies);

                EnableTargetSelection(validTargets);

                _selectedTarget = null;
                _waitingForTargetSelection = true;

                while (_waitingForTargetSelection)
                {
                    yield return null;
                }

                DisableTargetSelection();
                targets = new List<BattleCharacter> { _selectedTarget };
            }
            else
            {
                targets = TargetSelector.GetValidTargets(
                    _activeCharacter, _selectedAbility, _players, _enemies);
            }

            // Execute
            SetState(BattleState.ExecutingAction);
            ActionExecutor.ExecuteAbility(_activeCharacter, _selectedAbility, targets);

            // Refresh ability panel for remaining actions
            _battleScreen.AbilityPanel.SetCharacter(_activeCharacter);
        }

        private IEnumerator EnemyTurn()
        {
            SetState(BattleState.ExecutingAction);
            yield return new WaitForSeconds(0.5f);

            EnemyAI.DecideAction(_activeCharacter, _players, _enemies,
                out AbilityData ability, out List<BattleCharacter> targets);

            if (ability != null)
            {
                ActionExecutor.ExecuteAbility(_activeCharacter, ability, targets);
            }
            else
            {
                Log($"{_activeCharacter.Data.Name} passes!");
                // Force end of turn when no abilities available
                _activeCharacter.LongActionsRemaining = 0;
                _activeCharacter.ShortActionsRemaining = 0;
            }
        }

        private void EnableTargetSelection(List<BattleCharacter> validTargets)
        {
            foreach (BattleCharacter target in validTargets)
            {
                CharacterCardUI card = _battleScreen.BattleGrid.FindCard(target);
                if (card != null)
                {
                    card.SetTargetable(true);
                    card.OnCardClicked += HandleCardClicked;
                }
            }
        }

        private void DisableTargetSelection()
        {
            _battleScreen.BattleGrid.ClearAllHighlights();
            _battleScreen.BattleGrid.SetHighlight(_activeCharacter, true);

            // Unsubscribe from all cards
            foreach (CharacterCardUI card in _battleScreen.BattleGrid.EnemyCards)
                card.OnCardClicked -= HandleCardClicked;
            foreach (CharacterCardUI card in _battleScreen.BattleGrid.PlayerCards)
                card.OnCardClicked -= HandleCardClicked;
        }

        private void HandleCardClicked(BattleCharacter target)
        {
            GameEvents.RaiseTargetSelected(target);
        }

        private void HandleAbilitySelected(AbilityData ability)
        {
            if (!_waitingForAbilitySelection) return;
            if (_activeCharacter == null) return;
            if (!_activeCharacter.CanUseAbility(ability)) return;

            _selectedAbility = ability;
            _waitingForAbilitySelection = false;
        }

        private void HandleTargetSelected(BattleCharacter target)
        {
            if (!_waitingForTargetSelection) return;

            _selectedTarget = target;
            _waitingForTargetSelection = false;
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

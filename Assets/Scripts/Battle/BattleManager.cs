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

        // Staging fields
        private PlayerInputPhase _inputPhase;
        private AbilityData _stagedAbility;
        private BattleCharacter _stagedTarget;
        private List<BattleCharacter> _resolvedTargets;
        private bool _confirmed;
        private bool _cancelled;
        private bool _abilityJustSelected;

        public void StartBattle(List<BattleCharacter> players, List<BattleCharacter> enemies,
            BattleScreenUI battleScreen)
        {
            _players = players;
            _enemies = enemies;
            _battleScreen = battleScreen;

            GameEvents.OnAbilitySelected += HandleAbilitySelected;
            GameEvents.OnTargetSelected += HandleTargetSelected;
            GameEvents.OnActionConfirmed += HandleActionConfirmed;
            GameEvents.OnActionCancelled += HandleActionCancelled;

            StartCoroutine(BattleLoop());
        }

        private void OnDestroy()
        {
            GameEvents.OnAbilitySelected -= HandleAbilitySelected;
            GameEvents.OnTargetSelected -= HandleTargetSelected;
            GameEvents.OnActionConfirmed -= HandleActionConfirmed;
            GameEvents.OnActionCancelled -= HandleActionCancelled;
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
                    GameEvents.RaiseTurnOrderUpdated(_roundNumber, _activeCharacter, _turnQueue.ToList());
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

            _stagedAbility = null;
            _stagedTarget = null;
            _resolvedTargets = null;
            _confirmed = false;
            _cancelled = false;
            _abilityJustSelected = false;

            TransitionToPhase(PlayerInputPhase.SelectingAbility);

            while (true)
            {
                switch (_inputPhase)
                {
                    case PlayerInputPhase.SelectingAbility:
                        yield return WaitForAbilitySelection();
                        break;

                    case PlayerInputPhase.SelectingTarget:
                        yield return WaitForTargetSelection();
                        break;

                    case PlayerInputPhase.AwaitingConfirmation:
                        yield return WaitForConfirmation();
                        if (_confirmed)
                        {
                            ClearAllStagingVisuals();
                            SetState(BattleState.ExecutingAction);
                            ActionExecutor.ExecuteAbility(_activeCharacter, _stagedAbility, _resolvedTargets);
                            _battleScreen.AbilityPanel.SetCharacter(_activeCharacter);
                            yield break;
                        }
                        break;
                }
            }
        }

        private IEnumerator WaitForAbilitySelection()
        {
            _stagedAbility = null;
            _stagedTarget = null;
            _resolvedTargets = null;
            _confirmed = false;
            _cancelled = false;
            _abilityJustSelected = false;

            _battleScreen.AbilityPanel.ClearStagedHighlight();
            _battleScreen.BattleGrid.ClearStagedHighlights();

            while (!_abilityJustSelected)
            {
                yield return null;
            }

            // HandleAbilitySelected already set _stagedAbility, _inputPhase, and highlight
        }

        private IEnumerator WaitForTargetSelection()
        {
            _stagedTarget = null;
            _confirmed = false;
            _cancelled = false;
            _abilityJustSelected = false;

            List<BattleCharacter> validTargets = TargetSelector.GetValidTargets(
                _activeCharacter, _stagedAbility, _players, _enemies);
            EnableTargetSelection(validTargets);

            while (_stagedTarget == null && !_cancelled && !_abilityJustSelected)
            {
                yield return null;
            }

            DisableTargetSelection();

            if (_cancelled)
            {
                _cancelled = false;
                _battleScreen.AbilityPanel.ClearStagedHighlight();
                TransitionToPhase(PlayerInputPhase.SelectingAbility);
            }
            else if (_abilityJustSelected)
            {
                // HandleAbilitySelected already set new _stagedAbility and _inputPhase
            }
            // else: target was selected, HandleTargetSelected set _stagedTarget and transitioned
        }

        private IEnumerator WaitForConfirmation()
        {
            _confirmed = false;
            _cancelled = false;
            _abilityJustSelected = false;

            ShowStagedTargetHighlights();

            while (!_confirmed && !_cancelled && !_abilityJustSelected)
            {
                yield return null;
            }

            ClearStagedTargetHighlights();

            if (_cancelled)
            {
                _cancelled = false;
                if (TargetSelector.RequiresManualTargetSelection(_stagedAbility.TargetType))
                {
                    TransitionToPhase(PlayerInputPhase.SelectingTarget);
                }
                else
                {
                    _battleScreen.AbilityPanel.ClearStagedHighlight();
                    TransitionToPhase(PlayerInputPhase.SelectingAbility);
                }
            }
            else if (_abilityJustSelected)
            {
                // HandleAbilitySelected already set new _stagedAbility and _inputPhase
            }
            // else: _confirmed stays true, caller will execute
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
                _activeCharacter.LongActionsRemaining = 0;
                _activeCharacter.ShortActionsRemaining = 0;
            }
        }

        // --- Target Selection UI ---

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

            foreach (CharacterCardUI card in _battleScreen.BattleGrid.EnemyCards)
                card.OnCardClicked -= HandleCardClicked;
            foreach (CharacterCardUI card in _battleScreen.BattleGrid.PlayerCards)
                card.OnCardClicked -= HandleCardClicked;
        }

        // --- Staging Visuals ---

        private void ShowStagedTargetHighlights()
        {
            if (_resolvedTargets == null) return;

            _battleScreen.BattleGrid.SetStagedHighlightAll(_resolvedTargets, true);
        }

        private void ClearStagedTargetHighlights()
        {
            _battleScreen.BattleGrid.ClearStagedHighlights();
        }

        private void ClearAllStagingVisuals()
        {
            _battleScreen.BattleGrid.ClearAllHighlights();
            _battleScreen.BattleGrid.SetHighlight(_activeCharacter, true);
            _battleScreen.AbilityPanel.ClearStagedHighlight();
        }

        // --- Event Handlers ---

        private void HandleCardClicked(BattleCharacter target)
        {
            GameEvents.RaiseTargetSelected(target);
        }

        private void HandleAbilitySelected(AbilityData ability)
        {
            if (_currentState != BattleState.AwaitingInput) return;
            if (_activeCharacter == null) return;
            if (!_activeCharacter.CanUseAbility(ability)) return;

            _stagedAbility = ability;
            _stagedTarget = null;
            _resolvedTargets = null;
            _abilityJustSelected = true;

            _battleScreen.AbilityPanel.SetStagedHighlight(ability);

            if (TargetSelector.RequiresManualTargetSelection(ability.TargetType))
            {
                TransitionToPhase(PlayerInputPhase.SelectingTarget);
            }
            else
            {
                _resolvedTargets = TargetSelector.GetValidTargets(
                    _activeCharacter, ability, _players, _enemies);
                UpdateStagedActionDescription();
                TransitionToPhase(PlayerInputPhase.AwaitingConfirmation);
            }
        }

        private void HandleTargetSelected(BattleCharacter target)
        {
            if (_inputPhase != PlayerInputPhase.SelectingTarget) return;

            _stagedTarget = target;
            _resolvedTargets = new List<BattleCharacter> { target };
            UpdateStagedActionDescription();
            TransitionToPhase(PlayerInputPhase.AwaitingConfirmation);
        }

        private void HandleActionConfirmed()
        {
            if (_inputPhase != PlayerInputPhase.AwaitingConfirmation) return;
            _confirmed = true;
        }

        private void HandleActionCancelled()
        {
            if (_currentState != BattleState.AwaitingInput) return;
            _cancelled = true;
        }

        // --- Helpers ---

        private void TransitionToPhase(PlayerInputPhase phase)
        {
            _inputPhase = phase;
            GameEvents.RaisePlayerInputPhaseChanged(phase);
        }

        private void UpdateStagedActionDescription()
        {
            if (_stagedAbility == null || _resolvedTargets == null || _resolvedTargets.Count == 0)
            {
                GameEvents.RaiseStagedActionChanged("Confirm?");
                return;
            }

            string targetNames = _resolvedTargets.Count == 1
                ? _resolvedTargets[0].Data.Name
                : string.Join(", ", _resolvedTargets.Select(t => t.Data.Name));

            string description = $"{_stagedAbility.Name} > {targetNames}";
            GameEvents.RaiseStagedActionChanged(description);
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

using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PixelWarriors
{
    public class PlayerInputHandler
    {
        public AbilityData ChosenAbility { get; private set; }
        public List<BattleCharacter> ChosenTargets { get; private set; }

        private readonly BattleScreenUI _battleScreen;
        private readonly BattleVisualController _visuals;

        private BattleCharacter _activeCharacter;
        private List<BattleCharacter> _players;
        private List<BattleCharacter> _enemies;

        // Staging state
        private PlayerInputPhase _inputPhase;
        private AbilityData _stagedAbility;
        private BattleCharacter _stagedTarget;
        private List<BattleCharacter> _resolvedTargets;
        private bool _confirmed;
        private bool _cancelled;
        private bool _abilityJustSelected;

        public PlayerInputHandler(BattleScreenUI battleScreen, BattleVisualController visuals)
        {
            _battleScreen = battleScreen;
            _visuals = visuals;
        }

        public void SubscribeEvents()
        {
            GameEvents.OnAbilitySelected += HandleAbilitySelected;
            GameEvents.OnTargetSelected += HandleTargetSelected;
            GameEvents.OnActionConfirmed += HandleActionConfirmed;
            GameEvents.OnActionCancelled += HandleActionCancelled;
        }

        public void UnsubscribeEvents()
        {
            GameEvents.OnAbilitySelected -= HandleAbilitySelected;
            GameEvents.OnTargetSelected -= HandleTargetSelected;
            GameEvents.OnActionConfirmed -= HandleActionConfirmed;
            GameEvents.OnActionCancelled -= HandleActionCancelled;
        }

        public IEnumerator WaitForAction(BattleCharacter activeCharacter,
            List<BattleCharacter> players, List<BattleCharacter> enemies)
        {
            _activeCharacter = activeCharacter;
            _players = players;
            _enemies = enemies;

            _battleScreen.AbilityPanel.SetCharacter(_activeCharacter);

            ChosenAbility = null;
            ChosenTargets = null;
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
                            _visuals.ClearAllStagingVisuals(_activeCharacter);
                            ChosenAbility = _stagedAbility;
                            ChosenTargets = _resolvedTargets;
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
            _visuals.ClearStagedHighlights();

            while (!_abilityJustSelected)
            {
                yield return null;
            }
        }

        private IEnumerator WaitForTargetSelection()
        {
            _stagedTarget = null;
            _confirmed = false;
            _cancelled = false;
            _abilityJustSelected = false;

            List<BattleCharacter> validTargets = TargetSelector.GetValidTargets(
                _activeCharacter, _stagedAbility, _players, _enemies);
            _visuals.EnableTargetSelection(validTargets);

            while (_stagedTarget == null && !_cancelled && !_abilityJustSelected)
            {
                yield return null;
            }

            _visuals.DisableTargetSelection(_activeCharacter);

            if (_cancelled)
            {
                _cancelled = false;
                _battleScreen.AbilityPanel.ClearStagedHighlight();
                TransitionToPhase(PlayerInputPhase.SelectingAbility);
            }
        }

        private IEnumerator WaitForConfirmation()
        {
            _confirmed = false;
            _cancelled = false;
            _abilityJustSelected = false;

            _visuals.ShowStagedHighlights(_resolvedTargets);

            while (!_confirmed && !_cancelled && !_abilityJustSelected)
            {
                yield return null;
            }

            _visuals.ClearStagedHighlights();

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
        }

        // --- Event Handlers ---

        private void HandleAbilitySelected(AbilityData ability)
        {
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
    }
}

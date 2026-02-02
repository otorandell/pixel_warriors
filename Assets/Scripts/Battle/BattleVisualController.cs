using System.Collections.Generic;

namespace PixelWarriors
{
    public class BattleVisualController
    {
        private readonly BattleScreenUI _battleScreen;

        public BattleVisualController(BattleScreenUI battleScreen)
        {
            _battleScreen = battleScreen;
        }

        public void EnableTargetSelection(List<BattleCharacter> validTargets)
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

        public void DisableTargetSelection(BattleCharacter activeCharacter)
        {
            _battleScreen.BattleGrid.ClearAllHighlights();
            _battleScreen.BattleGrid.SetHighlight(activeCharacter, true);

            foreach (CharacterCardUI card in _battleScreen.BattleGrid.EnemyCards)
                card.OnCardClicked -= HandleCardClicked;
            foreach (CharacterCardUI card in _battleScreen.BattleGrid.PlayerCards)
                card.OnCardClicked -= HandleCardClicked;
        }

        public void ShowStagedHighlights(List<BattleCharacter> targets)
        {
            if (targets == null) return;
            _battleScreen.BattleGrid.SetStagedHighlightAll(targets, true);
        }

        public void ClearStagedHighlights()
        {
            _battleScreen.BattleGrid.ClearStagedHighlights();
        }

        public void ClearAllStagingVisuals(BattleCharacter activeCharacter)
        {
            _battleScreen.BattleGrid.ClearAllHighlights();
            _battleScreen.BattleGrid.SetHighlight(activeCharacter, true);
            _battleScreen.AbilityPanel.ClearStagedHighlight();
        }

        private void HandleCardClicked(BattleCharacter target)
        {
            GameEvents.RaiseTargetSelected(target);
        }
    }
}

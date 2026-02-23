using UnityEngine;

namespace PixelWarriors
{
    public class BattleScreenUI : IScreen
    {
        private GameObject _root;
        private BattleGridUI _battleGrid;
        private AbilityPanelUI _abilityPanel;
        private TurnInfoPanelUI _turnInfoPanel;
        private CombatLogUI _combatLog;
        private SelectionPanelUI _selectionPanel;
        private CharacterPopupUI _characterPopup;
        private AbilityPopupUI _abilityPopup;
        private TurnOrderPopupUI _turnOrderPopup;

        public BattleGridUI BattleGrid => _battleGrid;
        public AbilityPanelUI AbilityPanel => _abilityPanel;
        public CombatLogUI CombatLog => _combatLog;

        public void Build(Transform canvasParent)
        {
            _root = new GameObject("BattleScreen");
            RectTransform rootRect = _root.AddComponent<RectTransform>();
            rootRect.SetParent(canvasParent, false);
            PanelBuilder.SetFill(rootRect);

            BuildLayout(rootRect);
        }

        public void Show()
        {
            if (_root != null) _root.SetActive(true);
        }

        public void Hide()
        {
            if (_root != null) _root.SetActive(false);
        }

        public void Destroy()
        {
            _combatLog?.Unsubscribe();
            _selectionPanel?.Unsubscribe();
            _turnInfoPanel?.Unsubscribe();

            if (_characterPopup != null)
                GameEvents.OnCharacterDetailRequested -= _characterPopup.ShowCharacterPopup;
            if (_abilityPopup != null)
                GameEvents.OnAbilityDetailRequested -= _abilityPopup.ShowAbilityPopup;
            if (_turnOrderPopup != null)
            {
                GameEvents.OnTurnOrderDetailRequested -= _turnOrderPopup.ShowTurnOrderPopup;
                _turnOrderPopup.UnsubscribeEvents();
            }

            if (_root != null) Object.Destroy(_root);
        }

        private void BuildLayout(Transform rootTransform)
        {
            float topRatio = UIStyleConfig.TurnInfoHeightRatio;
            float bottomRatio = UIStyleConfig.BottomAreaHeightRatio;
            float selectionWidth = UIStyleConfig.BottomSelectionWidthRatio;

            // Turn info panel (top strip)
            _turnInfoPanel = new TurnInfoPanelUI();
            _turnInfoPanel.Build(rootTransform);
            PanelBuilder.SetAnchored(_turnInfoPanel.Root, 0, 1 - topRatio, 1, 1,
                4, 2, -4, -4);

            // Main area (middle: BattleGrid + AbilityPanel)
            RectTransform mainArea = PanelBuilder.CreateContainer("MainArea", rootTransform);
            PanelBuilder.SetAnchored(mainArea, 0, bottomRatio, 1, 1 - topRatio,
                4, 2, -4, -2);

            // Battle grid (left side of main area)
            _battleGrid = new BattleGridUI();
            _battleGrid.Build(mainArea);
            PanelBuilder.SetAnchored(_battleGrid.Root, 0, 0, UIStyleConfig.BattleGridWidthRatio, 1,
                0, 0, -2, 0);

            // Ability panel (right side of main area)
            _abilityPanel = new AbilityPanelUI();
            _abilityPanel.Build(mainArea);
            PanelBuilder.SetAnchored(_abilityPanel.Root, UIStyleConfig.BattleGridWidthRatio, 0, 1, 1,
                2, 0, 0, 0);

            // Combat log (bottom-left)
            _combatLog = new CombatLogUI();
            _combatLog.Build(rootTransform);
            PanelBuilder.SetAnchored(_combatLog.Root, 0, 0, 1 - selectionWidth, bottomRatio,
                4, 4, -2, -2);

            // Selection panel (bottom-right)
            _selectionPanel = new SelectionPanelUI();
            _selectionPanel.Build(rootTransform);
            PanelBuilder.SetAnchored(_selectionPanel.Root, 1 - selectionWidth, 0, 1, bottomRatio,
                2, 4, -4, -2);

            // Detail popups (overlay everything, hidden by default, created last for z-order)
            _characterPopup = new CharacterPopupUI();
            _characterPopup.Build(rootTransform);

            _abilityPopup = new AbilityPopupUI();
            _abilityPopup.Build(rootTransform);

            _turnOrderPopup = new TurnOrderPopupUI();
            _turnOrderPopup.Build(rootTransform);
            _turnOrderPopup.SubscribeEvents();

            GameEvents.OnCharacterDetailRequested += _characterPopup.ShowCharacterPopup;
            GameEvents.OnAbilityDetailRequested += _abilityPopup.ShowAbilityPopup;
            GameEvents.OnTurnOrderDetailRequested += _turnOrderPopup.ShowTurnOrderPopup;
        }
    }
}

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace PixelWarriors
{
    public class BattleScreenUI : MonoBehaviour
    {
        private Canvas _canvas;
        private BattleGridUI _battleGrid;
        private AbilityPanelUI _abilityPanel;
        private TurnInfoPanelUI _turnInfoPanel;
        private CombatLogUI _combatLog;
        private SelectionPanelUI _selectionPanel;
        private DetailPopupUI _detailPopup;

        public BattleGridUI BattleGrid => _battleGrid;
        public AbilityPanelUI AbilityPanel => _abilityPanel;
        public CombatLogUI CombatLog => _combatLog;

        private void Awake()
        {
            EnsureEventSystem();
            BuildCanvas();
            BuildLayout();
        }

        private void EnsureEventSystem()
        {
            if (FindAnyObjectByType<EventSystem>() != null) return;

            GameObject esGo = new GameObject("EventSystem");
            esGo.AddComponent<EventSystem>();
            esGo.AddComponent<InputSystemUIInputModule>();
        }

        private void BuildCanvas()
        {
            // Canvas
            GameObject canvasGo = new GameObject("BattleCanvas");
            canvasGo.transform.SetParent(transform, false);
            _canvas = canvasGo.AddComponent<Canvas>();
            _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            _canvas.sortingOrder = 0;

            CanvasScaler scaler = canvasGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(960, 540);
            scaler.matchWidthOrHeight = 0.5f;

            canvasGo.AddComponent<GraphicRaycaster>();

            // Background
            GameObject bgGo = new GameObject("Background");
            RectTransform bgRect = bgGo.AddComponent<RectTransform>();
            bgRect.SetParent(_canvas.transform, false);
            PanelBuilder.SetFill(bgRect);
            Image bgImage = bgGo.AddComponent<Image>();
            bgImage.color = UIStyleConfig.Background;
            bgImage.raycastTarget = false;
        }

        private void BuildLayout()
        {
            Transform canvasTransform = _canvas.transform;

            float topRatio = UIStyleConfig.TurnInfoHeightRatio;
            float bottomRatio = UIStyleConfig.BottomAreaHeightRatio;
            float selectionWidth = UIStyleConfig.BottomSelectionWidthRatio;

            // Turn info panel (top strip)
            _turnInfoPanel = new TurnInfoPanelUI();
            _turnInfoPanel.Build(canvasTransform);
            PanelBuilder.SetAnchored(_turnInfoPanel.Root, 0, 1 - topRatio, 1, 1,
                4, 2, -4, -4);

            // Main area (middle: BattleGrid + AbilityPanel)
            RectTransform mainArea = PanelBuilder.CreateContainer("MainArea", canvasTransform);
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
            _combatLog.Build(canvasTransform);
            PanelBuilder.SetAnchored(_combatLog.Root, 0, 0, 1 - selectionWidth, bottomRatio,
                4, 4, -2, -2);

            // Selection panel (bottom-right)
            _selectionPanel = new SelectionPanelUI();
            _selectionPanel.Build(canvasTransform);
            PanelBuilder.SetAnchored(_selectionPanel.Root, 1 - selectionWidth, 0, 1, bottomRatio,
                2, 4, -4, -2);

            // Detail popup (overlays everything, hidden by default, created last for z-order)
            _detailPopup = new DetailPopupUI();
            _detailPopup.Build(canvasTransform);

            GameEvents.OnCharacterDetailRequested += _detailPopup.ShowCharacterPopup;
            GameEvents.OnAbilityDetailRequested += _detailPopup.ShowAbilityPopup;
        }

        private void OnDestroy()
        {
            GameEvents.OnCharacterDetailRequested -= _detailPopup.ShowCharacterPopup;
            GameEvents.OnAbilityDetailRequested -= _detailPopup.ShowAbilityPopup;
        }
    }
}

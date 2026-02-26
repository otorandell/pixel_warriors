using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PixelWarriors
{
    public class MainMenuScreen : IScreen
    {
        private GameObject _root;
        private bool _startPressed;
        private bool _continuePressed;

        public bool StartPressed => _startPressed;
        public bool ContinuePressed => _continuePressed;

        public void Build(Transform canvasParent)
        {
            _startPressed = false;
            _continuePressed = false;

            bool hasSave = SaveManager.HasSave();

            _root = new GameObject("MainMenuScreen");
            RectTransform rootRect = _root.AddComponent<RectTransform>();
            rootRect.SetParent(canvasParent, false);
            PanelBuilder.SetFill(rootRect);

            // Title
            TextMeshProUGUI title = PanelBuilder.CreateText("Title", rootRect,
                "PIXEL WARRIORS", UIStyleConfig.FontSizeLarge * 1.5f,
                TextAlignmentOptions.Center, UIStyleConfig.AccentMagenta);
            RectTransform titleRect = title.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(titleRect, 0.1f, 0.55f, 0.9f, 0.75f);

            // Subtitle
            TextMeshProUGUI subtitle = PanelBuilder.CreateText("Subtitle", rootRect,
                "A Dungeon Crawler Roguelike", UIStyleConfig.FontSizeSmall,
                TextAlignmentOptions.Center, UIStyleConfig.TextDimmed);
            RectTransform subtitleRect = subtitle.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(subtitleRect, 0.2f, 0.48f, 0.8f, 0.55f);

            if (hasSave)
            {
                // Continue button (above New Run)
                Button continueBtn = PanelBuilder.CreateButton("ContinueButton", rootRect,
                    "CONTINUE", UIStyleConfig.AccentCyan, UIStyleConfig.FontSizeMedium);
                RectTransform continueRect = continueBtn.GetComponent<RectTransform>();
                PanelBuilder.SetAnchored(continueRect, 0.3f, 0.33f, 0.7f, 0.45f);
                continueBtn.onClick.AddListener(() => _continuePressed = true);

                // New Run button (shifted down)
                Button newRunBtn = PanelBuilder.CreateButton("NewRunButton", rootRect,
                    "NEW RUN", UIStyleConfig.AccentGreen, UIStyleConfig.FontSizeMedium);
                RectTransform btnRect = newRunBtn.GetComponent<RectTransform>();
                PanelBuilder.SetAnchored(btnRect, 0.3f, 0.19f, 0.7f, 0.31f);
                newRunBtn.onClick.AddListener(() => _startPressed = true);
            }
            else
            {
                // New Run button (centered)
                Button newRunBtn = PanelBuilder.CreateButton("NewRunButton", rootRect,
                    "NEW RUN", UIStyleConfig.AccentGreen, UIStyleConfig.FontSizeMedium);
                RectTransform btnRect = newRunBtn.GetComponent<RectTransform>();
                PanelBuilder.SetAnchored(btnRect, 0.3f, 0.30f, 0.7f, 0.42f);
                newRunBtn.onClick.AddListener(() => _startPressed = true);
            }

            // Version / credits
            TextMeshProUGUI credits = PanelBuilder.CreateText("Credits", rootRect,
                "v0.1 - Press Start 2P", UIStyleConfig.FontSizeTiny,
                TextAlignmentOptions.Center, UIStyleConfig.TextDimmed);
            RectTransform creditsRect = credits.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(creditsRect, 0.2f, 0.05f, 0.8f, 0.12f);
        }

        public void Show()
        {
            _startPressed = false;
            _continuePressed = false;
            if (_root != null) _root.SetActive(true);
        }

        public void Hide()
        {
            if (_root != null) _root.SetActive(false);
        }

        public void Destroy()
        {
            if (_root != null) Object.Destroy(_root);
        }
    }
}

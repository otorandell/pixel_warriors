using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PixelWarriors
{
    public class CombatLogUI
    {
        public RectTransform Root { get; private set; }

        private TextMeshProUGUI _logText;
        private ScrollRect _scrollRect;
        private RectTransform _contentRect;

        public void Build(Transform parent)
        {
            Root = PanelBuilder.CreatePanel("CombatLog", parent);

            float padding = UIStyleConfig.PanelPadding;
            RectTransform inner = PanelBuilder.CreateContainer("Inner", Root);
            PanelBuilder.SetFill(inner, padding);

            (ScrollRect scroll, RectTransform content) = PanelBuilder.CreateVerticalScrollView("LogScroll", inner);
            _scrollRect = scroll;
            _contentRect = content;

            // Remove the ContentSizeFitter from content — we'll manage size manually
            ContentSizeFitter autoFitter = content.GetComponent<ContentSizeFitter>();
            if (autoFitter != null) Object.Destroy(autoFitter);

            _logText = PanelBuilder.CreateText("LogText", content, "",
                UIStyleConfig.FontSizeTiny, TextAlignmentOptions.TopLeft, UIStyleConfig.AccentGreen);
            RectTransform textRect = _logText.GetComponent<RectTransform>();
            textRect.anchorMin = new Vector2(0, 1);
            textRect.anchorMax = new Vector2(1, 1);
            textRect.pivot = new Vector2(0.5f, 1);
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;

            _logText.textWrappingMode = TextWrappingModes.Normal;
            _logText.overflowMode = TextOverflowModes.Overflow;

            GameEvents.OnCombatLogMessage += AddMessage;
        }

        public void AddMessage(string message)
        {
            if (_logText.text.Length > 0)
                _logText.text += "\n";

            _logText.text += "> " + message;

            // Force TMP to recalculate so preferredHeight is current
            _logText.ForceMeshUpdate();
            float textHeight = _logText.preferredHeight;

            // Size the text rect and content rect to fit the text
            _logText.rectTransform.sizeDelta = new Vector2(0, textHeight);
            _contentRect.sizeDelta = new Vector2(0, textHeight);

            // Scroll to bottom by positioning content so its bottom aligns with viewport bottom
            float viewportHeight = _scrollRect.viewport.rect.height;
            float overflow = textHeight - viewportHeight;
            if (overflow > 0)
            {
                _contentRect.anchoredPosition = new Vector2(0, overflow);
            }
        }

        public void Clear()
        {
            _logText.text = "";
            _logText.rectTransform.sizeDelta = new Vector2(0, 0);
            _contentRect.sizeDelta = new Vector2(0, 0);
            _contentRect.anchoredPosition = Vector2.zero;
        }
    }
}

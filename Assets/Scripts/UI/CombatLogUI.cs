using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace PixelWarriors
{
    public class CombatLogUI
    {
        public RectTransform Root { get; private set; }

        private TextMeshProUGUI _logText;
        private readonly List<string> _messages = new();
        private const int MaxVisibleLines = 3;

        public void Build(Transform parent)
        {
            Root = PanelBuilder.CreatePanel("CombatLog", parent);

            float padding = UIStyleConfig.PanelPadding;
            RectTransform content = PanelBuilder.CreateContainer("Content", Root);
            PanelBuilder.SetFill(content, padding);

            _logText = PanelBuilder.CreateText("LogText", content, "",
                UIStyleConfig.FontSizeSmall, TextAlignmentOptions.BottomLeft, UIStyleConfig.AccentGreen);
            RectTransform textRect = _logText.GetComponent<RectTransform>();
            PanelBuilder.SetFill(textRect);
            _logText.textWrappingMode = TextWrappingModes.Normal;
            _logText.overflowMode = TextOverflowModes.Truncate;

            GameEvents.OnCombatLogMessage += AddMessage;
        }

        public void AddMessage(string message)
        {
            _messages.Add(message);

            int startIndex = Mathf.Max(0, _messages.Count - MaxVisibleLines);
            string display = "";
            for (int i = startIndex; i < _messages.Count; i++)
            {
                if (display.Length > 0) display += "\n";
                display += "> " + _messages[i];
            }

            _logText.text = display;
        }

        public void Clear()
        {
            _messages.Clear();
            _logText.text = "";
        }
    }
}

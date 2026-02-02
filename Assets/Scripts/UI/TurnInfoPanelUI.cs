using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace PixelWarriors
{
    public class TurnInfoPanelUI
    {
        public RectTransform Root { get; private set; }

        private TextMeshProUGUI _roundText;
        private TextMeshProUGUI _activeText;
        private TextMeshProUGUI _orderText;

        public void Build(Transform parent)
        {
            Root = PanelBuilder.CreatePanel("TurnInfoPanel", parent);

            float padding = UIStyleConfig.PanelPadding;
            RectTransform content = PanelBuilder.CreateContainer("Content", Root);
            PanelBuilder.SetFill(content, padding);

            // Round number (left ~15%)
            _roundText = PanelBuilder.CreateText("RoundText", content, "",
                UIStyleConfig.FontSizeTiny, TextAlignmentOptions.MidlineLeft, UIStyleConfig.TextDimmed);
            RectTransform roundRect = _roundText.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(roundRect, 0, 0, 0.15f, 1);

            // Active character name (center ~30%)
            _activeText = PanelBuilder.CreateText("ActiveText", content, "",
                UIStyleConfig.FontSizeTiny, TextAlignmentOptions.Center, UIStyleConfig.TextPrimary);
            RectTransform activeRect = _activeText.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(activeRect, 0.15f, 0, 0.45f, 1);

            // Turn order sequence (right ~55%)
            _orderText = PanelBuilder.CreateText("OrderText", content, "",
                UIStyleConfig.FontSizeTiny, TextAlignmentOptions.MidlineLeft, UIStyleConfig.TextDimmed);
            RectTransform orderRect = _orderText.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(orderRect, 0.45f, 0, 1, 1);
            _orderText.overflowMode = TextOverflowModes.Ellipsis;

            GameEvents.OnTurnOrderUpdated += HandleTurnOrderUpdated;
            GameEvents.OnBattleStateChanged += HandleBattleStateChanged;
        }

        private void HandleTurnOrderUpdated(int roundNumber, BattleCharacter active, List<BattleCharacter> remaining)
        {
            _roundText.text = $"R:{roundNumber}";

            _activeText.text = $"{active.Data.Name}'s Turn";
            _activeText.color = UIFormatUtil.GetClassColor(active.Data.Class);

            string order = active.Data.Name;
            if (remaining.Count > 0)
            {
                order += " > " + string.Join(" > ", remaining.Where(c => c.IsAlive).Select(c => c.Data.Name));
            }
            _orderText.text = order;
        }

        private void HandleBattleStateChanged(BattleState state)
        {
            if (state == BattleState.Victory)
            {
                _activeText.text = "Victory!";
                _activeText.color = UIStyleConfig.AccentGreen;
                _orderText.text = "";
            }
            else if (state == BattleState.Defeat)
            {
                _activeText.text = "Defeat...";
                _activeText.color = UIStyleConfig.AccentRed;
                _orderText.text = "";
            }
        }

    }
}

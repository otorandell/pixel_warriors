using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PixelWarriors
{
    public class TurnInfoPanelUI
    {
        public RectTransform Root { get; private set; }

        private TextMeshProUGUI _roundText;
        private TextMeshProUGUI _activeText;
        private TextMeshProUGUI _actionPointsText;
        private TextMeshProUGUI _orderText;
        private BattleCharacter _activeCharacter;

        public void Build(Transform parent)
        {
            Root = PanelBuilder.CreatePanel("TurnInfoPanel", parent);

            float padding = UIStyleConfig.PanelPadding;
            RectTransform content = PanelBuilder.CreateContainer("Content", Root);
            PanelBuilder.SetFill(content, padding);

            // Round number (left ~12%)
            _roundText = PanelBuilder.CreateText("RoundText", content, "",
                UIStyleConfig.FontSizeTiny, TextAlignmentOptions.MidlineLeft, UIStyleConfig.TextDimmed);
            PanelBuilder.SetAnchored(_roundText.GetComponent<RectTransform>(), 0, 0, 0.12f, 1);

            // Active character name (~28%)
            _activeText = PanelBuilder.CreateText("ActiveText", content, "",
                UIStyleConfig.FontSizeTiny, TextAlignmentOptions.Center, UIStyleConfig.TextPrimary);
            PanelBuilder.SetAnchored(_activeText.GetComponent<RectTransform>(), 0.12f, 0, 0.40f, 1);

            // Action points (~10%)
            _actionPointsText = PanelBuilder.CreateText("ActionPoints", content, "",
                UIStyleConfig.FontSizeTiny, TextAlignmentOptions.Center, UIStyleConfig.TextPrimary);
            _actionPointsText.richText = true;
            PanelBuilder.SetAnchored(_actionPointsText.GetComponent<RectTransform>(), 0.40f, 0, 0.50f, 1);

            // Turn order sequence (right ~50%)
            _orderText = PanelBuilder.CreateText("OrderText", content, "",
                UIStyleConfig.FontSizeTiny, TextAlignmentOptions.MidlineLeft, UIStyleConfig.TextDimmed);
            PanelBuilder.SetAnchored(_orderText.GetComponent<RectTransform>(), 0.50f, 0, 1, 1);
            _orderText.overflowMode = TextOverflowModes.Ellipsis;

            // Long-press for detail popup
            Button panelButton = Root.gameObject.AddComponent<Button>();
            panelButton.transition = Selectable.Transition.None;
            LongPressHandler longPress = Root.gameObject.AddComponent<LongPressHandler>();
            longPress.OnLongPress += () => GameEvents.RaiseTurnOrderDetailRequested();

            GameEvents.OnTurnOrderUpdated += HandleTurnOrderUpdated;
            GameEvents.OnAbilityUsed += HandleAbilityUsed;
            GameEvents.OnBattleStateChanged += HandleBattleStateChanged;
        }

        private void HandleTurnOrderUpdated(int roundNumber, BattleCharacter active, List<BattleCharacter> remaining)
        {
            _activeCharacter = active;
            _roundText.text = $"R:{roundNumber}";

            _activeText.text = $"{active.Data.Name}'s Turn";
            _activeText.color = UIFormatUtil.GetClassColor(active.Data.Class);

            string order = active.Data.Name;
            if (remaining.Count > 0)
            {
                order += " > " + string.Join(" > ", remaining.Where(c => c.IsAlive).Select(c => c.Data.Name));
            }
            _orderText.text = order;

            RefreshActionPoints();
        }

        private void HandleAbilityUsed(BattleCharacter user, AbilityData ability, BattleCharacter target)
        {
            if (user == _activeCharacter)
            {
                RefreshActionPoints();
            }
        }

        private void HandleBattleStateChanged(BattleState state)
        {
            if (state == BattleState.Victory)
            {
                _activeText.text = "Victory!";
                _activeText.color = UIStyleConfig.AccentGreen;
                _orderText.text = "";
                _actionPointsText.text = "";
            }
            else if (state == BattleState.Defeat)
            {
                _activeText.text = "Defeat...";
                _activeText.color = UIStyleConfig.AccentRed;
                _orderText.text = "";
                _actionPointsText.text = "";
            }
        }

        private void RefreshActionPoints()
        {
            if (_activeCharacter == null) return;

            string white = ColorUtility.ToHtmlStringRGB(UIStyleConfig.TextPrimary);
            string dim = ColorUtility.ToHtmlStringRGB(UIStyleConfig.TextDimmed);

            string longColor = _activeCharacter.LongActionsRemaining > 0 ? white : dim;
            string shortColor = _activeCharacter.ShortActionsRemaining > 0 ? white : dim;

            _actionPointsText.text = $"<color=#{longColor}>\u25CF</color> <color=#{shortColor}>\u2022</color>";
        }

    }
}

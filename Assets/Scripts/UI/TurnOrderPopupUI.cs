using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PixelWarriors
{
    public class TurnOrderPopupUI : PopupBase
    {
        private int _roundNumber;
        private BattleCharacter _activeCharacter;
        private List<BattleCharacter> _remaining = new();

        public void SubscribeEvents()
        {
            GameEvents.OnTurnOrderUpdated += HandleTurnOrderUpdated;
        }

        public void UnsubscribeEvents()
        {
            GameEvents.OnTurnOrderUpdated -= HandleTurnOrderUpdated;
        }

        private void HandleTurnOrderUpdated(int roundNumber, BattleCharacter active, List<BattleCharacter> remaining)
        {
            _roundNumber = roundNumber;
            _activeCharacter = active;
            _remaining = remaining;
        }

        public void ShowTurnOrderPopup()
        {
            if (_activeCharacter == null) return;

            ClearContent();

            // Header
            AddText($"Round {_roundNumber} - Turn Order", UIStyleConfig.FontSizeSmall,
                UIStyleConfig.AccentCyan, 0.90f, 1f);

            // Active character
            Color activeColor = UIFormatUtil.GetClassColor(_activeCharacter.Data.Class);
            string activeHP = $"HP:{_activeCharacter.CurrentHP}/{_activeCharacter.MaxHP}";
            string activeSide = _activeCharacter.Side == TeamSide.Player ? "" : " [E]";
            AddText($"> {_activeCharacter.Data.Name}{activeSide}  {activeHP}", UIStyleConfig.FontSizeTiny,
                activeColor, 0.80f, 0.88f);

            AddText("  (Acting now)", UIStyleConfig.FontSizeTiny,
                UIStyleConfig.TextDimmed, 0.74f, 0.80f);

            // Remaining characters
            List<BattleCharacter> alive = _remaining.Where(c => c.IsAlive).ToList();

            if (alive.Count == 0)
            {
                AddText("No more turns this round.", UIStyleConfig.FontSizeTiny,
                    UIStyleConfig.TextDimmed, 0.64f, 0.72f);
            }
            else
            {
                AddText("-- Upcoming --", UIStyleConfig.FontSizeTiny,
                    UIStyleConfig.AccentMagenta, 0.64f, 0.72f);

                float yTop = 0.64f;
                float lineHeight = 0.07f;
                int maxEntries = Mathf.Min(alive.Count, 8);

                for (int i = 0; i < maxEntries; i++)
                {
                    BattleCharacter c = alive[i];
                    Color color = UIFormatUtil.GetClassColor(c.Data.Class);
                    string side = c.Side == TeamSide.Player ? "" : " [E]";
                    string hp = $"HP:{c.CurrentHP}/{c.MaxHP}";
                    string line = $"  {c.Data.Name}{side}  {hp}";

                    float yMax = yTop - i * lineHeight;
                    float yMin = yMax - lineHeight;
                    AddText(line, UIStyleConfig.FontSizeTiny, color, yMin, yMax);
                }

                if (alive.Count > maxEntries)
                {
                    float yMax = yTop - maxEntries * lineHeight;
                    AddText($"  +{alive.Count - maxEntries} more...", UIStyleConfig.FontSizeTiny,
                        UIStyleConfig.TextDimmed, yMax - lineHeight, yMax);
                }
            }

            Show();
        }
    }
}

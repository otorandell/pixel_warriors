using TMPro;
using UnityEngine;

namespace PixelWarriors
{
    public class AbilityPopupUI : PopupBase
    {
        public void ShowAbilityPopup(AbilityData ability)
        {
            ClearContent();

            // Header: ability name
            AddText(ability.Name, UIStyleConfig.FontSizeSmall, UIStyleConfig.AccentCyan, 0.88f, 1f);

            // Description (wrapped)
            TextMeshProUGUI descText = AddText(ability.Description, UIStyleConfig.FontSizeTiny,
                UIStyleConfig.TextPrimary, 0.62f, 0.86f);
            descText.textWrappingMode = TextWrappingModes.Normal;
            descText.overflowMode = TextOverflowModes.Truncate;

            // Properties
            float y = 0.58f;
            float lineH = 0.07f;

            // Action cost
            string actionStr = ability.ActionCost == ActionPointType.Long
                ? $"Action: Long ({ability.LongPointCost} pt)"
                : $"Action: Quick ({ability.ShortPointCost} pt)";
            AddText(actionStr, UIStyleConfig.FontSizeTiny, UIStyleConfig.TextDimmed, y - lineH, y);
            y -= lineH;

            // Resource costs (only show non-zero)
            if (ability.EnergyCost > 0)
            {
                AddText($"Energy Cost: {ability.EnergyCost}", UIStyleConfig.FontSizeTiny,
                    UIStyleConfig.AccentYellow, y - lineH, y);
                y -= lineH;
            }

            if (ability.ManaCost > 0)
            {
                AddText($"Mana Cost: {ability.ManaCost}", UIStyleConfig.FontSizeTiny,
                    UIStyleConfig.AccentCyan, y - lineH, y);
                y -= lineH;
            }

            if (ability.HPCost > 0)
            {
                AddText($"HP Cost: {ability.HPCost}", UIStyleConfig.FontSizeTiny,
                    UIStyleConfig.AccentRed, y - lineH, y);
                y -= lineH;
            }

            // Damage/healing info
            if (ability.BasePower > 0)
            {
                AddText($"Power: {ability.BasePower}  Type: {ability.DamageType}",
                    UIStyleConfig.FontSizeTiny, UIStyleConfig.TextPrimary, y - lineH, y);
                y -= lineH;
            }

            if (ability.Element != Element.None)
            {
                AddText($"Element: {ability.Element}", UIStyleConfig.FontSizeTiny,
                    UIStyleConfig.AccentMagenta, y - lineH, y);
                y -= lineH;
            }

            if (ability.HitCount > 1)
            {
                AddText($"Hits: {ability.HitCount}", UIStyleConfig.FontSizeTiny,
                    UIStyleConfig.TextPrimary, y - lineH, y);
                y -= lineH;
            }

            // Target type
            string targetStr = UIFormatUtil.FormatTargetType(ability.TargetType);
            AddText($"Target: {targetStr}", UIStyleConfig.FontSizeTiny,
                UIStyleConfig.TextDimmed, y - lineH, y);

            Show();
        }
    }
}

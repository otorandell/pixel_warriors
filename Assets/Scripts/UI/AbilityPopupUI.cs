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
            string actionStr;
            if (ability.LongPointCost > 0 && ability.ShortPointCost > 0)
                actionStr = $"Action: {ability.LongPointCost}L + {ability.ShortPointCost}S";
            else if (ability.LongPointCost > 0)
                actionStr = $"Action: Long ({ability.LongPointCost} pt)";
            else
                actionStr = $"Action: Quick ({ability.ShortPointCost} pt)";
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
            if (ability.IsWeaponAttack)
            {
                AddText($"Damage: {ability.DamageMultiplier:0.0}x  Type: Physical",
                    UIStyleConfig.FontSizeTiny, UIStyleConfig.TextPrimary, y - lineH, y);
                y -= lineH;
            }
            else if (ability.BasePower > 0)
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
            y -= lineH;

            // Range
            string rangeStr = UIFormatUtil.FormatAbilityRange(ability.Range);
            if (!string.IsNullOrEmpty(rangeStr))
            {
                AddText($"Range: {rangeStr}", UIStyleConfig.FontSizeTiny,
                    UIStyleConfig.TextDimmed, y - lineH, y);
                y -= lineH;
            }

            // Requirements
            if (ability.RequiredWeapon != WeaponType.None)
            {
                AddText($"Requires: {ability.RequiredWeapon}", UIStyleConfig.FontSizeTiny,
                    UIStyleConfig.AccentYellow, y - lineH, y);
                y -= lineH;
            }

            if (ability.RequiresFrontline)
            {
                AddText("Frontline Only", UIStyleConfig.FontSizeTiny,
                    UIStyleConfig.AccentRed, y - lineH, y);
                y -= lineH;
            }

            if (ability.OncePerBattle)
            {
                AddText("Once Per Battle", UIStyleConfig.FontSizeTiny,
                    UIStyleConfig.AccentMagenta, y - lineH, y);
                y -= lineH;
            }

            if (ability.RequiresConcealed)
            {
                AddText("Requires: Concealed", UIStyleConfig.FontSizeTiny,
                    UIStyleConfig.AccentGreen, y - lineH, y);
            }

            Show();
        }
    }
}

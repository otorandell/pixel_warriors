using UnityEngine;

namespace PixelWarriors
{
    public static class UIFormatUtil
    {
        public static Color GetClassColor(CharacterClass characterClass)
        {
            return characterClass switch
            {
                CharacterClass.Warrior => UIStyleConfig.AccentRed,
                CharacterClass.Rogue => UIStyleConfig.AccentGreen,
                CharacterClass.Ranger => UIStyleConfig.AccentYellow,
                CharacterClass.Priest => UIStyleConfig.TextPrimary,
                CharacterClass.Elementalist => UIStyleConfig.AccentCyan,
                CharacterClass.Warlock => UIStyleConfig.AccentMagenta,
                _ => UIStyleConfig.TextPrimary
            };
        }

        public static string FormatAbilityCost(AbilityData ability)
        {
            string cost = "";
            if (ability.EnergyCost > 0) cost += $"[{ability.EnergyCost}E]";
            if (ability.ManaCost > 0) cost += $"[{ability.ManaCost}M]";
            if (ability.HPCost > 0) cost += $"[{ability.HPCost}HP]";
            if (cost == "" && ability.ActionCost == ActionPointType.Short) cost = "[Q]";

            // Action point indicators
            string ap = "";
            if (ability.LongPointCost > 0) ap += "A";
            if (ability.ShortPointCost > 0) ap += "S";
            if (ap.Length > 0 && cost.Length > 0) cost = $"[{ap}]{cost}";
            else if (ap.Length > 0) cost = $"[{ap}]";

            return cost;
        }

        public static string FormatAbilityRange(AbilityRange range)
        {
            return range switch
            {
                AbilityRange.Close => "Close",
                AbilityRange.Reach => "Reach",
                AbilityRange.Weapon => "Weapon",
                _ => ""
            };
        }

        public static string FormatTargetType(TargetType targetType)
        {
            return targetType switch
            {
                TargetType.SingleEnemy => "Single Enemy",
                TargetType.SingleAlly => "Single Ally",
                TargetType.Self => "Self",
                TargetType.AllEnemies => "All Enemies",
                TargetType.AllAllies => "All Allies",
                TargetType.All => "Everyone",
                _ => targetType.ToString()
            };
        }
    }
}

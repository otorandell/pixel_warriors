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
                CharacterClass.Wizard => UIStyleConfig.AccentCyan,
                CharacterClass.Warlock => UIStyleConfig.AccentMagenta,
                _ => UIStyleConfig.TextPrimary
            };
        }

        public static string FormatAbilityCost(AbilityData ability)
        {
            if (ability.EnergyCost > 0) return $"[{ability.EnergyCost}E]";
            if (ability.ManaCost > 0) return $"[{ability.ManaCost}M]";
            if (ability.HPCost > 0) return $"[{ability.HPCost}HP]";
            if (ability.ActionCost == ActionPointType.Short) return "[Q]";
            return "";
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

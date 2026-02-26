using System.Collections.Generic;
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

        public static string FormatSlotName(EquipmentSlot slot)
        {
            return slot switch
            {
                EquipmentSlot.Hand1 => "Weapon",
                EquipmentSlot.Offhand => "Off-Hand",
                EquipmentSlot.Head => "Head",
                EquipmentSlot.Body => "Body",
                EquipmentSlot.Trinket1 => "Trinket 1",
                EquipmentSlot.Trinket2 => "Trinket 2",
                _ => slot.ToString()
            };
        }

        public static string FormatItemStats(EquipmentData item)
        {
            List<string> parts = new();
            if (item.BaseDamage > 0) parts.Add($"{item.BaseDamage} dmg");
            if (item.BaseBlockChance > 0) parts.Add($"{Mathf.RoundToInt(item.BaseBlockChance * 100)}% blk");

            CharacterStats s = item.StatModifiers;
            if (s.Endurance != 0) parts.Add(StatString("END", s.Endurance));
            if (s.Stamina != 0) parts.Add(StatString("STA", s.Stamina));
            if (s.Intellect != 0) parts.Add(StatString("INT", s.Intellect));
            if (s.Strength != 0) parts.Add(StatString("STR", s.Strength));
            if (s.Dexterity != 0) parts.Add(StatString("DEX", s.Dexterity));
            if (s.Willpower != 0) parts.Add(StatString("WIL", s.Willpower));
            if (s.Armor != 0) parts.Add(StatString("ARM", s.Armor));
            if (s.MagicResist != 0) parts.Add(StatString("MRS", s.MagicResist));
            if (s.Initiative != 0) parts.Add(StatString("INI", s.Initiative));
            return string.Join("  ", parts);
        }

        public static string FormatStatComparison(CharacterStats before, CharacterStats after)
        {
            List<string> parts = new();
            AddDiff(parts, "END", before.Endurance, after.Endurance);
            AddDiff(parts, "STA", before.Stamina, after.Stamina);
            AddDiff(parts, "INT", before.Intellect, after.Intellect);
            AddDiff(parts, "STR", before.Strength, after.Strength);
            AddDiff(parts, "DEX", before.Dexterity, after.Dexterity);
            AddDiff(parts, "WIL", before.Willpower, after.Willpower);
            AddDiff(parts, "ARM", before.Armor, after.Armor);
            AddDiff(parts, "MRS", before.MagicResist, after.MagicResist);
            AddDiff(parts, "INI", before.Initiative, after.Initiative);
            return string.Join("  ", parts);
        }

        public static Color GetItemNameColor(EquipmentData item)
        {
            return item.IsUnique ? UIStyleConfig.AccentMagenta : UIStyleConfig.TextPrimary;
        }

        private static string StatString(string name, int value)
        {
            return value > 0 ? $"+{value} {name}" : $"{value} {name}";
        }

        private static void AddDiff(List<string> parts, string name, int before, int after)
        {
            int diff = after - before;
            if (diff == 0) return;
            string sign = diff > 0 ? "+" : "";
            parts.Add($"{name}:{before}>{after}({sign}{diff})");
        }
    }
}

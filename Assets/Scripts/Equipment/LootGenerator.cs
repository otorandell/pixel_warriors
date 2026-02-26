using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PixelWarriors
{
    public static class LootGenerator
    {
        public static List<EquipmentData> GenerateLoot(int act, RoomType roomType, RunData runData)
        {
            int dropCount = roomType switch
            {
                RoomType.EliteBattle => LootConfig.EliteBattleDrops,
                RoomType.BossBattle => LootConfig.BossBattleDrops,
                _ => LootConfig.NormalBattleDrops
            };

            float uniqueBonus = roomType switch
            {
                RoomType.EliteBattle => LootConfig.EliteUniqueDropBonus,
                RoomType.BossBattle => LootConfig.BossUniqueDropBonus,
                _ => 0f
            };

            List<EquipmentData> loot = new();

            for (int i = 0; i < dropCount; i++)
            {
                float uniqueRoll = Random.value;
                if (uniqueRoll < LootConfig.UniqueDropChance + uniqueBonus)
                {
                    EquipmentData unique = TryGenerateUnique(act, runData);
                    if (unique != null)
                    {
                        loot.Add(unique);
                        continue;
                    }
                }

                loot.Add(GenerateProceduralItem(act));
            }

            return loot;
        }

        public static EquipmentData GenerateProceduralItem(int act)
        {
            int actIndex = Mathf.Clamp(act - 1, 0, 2);

            // Pick random slot (weighted)
            EquipmentSlot slot = PickRandomSlot();

            // Get templates for this slot and act
            List<ItemTemplate> templates = ItemTemplateCatalog.GetTemplates()
                .Where(t => t.Slot == slot && t.MinAct <= act && t.MaxAct >= act)
                .ToList();

            // Handle trinket slot: templates use Trinket1 for both
            if (slot == EquipmentSlot.Trinket2)
            {
                templates = ItemTemplateCatalog.GetTemplates()
                    .Where(t => t.Slot == EquipmentSlot.Trinket1 && t.MinAct <= act && t.MaxAct >= act)
                    .ToList();
            }

            if (templates.Count == 0)
                return GenerateFallbackItem(act, actIndex);

            ItemTemplate template = templates[Random.Range(0, templates.Count)];
            ItemProfile profile = template.AllowedProfiles[Random.Range(0, template.AllowedProfiles.Length)];

            EquipmentData item = new EquipmentData
            {
                Slot = slot,
                WeaponType = template.WeaponType,
                ActLevel = act,
                IsUnique = false
            };

            // Weapon damage
            if (template.WeaponType != WeaponType.None && template.WeaponType != WeaponType.Shield)
            {
                item.BaseDamage = Random.Range(LootConfig.WeaponDamageMin[actIndex],
                    LootConfig.WeaponDamageMax[actIndex] + 1);
            }

            // Shield block chance
            if (template.WeaponType == WeaponType.Shield)
            {
                item.BaseBlockChance = Mathf.Lerp(LootConfig.BlockChanceMin[actIndex],
                    LootConfig.BlockChanceMax[actIndex], Random.value);
                item.BaseBlockChance = Mathf.Round(item.BaseBlockChance * 100f) / 100f;
            }

            // Generate stats based on profile
            item.StatModifiers = GenerateStatsForProfile(profile, actIndex);

            // Build name
            string prefix = LootConfig.ActPrefixes[actIndex][Random.Range(0, LootConfig.ActPrefixes[actIndex].Length)];
            item.Name = $"{prefix} {template.BaseName}";

            // Auto-generate description
            item.Description = BuildDescription(item);

            return item;
        }

        public static EquipmentData TryGenerateUnique(int act, RunData runData)
        {
            List<EquipmentData> uniques = ItemTemplateCatalog.GetUniqueItems()
                .Where(u => u.ActLevel <= act && !runData.DroppedUniques.Contains(u.Name))
                .ToList();

            if (uniques.Count == 0) return null;

            EquipmentData source = uniques[Random.Range(0, uniques.Count)];
            runData.DroppedUniques.Add(source.Name);

            // Clone so the catalog entry stays pristine
            return new EquipmentData
            {
                Name = source.Name,
                Description = source.Description,
                Slot = source.Slot,
                WeaponType = source.WeaponType,
                BaseDamage = source.BaseDamage,
                BaseBlockChance = source.BaseBlockChance,
                ArmorPenetration = source.ArmorPenetration,
                MagicPenetration = source.MagicPenetration,
                StatModifiers = source.StatModifiers.Clone(),
                IsUnique = true,
                FlavorText = source.FlavorText,
                ActLevel = source.ActLevel
            };
        }

        // --- Private Helpers ---

        private static EquipmentSlot PickRandomSlot()
        {
            int total = LootConfig.WeaponWeight + LootConfig.OffhandWeight + LootConfig.HeadWeight
                        + LootConfig.BodyWeight + LootConfig.TrinketWeight;
            int roll = Random.Range(0, total);

            if (roll < LootConfig.WeaponWeight) return EquipmentSlot.Hand1;
            roll -= LootConfig.WeaponWeight;

            if (roll < LootConfig.OffhandWeight) return EquipmentSlot.Offhand;
            roll -= LootConfig.OffhandWeight;

            if (roll < LootConfig.HeadWeight) return EquipmentSlot.Head;
            roll -= LootConfig.HeadWeight;

            if (roll < LootConfig.BodyWeight) return EquipmentSlot.Body;

            // Trinket — randomly pick slot 1 or 2
            return Random.value < 0.5f ? EquipmentSlot.Trinket1 : EquipmentSlot.Trinket2;
        }

        private static CharacterStats GenerateStatsForProfile(ItemProfile profile, int actIndex)
        {
            CharacterStats stats = new CharacterStats();

            // Get primary and secondary stat indices for this profile
            int[] primary = GetPrimaryStats(profile);
            int[] secondary = GetSecondaryStats(profile);

            // Roll primary stats
            foreach (int statIdx in primary)
            {
                int value = Random.Range(LootConfig.PrimaryStatMin[actIndex],
                    LootConfig.PrimaryStatMax[actIndex] + 1);
                SetStatByIndex(stats, statIdx, value);
            }

            // Roll 0-2 secondary stats
            int secondaryCount = Random.Range(0, Mathf.Min(3, secondary.Length + 1));
            List<int> shuffledSecondary = new List<int>(secondary);
            ShuffleList(shuffledSecondary);

            for (int i = 0; i < secondaryCount && i < shuffledSecondary.Count; i++)
            {
                int value = Random.Range(LootConfig.SecondaryStatMin[actIndex],
                    LootConfig.SecondaryStatMax[actIndex] + 1);
                if (value > 0)
                    SetStatByIndex(stats, shuffledSecondary[i], value);
            }

            return stats;
        }

        // Stat index mapping: 0=END, 1=STA, 2=INT, 3=STR, 4=DEX, 5=WIL, 6=ARM, 7=MRS, 8=INI
        private static int[] GetPrimaryStats(ItemProfile profile)
        {
            return profile switch
            {
                ItemProfile.Tank => new[] { 0, 6 },        // END, ARM
                ItemProfile.Agile => new[] { 4, 8 },       // DEX, INI
                ItemProfile.Mighty => new[] { 3, 0 },      // STR, END
                ItemProfile.Arcane => new[] { 2, 5 },      // INT, WIL
                ItemProfile.Cunning => new[] { 4, 3 },     // DEX, STR
                ItemProfile.Resilient => new[] { 0, 5 },   // END, WIL
                ItemProfile.Swift => new[] { 8, 1 },       // INI, STA
                _ => PickRandomPrimary()                    // Balanced: 2 random
            };
        }

        private static int[] GetSecondaryStats(ItemProfile profile)
        {
            return profile switch
            {
                ItemProfile.Tank => new[] { 7, 1 },        // MRS, STA
                ItemProfile.Agile => new[] { 1 },          // STA
                ItemProfile.Mighty => new[] { 6 },         // ARM
                ItemProfile.Arcane => new[] { 7 },         // MRS
                ItemProfile.Cunning => new[] { 8 },        // INI
                ItemProfile.Resilient => new[] { 7, 6 },   // MRS, ARM
                ItemProfile.Swift => new[] { 4 },          // DEX
                _ => new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8 } // Balanced: any
            };
        }

        private static int[] PickRandomPrimary()
        {
            List<int> all = new() { 0, 1, 2, 3, 4, 5, 6, 7, 8 };
            ShuffleList(all);
            return new[] { all[0], all[1] };
        }

        private static void SetStatByIndex(CharacterStats stats, int index, int value)
        {
            switch (index)
            {
                case 0: stats.Endurance += value; break;
                case 1: stats.Stamina += value; break;
                case 2: stats.Intellect += value; break;
                case 3: stats.Strength += value; break;
                case 4: stats.Dexterity += value; break;
                case 5: stats.Willpower += value; break;
                case 6: stats.Armor += value; break;
                case 7: stats.MagicResist += value; break;
                case 8: stats.Initiative += value; break;
            }
        }

        private static void ShuffleList<T>(List<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }

        private static EquipmentData GenerateFallbackItem(int act, int actIndex)
        {
            string prefix = LootConfig.ActPrefixes[actIndex][Random.Range(0, LootConfig.ActPrefixes[actIndex].Length)];
            EquipmentData item = new EquipmentData
            {
                Name = $"{prefix} Ring",
                Slot = EquipmentSlot.Trinket1,
                WeaponType = WeaponType.None,
                ActLevel = act,
                IsUnique = false,
                StatModifiers = GenerateStatsForProfile(ItemProfile.Balanced, actIndex)
            };
            item.Description = BuildDescription(item);
            return item;
        }

        private static string BuildDescription(EquipmentData item)
        {
            List<string> parts = new();

            if (item.BaseDamage > 0) parts.Add($"{item.BaseDamage} dmg");
            if (item.BaseBlockChance > 0) parts.Add($"{Mathf.RoundToInt(item.BaseBlockChance * 100)}% block");

            CharacterStats s = item.StatModifiers;
            if (s.Endurance != 0) parts.Add(FormatStat("END", s.Endurance));
            if (s.Stamina != 0) parts.Add(FormatStat("STA", s.Stamina));
            if (s.Intellect != 0) parts.Add(FormatStat("INT", s.Intellect));
            if (s.Strength != 0) parts.Add(FormatStat("STR", s.Strength));
            if (s.Dexterity != 0) parts.Add(FormatStat("DEX", s.Dexterity));
            if (s.Willpower != 0) parts.Add(FormatStat("WIL", s.Willpower));
            if (s.Armor != 0) parts.Add(FormatStat("ARM", s.Armor));
            if (s.MagicResist != 0) parts.Add(FormatStat("MRS", s.MagicResist));
            if (s.Initiative != 0) parts.Add(FormatStat("INI", s.Initiative));

            if (item.ArmorPenetration > 0) parts.Add($"+{Mathf.RoundToInt(item.ArmorPenetration * 100)}% ArPen");
            if (item.MagicPenetration > 0) parts.Add($"+{item.MagicPenetration} MPen");

            return string.Join(", ", parts);
        }

        private static string FormatStat(string name, int value)
        {
            return value > 0 ? $"+{value} {name}" : $"{value} {name}";
        }
    }
}

using System.Collections.Generic;
using UnityEngine;

namespace PixelWarriors
{
    public static class LevelingSystem
    {
        // Fibonacci-based ability unlock levels: 1, 2, 3, 5, 8, 13, 21...
        // Index 0 = level 1 (starting ability), index 1 = level 2, etc.
        private static readonly int[] AbilityUnlockLevels = { 1, 2, 3, 5, 8, 13, 21, 34, 55, 89 };

        // Passive unlock every 5 levels: 5, 10, 15, 20...
        private const int PassiveUnlockInterval = 5;

        public struct LevelUpResult
        {
            public int OldLevel;
            public int NewLevel;
            public CharacterStats StatGains;
            public List<AbilityData> NewAbilities;
            public List<AbilityData> NewPassives;
        }

        public static LevelUpResult AddXP(CharacterData character, int amount)
        {
            LevelUpResult result = new()
            {
                OldLevel = character.Level,
                StatGains = new CharacterStats(),
                NewAbilities = new List<AbilityData>(),
                NewPassives = new List<AbilityData>()
            };

            character.CurrentXP += amount;

            while (character.CurrentXP >= StatCalculator.CalculateXPToLevel(character.Level))
            {
                character.CurrentXP -= StatCalculator.CalculateXPToLevel(character.Level);
                character.Level++;

                CharacterStats gains = RollGrowth(character);
                character.BaseStats.Add(gains);
                result.StatGains.Add(gains);

                // Check ability unlocks
                List<AbilityData> classAbilities = GetFullAbilityList(character.Class);
                int unlockedCount = GetAbilityCountAtLevel(character.Level);
                int currentCount = CountClassAbilities(character, classAbilities);

                while (currentCount < unlockedCount && currentCount < classAbilities.Count)
                {
                    AbilityData newAbility = classAbilities[currentCount];
                    character.Abilities.Add(newAbility);
                    result.NewAbilities.Add(newAbility);
                    currentCount++;
                }

                // Check passive unlocks
                List<AbilityData> classPassives = GetFullPassiveList(character.Class);
                int passiveUnlocked = character.Level / PassiveUnlockInterval;
                int currentPassiveCount = CountClassPassives(character, classPassives);

                while (currentPassiveCount < passiveUnlocked && currentPassiveCount < classPassives.Count)
                {
                    AbilityData newPassive = classPassives[currentPassiveCount];
                    character.Passives.Add(newPassive);
                    result.NewPassives.Add(newPassive);
                    currentPassiveCount++;
                }

                // Update persistent resources to new max
                CharacterStats total = character.GetTotalStats();
                int newMaxHP = StatCalculator.CalculateMaxHP(total);
                int newMaxEnergy = StatCalculator.CalculateMaxEnergy(total);
                int newMaxMana = StatCalculator.CalculateMaxMana(total);

                // Gain the difference from stat growth
                character.CurrentHP += newMaxHP - StatCalculator.CalculateMaxHP(
                    SubtractStats(total, gains));
                character.CurrentEnergy = newMaxEnergy;
                character.CurrentMana = newMaxMana;
            }

            result.NewLevel = character.Level;
            return result;
        }

        private static CharacterStats RollGrowth(CharacterData character)
        {
            CharacterStats classGrowth = GrowthRates.GetClassGrowth(character.Class);
            CharacterStats modifiers = character.GrowthModifiers;
            CharacterStats gains = new CharacterStats();

            gains.Endurance = RollStat(classGrowth.Endurance + modifiers.Endurance);
            gains.Stamina = RollStat(classGrowth.Stamina + modifiers.Stamina);
            gains.Intellect = RollStat(classGrowth.Intellect + modifiers.Intellect);
            gains.Strength = RollStat(classGrowth.Strength + modifiers.Strength);
            gains.Dexterity = RollStat(classGrowth.Dexterity + modifiers.Dexterity);
            gains.Willpower = RollStat(classGrowth.Willpower + modifiers.Willpower);
            gains.Armor = RollStat(classGrowth.Armor + modifiers.Armor);
            gains.MagicResist = RollStat(classGrowth.MagicResist + modifiers.MagicResist);
            gains.Initiative = RollStat(classGrowth.Initiative + modifiers.Initiative);

            return gains;
        }

        private static int RollStat(int growthRate)
        {
            int guaranteed = growthRate / 100;
            int remainder = growthRate % 100;
            int bonus = Random.Range(0, 100) < remainder ? 1 : 0;
            return guaranteed + bonus;
        }

        public static int GetAbilityCountAtLevel(int level)
        {
            int count = 0;
            for (int i = 0; i < AbilityUnlockLevels.Length; i++)
            {
                if (level >= AbilityUnlockLevels[i])
                    count++;
                else
                    break;
            }
            return count;
        }

        private static int CountClassAbilities(CharacterData character, List<AbilityData> classAbilities)
        {
            int count = 0;
            foreach (AbilityData classAbility in classAbilities)
            {
                if (character.Abilities.Exists(a => a.Name == classAbility.Name))
                    count++;
            }
            return count;
        }

        private static int CountClassPassives(CharacterData character, List<AbilityData> classPassives)
        {
            // Don't count the basic passive (index 0 from ClassDefinitions)
            int count = 0;
            foreach (AbilityData classPassive in classPassives)
            {
                if (character.Passives.Exists(p => p.Name == classPassive.Name))
                    count++;
            }
            return count;
        }

        private static List<AbilityData> GetFullAbilityList(CharacterClass characterClass)
        {
            return characterClass switch
            {
                CharacterClass.Warrior => AbilityCatalog.GetWarriorAbilities(),
                CharacterClass.Rogue => AbilityCatalog.GetRogueAbilities(),
                CharacterClass.Ranger => AbilityCatalog.GetRangerAbilities(),
                CharacterClass.Priest => AbilityCatalog.GetPriestAbilities(),
                CharacterClass.Elementalist => AbilityCatalog.GetElementalistAbilities(),
                CharacterClass.Warlock => AbilityCatalog.GetWarlockAbilities(),
                _ => new List<AbilityData>()
            };
        }

        private static List<AbilityData> GetFullPassiveList(CharacterClass characterClass)
        {
            return characterClass switch
            {
                CharacterClass.Warrior => AbilityCatalog.GetWarriorPassives(),
                CharacterClass.Rogue => AbilityCatalog.GetRoguePassives(),
                CharacterClass.Warlock => AbilityCatalog.GetWarlockPassives(),
                _ => new List<AbilityData>()
            };
        }

        private static CharacterStats SubtractStats(CharacterStats total, CharacterStats gains)
        {
            return new CharacterStats(
                total.Endurance - gains.Endurance,
                total.Stamina - gains.Stamina,
                total.Intellect - gains.Intellect,
                total.Strength - gains.Strength,
                total.Dexterity - gains.Dexterity,
                total.Willpower - gains.Willpower,
                total.Armor - gains.Armor,
                total.MagicResist - gains.MagicResist,
                total.Initiative - gains.Initiative
            );
        }
    }
}

using System.Collections.Generic;
using UnityEngine;

namespace PixelWarriors
{
    public static class LevelingSystem
    {
        // Fibonacci-based ability unlock levels: 1, 2, 3, 5, 8, 13, 21
        private static readonly int[] AbilityUnlockLevels = { 1, 2, 3, 5, 8, 13, 21 };

        private const int PassiveUnlockInterval = 5;
        public const int MaxLevel = 21;

        // Track recent offers to avoid repeats (indices into party)
        private static readonly List<int> _recentOffers = new();
        private const int RecentOfferMemory = 2;

        public struct LevelUpResult
        {
            public CharacterData Character;
            public int OldLevel;
            public int NewLevel;
            public CharacterStats StatGains;
            public List<AbilityData> NewAbilities;
            public List<AbilityData> NewPassives;
        }

        /// <summary>
        /// Select 2 candidates from the party for the player to choose from.
        /// Pseudo-random: avoids offering the same character repeatedly, excludes max level.
        /// Returns indices into the party list.
        /// </summary>
        public static List<int> GetLevelUpCandidates(List<CharacterData> party)
        {
            List<int> eligible = new();
            for (int i = 0; i < party.Count; i++)
            {
                if (party[i].Level < MaxLevel)
                    eligible.Add(i);
            }

            if (eligible.Count == 0) return new List<int>();
            if (eligible.Count == 1) return new List<int> { eligible[0] };

            // Prefer characters not recently offered
            List<int> preferred = new();
            List<int> fallback = new();

            foreach (int idx in eligible)
            {
                if (_recentOffers.Contains(idx))
                    fallback.Add(idx);
                else
                    preferred.Add(idx);
            }

            List<int> result = new();

            // Pick from preferred first, then fallback
            List<int> pool = preferred.Count >= 2 ? preferred : eligible;
            while (result.Count < 2 && pool.Count > 0)
            {
                int pick = Random.Range(0, pool.Count);
                result.Add(pool[pick]);
                pool.RemoveAt(pick);

                // If we depleted preferred and need more, switch to fallback
                if (pool.Count == 0 && result.Count < 2 && preferred.Count < 2)
                {
                    pool = new List<int>(fallback);
                    foreach (int r in result) pool.Remove(r);
                }
            }

            // Update recent offer tracking
            foreach (int idx in result)
            {
                _recentOffers.Remove(idx);
                _recentOffers.Add(idx);
            }
            while (_recentOffers.Count > RecentOfferMemory)
                _recentOffers.RemoveAt(0);

            return result;
        }

        /// <summary>
        /// Level up a character by 1 level. Rolls growth, unlocks abilities/passives.
        /// </summary>
        public static LevelUpResult LevelUp(CharacterData character)
        {
            LevelUpResult result = new()
            {
                Character = character,
                OldLevel = character.Level,
                StatGains = new CharacterStats(),
                NewAbilities = new List<AbilityData>(),
                NewPassives = new List<AbilityData>()
            };

            if (character.Level >= MaxLevel) return result;

            character.Level++;

            CharacterStats gains = RollGrowth(character);
            character.BaseStats.Add(gains);
            result.StatGains = gains;

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
            character.CurrentHP += newMaxHP - StatCalculator.CalculateMaxHP(SubtractStats(total, gains));
            character.CurrentEnergy = newMaxEnergy;
            character.CurrentMana = newMaxMana;

            result.NewLevel = character.Level;
            return result;
        }

        /// <summary>
        /// Reset offer tracking (call at run start).
        /// </summary>
        public static void ResetOfferTracking()
        {
            _recentOffers.Clear();
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
            return ClassDefinitions.GetClassAbilityList(characterClass);
        }

        private static List<AbilityData> GetFullPassiveList(CharacterClass characterClass)
        {
            return ClassDefinitions.GetClassPassiveList(characterClass);
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

using System.Collections.Generic;

namespace PixelWarriors
{
    public static class ClassDefinitions
    {
        public static CharacterData CreateCharacter(string name, CharacterClass characterClass)
        {
            CharacterData data = new CharacterData
            {
                Name = name,
                Class = characterClass,
                BaseStats = GetBaseStats(characterClass)
            };

            // Starting abilities from catalog
            List<AbilityData> classAbilities = GetStartingAbilities(characterClass);
            data.Abilities.AddRange(classAbilities);
            data.Abilities.AddRange(AbilityCatalog.GetGenericAbilities());

            // Starting passive + class passives from catalog
            data.Passives.Add(GetBasicPassive(characterClass));
            data.Passives.AddRange(GetClassPassives(characterClass));

            return data;
        }

        private static CharacterStats GetBaseStats(CharacterClass characterClass)
        {
            return characterClass switch
            {
                //                              END STA INT STR DEX WIL ARM MRS INI
                CharacterClass.Warrior      => new CharacterStats(8,  5,  2,  7,  4,  3,  6,  8,  4),
                CharacterClass.Rogue        => new CharacterStats(4,  7,  3,  5,  8,  3,  3, 10,  7),
                CharacterClass.Ranger       => new CharacterStats(5,  6,  3,  4,  7,  4,  3,  9,  6),
                CharacterClass.Priest       => new CharacterStats(7,  3,  7,  3,  3,  7,  5, 15,  3),
                CharacterClass.Elementalist => new CharacterStats(3,  2,  9,  2,  4,  8,  1, 12,  5),
                CharacterClass.Warlock      => new CharacterStats(5,  3,  8,  3,  3,  7,  2, 18,  4),
                _ => new CharacterStats(5, 5, 5, 5, 5, 5, 3, 3, 5)
            };
        }

        private static List<AbilityData> GetStartingAbilities(CharacterClass characterClass)
        {
            // TODO: When progression system is built, restrict to unlocked abilities only.
            // For now, give all class abilities for testing.
            return characterClass switch
            {
                CharacterClass.Warrior      => AbilityCatalog.GetWarriorAbilities(),
                CharacterClass.Rogue        => AbilityCatalog.GetRogueAbilities(),
                CharacterClass.Ranger       => AbilityCatalog.GetRangerAbilities(),
                CharacterClass.Priest       => AbilityCatalog.GetPriestAbilities(),
                CharacterClass.Elementalist => AbilityCatalog.GetElementalistAbilities(),
                CharacterClass.Warlock      => AbilityCatalog.GetWarlockAbilities(),
                _ => new List<AbilityData>
                {
                    AbilityData.CreateAttack("Attack", "Basic attack.", basePower: 0, damageMultiplier: 1.0f)
                }
            };
        }

        private static List<AbilityData> GetClassPassives(CharacterClass characterClass)
        {
            // TODO: When progression system is built, restrict to unlocked passives only.
            return characterClass switch
            {
                CharacterClass.Warrior => AbilityCatalog.GetWarriorPassives(),
                CharacterClass.Rogue   => AbilityCatalog.GetRoguePassives(),
                CharacterClass.Warlock => AbilityCatalog.GetWarlockPassives(),
                _ => new List<AbilityData>()
            };
        }

        private static AbilityData GetBasicPassive(CharacterClass characterClass)
        {
            return characterClass switch
            {
                CharacterClass.Warrior => new AbilityData
                {
                    Name = "Tough",
                    Description = "Gains bonus HP from Endurance.",
                    IsPassive = true
                },
                CharacterClass.Rogue => new AbilityData
                {
                    Name = "Evasive",
                    Description = "Increased dodge chance.",
                    IsPassive = true
                },
                CharacterClass.Ranger => new AbilityData
                {
                    Name = "Keen Eye",
                    Description = "Increased accuracy and crit chance.",
                    IsPassive = true
                },
                CharacterClass.Priest => new AbilityData
                {
                    Name = "Faith",
                    Description = "Healing spells are more effective.",
                    IsPassive = true
                },
                CharacterClass.Elementalist => new AbilityData
                {
                    Name = "Arcane Affinity",
                    Description = "Spells cost less mana.",
                    IsPassive = true
                },
                CharacterClass.Warlock => new AbilityData
                {
                    Name = "Dark Pact",
                    Description = "Gains mana when taking damage.",
                    IsPassive = true
                },
                _ => new AbilityData { Name = "None", Description = "No passive.", IsPassive = true }
            };
        }
    }
}

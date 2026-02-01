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

            data.Abilities.Add(GetBasicAbility(characterClass));
            data.Passives.Add(GetBasicPassive(characterClass));
            data.Abilities.AddRange(GetGenericAbilities());

            return data;
        }

        private static CharacterStats GetBaseStats(CharacterClass characterClass)
        {
            return characterClass switch
            {
                //                          END STA INT STR DEX WIL ARM MRS INI
                CharacterClass.Warrior  => new CharacterStats(8,  5,  2,  7,  4,  3,  6,  2,  4),
                CharacterClass.Rogue    => new CharacterStats(4,  7,  3,  5,  8,  3,  3,  3,  7),
                CharacterClass.Ranger   => new CharacterStats(5,  6,  3,  4,  7,  4,  3,  3,  6),
                CharacterClass.Priest   => new CharacterStats(7,  3,  7,  3,  3,  7,  5,  5,  3),
                CharacterClass.Wizard   => new CharacterStats(3,  2,  9,  2,  4,  8,  1,  4,  5),
                CharacterClass.Warlock  => new CharacterStats(5,  3,  8,  3,  3,  7,  2,  6,  4),
                _ => new CharacterStats(5, 5, 5, 5, 5, 5, 3, 3, 5)
            };
        }

        private static AbilityData GetBasicAbility(CharacterClass characterClass)
        {
            return characterClass switch
            {
                CharacterClass.Warrior => AbilityData.CreateSkill(
                    "Crushing Blow",
                    "A powerful strike that deals bonus damage.",
                    basePower: 12, energyCost: 3),

                CharacterClass.Rogue => new AbilityData
                {
                    Name = "Quick Stab",
                    Description = "A fast attack that uses only a short action.",
                    Tab = AbilityTab.Attacks,
                    ActionCost = ActionPointType.Short,
                    ShortPointCost = 1,
                    DamageType = DamageType.Physical,
                    BasePower = 6,
                    HitCount = 1,
                    TargetType = TargetType.SingleEnemy
                },

                CharacterClass.Ranger => AbilityData.CreateSkill(
                    "Mark",
                    "Marks an enemy, boosting all attacks against it.",
                    basePower: 5, energyCost: 2),

                CharacterClass.Priest => new AbilityData
                {
                    Name = "Word of Protection",
                    Description = "Creates a small shield that absorbs damage for an ally.",
                    Tab = AbilityTab.Spells,
                    ActionCost = ActionPointType.Long,
                    LongPointCost = 1,
                    ManaCost = 4,
                    DamageType = DamageType.Magical,
                    Element = Element.Holy,
                    BasePower = 8,
                    TargetType = TargetType.SingleAlly
                },

                CharacterClass.Wizard => AbilityData.CreateSpell(
                    "Magic Bolt",
                    "A bolt of magic that takes the school of the last spell used.",
                    basePower: 10, manaCost: 4, element: Element.Arcane),

                CharacterClass.Warlock => new AbilityData
                {
                    Name = "Ritual",
                    Description = "Sacrifice HP to gain mana.",
                    Tab = AbilityTab.Spells,
                    ActionCost = ActionPointType.Long,
                    LongPointCost = 1,
                    HPCost = 5,
                    DamageType = DamageType.Magical,
                    Element = Element.Shadow,
                    TargetType = TargetType.Self
                },

                _ => AbilityData.CreateAttack("Attack", "Basic attack.", basePower: 5)
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
                CharacterClass.Wizard => new AbilityData
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

        private static List<AbilityData> GetGenericAbilities()
        {
            return new List<AbilityData>
            {
                AbilityData.CreateQuickAction("Swap Position", "Switch grid position with an ally."),
                new AbilityData
                {
                    Name = "Anticipate",
                    Description = "Act with priority next turn, but lose 1 short action.",
                    Tab = AbilityTab.Generic,
                    ActionCost = ActionPointType.Long,
                    LongPointCost = 1,
                    TargetType = TargetType.Self
                },
                new AbilityData
                {
                    Name = "Prepare",
                    Description = "Recover resources. Acts last next turn.",
                    Tab = AbilityTab.Generic,
                    ActionCost = ActionPointType.Long,
                    LongPointCost = 1,
                    TargetType = TargetType.Self
                },
                new AbilityData
                {
                    Name = "Protect",
                    Description = "Increase chance of being targeted, gain defense.",
                    Tab = AbilityTab.Generic,
                    ActionCost = ActionPointType.Long,
                    LongPointCost = 1,
                    TargetType = TargetType.Self
                },
                new AbilityData
                {
                    Name = "Hide",
                    Description = "Decrease chance of being targeted.",
                    Tab = AbilityTab.Generic,
                    ActionCost = ActionPointType.Long,
                    LongPointCost = 1,
                    TargetType = TargetType.Self
                }
            };
        }
    }
}

using System.Collections.Generic;
using UnityEngine;

namespace PixelWarriors
{
    public static class ConsumableCatalog
    {
        private static List<ConsumableData> _all;
        private static Dictionary<string, ConsumableData> _lookup;

        public static List<ConsumableData> GetAll()
        {
            if (_all == null) BuildCatalog();
            return _all;
        }

        public static ConsumableData Get(string id)
        {
            if (_lookup == null) BuildCatalog();
            return _lookup.TryGetValue(id, out ConsumableData data) ? data : null;
        }

        /// <summary>
        /// Creates an AbilityData for use in battle. For scrolls, clones the referenced class ability.
        /// For potions/bombs/utility, builds a fresh ability with act-scaled values.
        /// </summary>
        public static AbilityData GetBattleAbility(ConsumableData consumable, int act)
        {
            if (consumable.Category == ConsumableCategory.Scroll)
                return BuildScrollAbility(consumable);

            return consumable.Id switch
            {
                "health_potion" => BuildConsumableAbility(consumable, AbilityTag.ConsumableHeal,
                    TargetType.SingleAlly, ActScale(25, 40, 60, act)),
                "energy_potion" => BuildConsumableAbility(consumable, AbilityTag.ConsumableEnergyRestore,
                    TargetType.SingleAlly, ActScale(12, 20, 30, act)),
                "mana_potion" => BuildConsumableAbility(consumable, AbilityTag.ConsumableManaRestore,
                    TargetType.SingleAlly, ActScale(8, 15, 22, act)),
                "antidote" => BuildConsumableAbility(consumable, AbilityTag.ConsumableAntidote,
                    TargetType.SingleAlly, 0),
                "bandages" => BuildConsumableAbility(consumable, AbilityTag.ConsumableBandage,
                    TargetType.SingleAlly, ActScale(10, 16, 24, act)),
                "fire_bomb" => BuildBombAbility(consumable, Element.Fire,
                    ActScale(8, 12, 18, act)),
                "ice_bomb" => BuildBombAbility(consumable, Element.Water,
                    ActScale(6, 10, 15, act)),
                "poison_bomb" => BuildBombAbility(consumable, Element.None,
                    ActScale(4, 7, 10, act)),
                "smoke_bomb" => BuildConsumableAbility(consumable, AbilityTag.ConsumableSmokeBomb,
                    TargetType.AllAllies, 0),
                "throwing_knives" => BuildThrowingAbility(consumable,
                    ActScale(10, 16, 22, act)),
                _ => null
            };
        }

        /// <summary>
        /// Returns the AbilityData that a book teaches. Looks up from AbilityCatalog by class + name.
        /// </summary>
        public static AbilityData GetBookAbility(ConsumableData book)
        {
            if (book.BookAbilityClass == null || book.BookAbilityName == null) return null;
            List<AbilityData> classAbilities = GetClassAbilities(book.BookAbilityClass.Value);
            return classAbilities.Find(a => a.Name == book.BookAbilityName);
        }

        // --- Ability Builders ---

        private static AbilityData BuildConsumableAbility(ConsumableData data, AbilityTag tag,
            TargetType targetType, int basePower)
        {
            return new AbilityData
            {
                Name = data.Name,
                Description = data.Description,
                Tab = AbilityTab.Items,
                ActionCost = ActionPointType.Long,
                LongPointCost = 1,
                DamageType = DamageType.Magical,
                BasePower = basePower,
                TargetType = targetType,
                Tag = tag
            };
        }

        private static AbilityData BuildBombAbility(ConsumableData data, Element element, int basePower)
        {
            return new AbilityData
            {
                Name = data.Name,
                Description = data.Description,
                Tab = AbilityTab.Items,
                ActionCost = ActionPointType.Long,
                LongPointCost = 1,
                DamageType = DamageType.Magical,
                Element = element,
                BasePower = basePower,
                TargetType = TargetType.AllEnemies,
                Tag = AbilityTag.None
            };
        }

        private static AbilityData BuildThrowingAbility(ConsumableData data, int basePower)
        {
            return new AbilityData
            {
                Name = data.Name,
                Description = data.Description,
                Tab = AbilityTab.Items,
                ActionCost = ActionPointType.Long,
                LongPointCost = 1,
                DamageType = DamageType.Physical,
                BasePower = basePower,
                TargetType = TargetType.SingleEnemy,
                Tag = AbilityTag.None
            };
        }

        private static AbilityData BuildScrollAbility(ConsumableData scroll)
        {
            if (scroll.ScrollAbilityClass == null || scroll.ScrollAbilityName == null) return null;
            List<AbilityData> classAbilities = GetClassAbilities(scroll.ScrollAbilityClass.Value);
            AbilityData source = classAbilities.Find(a => a.Name == scroll.ScrollAbilityName);
            if (source == null) return null;

            // Clone the ability but override tab and remove class-specific costs
            return new AbilityData
            {
                Name = scroll.Name,
                Description = $"Scroll: {source.Description}",
                Tab = AbilityTab.Items,
                ActionCost = ActionPointType.Long,
                LongPointCost = 1,
                // No energy/mana cost — the scroll IS the cost
                DamageType = source.DamageType,
                Element = source.Element,
                BasePower = source.BasePower,
                HitCount = source.HitCount,
                TargetType = source.TargetType,
                Tag = source.Tag,
                DamageMultiplier = source.DamageMultiplier,
                Range = AbilityRange.Any, // Scrolls ignore range restrictions
                HitChanceModifier = source.HitChanceModifier
            };
        }

        private static List<AbilityData> GetClassAbilities(CharacterClass characterClass)
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

        private static int ActScale(int act1, int act2, int act3, int act)
        {
            return act switch
            {
                1 => act1,
                2 => act2,
                _ => act3
            };
        }

        // --- Catalog Definition ---

        private static void BuildCatalog()
        {
            _all = new List<ConsumableData>
            {
                // === Potions ===
                new ConsumableData
                {
                    Id = "health_potion", Name = "Health Potion",
                    Description = "Restore HP to an ally.",
                    Category = ConsumableCategory.Potion, BuyPrice = 10, MinAct = 1,
                    UsableInBattle = true
                },
                new ConsumableData
                {
                    Id = "energy_potion", Name = "Energy Potion",
                    Description = "Restore Energy to an ally.",
                    Category = ConsumableCategory.Potion, BuyPrice = 10, MinAct = 1,
                    UsableInBattle = true
                },
                new ConsumableData
                {
                    Id = "mana_potion", Name = "Mana Potion",
                    Description = "Restore Mana to an ally.",
                    Category = ConsumableCategory.Potion, BuyPrice = 12, MinAct = 1,
                    UsableInBattle = true
                },

                // === Curative ===
                new ConsumableData
                {
                    Id = "antidote", Name = "Antidote",
                    Description = "Cleanse all negative effects from an ally.",
                    Category = ConsumableCategory.Potion, BuyPrice = 12, MinAct = 1,
                    UsableInBattle = true
                },
                new ConsumableData
                {
                    Id = "bandages", Name = "Bandages",
                    Description = "Small heal and remove Bleed.",
                    Category = ConsumableCategory.Potion, BuyPrice = 8, MinAct = 1,
                    UsableInBattle = true
                },

                // === Bombs ===
                new ConsumableData
                {
                    Id = "fire_bomb", Name = "Fire Bomb",
                    Description = "Deal fire damage to all enemies.",
                    Category = ConsumableCategory.Bomb, BuyPrice = 15, MinAct = 1,
                    UsableInBattle = true
                },
                new ConsumableData
                {
                    Id = "ice_bomb", Name = "Ice Bomb",
                    Description = "Deal water damage and Chill all enemies.",
                    Category = ConsumableCategory.Bomb, BuyPrice = 18, MinAct = 1,
                    UsableInBattle = true
                },
                new ConsumableData
                {
                    Id = "poison_bomb", Name = "Poison Bomb",
                    Description = "Poison all enemies for 3 turns.",
                    Category = ConsumableCategory.Bomb, BuyPrice = 15, MinAct = 1,
                    UsableInBattle = true
                },

                // === Utility ===
                new ConsumableData
                {
                    Id = "smoke_bomb", Name = "Smoke Bomb",
                    Description = "All allies gain Hide for 1 turn.",
                    Category = ConsumableCategory.Utility, BuyPrice = 20, MinAct = 1,
                    UsableInBattle = true
                },
                new ConsumableData
                {
                    Id = "throwing_knives", Name = "Throwing Knives",
                    Description = "Deal physical damage to one enemy.",
                    Category = ConsumableCategory.Utility, BuyPrice = 10, MinAct = 1,
                    UsableInBattle = true
                },

                // === Scrolls ===
                new ConsumableData
                {
                    Id = "scroll_healing", Name = "Scroll of Healing",
                    Description = "Casts Prayer of Mending on an ally.",
                    Category = ConsumableCategory.Scroll, BuyPrice = 25, MinAct = 1,
                    UsableInBattle = true,
                    ScrollAbilityClass = CharacterClass.Priest,
                    ScrollAbilityName = "Prayer of Mending"
                },
                new ConsumableData
                {
                    Id = "scroll_shield", Name = "Scroll of Shield",
                    Description = "Casts Word of Protection on an ally.",
                    Category = ConsumableCategory.Scroll, BuyPrice = 22, MinAct = 1,
                    UsableInBattle = true,
                    ScrollAbilityClass = CharacterClass.Priest,
                    ScrollAbilityName = "Word of Protection"
                },
                new ConsumableData
                {
                    Id = "scroll_lightning", Name = "Scroll of Lightning",
                    Description = "Casts Chain Lightning on an enemy.",
                    Category = ConsumableCategory.Scroll, BuyPrice = 30, MinAct = 2,
                    UsableInBattle = true,
                    ScrollAbilityClass = CharacterClass.Elementalist,
                    ScrollAbilityName = "Chain Lightning"
                },
                new ConsumableData
                {
                    Id = "scroll_purify", Name = "Scroll of Purify",
                    Description = "Cleanse all negative effects from an ally.",
                    Category = ConsumableCategory.Scroll, BuyPrice = 20, MinAct = 1,
                    UsableInBattle = true,
                    ScrollAbilityClass = CharacterClass.Priest,
                    ScrollAbilityName = "Purify"
                },
                new ConsumableData
                {
                    Id = "scroll_terror", Name = "Scroll of Terror",
                    Description = "Casts Terror on an enemy.",
                    Category = ConsumableCategory.Scroll, BuyPrice = 25, MinAct = 2,
                    UsableInBattle = true,
                    ScrollAbilityClass = CharacterClass.Warlock,
                    ScrollAbilityName = "Terror"
                },
                new ConsumableData
                {
                    Id = "scroll_mark", Name = "Scroll of Mark",
                    Description = "Casts Mark on an enemy.",
                    Category = ConsumableCategory.Scroll, BuyPrice = 20, MinAct = 1,
                    UsableInBattle = true,
                    ScrollAbilityClass = CharacterClass.Ranger,
                    ScrollAbilityName = "Mark"
                },

                // === Books ===
                new ConsumableData
                {
                    Id = "book_cleave", Name = "Book of Cleave",
                    Description = "Teaches Cleave (Warrior skill).",
                    Category = ConsumableCategory.Book, BuyPrice = 70, MinAct = 1,
                    UsableOutOfBattle = true,
                    BookAbilityClass = CharacterClass.Warrior,
                    BookAbilityName = "Cleave"
                },
                new ConsumableData
                {
                    Id = "book_envenom", Name = "Book of Envenom",
                    Description = "Teaches Envenom (Rogue skill).",
                    Category = ConsumableCategory.Book, BuyPrice = 75, MinAct = 2,
                    UsableOutOfBattle = true,
                    BookAbilityClass = CharacterClass.Rogue,
                    BookAbilityName = "Envenom"
                },
                new ConsumableData
                {
                    Id = "book_ignite", Name = "Book of Ignite",
                    Description = "Teaches Ignite (Elementalist spell).",
                    Category = ConsumableCategory.Book, BuyPrice = 65, MinAct = 1,
                    UsableOutOfBattle = true,
                    BookAbilityClass = CharacterClass.Elementalist,
                    BookAbilityName = "Ignite"
                },
                new ConsumableData
                {
                    Id = "book_smite", Name = "Book of Smite",
                    Description = "Teaches Smite (Priest spell).",
                    Category = ConsumableCategory.Book, BuyPrice = 65, MinAct = 1,
                    UsableOutOfBattle = true,
                    BookAbilityClass = CharacterClass.Priest,
                    BookAbilityName = "Smite"
                },
                new ConsumableData
                {
                    Id = "book_snipe", Name = "Book of Snipe",
                    Description = "Teaches Snipe (Ranger skill).",
                    Category = ConsumableCategory.Book, BuyPrice = 80, MinAct = 2,
                    UsableOutOfBattle = true,
                    BookAbilityClass = CharacterClass.Ranger,
                    BookAbilityName = "Snipe"
                },
                new ConsumableData
                {
                    Id = "book_terror", Name = "Book of Terror",
                    Description = "Teaches Terror (Warlock spell).",
                    Category = ConsumableCategory.Book, BuyPrice = 75, MinAct = 2,
                    UsableOutOfBattle = true,
                    BookAbilityClass = CharacterClass.Warlock,
                    BookAbilityName = "Terror"
                }
            };

            _lookup = new Dictionary<string, ConsumableData>();
            foreach (ConsumableData data in _all)
                _lookup[data.Id] = data;
        }
    }
}

using System.Collections.Generic;

namespace PixelWarriors
{
    public class ItemTemplate
    {
        public string BaseName;
        public EquipmentSlot Slot;
        public WeaponType WeaponType;
        public ItemProfile[] AllowedProfiles;
        public int MinAct;
        public int MaxAct;
    }

    public static class ItemTemplateCatalog
    {
        private static List<ItemTemplate> _templates;
        private static List<EquipmentData> _uniques;

        public static List<ItemTemplate> GetTemplates()
        {
            if (_templates != null) return _templates;

            _templates = new List<ItemTemplate>
            {
                // === Hand1: Weapons ===
                new ItemTemplate
                {
                    BaseName = "Sword", Slot = EquipmentSlot.Hand1, WeaponType = WeaponType.Sword,
                    AllowedProfiles = new[] { ItemProfile.Balanced, ItemProfile.Mighty, ItemProfile.Tank },
                    MinAct = 1, MaxAct = 3
                },
                new ItemTemplate
                {
                    BaseName = "Dagger", Slot = EquipmentSlot.Hand1, WeaponType = WeaponType.Dagger,
                    AllowedProfiles = new[] { ItemProfile.Agile, ItemProfile.Cunning, ItemProfile.Swift },
                    MinAct = 1, MaxAct = 3
                },
                new ItemTemplate
                {
                    BaseName = "Bow", Slot = EquipmentSlot.Hand1, WeaponType = WeaponType.Bow,
                    AllowedProfiles = new[] { ItemProfile.Agile, ItemProfile.Cunning, ItemProfile.Balanced },
                    MinAct = 1, MaxAct = 3
                },
                new ItemTemplate
                {
                    BaseName = "Staff", Slot = EquipmentSlot.Hand1, WeaponType = WeaponType.Staff,
                    AllowedProfiles = new[] { ItemProfile.Arcane, ItemProfile.Resilient, ItemProfile.Balanced },
                    MinAct = 1, MaxAct = 3
                },
                new ItemTemplate
                {
                    BaseName = "Greatsword", Slot = EquipmentSlot.Hand1, WeaponType = WeaponType.TwoHanded,
                    AllowedProfiles = new[] { ItemProfile.Mighty, ItemProfile.Balanced },
                    MinAct = 1, MaxAct = 3
                },
                new ItemTemplate
                {
                    BaseName = "Mace", Slot = EquipmentSlot.Hand1, WeaponType = WeaponType.Mace,
                    AllowedProfiles = new[] { ItemProfile.Mighty, ItemProfile.Tank, ItemProfile.Resilient },
                    MinAct = 1, MaxAct = 3
                },

                // === Offhand ===
                new ItemTemplate
                {
                    BaseName = "Shield", Slot = EquipmentSlot.Offhand, WeaponType = WeaponType.Shield,
                    AllowedProfiles = new[] { ItemProfile.Tank, ItemProfile.Balanced, ItemProfile.Resilient },
                    MinAct = 1, MaxAct = 3
                },
                new ItemTemplate
                {
                    BaseName = "Tome", Slot = EquipmentSlot.Offhand, WeaponType = WeaponType.None,
                    AllowedProfiles = new[] { ItemProfile.Arcane, ItemProfile.Resilient },
                    MinAct = 1, MaxAct = 3
                },
                new ItemTemplate
                {
                    BaseName = "Parrying Dagger", Slot = EquipmentSlot.Offhand, WeaponType = WeaponType.Dagger,
                    AllowedProfiles = new[] { ItemProfile.Agile, ItemProfile.Cunning },
                    MinAct = 1, MaxAct = 3
                },

                // === Head ===
                new ItemTemplate
                {
                    BaseName = "Helm", Slot = EquipmentSlot.Head, WeaponType = WeaponType.None,
                    AllowedProfiles = new[] { ItemProfile.Tank, ItemProfile.Mighty, ItemProfile.Balanced },
                    MinAct = 1, MaxAct = 3
                },
                new ItemTemplate
                {
                    BaseName = "Hood", Slot = EquipmentSlot.Head, WeaponType = WeaponType.None,
                    AllowedProfiles = new[] { ItemProfile.Agile, ItemProfile.Cunning, ItemProfile.Swift },
                    MinAct = 1, MaxAct = 3
                },
                new ItemTemplate
                {
                    BaseName = "Circlet", Slot = EquipmentSlot.Head, WeaponType = WeaponType.None,
                    AllowedProfiles = new[] { ItemProfile.Arcane, ItemProfile.Resilient },
                    MinAct = 1, MaxAct = 3
                },

                // === Body ===
                new ItemTemplate
                {
                    BaseName = "Plate Armor", Slot = EquipmentSlot.Body, WeaponType = WeaponType.None,
                    AllowedProfiles = new[] { ItemProfile.Tank, ItemProfile.Mighty },
                    MinAct = 1, MaxAct = 3
                },
                new ItemTemplate
                {
                    BaseName = "Leather Armor", Slot = EquipmentSlot.Body, WeaponType = WeaponType.None,
                    AllowedProfiles = new[] { ItemProfile.Agile, ItemProfile.Balanced, ItemProfile.Cunning },
                    MinAct = 1, MaxAct = 3
                },
                new ItemTemplate
                {
                    BaseName = "Robes", Slot = EquipmentSlot.Body, WeaponType = WeaponType.None,
                    AllowedProfiles = new[] { ItemProfile.Arcane, ItemProfile.Resilient },
                    MinAct = 1, MaxAct = 3
                },

                // === Trinkets ===
                new ItemTemplate
                {
                    BaseName = "Ring", Slot = EquipmentSlot.Trinket1, WeaponType = WeaponType.None,
                    AllowedProfiles = new[] { ItemProfile.Balanced, ItemProfile.Agile, ItemProfile.Arcane, ItemProfile.Swift },
                    MinAct = 1, MaxAct = 3
                },
                new ItemTemplate
                {
                    BaseName = "Amulet", Slot = EquipmentSlot.Trinket1, WeaponType = WeaponType.None,
                    AllowedProfiles = new[] { ItemProfile.Resilient, ItemProfile.Tank, ItemProfile.Mighty, ItemProfile.Balanced },
                    MinAct = 1, MaxAct = 3
                },
                new ItemTemplate
                {
                    BaseName = "Charm", Slot = EquipmentSlot.Trinket1, WeaponType = WeaponType.None,
                    AllowedProfiles = new[] { ItemProfile.Arcane, ItemProfile.Cunning, ItemProfile.Swift },
                    MinAct = 1, MaxAct = 3
                }
            };

            return _templates;
        }

        public static List<EquipmentData> GetUniqueItems()
        {
            if (_uniques != null) return _uniques;

            _uniques = new List<EquipmentData>
            {
                // --- Weapons ---
                new EquipmentData
                {
                    Name = "Widow's Fang", Description = "Dagger. 5 dmg, +2 DEX, +1 INI.",
                    Slot = EquipmentSlot.Hand1, WeaponType = WeaponType.Dagger,
                    BaseDamage = 5, StatModifiers = new CharacterStats(0, 0, 0, 0, 2, 0, 0, 0, 1),
                    IsUnique = true, FlavorText = "Taken from a spider's mandible.", ActLevel = 1
                },
                new EquipmentData
                {
                    Name = "Executioner's Greatsword", Description = "Greatsword. 10 dmg, +3 STR, -2 INI.",
                    Slot = EquipmentSlot.Hand1, WeaponType = WeaponType.TwoHanded,
                    BaseDamage = 10, StatModifiers = new CharacterStats(0, 0, 0, 3, 0, 0, 0, 0, -2),
                    IsUnique = true, FlavorText = "Slow, but certain.", ActLevel = 2
                },
                new EquipmentData
                {
                    Name = "Serpent Staff", Description = "Staff. 4 dmg, +2 WIL, +1 INT.",
                    Slot = EquipmentSlot.Hand1, WeaponType = WeaponType.Staff,
                    BaseDamage = 4, StatModifiers = new CharacterStats(0, 0, 1, 0, 0, 2, 0, 0, 0),
                    IsUnique = true, FlavorText = "Carved from petrified snakewood.", ActLevel = 1
                },
                new EquipmentData
                {
                    Name = "Nightwhisper Bow", Description = "Bow. 6 dmg, +3 DEX, -1 STR.",
                    Slot = EquipmentSlot.Hand1, WeaponType = WeaponType.Bow,
                    BaseDamage = 6, StatModifiers = new CharacterStats(0, 0, 0, -1, 3, 0, 0, 0, 0),
                    IsUnique = true, FlavorText = "Its string hums with silence.", ActLevel = 1
                },
                new EquipmentData
                {
                    Name = "Martyr's Mace", Description = "Mace. 5 dmg, +2 WIL, +2 END.",
                    Slot = EquipmentSlot.Hand1, WeaponType = WeaponType.Mace,
                    BaseDamage = 5, StatModifiers = new CharacterStats(2, 0, 0, 0, 0, 2, 0, 0, 0),
                    IsUnique = true, FlavorText = "Blessed by suffering.", ActLevel = 1
                },

                // --- Offhand ---
                new EquipmentData
                {
                    Name = "Ironbark Shield", Description = "Shield. 12% block, +3 END, +1 ARM.",
                    Slot = EquipmentSlot.Offhand, WeaponType = WeaponType.Shield,
                    BaseBlockChance = 0.12f, StatModifiers = new CharacterStats(3, 0, 0, 0, 0, 0, 1, 0, 0),
                    IsUnique = true, FlavorText = "The bark of the ancient oak endures.", ActLevel = 1
                },

                // --- Head ---
                new EquipmentData
                {
                    Name = "Crown of Clarity", Description = "Circlet. +4 WIL, +2 INT, +1 INI.",
                    Slot = EquipmentSlot.Head, WeaponType = WeaponType.None,
                    StatModifiers = new CharacterStats(0, 0, 2, 0, 0, 4, 0, 0, 1),
                    IsUnique = true, FlavorText = "Silence the noise. See the truth.", ActLevel = 2
                },
                new EquipmentData
                {
                    Name = "Stonewall Helm", Description = "Helm. +4 ARM, +2 END, -1 INI.",
                    Slot = EquipmentSlot.Head, WeaponType = WeaponType.None,
                    StatModifiers = new CharacterStats(2, 0, 0, 0, 0, 0, 4, 0, -1),
                    IsUnique = true, FlavorText = "Immovable, like the mountain.", ActLevel = 2
                },
                new EquipmentData
                {
                    Name = "Mindfire Circlet", Description = "Circlet. +3 INT, +2 WIL, +1 STA.",
                    Slot = EquipmentSlot.Head, WeaponType = WeaponType.None,
                    StatModifiers = new CharacterStats(0, 1, 3, 0, 0, 2, 0, 0, 0),
                    IsUnique = true, FlavorText = "Thoughts burn brighter than flame.", ActLevel = 1
                },

                // --- Body ---
                new EquipmentData
                {
                    Name = "Bloodrite Robes", Description = "Robes. +3 INT, +2 WIL, -1 END, +3 MRS.",
                    Slot = EquipmentSlot.Body, WeaponType = WeaponType.None,
                    StatModifiers = new CharacterStats(-1, 0, 3, 0, 0, 2, 0, 3, 0),
                    IsUnique = true, FlavorText = "Pain is the offering.", ActLevel = 2
                },
                new EquipmentData
                {
                    Name = "Phantom Cloak", Description = "Leather. +3 DEX, +2 INI, -1 ARM.",
                    Slot = EquipmentSlot.Body, WeaponType = WeaponType.None,
                    StatModifiers = new CharacterStats(0, 0, 0, 0, 3, 0, -1, 0, 2),
                    IsUnique = true, FlavorText = "You see it from the corner of your eye.", ActLevel = 1
                },

                // --- Trinkets ---
                new EquipmentData
                {
                    Name = "Vagrant's Ring", Description = "Ring. +1 to all primary stats.",
                    Slot = EquipmentSlot.Trinket1, WeaponType = WeaponType.None,
                    StatModifiers = new CharacterStats(1, 1, 1, 1, 1, 1, 0, 0, 0),
                    IsUnique = true, FlavorText = "Worn smooth by countless fingers.", ActLevel = 1
                },
                new EquipmentData
                {
                    Name = "Chains of Binding", Description = "Trinket. +3 ARM, +2 MRS, -2 INI.",
                    Slot = EquipmentSlot.Trinket1, WeaponType = WeaponType.None,
                    StatModifiers = new CharacterStats(0, 0, 0, 0, 0, 0, 3, 2, -2),
                    IsUnique = true, FlavorText = "They hold you together.", ActLevel = 2
                },
                new EquipmentData
                {
                    Name = "Featherfoot Ring", Description = "Ring. +4 INI, +2 DEX.",
                    Slot = EquipmentSlot.Trinket1, WeaponType = WeaponType.None,
                    StatModifiers = new CharacterStats(0, 0, 0, 0, 2, 0, 0, 0, 4),
                    IsUnique = true, FlavorText = "Light as a whisper.", ActLevel = 1
                },
                new EquipmentData
                {
                    Name = "Lifeweave Amulet", Description = "Amulet. +3 END, +2 STA, +1 WIL.",
                    Slot = EquipmentSlot.Trinket1, WeaponType = WeaponType.None,
                    StatModifiers = new CharacterStats(3, 2, 0, 0, 0, 1, 0, 0, 0),
                    IsUnique = true, FlavorText = "Threads of life, woven tight.", ActLevel = 1
                }
            };

            return _uniques;
        }
    }
}

using System;

namespace PixelWarriors
{
    [Serializable]
    public class EquipmentData
    {
        public string Name;
        public string Description;
        public EquipmentSlot Slot;
        public CharacterStats StatModifiers;
        public WeaponType WeaponType;
        public int BaseDamage;
        public float BaseBlockChance;
        public float ArmorPenetration;
        public int MagicPenetration;

        // Loot system fields
        public bool IsUnique;
        public string FlavorText;
        public int ActLevel;

        // Smithy fields
        public int ImbuedGemInt = -1; // -1 = none, otherwise cast to GemType
        public int UpgradeLevel;

        public GemType? ImbuedGem
        {
            get => ImbuedGemInt >= 0 ? (GemType)ImbuedGemInt : null;
            set => ImbuedGemInt = value.HasValue ? (int)value.Value : -1;
        }

        public EquipmentData()
        {
            StatModifiers = new CharacterStats();
            WeaponType = WeaponType.None;
        }

        public static AbilityRange GetRangeForWeapon(WeaponType weaponType)
        {
            return weaponType switch
            {
                WeaponType.Bow   => AbilityRange.Reach,
                WeaponType.Staff => AbilityRange.Reach,
                _                => AbilityRange.Close
            };
        }
    }
}

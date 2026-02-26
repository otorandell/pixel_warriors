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

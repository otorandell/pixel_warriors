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

        public EquipmentData()
        {
            StatModifiers = new CharacterStats();
        }
    }
}

using System;
using System.Collections.Generic;

namespace PixelWarriors
{
    [Serializable]
    public class CharacterData
    {
        public string Name;
        public CharacterClass Class;
        public int Level;
        public int CurrentXP;
        public CharacterStats BaseStats;
        public List<AbilityData> Abilities;
        public List<AbilityData> Passives;
        public EquipmentData[] Equipment;

        public CharacterData()
        {
            Level = 1;
            CurrentXP = 0;
            Abilities = new List<AbilityData>();
            Passives = new List<AbilityData>();
            Equipment = new EquipmentData[7]; // 7 slots
        }

        public CharacterStats GetTotalStats()
        {
            CharacterStats total = BaseStats.Clone();

            for (int i = 0; i < Equipment.Length; i++)
            {
                if (Equipment[i] != null)
                {
                    total.Add(Equipment[i].StatModifiers);
                }
            }

            return total;
        }
    }
}

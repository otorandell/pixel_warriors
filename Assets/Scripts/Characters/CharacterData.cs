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
        public CharacterStats GrowthModifiers;
        public List<AbilityData> Abilities;
        public List<AbilityData> Passives;
        public EquipmentData[] Equipment;

        // Persistent resources (carry between battles)
        public int CurrentHP;
        public int CurrentEnergy;
        public int CurrentMana;
        public bool ResourcesInitialized;

        public CharacterData()
        {
            Level = 1;
            CurrentXP = 0;
            Abilities = new List<AbilityData>();
            Passives = new List<AbilityData>();
            Equipment = new EquipmentData[6];
            GrowthModifiers = new CharacterStats();
        }

        public void InitializeResources()
        {
            CharacterStats total = GetTotalStats();
            CurrentHP = StatCalculator.CalculateMaxHP(total);
            CurrentEnergy = StatCalculator.CalculateMaxEnergy(total);
            CurrentMana = StatCalculator.CalculateMaxMana(total);
            ResourcesInitialized = true;
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

        public bool HasWeaponType(WeaponType type)
        {
            for (int i = 0; i < Equipment.Length; i++)
            {
                if (Equipment[i] != null && Equipment[i].WeaponType == type)
                    return true;
            }
            return false;
        }

        public float GetBaseBlockChance()
        {
            float total = 0f;
            for (int i = 0; i < Equipment.Length; i++)
            {
                if (Equipment[i] != null)
                    total += Equipment[i].BaseBlockChance;
            }
            return total;
        }

        public int GetWeaponDamage()
        {
            int total = 0;
            for (int i = 0; i < Equipment.Length; i++)
            {
                if (Equipment[i] != null)
                    total += Equipment[i].BaseDamage;
            }
            return total;
        }

        public float GetArmorPenetration()
        {
            float total = 0f;
            for (int i = 0; i < Equipment.Length; i++)
            {
                if (Equipment[i] != null)
                    total += Equipment[i].ArmorPenetration;
            }
            return Math.Min(total, 1f);
        }

        public int GetMagicPenetration()
        {
            int total = 0;
            for (int i = 0; i < Equipment.Length; i++)
            {
                if (Equipment[i] != null)
                    total += Equipment[i].MagicPenetration;
            }
            return total;
        }
    }
}

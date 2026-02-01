using System;

namespace PixelWarriors
{
    [Serializable]
    public class CharacterStats
    {
        public int Endurance;
        public int Stamina;
        public int Intellect;
        public int Strength;
        public int Dexterity;
        public int Willpower;
        public int Armor;
        public int MagicResist;
        public int Initiative;

        public CharacterStats() { }

        public CharacterStats(int endurance, int stamina, int intellect, int strength,
            int dexterity, int willpower, int armor, int magicResist, int initiative)
        {
            Endurance = endurance;
            Stamina = stamina;
            Intellect = intellect;
            Strength = strength;
            Dexterity = dexterity;
            Willpower = willpower;
            Armor = armor;
            MagicResist = magicResist;
            Initiative = initiative;
        }

        public CharacterStats Clone()
        {
            return new CharacterStats(Endurance, Stamina, Intellect, Strength,
                Dexterity, Willpower, Armor, MagicResist, Initiative);
        }

        public void Add(CharacterStats other)
        {
            Endurance += other.Endurance;
            Stamina += other.Stamina;
            Intellect += other.Intellect;
            Strength += other.Strength;
            Dexterity += other.Dexterity;
            Willpower += other.Willpower;
            Armor += other.Armor;
            MagicResist += other.MagicResist;
            Initiative += other.Initiative;
        }
    }
}

using UnityEngine;

namespace PixelWarriors
{
    public static class StatCalculator
    {
        public static int CalculateMaxHP(CharacterStats stats)
        {
            return GameplayConfig.BaseHP + Mathf.RoundToInt(stats.Endurance * GameplayConfig.HPPerEndurance);
        }

        public static int CalculateMaxEnergy(CharacterStats stats)
        {
            return GameplayConfig.BaseEnergy + Mathf.RoundToInt(stats.Stamina * GameplayConfig.EnergyPerStamina);
        }

        public static int CalculateMaxMana(CharacterStats stats)
        {
            return GameplayConfig.BaseMana + Mathf.RoundToInt(stats.Intellect * GameplayConfig.ManaPerIntellect);
        }

        public static int CalculatePhysicalDamage(int baseDamage, int strength, int targetArmor)
        {
            float raw = baseDamage + strength * GameplayConfig.StrengthDamageMultiplier;
            int afterArmor = Mathf.RoundToInt(raw) - targetArmor;
            return Mathf.Max(afterArmor, GameplayConfig.MinDamageAfterArmor);
        }

        public static int CalculateMagicalDamage(int baseDamage, int targetMagicResist)
        {
            float reduction = targetMagicResist / 100f;
            float afterResist = baseDamage * (1f - reduction);
            return Mathf.Max(Mathf.RoundToInt(afterResist), GameplayConfig.MinDamageAfterArmor);
        }

        public static float CalculateAccuracy(int dexterity)
        {
            return GameplayConfig.BaseAccuracy + dexterity * GameplayConfig.AccuracyPerDexterity;
        }

        public static float CalculateCritChance(int dexterity)
        {
            return GameplayConfig.BaseCritChance + dexterity * GameplayConfig.CritPerDexterity;
        }

        public static float CalculateDodgeChance(int dexterity)
        {
            return GameplayConfig.BaseDodgeChance + dexterity * GameplayConfig.DodgePerDexterity;
        }

        public static float CalculateEffectChance(int willpower)
        {
            return GameplayConfig.BaseEffectChance + willpower * GameplayConfig.EffectChancePerWillpower;
        }

        public static int CalculateXPToLevel(int currentLevel)
        {
            return Mathf.RoundToInt(GameplayConfig.BaseXPToLevel * Mathf.Pow(GameplayConfig.XPScalingFactor, currentLevel - 1));
        }

        public static int CalculateEnergyRegen(int maxEnergy)
        {
            return Mathf.Max(1, Mathf.RoundToInt(maxEnergy * GameplayConfig.EnergyRegenPerTurn));
        }

        public static int CalculateManaRegen(int maxMana)
        {
            return Mathf.Max(1, Mathf.RoundToInt(maxMana * GameplayConfig.ManaRegenPerTurn));
        }
    }
}

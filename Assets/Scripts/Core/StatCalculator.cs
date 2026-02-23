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

        public static int CalculateWeaponDamage(int weaponDmg, int strength, float multiplier,
            int targetArmor, float armorPen)
        {
            float raw = (weaponDmg + strength * GameplayConfig.WeaponStrengthScaling) * multiplier;
            float effectiveArmor = targetArmor * (1f - Mathf.Clamp01(armorPen));
            int afterArmor = Mathf.RoundToInt(raw - effectiveArmor);
            return Mathf.Max(afterArmor, GameplayConfig.MinDamageAfterArmor);
        }

        public static int CalculatePhysicalSpellDamage(int basePower, int willpower,
            int targetArmor, float armorPen)
        {
            float raw = basePower + willpower * GameplayConfig.SpellWillpowerScaling;
            float effectiveArmor = targetArmor * (1f - Mathf.Clamp01(armorPen));
            int afterArmor = Mathf.RoundToInt(raw - effectiveArmor);
            return Mathf.Max(afterArmor, GameplayConfig.MinDamageAfterArmor);
        }

        public static int CalculateSpellDamage(int basePower, int willpower,
            int targetMR, int magicPen)
        {
            float raw = basePower + willpower * GameplayConfig.SpellWillpowerScaling;
            int effectiveMR = Mathf.Max(0, targetMR - magicPen);
            float afterResist = raw * (1f - effectiveMR / 100f);
            return Mathf.Max(Mathf.RoundToInt(afterResist), GameplayConfig.MinDamageAfterArmor);
        }

        public static float CalculateHitChance(int attackerDex, int targetDex)
        {
            return GameplayConfig.BaseHitChance
                + (attackerDex - targetDex) * GameplayConfig.DexterityHitScaling;
        }

        public static float CalculateCritChance(int dexterity)
        {
            return GameplayConfig.BaseCritChance + dexterity * GameplayConfig.CritPerDexterity;
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

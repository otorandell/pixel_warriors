namespace PixelWarriors
{
    public static class GameplayConfig
    {
        // --- Action Points ---
        public const int LongActionPoints = 1;
        public const int ShortActionPoints = 1;

        // --- Targeting / Aggro ---
        public const float FrontlineBaseAggro = 0.30f;
        public const float BacklineBaseAggro = 0.20f;
        public const float RangedFixedAggro = 0.25f;

        // --- Stat Formulas ---
        public const int BaseHP = 20;
        public const float HPPerEndurance = 5f;

        public const int BaseEnergy = 10;
        public const float EnergyPerStamina = 3f;

        public const int BaseMana = 8;
        public const float ManaPerIntellect = 4f;

        // --- Damage ---
        public const float StrengthDamageMultiplier = 1.5f;
        public const int MinDamageAfterArmor = 1;

        // --- Crit ---
        public const float BaseCritChance = 0.05f;
        public const float CritPerDexterity = 0.01f;
        public const float CritDamageMultiplier = 1.5f;

        // --- Dodge ---
        public const float BaseDodgeChance = 0.03f;
        public const float DodgePerDexterity = 0.005f;

        // --- Accuracy ---
        public const float BaseAccuracy = 0.90f;
        public const float AccuracyPerDexterity = 0.01f;

        // --- Willpower ---
        public const float BaseEffectChance = 0.10f;
        public const float EffectChancePerWillpower = 0.02f;

        // --- XP / Leveling ---
        public const int BaseXPToLevel = 100;
        public const float XPScalingFactor = 1.25f;

        // --- Status Effects ---
        public const float MarkDamageBonus = 0.10f;
        public const int MarkDuration = 2;
        public const float ProtectAggroMultiplier = 2.0f;
        public const float HideAggroMultiplier = 0.25f;
        public const float RitualManaPerHP = 2.0f;

        // --- Battle Timing ---
        public const float BattleStartDelay = 0.5f;
        public const float TurnStartDelay = 0.3f;
        public const float PostActionDelay = 0.3f;
        public const float EnemyThinkDelay = 0.5f;

        // --- Energy / Mana Regen ---
        public const float EnergyRegenPerTurn = 0.20f;  // 20% of max
        public const float ManaRegenPerTurn = 0.05f;     // 5% of max
        public const float PrepareRegenBonus = 0.10f;    // Extra regen from Prepare
    }
}

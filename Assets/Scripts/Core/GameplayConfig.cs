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
        public const float WeaponStrengthScaling = 1.0f;
        public const float SpellWillpowerScaling = 1.0f;
        public const int MinDamageAfterArmor = 1;

        // --- Hit ---
        public const float BaseHitChance = 0.75f;
        public const float DexterityHitScaling = 0.03f;

        // --- Crit ---
        public const float BaseCritChance = 0.03f;
        public const float CritPerDexterity = 0.015f;
        public const float CritDamageMultiplier = 1.5f;

        // --- Willpower ---
        public const float BaseEffectChance = 0.10f;
        public const float EffectChancePerWillpower = 0.02f;

        // --- XP / Leveling ---
        public const int BaseXPToLevel = 100;
        public const float XPScalingFactor = 1.25f;

        // --- Status Effects: Aggro ---
        public const float TauntAggroMultiplier = 2.0f;
        public const float HideAggroMultiplier = 0.25f;
        public const float ConcealAggroMultiplier = 0f;
        public const int TauntDuration = 2;

        // --- Status Effects: Mark ---
        public const float MarkDamageBonus = 0.10f;
        public const int MarkDuration = 2;

        // --- Status Effects: DoTs ---
        public const int BleedDuration = 3;
        public const int BleedDamagePerStack = 3;
        public const int PoisonDuration = 5;
        public const int PoisonDamagePerTurn = 4;
        public const float PoisonHealingReduction = 0.50f;
        public const int BurnDuration = 3;
        public const int BurnDamagePerTurn = 6;
        public const int ChilledDefaultDuration = 2;

        // --- Block ---
        public const float BaseBlockChance = 0f;

        // --- Stances ---
        public const int StanceEnergyCostPerTrigger = 2;
        public const int StanceBerserkerEnergyGain = 2;
        public const float DefensiveStanceBlockBonus = 0.10f;

        // --- Assassination ---
        public const float AssassinationThreshold = 0.20f;
        public const float AssassinationBossThreshold = 0.05f;

        // --- Mass Confusion ---
        public const float ConfusionAllyTargetChance = 0.33f;

        // --- Caltrops ---
        public const int CaltropsDamage = 5;

        // --- Imbue ---
        public const int ImbueBonusDamage = 3;

        // --- Ritual ---
        public const float RitualManaPerHP = 2.0f;

        // --- Battle Timing ---
        public const float BattleStartDelay = 0.5f;
        public const float TurnStartDelay = 0.3f;
        public const float PostActionDelay = 0.3f;
        public const float EnemyThinkDelay = 0.5f;

        // --- Reinforcements ---
        public const float ReinforcementSpawnDelay = 0.8f;

        // --- Energy / Mana Regen ---
        public const float EnergyRegenPerTurn = 0.20f;
        public const float ManaRegenPerTurn = 0.05f;
    }
}

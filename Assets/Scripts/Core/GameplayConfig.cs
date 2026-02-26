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
        public const float PostActionDelay = 0.5f;
        public const float EnemyThinkDelay = 0.5f;

        // --- Reinforcements ---
        public const float ReinforcementSpawnDelay = 0.8f;

        // --- Energy / Mana Regen ---
        public const float EnergyRegenPerTurn = 0.20f;
        public const float ManaRegenPerTurn = 0.05f;

        // --- Ranger ---
        public const int TrapDamage = 8;
        public const int HuntersFocusBonusDamage = 3;
        public const int HuntersFocusDuration = 3;
        public const float TrackingShotBaseMultiplier = 0.8f;
        public const float TrackingShotPerEnemyBonus = 0.15f;
        public const float SniperBonusCrit = 0.10f;
        public const int PinDuration = 2;

        // --- Priest ---
        public const int RegenerationHealPerTurn = 4;
        public const int RegenerationDuration = 3;
        public const float ResurrectHPPercent = 0.30f;
        public const float BlessingDamageBonus = 0.20f;
        public const int BlessingDuration = 3;
        public const int DevotionShieldValue = 3;
        public const float FaithHealingBonus = 0.20f;

        // --- Warrior (additions) ---
        public const int IronWillDuration = 2;

        // --- Elementalist (additions) ---
        public const float ChainLightningBounceMultiplier = 0.50f;
        public const int ChainLightningMaxBounces = 2;
        public const int FrozenTombDuration = 2;

        // --- Warlock (additions) ---
        public const float SoulLinkSplashPercent = 0.30f;
        public const int SoulLinkDuration = 3;
        public const int DrainSoulBaseDamage = 3;
        public const int DrainSoulEscalation = 2;
        public const int DrainSoulDuration = 3;

        // --- Passives ---
        public const int SurvivalistEnergyRegen = 2;
        public const float PredatorDamageBonus = 0.15f;
        public const float SteadyAimHitBonus = 0.05f;
        public const float SteadyAimCritBonus = 0.05f;
        public const float KeenEyeHitBonus = 0.05f;
        public const float KeenEyeCritBonus = 0.05f;
        public const int InnerLightManaRegen = 2;
    }
}

namespace PixelWarriors
{
    public enum CharacterClass
    {
        Warrior,
        Rogue,
        Ranger,
        Priest,
        Elementalist,
        Warlock
    }

    public enum AbilityTab
    {
        Attacks,
        Skills,
        Spells,
        Items,
        Generic
    }

    public enum ActionPointType
    {
        Long,
        Short
    }

    public enum DamageType
    {
        Physical,
        Magical
    }

    public enum Element
    {
        None,
        Fire,
        Earth,
        Water,
        Air,
        Arcane
    }

    public enum AbilityRange
    {
        Any,
        Close,
        Reach,
        Weapon
    }

    public enum WeaponType
    {
        None,
        Sword,
        Dagger,
        TwoHanded,
        Shield,
        Staff,
        Bow,
        Mace
    }

    public enum GridRow
    {
        Front,
        Back
    }

    public enum GridColumn
    {
        Left,
        Right
    }

    public enum BattleState
    {
        Setup,
        TurnStart,
        AwaitingInput,
        ExecutingAction,
        TurnEnd,
        Victory,
        Defeat
    }

    public enum Priority
    {
        Negative = -1,
        Normal = 0,
        Positive = 1
    }

    public enum TeamSide
    {
        Player,
        Enemy
    }

    public enum EquipmentSlot
    {
        Hand1,
        Offhand,
        Head,
        Body,
        Trinket1,
        Trinket2
    }

    public enum StatusEffect
    {
        None,
        Shield,
        Mark,
        Taunt,
        Hide,
        Anticipate,
        React,
        Bleed,
        Poison,
        Burn,
        Chilled,
        Conceal,
        Stun,
        Silence,
        StanceDefensive,
        StanceBrawling,
        StanceBerserker,
        Block,
        Envenom,
        UltimateReflexes,
        Caltrops,
        LeechLife,
        SteamBeamDebuff,
        Terror,
        Confusion,
        CorpseExplosion,
        Levitate,
        Imbue,
        ElementalArmor,
        Bodyguard,
        // Ranger
        HuntersFocus,
        Trap,
        Pin,
        // Priest
        Regeneration,
        Blessing,
        DivineIntervention,
        // Warrior
        IronWill,
        // Elementalist
        FrozenTomb,
        // Warlock
        SoulLink,
        DrainSoul
    }

    public enum AbilityTag
    {
        None,
        // Generic
        Swap,
        Reposition,
        Anticipate,
        React,
        Taunt,
        Hide,
        Pass,
        // Warrior
        CrushArmor,
        Bulwark,
        StanceDefensive,
        StanceBrawling,
        StanceBerserker,
        Cleave,
        SecondWind,
        BlockAbility,
        Bodyguard,
        Bladedance,
        // Rogue
        EscapePlan,
        QuickStab,
        SuckerPunch,
        Ambush,
        Vanish,
        Envenom,
        UltimateReflexes,
        DaggerThrow,
        Assassination,
        PowderBomb,
        Caltrops,
        // Elementalist
        EnergyBolt,
        Ignite,
        Earthquake,
        SteamBeam,
        WaveCrash,
        Levitate,
        SealOfElements,
        ArcaneBurst,
        Splinters,
        Invisibility,
        Meltdown,
        ElementalArmor,
        ImbueStaff,
        // Warlock
        Ritual,
        Terror,
        Curse,
        Hex,
        Consume,
        MassConfusion,
        CorpseExplosion,
        LeechLife,
        // Priest
        WordOfProtection,
        Smite,
        PrayerOfMending,
        HolyWard,
        Purify,
        Resurrect,
        Blessing,
        DivineIntervention,
        // Ranger
        Mark,
        Volley,
        Snipe,
        Barrage,
        HuntersFocus,
        Trap,
        Pin,
        TrackingShot,
        // Warrior (additions)
        RallyCry,
        IronWill,
        // Rogue (additions)
        FanOfKnives,
        ShadowStep,
        // Elementalist (additions)
        ChainLightning,
        FrozenTomb,
        // Warlock (additions)
        SoulLink,
        DrainSoul,
        // Passive flags
        Martyr,
        // Debug
        Annihilation,
        // Consumables
        ConsumableHeal,
        ConsumableEnergyRestore,
        ConsumableManaRestore,
        ConsumableAntidote,
        ConsumableBandage,
        ConsumableSmokeBomb
    }

    public enum PassiveEffect
    {
        None,
        // Basic passives (one per class, granted at level 1)
        Tough,              // Warrior: bonus HP from Endurance
        Evasive,            // Rogue: increased dodge chance
        KeenEye,            // Ranger: increased accuracy and crit
        Faith,              // Priest: +20% healing effectiveness
        ArcaneAffinity,     // Elementalist: spells cost less mana
        DarkPact,           // Warlock: gain mana when taking damage

        // Warrior passives
        OpenWounds,         // Chance to apply bleed on hit
        Executioner,        // Crit chance increased vs low HP enemies
        Frontline,          // Aggro is increased
        Enrage,             // +1 long action below 30% HP

        // Rogue passives
        Swift,              // Close range skills can hit any target
        Footwork,           // Swapping is a short action
        QuickPockets,       // Using an item is a short action
        Strategist,         // Start battle concealed
        EscapePlan,         // Become concealed below 50% HP (once per battle)
        DaggerMastery,      // Can equip daggers in offhand
        // OpenWounds shared with Warrior

        // Ranger passives
        Survivalist,        // Regen energy at turn start
        Predator,           // +15% damage vs marked targets
        SteadyAim,          // +5% accuracy and crit with ranged attacks

        // Priest passives
        InnerLight,         // Regen mana at turn start
        Martyr,             // Prevent ally death once per battle
        Devotion,           // Healing spells grant bonus shield

        // Warlock passives
        DeathDance,         // Lower HP means more effective healing
        Reaper,             // Lower target HP means more damage
        SoulHarvest         // Recover HP/EN/MP when any character dies
    }

    public enum ConsumableCategory
    {
        Potion,
        Bomb,
        Scroll,
        Book,
        Utility
    }

    public enum TargetType
    {
        SingleEnemy,
        SingleAlly,
        Self,
        AllEnemies,
        AllAllies,
        All,
        SingleDeadAlly
    }

    public enum EnemyType
    {
        // ===== ACT 1: Sewers/Catacombs =====
        // Regular - Frontline
        Ratman,
        Skeleton,
        ZombieShambler,
        FungusCreeper,
        // Regular - Backline
        GoblinArcher,
        SwarmBat,
        TunnelRat,
        // Elite
        Minotaur,
        GiantSpider,
        BoneLord,
        // Boss
        GoblinKing,
        CatacombGuardian,

        // ===== ACT 2: Wilderness/Fortress =====
        // Regular - Frontline
        Spider,
        Bandit,
        OrcWarrior,
        StoneSentinel,
        BerserkerCultist,
        // Regular - Backline
        DarkMage,
        HerbalistShaman,
        CrossbowBandit,
        FireImp,
        // Elite
        OrcBrute,
        WyvernKnight,
        NecromancerAdept,
        BladeDancer,
        // Boss
        MinotaurLord,
        BanditWarlord,

        // ===== ACT 3: Dark Citadel =====
        // Regular - Frontline
        DarkKnight,
        AbyssalGolem,
        PlagueBringer,
        DeathCultist,
        ChainDevil,
        // Regular - Backline
        ShadowAssassin,
        LichAcolyte,
        BloodMage,
        VoidSpeaker,
        // Elite
        VampireLord,
        TwinWraith,
        DemonChampion,
        // Boss
        Lich,
        ArchDemon
    }

    public enum GameScreen
    {
        MainMenu,
        PartySetup,
        RoomChoice,
        Battle,
        PostBattle,
        Shop,
        Event,
        Rest,
        Smithy,
        GameOver
    }

    public enum RoomType
    {
        Battle,
        EliteBattle,
        BossBattle,
        Shop,
        Rest,
        Event,
        Smithy
    }

    /// <summary>
    /// High-level floor category used for floor plan generation.
    /// Battle = mandatory fight. Bonus = choose bonus or fight. Event = narrative.
    /// </summary>
    public enum FloorCategory
    {
        Battle,
        Bonus,
        Event,
        Boss
    }

    public enum BattleResult
    {
        None,
        Victory,
        Defeat
    }

    public enum ReinforcementTrigger
    {
        OnEnemyCount,      // Spawn when alive enemy count <= TriggerValue
        OnRoundNumber,     // Spawn when round number >= TriggerValue
        OnBossHPPercent    // Spawn when any boss HP% <= TriggerValue (0-100)
    }

    public enum PlayerInputPhase
    {
        SelectingAbility,
        SelectingTarget,
        AwaitingConfirmation
    }

    public enum SFXType
    {
        // UI
        UIClick,
        UIConfirm,
        UICancel,
        UIPopup,
        TabClick,

        // Combat
        PhysicalHit,
        MagicalHit,
        CriticalHit,
        Miss,
        Dodge,
        Heal,
        Defeated,
        Blocked,

        // Battle Flow
        BattleStart,
        Victory,
        BattleDefeat,
        TurnNotify,
        Reinforcements
    }
}

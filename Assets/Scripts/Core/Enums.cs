namespace PixelWarriors
{
    public enum CharacterClass
    {
        Warrior,
        Rogue,
        Ranger,
        Priest,
        Wizard,
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
        Ice,
        Lightning,
        Earth,
        Shadow,
        Holy,
        Arcane
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
        Hand2,
        Chest,
        Pants,
        Helmet,
        Trinket1,
        Trinket2
    }

    public enum StatusEffect
    {
        None,
        Shield,
        Mark,
        Protect,
        Hide,
        Anticipate,
        Prepare
    }

    public enum AbilityTag
    {
        None,
        Swap,
        Anticipate,
        Prepare,
        Protect,
        Hide,
        Ritual,
        MagicBolt,
        Mark,
        WordOfProtection
    }

    public enum TargetType
    {
        SingleEnemy,
        SingleAlly,
        Self,
        AllEnemies,
        AllAllies,
        All
    }

    public enum EnemyType
    {
        Ratman,
        GoblinArcher,
        Minotaur
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

        // Battle Flow
        BattleStart,
        Victory,
        BattleDefeat,
        TurnNotify
    }
}

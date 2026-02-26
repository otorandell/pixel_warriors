using System.Collections.Generic;

namespace PixelWarriors
{
    public static class AbilityCatalog
    {
        // --- Generic (all classes) ---

        public static List<AbilityData> GetGenericAbilities()
        {
            return new List<AbilityData>
            {
                AbilityData.CreateAttack("Attack", "Basic weapon attack.", basePower: 0, range: AbilityRange.Weapon, damageMultiplier: 1.0f),
                new AbilityData
                {
                    Name = "Swap Position",
                    Description = "Switch grid position with an ally.",
                    Tab = AbilityTab.Generic,
                    ActionCost = ActionPointType.Long,
                    LongPointCost = 1,
                    TargetType = TargetType.SingleAlly,
                    Tag = AbilityTag.Swap,
                    ExcludeSelf = true
                },
                new AbilityData
                {
                    Name = "Reposition",
                    Description = "Move between front and back row.",
                    Tab = AbilityTab.Generic,
                    ActionCost = ActionPointType.Short,
                    ShortPointCost = 1,
                    TargetType = TargetType.Self,
                    Tag = AbilityTag.Reposition
                },
                new AbilityData
                {
                    Name = "Anticipate",
                    Description = "Act with priority next turn, but lose 1 short action.",
                    Tab = AbilityTab.Generic,
                    ActionCost = ActionPointType.Short,
                    ShortPointCost = 1,
                    TargetType = TargetType.Self,
                    Tag = AbilityTag.Anticipate
                },
                new AbilityData
                {
                    Name = "React",
                    Description = "Move last next turn.",
                    Tab = AbilityTab.Generic,
                    ActionCost = ActionPointType.Short,
                    ShortPointCost = 1,
                    TargetType = TargetType.Self,
                    Tag = AbilityTag.React
                },
                new AbilityData
                {
                    Name = "Hide",
                    Description = "Decrease chance of being targeted next turn.",
                    Tab = AbilityTab.Generic,
                    ActionCost = ActionPointType.Short,
                    ShortPointCost = 1,
                    TargetType = TargetType.Self,
                    Tag = AbilityTag.Hide
                },
                new AbilityData
                {
                    Name = "Taunt",
                    Description = "Increase aggro for 2 turns. Frontline only.",
                    Tab = AbilityTab.Generic,
                    ActionCost = ActionPointType.Short,
                    ShortPointCost = 1,
                    TargetType = TargetType.Self,
                    Tag = AbilityTag.Taunt,
                    RequiresFrontline = true
                },
                new AbilityData
                {
                    Name = "Pass",
                    Description = "End your activation.",
                    Tab = AbilityTab.Generic,
                    ActionCost = ActionPointType.Short,
                    ShortPointCost = 0,
                    TargetType = TargetType.Self,
                    Tag = AbilityTag.Pass
                }
};
        }

        // --- Warrior ---

        public static List<AbilityData> GetWarriorAbilities()
        {
            return new List<AbilityData>
            {
                AbilityData.CreateSkill("Crushing Blow", "A powerful strike that deals bonus damage.",
                    basePower: 0, energyCost: 3, range: AbilityRange.Close, damageMultiplier: 1.5f),
                new AbilityData
                {
                    Name = "Powerful Bash",
                    Description = "Requires 2H weapon. Attacks with weapon dealing bonus damage.",
                    Tab = AbilityTab.Attacks,
                    ActionCost = ActionPointType.Long,
                    LongPointCost = 1,
                    EnergyCost = 4,
                    DamageType = DamageType.Physical,
                    BasePower = 0,
                    DamageMultiplier = 2.0f,
                    TargetType = TargetType.SingleEnemy,
                    RequiredWeapon = WeaponType.TwoHanded,
                    Range = AbilityRange.Close
                },
                new AbilityData
                {
                    Name = "Crush Armor",
                    Description = "Destroys part of the enemy armor.",
                    Tab = AbilityTab.Attacks,
                    ActionCost = ActionPointType.Long,
                    LongPointCost = 1,
                    EnergyCost = 3,
                    DamageType = DamageType.Physical,
                    BasePower = 0,
                    DamageMultiplier = 0.8f,
                    TargetType = TargetType.SingleEnemy,
                    Tag = AbilityTag.CrushArmor,
                    Range = AbilityRange.Close
                },
                new AbilityData
                {
                    Name = "Bulwark",
                    Description = "Requires shield. Greatly increases aggro and defense for 3 turns.",
                    Tab = AbilityTab.Skills,
                    ActionCost = ActionPointType.Long,
                    LongPointCost = 1,
                    EnergyCost = 4,
                    TargetType = TargetType.Self,
                    Tag = AbilityTag.Bulwark,
                    RequiredWeapon = WeaponType.Shield
                },
                new AbilityData
                {
                    Name = "Defensive Stance",
                    Description = "Block chance is increased but blocking consumes energy. One stance at a time.",
                    Tab = AbilityTab.Skills,
                    ActionCost = ActionPointType.Short,
                    ShortPointCost = 1,
                    EnergyCost = 2,
                    TargetType = TargetType.Self,
                    Tag = AbilityTag.StanceDefensive
                },
                new AbilityData
                {
                    Name = "Brawling Stance",
                    Description = "Chance to counter-hit attackers (costs energy). One stance at a time.",
                    Tab = AbilityTab.Skills,
                    ActionCost = ActionPointType.Short,
                    ShortPointCost = 1,
                    EnergyCost = 2,
                    TargetType = TargetType.Self,
                    Tag = AbilityTag.StanceBrawling
                },
                new AbilityData
                {
                    Name = "Berserker Stance",
                    Description = "Can't block. Taking damage gains energy. One stance at a time.",
                    Tab = AbilityTab.Skills,
                    ActionCost = ActionPointType.Short,
                    ShortPointCost = 1,
                    EnergyCost = 2,
                    TargetType = TargetType.Self,
                    Tag = AbilityTag.StanceBerserker
                },
                new AbilityData
                {
                    Name = "Cleave",
                    Description = "Hits opponents in the same row for reduced damage.",
                    Tab = AbilityTab.Attacks,
                    ActionCost = ActionPointType.Long,
                    LongPointCost = 1,
                    EnergyCost = 3,
                    DamageType = DamageType.Physical,
                    BasePower = 0,
                    DamageMultiplier = 0.8f,
                    TargetType = TargetType.AllEnemies,
                    Tag = AbilityTag.Cleave,
                    Range = AbilityRange.Close
                },
                new AbilityData
                {
                    Name = "Second Wind",
                    Description = "Restore HP and energy based on missing amount. Once per battle.",
                    Tab = AbilityTab.Skills,
                    ActionCost = ActionPointType.Long,
                    LongPointCost = 1,
                    TargetType = TargetType.Self,
                    Tag = AbilityTag.SecondWind,
                    OncePerBattle = true
                },
                new AbilityData
                {
                    Name = "Block",
                    Description = "Ensures the next received attack is blocked.",
                    Tab = AbilityTab.Skills,
                    ActionCost = ActionPointType.Short,
                    ShortPointCost = 1,
                    EnergyCost = 2,
                    TargetType = TargetType.Self,
                    Tag = AbilityTag.BlockAbility
                },
                new AbilityData
                {
                    Name = "Bodyguard",
                    Description = "Takes all aggro from an ally.",
                    Tab = AbilityTab.Skills,
                    ActionCost = ActionPointType.Long,
                    LongPointCost = 1,
                    EnergyCost = 3,
                    TargetType = TargetType.SingleAlly,
                    Tag = AbilityTag.Bodyguard,
                    ExcludeSelf = true
                },
                new AbilityData
                {
                    Name = "Bladedance",
                    Description = "Requires sword. Bonus damage per consecutive hit on same target.",
                    Tab = AbilityTab.Attacks,
                    ActionCost = ActionPointType.Long,
                    LongPointCost = 1,
                    EnergyCost = 3,
                    DamageType = DamageType.Physical,
                    BasePower = 0,
                    DamageMultiplier = 1.0f,
                    TargetType = TargetType.SingleEnemy,
                    Tag = AbilityTag.Bladedance,
                    RequiredWeapon = WeaponType.Sword,
                    Range = AbilityRange.Close
                },
                new AbilityData
                {
                    Name = "Rally Cry",
                    Description = "All allies gain +1 short action this turn.",
                    Tab = AbilityTab.Skills,
                    ActionCost = ActionPointType.Long,
                    LongPointCost = 1,
                    EnergyCost = 5,
                    TargetType = TargetType.AllAllies,
                    Tag = AbilityTag.RallyCry
                },
                new AbilityData
                {
                    Name = "Iron Will",
                    Description = "Become immune to negative status effects for 2 turns.",
                    Tab = AbilityTab.Skills,
                    ActionCost = ActionPointType.Short,
                    ShortPointCost = 1,
                    EnergyCost = 3,
                    TargetType = TargetType.Self,
                    Tag = AbilityTag.IronWill
                }
            };
        }

        public static List<AbilityData> GetWarriorPassives()
        {
            return new List<AbilityData>
            {
                new AbilityData { Name = "Open Wounds", Description = "Chance to apply bleed on hit.", IsPassive = true, Tag = AbilityTag.None },
                new AbilityData { Name = "Executioner", Description = "Critical chance increased vs low health enemies.", IsPassive = true, Tag = AbilityTag.None },
                new AbilityData { Name = "Frontline", Description = "Aggro is increased.", IsPassive = true, Tag = AbilityTag.None },
                new AbilityData { Name = "Enrage", Description = "Gets +1 long action if below 30% HP.", IsPassive = true, Tag = AbilityTag.None }
            };
        }

        // --- Rogue ---

        public static List<AbilityData> GetRogueAbilities()
        {
            return new List<AbilityData>
            {
                new AbilityData
                {
                    Name = "Quick Stab",
                    Description = "A fast dagger attack. Requires dagger.",
                    Tab = AbilityTab.Attacks,
                    ActionCost = ActionPointType.Short,
                    ShortPointCost = 1,
                    EnergyCost = 2,
                    DamageType = DamageType.Physical,
                    BasePower = 0,
                    DamageMultiplier = 0.8f,
                    TargetType = TargetType.SingleEnemy,
                    Tag = AbilityTag.QuickStab,
                    RequiredWeapon = WeaponType.Dagger,
                    Range = AbilityRange.Close
                },
                new AbilityData
                {
                    Name = "Sucker Punch",
                    Description = "Deals damage and prevents skill usage on the target next turn.",
                    Tab = AbilityTab.Attacks,
                    ActionCost = ActionPointType.Long,
                    LongPointCost = 1,
                    EnergyCost = 3,
                    DamageType = DamageType.Physical,
                    BasePower = 0,
                    DamageMultiplier = 1.2f,
                    TargetType = TargetType.SingleEnemy,
                    Tag = AbilityTag.SuckerPunch,
                    Range = AbilityRange.Close
                },
                new AbilityData
                {
                    Name = "Ambush",
                    Description = "Powerful attack. Can only be used while concealed.",
                    Tab = AbilityTab.Attacks,
                    ActionCost = ActionPointType.Long,
                    LongPointCost = 1,
                    EnergyCost = 4,
                    DamageType = DamageType.Physical,
                    BasePower = 0,
                    DamageMultiplier = 2.0f,
                    TargetType = TargetType.SingleEnemy,
                    Tag = AbilityTag.Ambush,
                    RequiresConcealed = true,
                    Range = AbilityRange.Reach
                },
                new AbilityData
                {
                    Name = "Vanish",
                    Description = "Gain concealment, becoming untargetable.",
                    Tab = AbilityTab.Skills,
                    ActionCost = ActionPointType.Long,
                    LongPointCost = 1,
                    ManaCost = 3,
                    TargetType = TargetType.Self,
                    Tag = AbilityTag.Vanish
                },
                new AbilityData
                {
                    Name = "Envenom",
                    Description = "Coat your weapons in poison. Attacks apply poison.",
                    Tab = AbilityTab.Skills,
                    ActionCost = ActionPointType.Short,
                    ShortPointCost = 1,
                    ManaCost = 3,
                    TargetType = TargetType.Self,
                    Tag = AbilityTag.Envenom
                },
                new AbilityData
                {
                    Name = "Ultimate Reflexes",
                    Description = "Dodge all attacks for the rest of the turn.",
                    Tab = AbilityTab.Skills,
                    ActionCost = ActionPointType.Short,
                    ShortPointCost = 1,
                    EnergyCost = 4,
                    TargetType = TargetType.Self,
                    Tag = AbilityTag.UltimateReflexes
                },
                new AbilityData
                {
                    Name = "Dagger Throw",
                    Description = "Throw a dagger at an enemy. Increased critical chance.",
                    Tab = AbilityTab.Attacks,
                    ActionCost = ActionPointType.Long,
                    LongPointCost = 1,
                    EnergyCost = 2,
                    DamageType = DamageType.Physical,
                    BasePower = 0,
                    DamageMultiplier = 1.0f,
                    TargetType = TargetType.SingleEnemy,
                    Tag = AbilityTag.DaggerThrow,
                    Range = AbilityRange.Reach
                },
                new AbilityData
                {
                    Name = "Assassination",
                    Description = "Kills an enemy below 20% HP (5% for bosses). Killing restores energy.",
                    Tab = AbilityTab.Attacks,
                    ActionCost = ActionPointType.Long,
                    LongPointCost = 1,
                    EnergyCost = 5,
                    DamageType = DamageType.Physical,
                    BasePower = 0,
                    DamageMultiplier = 1.3f,
                    TargetType = TargetType.SingleEnemy,
                    Tag = AbilityTag.Assassination,
                    Range = AbilityRange.Close
                },
                new AbilityData
                {
                    Name = "Powder Bomb",
                    Description = "Deals damage to all enemies. Imprecise.",
                    Tab = AbilityTab.Attacks,
                    ActionCost = ActionPointType.Long,
                    LongPointCost = 1,
                    EnergyCost = 4,
                    DamageType = DamageType.Physical,
                    BasePower = 0,
                    DamageMultiplier = 0.7f,
                    TargetType = TargetType.AllEnemies,
                    Tag = AbilityTag.PowderBomb,
                    Range = AbilityRange.Reach,
                    HitChanceModifier = -0.10f
                },
                new AbilityData
                {
                    Name = "Caltrops",
                    Description = "Enemies take damage when changing position.",
                    Tab = AbilityTab.Skills,
                    ActionCost = ActionPointType.Long,
                    LongPointCost = 1,
                    EnergyCost = 3,
                    TargetType = TargetType.Self,
                    Tag = AbilityTag.Caltrops
                },
                new AbilityData
                {
                    Name = "Fan of Knives",
                    Description = "Throw knives at all enemies. Applies poison.",
                    Tab = AbilityTab.Attacks,
                    ActionCost = ActionPointType.Long,
                    LongPointCost = 1,
                    EnergyCost = 4,
                    DamageType = DamageType.Physical,
                    DamageMultiplier = 0.5f,
                    TargetType = TargetType.AllEnemies,
                    Tag = AbilityTag.FanOfKnives,
                    Range = AbilityRange.Reach
                },
                new AbilityData
                {
                    Name = "Shadow Step",
                    Description = "Teleport behind an enemy and strike. Requires dagger.",
                    Tab = AbilityTab.Attacks,
                    ActionCost = ActionPointType.Long,
                    LongPointCost = 1,
                    EnergyCost = 4,
                    DamageType = DamageType.Physical,
                    DamageMultiplier = 1.5f,
                    TargetType = TargetType.SingleEnemy,
                    Tag = AbilityTag.ShadowStep,
                    RequiredWeapon = WeaponType.Dagger,
                    Range = AbilityRange.Reach
                }
            };
        }

        public static List<AbilityData> GetRoguePassives()
        {
            return new List<AbilityData>
            {
                new AbilityData { Name = "Swift", Description = "Close range skills can hit any target.", IsPassive = true, Tag = AbilityTag.None },
                new AbilityData { Name = "Footwork", Description = "Swapping is a short action.", IsPassive = true, Tag = AbilityTag.None },
                new AbilityData { Name = "Quick Pockets", Description = "Using an item is a short action.", IsPassive = true, Tag = AbilityTag.None },
                new AbilityData { Name = "Strategist", Description = "Start the battle concealed.", IsPassive = true, Tag = AbilityTag.None },
                new AbilityData { Name = "Escape Plan", Description = "Become concealed if HP drops below 50%. Once per battle.", IsPassive = true, Tag = AbilityTag.None },
                new AbilityData { Name = "Dagger Mastery", Description = "Can equip daggers in the offhand slot.", IsPassive = true, Tag = AbilityTag.None },
                new AbilityData { Name = "Open Wounds", Description = "Chance to apply bleed on hit.", IsPassive = true, Tag = AbilityTag.None }
            };
        }

        // --- Elementalist ---

        public static List<AbilityData> GetElementalistAbilities()
        {
            return new List<AbilityData>
            {
                new AbilityData
                {
                    Name = "Energy Bolt",
                    Description = "A bolt of energy. Effect changes based on last element used.",
                    Tab = AbilityTab.Spells,
                    ActionCost = ActionPointType.Long,
                    LongPointCost = 1,
                    ManaCost = 4,
                    DamageType = DamageType.Magical,
                    Element = Element.Arcane,
                    BasePower = 10,
                    TargetType = TargetType.SingleEnemy,
                    Tag = AbilityTag.EnergyBolt,
                    Range = AbilityRange.Reach
                },
                new AbilityData
                {
                    Name = "Ignite",
                    Description = "Sets an enemy on fire.",
                    Tab = AbilityTab.Spells,
                    ActionCost = ActionPointType.Short,
                    ShortPointCost = 1,
                    ManaCost = 3,
                    DamageType = DamageType.Magical,
                    Element = Element.Fire,
                    TargetType = TargetType.SingleEnemy,
                    Tag = AbilityTag.Ignite,
                    Range = AbilityRange.Reach
                },
                new AbilityData
                {
                    Name = "Earthquake",
                    Description = "Deals earth damage to all enemies. Costs a full turn.",
                    Tab = AbilityTab.Spells,
                    ActionCost = ActionPointType.Long,
                    LongPointCost = 1,
                    ShortPointCost = 1,
                    ManaCost = 6,
                    DamageType = DamageType.Physical,
                    Element = Element.Earth,
                    BasePower = 10,
                    TargetType = TargetType.AllEnemies,
                    Tag = AbilityTag.Earthquake,
                    Range = AbilityRange.Reach
                },
                new AbilityData
                {
                    Name = "Steam Beam",
                    Description = "Deals damage. Target receives increased damage.",
                    Tab = AbilityTab.Spells,
                    ActionCost = ActionPointType.Long,
                    LongPointCost = 1,
                    ManaCost = 4,
                    DamageType = DamageType.Magical,
                    Element = Element.Water,
                    BasePower = 8,
                    TargetType = TargetType.SingleEnemy,
                    Tag = AbilityTag.SteamBeam,
                    Range = AbilityRange.Reach
                },
                new AbilityData
                {
                    Name = "Wave Crash",
                    Description = "Deals water damage and swaps target to backline.",
                    Tab = AbilityTab.Spells,
                    ActionCost = ActionPointType.Long,
                    LongPointCost = 1,
                    ManaCost = 4,
                    DamageType = DamageType.Magical,
                    Element = Element.Water,
                    BasePower = 9,
                    TargetType = TargetType.SingleEnemy,
                    Tag = AbilityTag.WaveCrash,
                    Range = AbilityRange.Close
                },
                new AbilityData
                {
                    Name = "Levitate",
                    Description = "Ally gains lowered aggro and close skills can hit any target.",
                    Tab = AbilityTab.Spells,
                    ActionCost = ActionPointType.Short,
                    ShortPointCost = 1,
                    ManaCost = 3,
                    Element = Element.Air,
                    TargetType = TargetType.SingleAlly,
                    Tag = AbilityTag.Levitate
                },
                new AbilityData
                {
                    Name = "Seal of Elements",
                    Description = "Seals an enemy. Effect changes based on last element used.",
                    Tab = AbilityTab.Spells,
                    ActionCost = ActionPointType.Long,
                    LongPointCost = 1,
                    ManaCost = 5,
                    DamageType = DamageType.Magical,
                    Element = Element.Arcane,
                    TargetType = TargetType.SingleEnemy,
                    Tag = AbilityTag.SealOfElements,
                    Range = AbilityRange.Reach
                },
                new AbilityData
                {
                    Name = "Arcane Burst",
                    Description = "Deals damage based on enemy mana pool.",
                    Tab = AbilityTab.Spells,
                    ActionCost = ActionPointType.Long,
                    LongPointCost = 1,
                    ManaCost = 5,
                    DamageType = DamageType.Magical,
                    Element = Element.Arcane,
                    BasePower = 8,
                    TargetType = TargetType.SingleEnemy,
                    Tag = AbilityTag.ArcaneBurst,
                    Range = AbilityRange.Reach
                },
                new AbilityData
                {
                    Name = "Splinters",
                    Description = "Deals damage, can apply bleed.",
                    Tab = AbilityTab.Spells,
                    ActionCost = ActionPointType.Short,
                    ShortPointCost = 1,
                    ManaCost = 2,
                    DamageType = DamageType.Physical,
                    Element = Element.Earth,
                    BasePower = 5,
                    TargetType = TargetType.SingleEnemy,
                    Tag = AbilityTag.Splinters,
                    Range = AbilityRange.Close
                },
                new AbilityData
                {
                    Name = "Invisibility",
                    Description = "An ally gains concealment.",
                    Tab = AbilityTab.Spells,
                    ActionCost = ActionPointType.Long,
                    LongPointCost = 1,
                    ManaCost = 4,
                    Element = Element.Arcane,
                    TargetType = TargetType.SingleAlly,
                    Tag = AbilityTag.Invisibility
                },
                new AbilityData
                {
                    Name = "Meltdown",
                    Description = "Deals damage increased by target's armor.",
                    Tab = AbilityTab.Spells,
                    ActionCost = ActionPointType.Long,
                    LongPointCost = 1,
                    ManaCost = 4,
                    DamageType = DamageType.Magical,
                    Element = Element.Fire,
                    BasePower = 6,
                    TargetType = TargetType.SingleEnemy,
                    Tag = AbilityTag.Meltdown,
                    Range = AbilityRange.Reach
                },
                new AbilityData
                {
                    Name = "Elemental Armor",
                    Description = "Buff an ally. Effect changes based on last element used.",
                    Tab = AbilityTab.Spells,
                    ActionCost = ActionPointType.Long,
                    LongPointCost = 1,
                    ManaCost = 4,
                    Element = Element.Arcane,
                    TargetType = TargetType.SingleAlly,
                    Tag = AbilityTag.ElementalArmor
                },
                new AbilityData
                {
                    Name = "Imbue Staff",
                    Description = "Attacks deal bonus magical damage for 3 turns.",
                    Tab = AbilityTab.Spells,
                    ActionCost = ActionPointType.Short,
                    ShortPointCost = 1,
                    ManaCost = 3,
                    Element = Element.Arcane,
                    TargetType = TargetType.Self,
                    Tag = AbilityTag.ImbueStaff
                },
                new AbilityData
                {
                    Name = "Chain Lightning",
                    Description = "Lightning that bounces between enemies.",
                    Tab = AbilityTab.Spells,
                    ActionCost = ActionPointType.Long,
                    LongPointCost = 1,
                    ManaCost = 5,
                    DamageType = DamageType.Magical,
                    Element = Element.Air,
                    BasePower = 10,
                    TargetType = TargetType.SingleEnemy,
                    Tag = AbilityTag.ChainLightning,
                    Range = AbilityRange.Reach
                },
                new AbilityData
                {
                    Name = "Frozen Tomb",
                    Description = "Entombs an enemy in ice. Stunned but immune to damage.",
                    Tab = AbilityTab.Spells,
                    ActionCost = ActionPointType.Long,
                    LongPointCost = 1,
                    ManaCost = 5,
                    DamageType = DamageType.Magical,
                    Element = Element.Water,
                    TargetType = TargetType.SingleEnemy,
                    Tag = AbilityTag.FrozenTomb,
                    Range = AbilityRange.Reach
                }
            };
        }

        // --- Warlock ---

        public static List<AbilityData> GetWarlockAbilities()
        {
            return new List<AbilityData>
            {
                new AbilityData
                {
                    Name = "Ritual",
                    Description = "Sacrifice HP to gain mana. Requires dagger.",
                    Tab = AbilityTab.Spells,
                    ActionCost = ActionPointType.Short,
                    ShortPointCost = 1,
                    HPCost = 5,
                    DamageType = DamageType.Magical,
                    Element = Element.Arcane,
                    TargetType = TargetType.Self,
                    Tag = AbilityTag.Ritual,
                    RequiredWeapon = WeaponType.Dagger
                },
                new AbilityData
                {
                    Name = "Terror",
                    Description = "Target swaps to backline and is unable to attack for 1 turn.",
                    Tab = AbilityTab.Spells,
                    ActionCost = ActionPointType.Long,
                    LongPointCost = 1,
                    ManaCost = 4,
                    DamageType = DamageType.Magical,
                    Element = Element.Arcane,
                    BasePower = 6,
                    TargetType = TargetType.SingleEnemy,
                    Tag = AbilityTag.Terror,
                    Range = AbilityRange.Reach
                },
                new AbilityData
                {
                    Name = "Curse",
                    Description = "Target receives an extra stack of active negative statuses.",
                    Tab = AbilityTab.Spells,
                    ActionCost = ActionPointType.Long,
                    LongPointCost = 1,
                    ManaCost = 4,
                    TargetType = TargetType.SingleEnemy,
                    Tag = AbilityTag.Curse,
                    Range = AbilityRange.Reach
                },
                new AbilityData
                {
                    Name = "Hex",
                    Description = "Apply 1 bleed stack and a random negative status.",
                    Tab = AbilityTab.Spells,
                    ActionCost = ActionPointType.Long,
                    LongPointCost = 1,
                    ManaCost = 3,
                    TargetType = TargetType.SingleEnemy,
                    Tag = AbilityTag.Hex,
                    Range = AbilityRange.Reach
                },
                new AbilityData
                {
                    Name = "Consume",
                    Description = "Deal damage and heal based on damage dealt.",
                    Tab = AbilityTab.Spells,
                    ActionCost = ActionPointType.Long,
                    LongPointCost = 1,
                    ManaCost = 4,
                    DamageType = DamageType.Magical,
                    Element = Element.Arcane,
                    BasePower = 10,
                    TargetType = TargetType.SingleEnemy,
                    Tag = AbilityTag.Consume,
                    Range = AbilityRange.Reach
                },
                new AbilityData
                {
                    Name = "Mass Confusion",
                    Description = "Enemies may attack their own allies. Imprecise.",
                    Tab = AbilityTab.Spells,
                    ActionCost = ActionPointType.Long,
                    LongPointCost = 1,
                    ManaCost = 6,
                    Element = Element.Arcane,
                    TargetType = TargetType.AllEnemies,
                    Tag = AbilityTag.MassConfusion,
                    Range = AbilityRange.Reach,
                    HitChanceModifier = -0.15f
                },
                new AbilityData
                {
                    Name = "Corpse Explosion",
                    Description = "Marks an enemy. If killed this turn, damages all enemies.",
                    Tab = AbilityTab.Spells,
                    ActionCost = ActionPointType.Long,
                    LongPointCost = 1,
                    ManaCost = 4,
                    DamageType = DamageType.Magical,
                    Element = Element.Arcane,
                    BasePower = 12,
                    TargetType = TargetType.SingleEnemy,
                    Tag = AbilityTag.CorpseExplosion,
                    Range = AbilityRange.Reach
                },
                new AbilityData
                {
                    Name = "Leech Life",
                    Description = "Drains health from an enemy to the caster each turn.",
                    Tab = AbilityTab.Spells,
                    ActionCost = ActionPointType.Long,
                    LongPointCost = 1,
                    ManaCost = 5,
                    DamageType = DamageType.Magical,
                    Element = Element.Arcane,
                    TargetType = TargetType.SingleEnemy,
                    Tag = AbilityTag.LeechLife,
                    Range = AbilityRange.Reach
                },
                new AbilityData
                {
                    Name = "Soul Link",
                    Description = "Links an enemy's soul. Damage splashes to other enemies.",
                    Tab = AbilityTab.Spells,
                    ActionCost = ActionPointType.Long,
                    LongPointCost = 1,
                    ManaCost = 5,
                    TargetType = TargetType.SingleEnemy,
                    Tag = AbilityTag.SoulLink,
                    Range = AbilityRange.Reach
                },
                new AbilityData
                {
                    Name = "Drain Soul",
                    Description = "Drains the target's soul. Escalating damage, heals caster.",
                    Tab = AbilityTab.Spells,
                    ActionCost = ActionPointType.Long,
                    LongPointCost = 1,
                    ManaCost = 4,
                    DamageType = DamageType.Magical,
                    Element = Element.Arcane,
                    TargetType = TargetType.SingleEnemy,
                    Tag = AbilityTag.DrainSoul,
                    Range = AbilityRange.Reach
                }
            };
        }

        public static List<AbilityData> GetWarlockPassives()
        {
            return new List<AbilityData>
            {
                new AbilityData { Name = "Death Dance", Description = "Lower HP means more effective healing.", IsPassive = true, Tag = AbilityTag.None },
                new AbilityData { Name = "Reaper", Description = "Lower target HP means more damage.", IsPassive = true, Tag = AbilityTag.None },
                new AbilityData { Name = "Soul Harvest", Description = "When a character dies, recover HP, energy, and mana.", IsPassive = true, Tag = AbilityTag.None }
            };
        }

        // --- Ranger ---

        public static List<AbilityData> GetRangerAbilities()
        {
            return new List<AbilityData>
            {
                new AbilityData
                {
                    Name = "Mark",
                    Description = "Marks an enemy, boosting accuracy of attacks against it.",
                    Tab = AbilityTab.Skills,
                    ActionCost = ActionPointType.Short,
                    ShortPointCost = 1,
                    EnergyCost = 2,
                    DamageType = DamageType.Physical,
                    BasePower = 0,
                    DamageMultiplier = 0.7f,
                    TargetType = TargetType.SingleEnemy,
                    Tag = AbilityTag.Mark,
                    Range = AbilityRange.Reach
                },
                AbilityData.CreateAttack("Volley", "Fires a volley of arrows at all enemies.",
                    basePower: 0, energyCost: 3, targetType: TargetType.AllEnemies,
                    range: AbilityRange.Reach, damageMultiplier: 0.6f),
                new AbilityData
                {
                    Name = "Snipe",
                    Description = "A precise, high-damage shot. Increased critical chance.",
                    Tab = AbilityTab.Attacks,
                    ActionCost = ActionPointType.Long,
                    LongPointCost = 1,
                    EnergyCost = 4,
                    DamageType = DamageType.Physical,
                    DamageMultiplier = 1.8f,
                    TargetType = TargetType.SingleEnemy,
                    Tag = AbilityTag.Snipe,
                    Range = AbilityRange.Reach
                },
                AbilityData.CreateAttack("Barrage", "A rapid flurry of arrows.",
                    basePower: 0, energyCost: 3, range: AbilityRange.Reach,
                    damageMultiplier: 0.5f, hitCount: 3),
                new AbilityData
                {
                    Name = "Hunter's Focus",
                    Description = "Focus on targets. Attacks deal bonus damage.",
                    Tab = AbilityTab.Skills,
                    ActionCost = ActionPointType.Short,
                    ShortPointCost = 1,
                    EnergyCost = 2,
                    TargetType = TargetType.Self,
                    Tag = AbilityTag.HuntersFocus
                },
                new AbilityData
                {
                    Name = "Trap",
                    Description = "Set a trap. Next enemy that moves takes damage and is stunned.",
                    Tab = AbilityTab.Skills,
                    ActionCost = ActionPointType.Long,
                    LongPointCost = 1,
                    EnergyCost = 3,
                    TargetType = TargetType.Self,
                    Tag = AbilityTag.Trap
                },
                new AbilityData
                {
                    Name = "Pin",
                    Description = "Pins an enemy in place, preventing movement.",
                    Tab = AbilityTab.Attacks,
                    ActionCost = ActionPointType.Short,
                    ShortPointCost = 1,
                    EnergyCost = 2,
                    DamageType = DamageType.Physical,
                    DamageMultiplier = 0.5f,
                    TargetType = TargetType.SingleEnemy,
                    Tag = AbilityTag.Pin,
                    Range = AbilityRange.Reach
                },
                new AbilityData
                {
                    Name = "Tracking Shot",
                    Description = "Damage increases with more enemies alive.",
                    Tab = AbilityTab.Attacks,
                    ActionCost = ActionPointType.Long,
                    LongPointCost = 1,
                    EnergyCost = 4,
                    DamageType = DamageType.Physical,
                    DamageMultiplier = 0.8f,
                    TargetType = TargetType.SingleEnemy,
                    Tag = AbilityTag.TrackingShot,
                    Range = AbilityRange.Reach
                }
            };
        }

        public static List<AbilityData> GetRangerPassives()
        {
            return new List<AbilityData>
            {
                new AbilityData { Name = "Survivalist", Description = "Regenerate 2 energy at turn start.", IsPassive = true, Tag = AbilityTag.None },
                new AbilityData { Name = "Predator", Description = "+15% damage against marked targets.", IsPassive = true, Tag = AbilityTag.None },
                new AbilityData { Name = "Steady Aim", Description = "+5% accuracy and crit with ranged attacks.", IsPassive = true, Tag = AbilityTag.None }
            };
        }

        // --- Priest ---

        public static List<AbilityData> GetPriestAbilities()
        {
            return new List<AbilityData>
            {
                new AbilityData
                {
                    Name = "Word of Protection",
                    Description = "Creates a shield that absorbs damage for an ally.",
                    Tab = AbilityTab.Spells,
                    ActionCost = ActionPointType.Long,
                    LongPointCost = 1,
                    ManaCost = 4,
                    DamageType = DamageType.Magical,
                    Element = Element.Arcane,
                    BasePower = 8,
                    TargetType = TargetType.SingleAlly,
                    Tag = AbilityTag.WordOfProtection
                },
                new AbilityData
                {
                    Name = "Smite",
                    Description = "Strikes an enemy with holy power.",
                    Tab = AbilityTab.Spells,
                    ActionCost = ActionPointType.Long,
                    LongPointCost = 1,
                    ManaCost = 3,
                    DamageType = DamageType.Magical,
                    Element = Element.Arcane,
                    BasePower = 8,
                    TargetType = TargetType.SingleEnemy,
                    Tag = AbilityTag.Smite,
                    Range = AbilityRange.Reach
                },
                new AbilityData
                {
                    Name = "Prayer of Mending",
                    Description = "Heals an ally over time.",
                    Tab = AbilityTab.Spells,
                    ActionCost = ActionPointType.Long,
                    LongPointCost = 1,
                    ManaCost = 4,
                    DamageType = DamageType.Magical,
                    Element = Element.Arcane,
                    TargetType = TargetType.SingleAlly,
                    Tag = AbilityTag.PrayerOfMending
                },
                new AbilityData
                {
                    Name = "Holy Ward",
                    Description = "Shields all allies with a protective ward.",
                    Tab = AbilityTab.Spells,
                    ActionCost = ActionPointType.Long,
                    LongPointCost = 1,
                    ManaCost = 6,
                    DamageType = DamageType.Magical,
                    Element = Element.Arcane,
                    BasePower = 5,
                    TargetType = TargetType.AllAllies,
                    Tag = AbilityTag.HolyWard
                },
                new AbilityData
                {
                    Name = "Purify",
                    Description = "Removes all negative effects from an ally.",
                    Tab = AbilityTab.Spells,
                    ActionCost = ActionPointType.Short,
                    ShortPointCost = 1,
                    ManaCost = 3,
                    TargetType = TargetType.SingleAlly,
                    Tag = AbilityTag.Purify
                },
                new AbilityData
                {
                    Name = "Resurrect",
                    Description = "Revives a fallen ally at 30% HP. Once per battle.",
                    Tab = AbilityTab.Spells,
                    ActionCost = ActionPointType.Long,
                    LongPointCost = 1,
                    ManaCost = 8,
                    TargetType = TargetType.SingleDeadAlly,
                    Tag = AbilityTag.Resurrect,
                    OncePerBattle = true
                },
                new AbilityData
                {
                    Name = "Blessing",
                    Description = "Blesses an ally, increasing their damage.",
                    Tab = AbilityTab.Spells,
                    ActionCost = ActionPointType.Long,
                    LongPointCost = 1,
                    ManaCost = 4,
                    Element = Element.Arcane,
                    TargetType = TargetType.SingleAlly,
                    Tag = AbilityTag.Blessing
                },
                new AbilityData
                {
                    Name = "Divine Intervention",
                    Description = "Makes an ally immune to all damage for 1 turn. Once per battle.",
                    Tab = AbilityTab.Spells,
                    ActionCost = ActionPointType.Long,
                    LongPointCost = 1,
                    ManaCost = 7,
                    Element = Element.Arcane,
                    TargetType = TargetType.SingleAlly,
                    Tag = AbilityTag.DivineIntervention,
                    OncePerBattle = true
                }
            };
        }

        public static List<AbilityData> GetPriestPassives()
        {
            return new List<AbilityData>
            {
                new AbilityData { Name = "Inner Light", Description = "Regenerate 2 mana at turn start.", IsPassive = true, Tag = AbilityTag.None },
                new AbilityData { Name = "Martyr", Description = "When an ally would die, heal them to 1 HP instead. Once per battle.", IsPassive = true, Tag = AbilityTag.None },
                new AbilityData { Name = "Devotion", Description = "Healing spells also grant a small shield.", IsPassive = true, Tag = AbilityTag.None }
            };
        }
    }
}

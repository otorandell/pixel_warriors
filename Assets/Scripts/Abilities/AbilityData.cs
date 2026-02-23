using System;

namespace PixelWarriors
{
    [Serializable]
    public class AbilityData
    {
        public string Name;
        public string Description;
        public AbilityTab Tab;
        public ActionPointType ActionCost;
        public int LongPointCost;
        public int ShortPointCost;
        public int EnergyCost;
        public int ManaCost;
        public int HPCost;
        public DamageType DamageType;
        public Element Element;
        public int BasePower;
        public int HitCount;
        public TargetType TargetType;
        public bool IsPassive;
        public AbilityTag Tag;
        public bool ExcludeSelf;
        public AbilityRange Range;
        public bool OncePerBattle;
        public bool RequiresConcealed;
        public WeaponType RequiredWeapon;
        public bool RequiresFrontline;
        public float DamageMultiplier;
        public float AbilityArmorPen;
        public int AbilityMagicPen;

        public bool IsWeaponAttack => DamageMultiplier > 0f;

        public AbilityData()
        {
            HitCount = 1;
            Element = Element.None;
            Range = AbilityRange.Any;
            RequiredWeapon = WeaponType.None;
        }

        public static AbilityData CreateAttack(string name, string description, int basePower,
            int energyCost = 0, int hitCount = 1, TargetType targetType = TargetType.SingleEnemy,
            AbilityRange range = AbilityRange.Any, float damageMultiplier = 0f)
        {
            return new AbilityData
            {
                Name = name,
                Description = description,
                Tab = AbilityTab.Attacks,
                ActionCost = ActionPointType.Long,
                LongPointCost = 1,
                EnergyCost = energyCost,
                DamageType = DamageType.Physical,
                BasePower = damageMultiplier > 0f ? 0 : basePower,
                DamageMultiplier = damageMultiplier,
                HitCount = hitCount,
                TargetType = targetType,
                Range = range
            };
        }

        public static AbilityData CreateSkill(string name, string description, int basePower,
            int energyCost, TargetType targetType = TargetType.SingleEnemy,
            AbilityRange range = AbilityRange.Any, float damageMultiplier = 0f)
        {
            return new AbilityData
            {
                Name = name,
                Description = description,
                Tab = AbilityTab.Skills,
                ActionCost = ActionPointType.Long,
                LongPointCost = 1,
                EnergyCost = energyCost,
                DamageType = DamageType.Physical,
                BasePower = damageMultiplier > 0f ? 0 : basePower,
                DamageMultiplier = damageMultiplier,
                HitCount = 1,
                TargetType = targetType,
                Range = range
            };
        }

        public static AbilityData CreateSpell(string name, string description, int basePower,
            int manaCost, Element element = Element.Arcane, TargetType targetType = TargetType.SingleEnemy,
            AbilityRange range = AbilityRange.Any)
        {
            return new AbilityData
            {
                Name = name,
                Description = description,
                Tab = AbilityTab.Spells,
                ActionCost = ActionPointType.Long,
                LongPointCost = 1,
                ManaCost = manaCost,
                DamageType = DamageType.Magical,
                Element = element,
                BasePower = basePower,
                HitCount = 1,
                TargetType = targetType,
                Range = range
            };
        }

        public static AbilityData CreateQuickAction(string name, string description)
        {
            return new AbilityData
            {
                Name = name,
                Description = description,
                Tab = AbilityTab.Generic,
                ActionCost = ActionPointType.Short,
                ShortPointCost = 1,
                TargetType = TargetType.Self
            };
        }
    }
}

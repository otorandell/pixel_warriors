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

        public AbilityData()
        {
            HitCount = 1;
            Element = Element.None;
        }

        public static AbilityData CreateAttack(string name, string description, int basePower,
            int energyCost = 0, int hitCount = 1, TargetType targetType = TargetType.SingleEnemy)
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
                BasePower = basePower,
                HitCount = hitCount,
                TargetType = targetType
            };
        }

        public static AbilityData CreateSkill(string name, string description, int basePower,
            int energyCost, TargetType targetType = TargetType.SingleEnemy)
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
                BasePower = basePower,
                HitCount = 1,
                TargetType = targetType
            };
        }

        public static AbilityData CreateSpell(string name, string description, int basePower,
            int manaCost, Element element = Element.Arcane, TargetType targetType = TargetType.SingleEnemy)
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
                TargetType = targetType
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

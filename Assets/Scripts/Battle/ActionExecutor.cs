using System.Collections.Generic;
using UnityEngine;

namespace PixelWarriors
{
    public static class ActionExecutor
    {
        public static void ExecuteAbility(BattleCharacter user, AbilityData ability, List<BattleCharacter> targets)
        {
            if (!user.CanUseAbility(ability))
            {
                Log($"{user.Data.Name} cannot use {ability.Name}!");
                return;
            }

            user.ConsumeAbilityCost(ability);

            if (IsHealingAbility(ability))
            {
                ExecuteHealing(user, ability, targets);
            }
            else if (ability.BasePower > 0)
            {
                ExecuteDamage(user, ability, targets);
            }
            else
            {
                // Non-damage, non-healing ability (buffs, self-targeting utilities)
                foreach (BattleCharacter target in targets)
                {
                    GameEvents.RaiseAbilityUsed(user, ability, target);
                    Log($"{user.Data.Name} uses {ability.Name}!");
                }
            }
        }

        private static void ExecuteDamage(BattleCharacter user, AbilityData ability, List<BattleCharacter> targets)
        {
            foreach (BattleCharacter target in targets)
            {
                if (!target.IsAlive) continue;

                GameEvents.RaiseAbilityUsed(user, ability, target);
                Log($"{user.Data.Name} uses {ability.Name} on {target.Data.Name}!");

                int totalDamage = 0;

                for (int hit = 0; hit < ability.HitCount; hit++)
                {
                    HitResult result = ResolveHit(user, ability, target);
                    LogHitResult(user, target, result, hit + 1, ability.HitCount);

                    if (result.IsEffective)
                    {
                        target.CurrentHP = Mathf.Max(0, target.CurrentHP - result.Damage);
                        totalDamage += result.Damage;
                        GameEvents.RaiseDamageDealt(target, result.Damage, ability.DamageType);
                    }
                }

                if (!target.IsAlive)
                {
                    Log($"{target.Data.Name} was defeated!");
                    GameEvents.RaiseCharacterDefeated(target);
                }
            }
        }

        private static void ExecuteHealing(BattleCharacter user, AbilityData ability, List<BattleCharacter> targets)
        {
            foreach (BattleCharacter target in targets)
            {
                if (!target.IsAlive) continue;

                GameEvents.RaiseAbilityUsed(user, ability, target);

                int healAmount = ability.BasePower;
                int previousHP = target.CurrentHP;
                target.CurrentHP = Mathf.Min(target.CurrentHP + healAmount, target.MaxHP);
                int actualHeal = target.CurrentHP - previousHP;

                if (actualHeal > 0)
                {
                    Log($"{user.Data.Name} uses {ability.Name} on {target.Data.Name}! Heals {actualHeal} HP!");
                    GameEvents.RaiseHealingReceived(target, actualHeal);
                }
                else
                {
                    Log($"{user.Data.Name} uses {ability.Name} on {target.Data.Name}! Already at full HP.");
                }
            }
        }

        private static HitResult ResolveHit(BattleCharacter user, AbilityData ability, BattleCharacter target)
        {
            // Accuracy check
            float accuracy = StatCalculator.CalculateAccuracy(user.EffectiveStats.Dexterity);
            if (Random.value > accuracy)
            {
                return HitResult.Miss();
            }

            // Dodge check
            float dodgeChance = StatCalculator.CalculateDodgeChance(target.EffectiveStats.Dexterity);
            if (Random.value < dodgeChance)
            {
                return HitResult.Dodge();
            }

            // Calculate base damage
            int damage;
            if (ability.DamageType == DamageType.Physical)
            {
                damage = StatCalculator.CalculatePhysicalDamage(
                    ability.BasePower, user.EffectiveStats.Strength, target.EffectiveStats.Armor);
            }
            else
            {
                damage = StatCalculator.CalculateMagicalDamage(
                    ability.BasePower, target.EffectiveStats.MagicResist);
            }

            // Crit check
            float critChance = StatCalculator.CalculateCritChance(user.EffectiveStats.Dexterity);
            bool isCrit = Random.value < critChance;
            if (isCrit)
            {
                damage = Mathf.RoundToInt(damage * GameplayConfig.CritDamageMultiplier);
            }

            return HitResult.Hit(damage, isCrit);
        }

        private static void LogHitResult(BattleCharacter user, BattleCharacter target,
            HitResult result, int hitNumber, int totalHits)
        {
            string prefix = totalHits > 1 ? $"Hit {hitNumber}: " : "";

            if (result.Missed)
            {
                Log($"{prefix}{user.Data.Name} missed!");
            }
            else if (result.Dodged)
            {
                Log($"{prefix}{target.Data.Name} dodged!");
            }
            else if (result.IsCrit)
            {
                Log($"{prefix}CRITICAL! {result.Damage} damage!");
            }
            else
            {
                Log($"{prefix}{result.Damage} damage!");
            }
        }

        private static bool IsHealingAbility(AbilityData ability)
        {
            return ability.BasePower > 0 &&
                   (ability.TargetType == TargetType.SingleAlly ||
                    ability.TargetType == TargetType.AllAllies);
        }

        private static void Log(string message)
        {
            GameEvents.RaiseCombatLogMessage(message);
        }
    }
}

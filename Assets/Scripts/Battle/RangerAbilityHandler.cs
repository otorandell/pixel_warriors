using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PixelWarriors
{
    public static class RangerAbilityHandler
    {
        public static void ExecuteMark(BattleCharacter user, AbilityData ability, List<BattleCharacter> targets)
        {
            foreach (BattleCharacter target in targets)
            {
                if (!target.IsAlive) continue;

                GameEvents.RaiseAbilityUsed(user, ability, target);

                int totalDamage = 0;
                for (int hit = 0; hit < ability.HitCount; hit++)
                {
                    HitResult result = ActionExecutor.ResolveHit(user, ability, target);
                    GameEvents.RaiseHitResolved(target, result, ability.DamageType);
                    ActionExecutor.LogHitResult(user, target, result, hit + 1, ability.HitCount);

                    if (result.IsEffective)
                        totalDamage += ActionExecutor.DealDamage(user, target, result.Damage, ability.DamageType);
                }

                if (totalDamage > 0)
                    ActionExecutor.ProcessPostHitEffects(user, target, totalDamage, ability.DamageType);

                if (target.IsAlive)
                {
                    var effect = new StatusEffectInstance(StatusEffect.Mark, GameplayConfig.MarkDuration, 0, user);
                    target.AddEffect(effect);
                    GameEvents.RaiseStatusEffectApplied(target, StatusEffect.Mark, 0);
                    Log($"{target.Data.Name} is marked! Attacks have boosted accuracy.");
                }

                ActionExecutor.CheckDefeated(target);
            }
        }

        public static void ExecuteSnipe(BattleCharacter user, AbilityData ability, List<BattleCharacter> targets)
        {
            foreach (BattleCharacter target in targets)
            {
                if (!target.IsAlive) continue;

                GameEvents.RaiseAbilityUsed(user, ability, target);
                Log($"{user.Data.Name} takes aim at {target.Data.Name}!");

                HitResult result = ActionExecutor.ResolveHit(user, ability, target,
                    bonusCrit: GameplayConfig.SniperBonusCrit);
                GameEvents.RaiseHitResolved(target, result, ability.DamageType);

                if (result.IsEffective)
                {
                    int finalDamage = ActionExecutor.DealDamage(user, target, result.Damage, ability.DamageType);
                    ActionExecutor.LogHitResult(user, target, result);

                    if (finalDamage > 0)
                        ActionExecutor.ProcessPostHitEffects(user, target, finalDamage, ability.DamageType);
                }
                else
                {
                    ActionExecutor.LogHitResult(user, target, result);
                }

                ActionExecutor.CheckDefeated(target);
            }
        }

        public static void ExecuteHuntersFocus(BattleCharacter user)
        {
            var effect = new StatusEffectInstance(StatusEffect.HuntersFocus,
                GameplayConfig.HuntersFocusDuration, GameplayConfig.HuntersFocusBonusDamage, user);
            user.AddEffect(effect);

            GameEvents.RaiseAbilityUsed(user, null, user);
            GameEvents.RaiseStatusEffectApplied(user, StatusEffect.HuntersFocus, 0);
            Log($"{user.Data.Name} focuses! +{GameplayConfig.HuntersFocusBonusDamage} damage to targets.");
        }

        public static void ExecuteTrap(BattleCharacter user)
        {
            var effect = new StatusEffectInstance(StatusEffect.Trap, -1, 1, user);
            user.AddEffect(effect);

            GameEvents.RaiseAbilityUsed(user, null, user);
            GameEvents.RaiseStatusEffectApplied(user, StatusEffect.Trap, 0);
            Log($"{user.Data.Name} sets a trap!");
        }

        public static void ExecutePin(BattleCharacter user, AbilityData ability, List<BattleCharacter> targets)
        {
            foreach (BattleCharacter target in targets)
            {
                if (!target.IsAlive) continue;

                GameEvents.RaiseAbilityUsed(user, ability, target);

                HitResult result = ActionExecutor.ResolveHit(user, ability, target);
                GameEvents.RaiseHitResolved(target, result, ability.DamageType);

                if (result.IsEffective)
                {
                    int finalDamage = ActionExecutor.DealDamage(user, target, result.Damage, ability.DamageType);
                    ActionExecutor.LogHitResult(user, target, result);

                    if (finalDamage > 0)
                        ActionExecutor.ProcessPostHitEffects(user, target, finalDamage, ability.DamageType);

                    if (target.IsAlive)
                    {
                        var pin = new StatusEffectInstance(StatusEffect.Pin, GameplayConfig.PinDuration, 0, user);
                        target.AddEffect(pin);
                        GameEvents.RaiseStatusEffectApplied(target, StatusEffect.Pin, 0);
                        Log($"{target.Data.Name} is pinned in place!");
                    }
                }
                else
                {
                    ActionExecutor.LogHitResult(user, target, result);
                }

                ActionExecutor.CheckDefeated(target);
            }
        }

        public static void ExecuteTrackingShot(BattleCharacter user, AbilityData ability,
            List<BattleCharacter> targets, List<BattleCharacter> allEnemies)
        {
            int aliveEnemies = allEnemies.Count(e => e.IsAlive);
            float scaledMultiplier = GameplayConfig.TrackingShotBaseMultiplier +
                                     GameplayConfig.TrackingShotPerEnemyBonus * aliveEnemies;

            foreach (BattleCharacter target in targets)
            {
                if (!target.IsAlive) continue;

                GameEvents.RaiseAbilityUsed(user, ability, target);
                Log($"{user.Data.Name} fires a tracking shot at {target.Data.Name}! ({aliveEnemies} targets tracked)");

                // Use scaled multiplier: override ability's multiplier for damage calc
                int weaponDmg = user.Data.GetWeaponDamage();
                float armorPen = user.Data.GetArmorPenetration() + ability.AbilityArmorPen;
                int damage = StatCalculator.CalculateWeaponDamage(
                    weaponDmg, user.EffectiveStats.Strength, scaledMultiplier,
                    target.EffectiveStats.Armor, armorPen);

                // Hit check
                float hitChance = StatCalculator.CalculateHitChance(
                    user.EffectiveStats.Dexterity, target.EffectiveStats.Dexterity);
                if (Random.value > hitChance)
                {
                    GameEvents.RaiseHitResolved(target, HitResult.Miss(), ability.DamageType);
                    Log($"{user.Data.Name} missed!");
                    continue;
                }

                // Crit
                float critChance = StatCalculator.CalculateCritChance(user.EffectiveStats.Dexterity);
                bool isCrit = Random.value < critChance;
                if (isCrit) damage = Mathf.RoundToInt(damage * GameplayConfig.CritDamageMultiplier);

                HitResult result = HitResult.Hit(damage, isCrit);
                GameEvents.RaiseHitResolved(target, result, ability.DamageType);
                ActionExecutor.LogHitResult(user, target, result);

                int finalDamage = ActionExecutor.DealDamage(user, target, damage, ability.DamageType);
                if (finalDamage > 0)
                    ActionExecutor.ProcessPostHitEffects(user, target, finalDamage, ability.DamageType);

                ActionExecutor.CheckDefeated(target);
            }
        }

        private static void Log(string message) => GameEvents.RaiseCombatLogMessage(message);
    }
}

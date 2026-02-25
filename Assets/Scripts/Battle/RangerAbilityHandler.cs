using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PixelWarriors
{
    public static class RangerAbilityHandler
    {
        public static void ExecuteSnipe(BattleCharacter user, AbilityData ability, List<BattleCharacter> targets)
        {
            foreach (BattleCharacter target in targets)
            {
                if (!target.IsAlive) continue;

                GameEvents.RaiseAbilityUsed(user, ability, target);
                GameEvents.RaiseCombatLogMessage($"{user.Data.Name} takes aim at {target.Data.Name}!");

                HitResult result = ActionExecutor.ResolveHit(user, ability, target,
                    bonusCrit: GameplayConfig.SniperBonusCrit);
                GameEvents.RaiseHitResolved(target, result, ability.DamageType);
                LogMissOrDodge(user, target, result);

                if (result.IsEffective)
                {
                    int finalDamage = ApplyModifiersAndDeal(user, target, result.Damage, ability.DamageType);
                    if (finalDamage > 0)
                        ActionExecutor.ProcessPostHitEffects(user, target, finalDamage, ability.DamageType);
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
            GameEvents.RaiseCombatLogMessage(
                $"{user.Data.Name} focuses! +{GameplayConfig.HuntersFocusBonusDamage} damage to targets.");
        }

        public static void ExecuteTrap(BattleCharacter user)
        {
            var effect = new StatusEffectInstance(StatusEffect.Trap, -1, 1, user);
            user.AddEffect(effect);

            GameEvents.RaiseAbilityUsed(user, null, user);
            GameEvents.RaiseStatusEffectApplied(user, StatusEffect.Trap, 0);
            GameEvents.RaiseCombatLogMessage($"{user.Data.Name} sets a trap!");
        }

        public static void ExecutePin(BattleCharacter user, AbilityData ability, List<BattleCharacter> targets)
        {
            foreach (BattleCharacter target in targets)
            {
                if (!target.IsAlive) continue;

                GameEvents.RaiseAbilityUsed(user, ability, target);

                HitResult result = ActionExecutor.ResolveHit(user, ability, target);
                GameEvents.RaiseHitResolved(target, result, ability.DamageType);
                LogMissOrDodge(user, target, result);

                if (result.IsEffective)
                {
                    int finalDamage = ApplyModifiersAndDeal(user, target, result.Damage, ability.DamageType);
                    if (finalDamage > 0)
                        ActionExecutor.ProcessPostHitEffects(user, target, finalDamage, ability.DamageType);

                    if (target.IsAlive)
                    {
                        var pin = new StatusEffectInstance(StatusEffect.Pin, GameplayConfig.PinDuration, 0, user);
                        target.AddEffect(pin);
                        GameEvents.RaiseStatusEffectApplied(target, StatusEffect.Pin, 0);
                        GameEvents.RaiseCombatLogMessage($"{target.Data.Name} is pinned in place!");
                    }
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
                GameEvents.RaiseCombatLogMessage(
                    $"{user.Data.Name} fires a tracking shot at {target.Data.Name}! ({aliveEnemies} targets tracked)");

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
                    GameEvents.RaiseCombatLogMessage($"{user.Data.Name} missed!");
                    continue;
                }

                // Crit
                float critChance = StatCalculator.CalculateCritChance(user.EffectiveStats.Dexterity);
                bool isCrit = Random.value < critChance;
                if (isCrit) damage = Mathf.RoundToInt(damage * GameplayConfig.CritDamageMultiplier);

                HitResult result = HitResult.Hit(damage, isCrit);
                GameEvents.RaiseHitResolved(target, result, ability.DamageType);

                if (isCrit)
                    GameEvents.RaiseCombatLogMessage($"CRITICAL! {damage} damage!");
                else
                    GameEvents.RaiseCombatLogMessage($"{damage} damage!");

                int finalDamage = ApplyModifiersAndDeal(user, target, damage, ability.DamageType);
                if (finalDamage > 0)
                    ActionExecutor.ProcessPostHitEffects(user, target, finalDamage, ability.DamageType);

                ActionExecutor.CheckDefeated(target);
            }
        }

        private static int ApplyModifiersAndDeal(BattleCharacter user, BattleCharacter target,
            int damage, DamageType type)
        {
            // Reuse the same modifier pipeline as the main executor
            // Imbue
            if (user.HasEffect(StatusEffect.Imbue))
                damage += GameplayConfig.ImbueBonusDamage;
            // Hunter's Focus
            if (user.HasEffect(StatusEffect.HuntersFocus))
                damage += GameplayConfig.HuntersFocusBonusDamage;
            // Mark
            float markMult = StatusEffectProcessor.GetMarkBonus(target);
            damage = Mathf.RoundToInt(damage * markMult);
            // Blessing
            if (user.HasEffect(StatusEffect.Blessing))
                damage = Mathf.RoundToInt(damage * (1f + GameplayConfig.BlessingDamageBonus));
            // Predator
            if (target.HasEffect(StatusEffect.Mark) && PassiveProcessor.HasPassive(user, "Predator"))
                damage = Mathf.RoundToInt(damage * (1f + GameplayConfig.PredatorDamageBonus));
            // Shield absorption
            damage = StatusEffectProcessor.AbsorbDamage(target, damage);

            target.CurrentHP = Mathf.Max(0, target.CurrentHP - damage);
            if (damage > 0)
                GameEvents.RaiseDamageDealt(target, damage, type);
            return damage;
        }

        private static void LogMissOrDodge(BattleCharacter user, BattleCharacter target, HitResult result)
        {
            if (result.Missed)
                GameEvents.RaiseCombatLogMessage($"{user.Data.Name} missed!");
            else if (result.Dodged)
                GameEvents.RaiseCombatLogMessage($"{target.Data.Name} dodged!");
            else if (result.Blocked)
                GameEvents.RaiseCombatLogMessage($"{target.Data.Name} blocked!");
            else if (result.IsCrit)
                GameEvents.RaiseCombatLogMessage($"CRITICAL! {result.Damage} damage!");
            else
                GameEvents.RaiseCombatLogMessage($"{result.Damage} damage!");
        }
    }
}

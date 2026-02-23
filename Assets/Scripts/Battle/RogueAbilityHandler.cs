using System.Collections.Generic;
using UnityEngine;

namespace PixelWarriors
{
    public static class RogueAbilityHandler
    {
        public static void ExecuteSuckerPunch(BattleCharacter user, AbilityData ability, List<BattleCharacter> targets)
        {
            foreach (BattleCharacter target in targets)
            {
                if (!target.IsAlive) continue;

                GameEvents.RaiseAbilityUsed(user, ability, target);
                Log($"{user.Data.Name} sucker punches {target.Data.Name}!");

                HitResult result = ActionExecutor.ResolveHit(user, ability, target);
                GameEvents.RaiseHitResolved(target, result, ability.DamageType);

                if (result.IsEffective)
                {
                    target.CurrentHP = Mathf.Max(0, target.CurrentHP - result.Damage);
                    GameEvents.RaiseDamageDealt(target, result.Damage, ability.DamageType);
                    Log($"{result.Damage} damage!");

                    // Apply Silence (prevents skill/spell usage)
                    if (target.IsAlive)
                    {
                        var silence = new StatusEffectInstance(StatusEffect.Silence, 1, 0, user);
                        target.AddEffect(silence);
                        GameEvents.RaiseStatusEffectApplied(target, StatusEffect.Silence, 0);
                        Log($"{target.Data.Name} is dazed! Can't use skills next turn.");
                    }
                }
                else
                {
                    LogMissOrDodge(user, target, result);
                }

                ActionExecutor.CheckDefeated(target);
            }
        }

        public static void ExecuteAmbush(BattleCharacter user, AbilityData ability, List<BattleCharacter> targets)
        {
            // Remove concealment when attacking
            user.RemoveEffect(StatusEffect.Conceal);
            GameEvents.RaiseStatusEffectRemoved(user, StatusEffect.Conceal);
            Log($"{user.Data.Name} emerges from the shadows!");

            ActionExecutor.ExecuteDamage(user, ability, targets);
        }

        public static void ExecuteVanish(BattleCharacter user)
        {
            var effect = new StatusEffectInstance(StatusEffect.Conceal, -1, 0, user);
            user.AddEffect(effect);
            GameEvents.RaiseAbilityUsed(user, null, user);
            GameEvents.RaiseStatusEffectApplied(user, StatusEffect.Conceal, 0);
            Log($"{user.Data.Name} vanishes into the shadows!");
        }

        public static void ExecuteEnvenom(BattleCharacter user)
        {
            var effect = new StatusEffectInstance(StatusEffect.Envenom, 3, 0, user);
            user.AddEffect(effect);
            GameEvents.RaiseAbilityUsed(user, null, user);
            GameEvents.RaiseStatusEffectApplied(user, StatusEffect.Envenom, 0);
            Log($"{user.Data.Name} coats weapons in poison!");
        }

        public static void ExecuteUltimateReflexes(BattleCharacter user)
        {
            var effect = new StatusEffectInstance(StatusEffect.UltimateReflexes, 1, 0, user);
            user.AddEffect(effect);
            GameEvents.RaiseAbilityUsed(user, null, user);
            GameEvents.RaiseStatusEffectApplied(user, StatusEffect.UltimateReflexes, 0);
            Log($"{user.Data.Name} focuses! Will dodge all attacks this turn.");
        }

        public static void ExecuteDaggerThrow(BattleCharacter user, AbilityData ability, List<BattleCharacter> targets)
        {
            foreach (BattleCharacter target in targets)
            {
                if (!target.IsAlive) continue;

                GameEvents.RaiseAbilityUsed(user, ability, target);
                Log($"{user.Data.Name} throws a dagger at {target.Data.Name}!");

                HitResult result = ActionExecutor.ResolveHit(user, ability, target, bonusCrit: 0.15f);
                GameEvents.RaiseHitResolved(target, result, ability.DamageType);

                if (result.IsEffective)
                {
                    target.CurrentHP = Mathf.Max(0, target.CurrentHP - result.Damage);
                    GameEvents.RaiseDamageDealt(target, result.Damage, ability.DamageType);
                    Log(result.IsCrit ? $"CRITICAL! {result.Damage} damage!" : $"{result.Damage} damage!");
                }
                else
                {
                    LogMissOrDodge(user, target, result);
                }

                ActionExecutor.CheckDefeated(target);
            }
        }

        public static void ExecuteAssassination(BattleCharacter user, AbilityData ability, List<BattleCharacter> targets)
        {
            foreach (BattleCharacter target in targets)
            {
                if (!target.IsAlive) continue;

                GameEvents.RaiseAbilityUsed(user, ability, target);

                float hpPercent = (float)target.CurrentHP / target.MaxHP;
                float threshold = GameplayConfig.AssassinationThreshold;
                // TODO: Add boss check when boss flag exists

                if (hpPercent <= threshold)
                {
                    target.CurrentHP = 0;
                    GameEvents.RaiseDamageDealt(target, target.CurrentHP, ability.DamageType);
                    Log($"{user.Data.Name} assassinates {target.Data.Name}!");

                    // Restore energy on kill
                    int energyRestore = Mathf.RoundToInt(user.MaxEnergy * 0.3f);
                    user.CurrentEnergy = Mathf.Min(user.CurrentEnergy + energyRestore, user.MaxEnergy);
                    Log($"{user.Data.Name} restores {energyRestore} energy!");

                    ActionExecutor.CheckDefeated(target);
                }
                else
                {
                    // Below threshold: deal normal damage
                    Log($"{user.Data.Name} attempts to assassinate {target.Data.Name}!");
                    ActionExecutor.ExecuteDamage(user, ability, targets);
                }
            }
        }

        public static void ExecutePowderBomb(BattleCharacter user, AbilityData ability, List<BattleCharacter> targets)
        {
            GameEvents.RaiseAbilityUsed(user, ability, user);
            Log($"{user.Data.Name} throws a powder bomb!");

            foreach (BattleCharacter target in targets)
            {
                if (!target.IsAlive) continue;

                HitResult result = ActionExecutor.ResolveHit(user, ability, target);
                GameEvents.RaiseHitResolved(target, result, ability.DamageType);

                if (result.IsEffective)
                {
                    target.CurrentHP = Mathf.Max(0, target.CurrentHP - result.Damage);
                    GameEvents.RaiseDamageDealt(target, result.Damage, ability.DamageType);
                    Log($"{target.Data.Name} takes {result.Damage} damage!");
                }
                else
                {
                    LogMissOrDodge(user, target, result);
                }

                ActionExecutor.CheckDefeated(target);
            }
        }

        public static void ExecuteCaltrops(BattleCharacter user)
        {
            var effect = new StatusEffectInstance(StatusEffect.Caltrops, -1, 0, user);
            user.AddEffect(effect);
            GameEvents.RaiseAbilityUsed(user, null, user);
            GameEvents.RaiseStatusEffectApplied(user, StatusEffect.Caltrops, 0);
            Log($"{user.Data.Name} scatters caltrops! Enemies take damage on position change.");
        }

        private static void LogMissOrDodge(BattleCharacter user, BattleCharacter target, HitResult result)
        {
            if (result.Missed) Log($"{user.Data.Name} missed!");
            else if (result.Dodged) Log($"{target.Data.Name} dodged!");
            else if (result.Blocked) Log($"{target.Data.Name} blocked!");
        }

        private static void Log(string message) => GameEvents.RaiseCombatLogMessage(message);
    }
}

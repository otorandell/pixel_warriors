using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PixelWarriors
{
    public static class WarlockAbilityHandler
    {
        public static void ExecuteTerror(BattleCharacter user, AbilityData ability, List<BattleCharacter> targets)
        {
            foreach (BattleCharacter target in targets)
            {
                if (!target.IsAlive) continue;

                GameEvents.RaiseAbilityUsed(user, ability, target);
                Log($"{user.Data.Name} casts Terror on {target.Data.Name}!");

                HitResult result = ActionExecutor.ResolveHit(user, ability, target);
                GameEvents.RaiseHitResolved(target, result, ability.DamageType);

                if (result.IsEffective)
                {
                    target.CurrentHP = Mathf.Max(0, target.CurrentHP - result.Damage);
                    GameEvents.RaiseDamageDealt(target, result.Damage, ability.DamageType);
                    Log($"{result.Damage} damage!");

                    if (target.IsAlive)
                    {
                        // Push to backline
                        if (target.Row == GridRow.Front)
                        {
                            target.Row = GridRow.Back;
                            GameEvents.RaisePositionSwapped(target, target);
                            Log($"{target.Data.Name} flees to the back row!");
                        }

                        // Unable to attack for 1 turn
                        var terror = new StatusEffectInstance(StatusEffect.Terror, 1, 0, user);
                        target.AddEffect(terror);
                        GameEvents.RaiseStatusEffectApplied(target, StatusEffect.Terror, 0);
                        Log($"{target.Data.Name} is terrified! Unable to attack.");
                    }
                }
                else
                {
                    LogMissOrDodge(user, target, result);
                }

                ActionExecutor.CheckDefeated(target);
            }
        }

        public static void ExecuteCurse(BattleCharacter user, AbilityData ability, List<BattleCharacter> targets)
        {
            foreach (BattleCharacter target in targets)
            {
                if (!target.IsAlive) continue;

                GameEvents.RaiseAbilityUsed(user, ability, target);
                Log($"{user.Data.Name} curses {target.Data.Name}!");

                // Duplicate existing negative status effects
                StatusEffect[] negativeTypes = {
                    StatusEffect.Bleed, StatusEffect.Poison, StatusEffect.Burn,
                    StatusEffect.Chilled, StatusEffect.Silence, StatusEffect.Stun
                };

                int stacked = 0;
                foreach (StatusEffect type in negativeTypes)
                {
                    if (target.HasEffect(type))
                    {
                        StatusEffectInstance existing = target.GetEffect(type);
                        if (type == StatusEffect.Bleed)
                        {
                            // Add another bleed stack
                            var extra = new StatusEffectInstance(StatusEffect.Bleed,
                                GameplayConfig.BleedDuration, 0, user);
                            target.AddEffect(extra);
                        }
                        else
                        {
                            // Extend duration by 1
                            existing.RemainingTurns++;
                        }
                        stacked++;
                    }
                }

                if (stacked > 0)
                    Log($"Curse amplifies {stacked} negative effects!");
                else
                    Log($"No negative effects to amplify.");
            }
        }

        public static void ExecuteHex(BattleCharacter user, AbilityData ability, List<BattleCharacter> targets)
        {
            foreach (BattleCharacter target in targets)
            {
                if (!target.IsAlive) continue;

                GameEvents.RaiseAbilityUsed(user, ability, target);
                Log($"{user.Data.Name} hexes {target.Data.Name}!");

                // Apply 1 bleed
                var bleed = new StatusEffectInstance(StatusEffect.Bleed, GameplayConfig.BleedDuration, 0, user);
                target.AddEffect(bleed);
                GameEvents.RaiseStatusEffectApplied(target, StatusEffect.Bleed, 0);
                Log($"{target.Data.Name} is bleeding!");

                // Apply random negative status
                StatusEffect[] randomStatuses = {
                    StatusEffect.Poison, StatusEffect.Burn, StatusEffect.Chilled, StatusEffect.Silence
                };
                StatusEffect chosen = randomStatuses[Random.Range(0, randomStatuses.Length)];

                int duration = chosen switch
                {
                    StatusEffect.Poison => GameplayConfig.PoisonDuration,
                    StatusEffect.Burn => GameplayConfig.BurnDuration,
                    StatusEffect.Chilled => GameplayConfig.ChilledDefaultDuration,
                    StatusEffect.Silence => 1,
                    _ => 2
                };

                var status = new StatusEffectInstance(chosen, duration, 0, user);
                target.AddEffect(status);
                GameEvents.RaiseStatusEffectApplied(target, chosen, 0);
                Log($"{target.Data.Name} is afflicted with {chosen}!");
            }
        }

        public static void ExecuteConsume(BattleCharacter user, AbilityData ability, List<BattleCharacter> targets)
        {
            foreach (BattleCharacter target in targets)
            {
                if (!target.IsAlive) continue;

                GameEvents.RaiseAbilityUsed(user, ability, target);
                Log($"{user.Data.Name} consumes {target.Data.Name}'s life force!");

                HitResult result = ActionExecutor.ResolveHit(user, ability, target);
                GameEvents.RaiseHitResolved(target, result, ability.DamageType);

                if (result.IsEffective)
                {
                    target.CurrentHP = Mathf.Max(0, target.CurrentHP - result.Damage);
                    GameEvents.RaiseDamageDealt(target, result.Damage, ability.DamageType);
                    Log($"{result.Damage} damage!");

                    // Heal caster for portion of damage dealt
                    int healAmount = Mathf.Max(1, Mathf.RoundToInt(result.Damage * 0.5f));
                    int prevHP = user.CurrentHP;
                    user.CurrentHP = Mathf.Min(user.CurrentHP + healAmount, user.MaxHP);
                    int actualHeal = user.CurrentHP - prevHP;
                    if (actualHeal > 0)
                    {
                        GameEvents.RaiseHealingReceived(user, actualHeal);
                        Log($"{user.Data.Name} drains {actualHeal} HP!");
                    }
                }
                else
                {
                    LogMissOrDodge(user, target, result);
                }

                ActionExecutor.CheckDefeated(target);
            }
        }

        public static void ExecuteMassConfusion(BattleCharacter user, AbilityData ability, List<BattleCharacter> targets)
        {
            GameEvents.RaiseAbilityUsed(user, ability, user);
            Log($"{user.Data.Name} casts Mass Confusion!");

            foreach (BattleCharacter target in targets)
            {
                if (!target.IsAlive) continue;

                if (!ActionExecutor.RollSpellHit(user, ability, target))
                {
                    Log($"{target.Data.Name} resists the confusion!");
                    continue;
                }

                var effect = new StatusEffectInstance(StatusEffect.Confusion, 2, 0, user);
                target.AddEffect(effect);
                GameEvents.RaiseStatusEffectApplied(target, StatusEffect.Confusion, 0);
                Log($"{target.Data.Name} is confused!");
            }
        }

        public static void ExecuteCorpseExplosion(BattleCharacter user, AbilityData ability, List<BattleCharacter> targets)
        {
            foreach (BattleCharacter target in targets)
            {
                if (!target.IsAlive) continue;

                GameEvents.RaiseAbilityUsed(user, ability, target);

                var effect = new StatusEffectInstance(StatusEffect.CorpseExplosion, 1, ability.BasePower, user);
                target.AddEffect(effect);
                GameEvents.RaiseStatusEffectApplied(target, StatusEffect.CorpseExplosion, ability.BasePower);
                Log($"{user.Data.Name} marks {target.Data.Name} for corpse explosion!");
            }
        }

        public static void ExecuteLeechLife(BattleCharacter user, AbilityData ability, List<BattleCharacter> targets)
        {
            foreach (BattleCharacter target in targets)
            {
                if (!target.IsAlive) continue;

                GameEvents.RaiseAbilityUsed(user, ability, target);

                int drainValue = Mathf.Max(1, Mathf.RoundToInt(target.MaxHP * 0.08f));
                var effect = new StatusEffectInstance(StatusEffect.LeechLife, 3, drainValue, user);
                target.AddEffect(effect);
                GameEvents.RaiseStatusEffectApplied(target, StatusEffect.LeechLife, drainValue);
                Log($"{user.Data.Name} leeches life from {target.Data.Name}! Draining {drainValue} HP/turn.");
            }
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

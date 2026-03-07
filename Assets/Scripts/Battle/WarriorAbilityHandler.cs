using System.Collections.Generic;
using UnityEngine;

namespace PixelWarriors
{
    public static class WarriorAbilityHandler
    {
        public static void ExecuteCrushArmor(BattleCharacter user, AbilityData ability, List<BattleCharacter> targets)
        {
            foreach (BattleCharacter target in targets)
            {
                if (!target.IsAlive) continue;

                GameEvents.RaiseAbilityUsed(user, ability, target);
                Log($"{user.Data.Name} uses Crush Armor on {target.Data.Name}!");

                HitResult result = ActionExecutor.ResolveHit(user, ability, target);
                GameEvents.RaiseHitResolved(target, result, ability.DamageType);

                if (result.IsEffective)
                {
                    int finalDamage = ActionExecutor.DealDamage(user, target, Mathf.Max(1, result.Damage), ability.DamageType);

                    if (finalDamage > 0)
                        ActionExecutor.ProcessPostHitEffects(user, target, finalDamage, ability.DamageType);

                    // Reduce armor temporarily (permanent for simplicity — balance later)
                    if (target.IsAlive)
                    {
                        int armorReduction = Mathf.Max(1, target.EffectiveStats.Armor / 3);
                        target.EffectiveStats.Armor = Mathf.Max(0, target.EffectiveStats.Armor - armorReduction);
                        Log($"{target.Data.Name}'s armor reduced by {armorReduction}!");
                    }
                }
                else
                {
                    ActionExecutor.LogHitResult(user, target, result);
                }

                ActionExecutor.CheckDefeated(target);
            }
        }

        public static void ExecuteBulwark(BattleCharacter user)
        {
            var taunt = new StatusEffectInstance(StatusEffect.Taunt, 3, 0, user);
            user.AddEffect(taunt);
            GameEvents.RaiseStatusEffectApplied(user, StatusEffect.Taunt, 0);

            var block = new StatusEffectInstance(StatusEffect.Block, 3, 0, user);
            user.AddEffect(block);
            GameEvents.RaiseStatusEffectApplied(user, StatusEffect.Block, 0);

            GameEvents.RaiseAbilityUsed(user, null, user);
            Log($"{user.Data.Name} raises their shield! Greatly increased aggro and defense for 3 turns.");
        }

        public static void ExecuteStance(BattleCharacter user, StatusEffect stanceType, string name)
        {
            var effect = new StatusEffectInstance(stanceType, -1, 0, user);
            user.AddEffect(effect);
            GameEvents.RaiseAbilityUsed(user, null, user);
            GameEvents.RaiseStatusEffectApplied(user, stanceType, 0);
            Log($"{user.Data.Name} enters {name}!");
        }

        public static void ExecuteCleave(BattleCharacter user, AbilityData ability, List<BattleCharacter> targets)
        {
            GameEvents.RaiseAbilityUsed(user, ability, user);
            Log($"{user.Data.Name} cleaves!");

            int totalDamage = 0;
            foreach (BattleCharacter target in targets)
            {
                if (!target.IsAlive) continue;

                HitResult result = ActionExecutor.ResolveHit(user, ability, target);
                GameEvents.RaiseHitResolved(target, result, ability.DamageType);

                if (result.IsEffective)
                {
                    int finalDamage = ActionExecutor.DealDamage(user, target, result.Damage, ability.DamageType);
                    totalDamage += finalDamage;
                    Log($"{target.Data.Name} takes {finalDamage} damage!");

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

        public static void ExecuteSecondWind(BattleCharacter user)
        {
            int missingHP = user.MaxHP - user.CurrentHP;
            int missingEnergy = user.MaxEnergy - user.CurrentEnergy;

            int healAmount = Mathf.Max(1, Mathf.RoundToInt(missingHP * 0.5f));
            int energyAmount = Mathf.Max(1, Mathf.RoundToInt(missingEnergy * 0.5f));

            user.CurrentHP = Mathf.Min(user.CurrentHP + healAmount, user.MaxHP);
            user.CurrentEnergy = Mathf.Min(user.CurrentEnergy + energyAmount, user.MaxEnergy);

            GameEvents.RaiseAbilityUsed(user, null, user);
            GameEvents.RaiseHealingReceived(user, healAmount);
            Log($"{user.Data.Name} catches a second wind! Heals {healAmount} HP, restores {energyAmount} energy.");
        }

        public static void ExecuteBlock(BattleCharacter user)
        {
            var effect = new StatusEffectInstance(StatusEffect.Block, -1, 0, user);
            user.AddEffect(effect);
            GameEvents.RaiseAbilityUsed(user, null, user);
            GameEvents.RaiseStatusEffectApplied(user, StatusEffect.Block, 0);
            Log($"{user.Data.Name} braces! Next attack will be blocked.");
        }

        public static void ExecuteBodyguard(BattleCharacter user, List<BattleCharacter> targets)
        {
            if (targets.Count == 0) return;
            BattleCharacter ally = targets[0];

            var taunt = new StatusEffectInstance(StatusEffect.Taunt, 1, 0, user);
            user.AddEffect(taunt);
            GameEvents.RaiseStatusEffectApplied(user, StatusEffect.Taunt, 0);

            var hide = new StatusEffectInstance(StatusEffect.Bodyguard, 1, 0, user);
            ally.AddEffect(hide);
            GameEvents.RaiseStatusEffectApplied(ally, StatusEffect.Bodyguard, 0);

            GameEvents.RaiseAbilityUsed(user, null, ally);
            Log($"{user.Data.Name} protects {ally.Data.Name}! Taking all their aggro.");
        }

        public static void ExecuteBladedance(BattleCharacter user, AbilityData ability, List<BattleCharacter> targets)
        {
            foreach (BattleCharacter target in targets)
            {
                if (!target.IsAlive) continue;

                GameEvents.RaiseAbilityUsed(user, ability, target);

                HitResult result = ActionExecutor.ResolveHit(user, ability, target);
                GameEvents.RaiseHitResolved(target, result, ability.DamageType);

                if (result.IsEffective)
                {
                    int bonusDamage = GetBladedanceBonus(user, target);
                    int totalDamage = result.Damage + bonusDamage;
                    int finalDamage = ActionExecutor.DealDamage(user, target, totalDamage, ability.DamageType);

                    IncrementBladedanceCounter(user, target);

                    if (finalDamage > 0)
                        ActionExecutor.ProcessPostHitEffects(user, target, finalDamage, ability.DamageType);

                    if (bonusDamage > 0)
                        Log($"{user.Data.Name} bladedances! {finalDamage} damage (+{bonusDamage} combo)!");
                    else
                        Log($"{user.Data.Name} bladedances! {finalDamage} damage!");
                }
                else
                {
                    ActionExecutor.LogHitResult(user, target, result);
                }

                ActionExecutor.CheckDefeated(target);
            }
        }

        // Bladedance tracking: use a dictionary keyed by (user, target) pair
        private static readonly Dictionary<(BattleCharacter, BattleCharacter), int> _bladedanceCounts = new();

        public static void ResetBladedanceCounts() => _bladedanceCounts.Clear();

        private static int GetBladedanceBonus(BattleCharacter user, BattleCharacter target)
        {
            var key = (user, target);
            return _bladedanceCounts.TryGetValue(key, out int count) ? count * 2 : 0;
        }

        private static void IncrementBladedanceCounter(BattleCharacter user, BattleCharacter target)
        {
            var key = (user, target);
            if (_bladedanceCounts.ContainsKey(key))
                _bladedanceCounts[key]++;
            else
                _bladedanceCounts[key] = 1;
        }

        public static void ExecuteRallyCry(BattleCharacter user, AbilityData ability, List<BattleCharacter> targets)
        {
            GameEvents.RaiseAbilityUsed(user, ability, user);
            Log($"{user.Data.Name} rallies the team!");

            foreach (BattleCharacter target in targets)
            {
                if (!target.IsAlive) continue;
                target.ShortActionsRemaining += 1;
                Log($"  {target.Data.Name} gains +1 short action!");
            }
        }

        public static void ExecuteIronWill(BattleCharacter user)
        {
            var effect = new StatusEffectInstance(StatusEffect.IronWill, GameplayConfig.IronWillDuration, 0, user);
            user.AddEffect(effect);
            GameEvents.RaiseAbilityUsed(user, null, user);
            GameEvents.RaiseStatusEffectApplied(user, StatusEffect.IronWill, 0);
            Log($"{user.Data.Name}'s will is iron! Immune to negative effects for {GameplayConfig.IronWillDuration} turns.");
        }

        // Stance on-damage triggers (called from damage pipeline)
        public static void ProcessStanceTriggers(BattleCharacter attacker, BattleCharacter target, int damage)
        {
            if (!target.IsAlive) return;

            // Brawling Stance: counter-attack
            if (target.HasEffect(StatusEffect.StanceBrawling) &&
                target.CurrentEnergy >= GameplayConfig.StanceEnergyCostPerTrigger)
            {
                target.CurrentEnergy -= GameplayConfig.StanceEnergyCostPerTrigger;
                int weaponDmg = target.Data.GetWeaponDamage();
                float armorPen = target.Data.GetArmorPenetration();
                int counterDamage = StatCalculator.CalculateWeaponDamage(
                    weaponDmg, target.EffectiveStats.Strength, 0.5f, attacker.EffectiveStats.Armor, armorPen);
                attacker.CurrentHP = Mathf.Max(0, attacker.CurrentHP - counterDamage);
                GameEvents.RaiseDamageDealt(attacker, counterDamage, DamageType.Physical);
                Log($"{target.Data.Name} counter-attacks for {counterDamage}!");

                if (target.CurrentEnergy <= 0)
                {
                    target.RemoveEffect(StatusEffect.StanceBrawling);
                    GameEvents.RaiseStatusEffectRemoved(target, StatusEffect.StanceBrawling);
                    Log($"{target.Data.Name}'s Brawling Stance fades!");
                }

                ActionExecutor.CheckDefeated(attacker);
            }

            // Berserker Stance: gain energy when hit
            if (target.HasEffect(StatusEffect.StanceBerserker))
            {
                int gain = GameplayConfig.StanceBerserkerEnergyGain;
                target.CurrentEnergy = Mathf.Min(target.CurrentEnergy + gain, target.MaxEnergy);
                Log($"{target.Data.Name} gains {gain} energy from rage!");
            }
        }

        private static void Log(string message) => GameEvents.RaiseCombatLogMessage(message);
    }
}

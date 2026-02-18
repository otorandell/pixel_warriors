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

            // Track last spell element for Wizard (excluding MagicBolt itself)
            if (ability.DamageType == DamageType.Magical && ability.Element != Element.None
                && ability.Tag != AbilityTag.MagicBolt)
            {
                user.LastSpellElement = ability.Element;
            }

            // Route by tag
            switch (ability.Tag)
            {
                case AbilityTag.Swap:
                    ExecuteSwap(user, targets);
                    return;
                case AbilityTag.Anticipate:
                    ExecuteAnticipate(user);
                    return;
                case AbilityTag.Prepare:
                    ExecutePrepare(user);
                    return;
                case AbilityTag.Protect:
                    ExecuteProtect(user);
                    return;
                case AbilityTag.Hide:
                    ExecuteHide(user);
                    return;
                case AbilityTag.Ritual:
                    ExecuteRitual(user, ability);
                    return;
                case AbilityTag.MagicBolt:
                    ExecuteMagicBolt(user, ability, targets);
                    return;
                case AbilityTag.Mark:
                    ExecuteMark(user, ability, targets);
                    return;
                case AbilityTag.WordOfProtection:
                    ExecuteWordOfProtection(user, ability, targets);
                    return;
            }

            // Default: damage or healing
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
                foreach (BattleCharacter target in targets)
                {
                    GameEvents.RaiseAbilityUsed(user, ability, target);
                    Log($"{user.Data.Name} uses {ability.Name}!");
                }
            }
        }

        // --- Special Ability Implementations ---

        private static void ExecuteSwap(BattleCharacter user, List<BattleCharacter> targets)
        {
            if (targets.Count == 0) return;
            BattleCharacter target = targets[0];

            GridRow tempRow = user.Row;
            GridColumn tempCol = user.Column;
            user.Row = target.Row;
            user.Column = target.Column;
            target.Row = tempRow;
            target.Column = tempCol;

            GameEvents.RaiseAbilityUsed(user, null, target);
            GameEvents.RaisePositionSwapped(user, target);
            Log($"{user.Data.Name} swaps position with {target.Data.Name}!");
        }

        private static void ExecuteAnticipate(BattleCharacter user)
        {
            user.Priority = Priority.Positive;
            var effect = new StatusEffectInstance(StatusEffect.Anticipate, -1, 0, user);
            user.AddEffect(effect);

            GameEvents.RaiseAbilityUsed(user, null, user);
            GameEvents.RaiseStatusEffectApplied(user, StatusEffect.Anticipate, 0);
            Log($"{user.Data.Name} anticipates! Will act first next turn.");
        }

        private static void ExecutePrepare(BattleCharacter user)
        {
            user.Priority = Priority.Negative;
            var effect = new StatusEffectInstance(StatusEffect.Prepare, -1, 0, user);
            user.AddEffect(effect);

            GameEvents.RaiseAbilityUsed(user, null, user);
            GameEvents.RaiseStatusEffectApplied(user, StatusEffect.Prepare, 0);
            Log($"{user.Data.Name} prepares! Will recover resources but act last next turn.");
        }

        private static void ExecuteProtect(BattleCharacter user)
        {
            var effect = new StatusEffectInstance(StatusEffect.Protect, -1, 0, user);
            user.AddEffect(effect);

            GameEvents.RaiseAbilityUsed(user, null, user);
            GameEvents.RaiseStatusEffectApplied(user, StatusEffect.Protect, 0);
            Log($"{user.Data.Name} takes a protective stance! Drawing enemy attention.");
        }

        private static void ExecuteHide(BattleCharacter user)
        {
            var effect = new StatusEffectInstance(StatusEffect.Hide, -1, 0, user);
            user.AddEffect(effect);

            GameEvents.RaiseAbilityUsed(user, null, user);
            GameEvents.RaiseStatusEffectApplied(user, StatusEffect.Hide, 0);
            Log($"{user.Data.Name} hides! Less likely to be targeted.");
        }

        private static void ExecuteRitual(BattleCharacter user, AbilityData ability)
        {
            // HPCost already consumed in ConsumeAbilityCost
            int manaGain = Mathf.RoundToInt(ability.HPCost * GameplayConfig.RitualManaPerHP);
            user.CurrentMana = Mathf.Min(user.CurrentMana + manaGain, user.MaxMana);

            GameEvents.RaiseAbilityUsed(user, ability, user);
            Log($"{user.Data.Name} performs a dark ritual! Gains {manaGain} mana.");
        }

        private static void ExecuteMagicBolt(BattleCharacter user, AbilityData ability, List<BattleCharacter> targets)
        {
            // Inherit element from last spell cast
            Element element = user.LastSpellElement;
            Log($"{user.Data.Name}'s Magic Bolt takes on {element} element!");

            // Execute as a normal damage ability but with inherited element
            foreach (BattleCharacter target in targets)
            {
                if (!target.IsAlive) continue;

                GameEvents.RaiseAbilityUsed(user, ability, target);

                for (int hit = 0; hit < ability.HitCount; hit++)
                {
                    HitResult result = ResolveHit(user, ability, target);
                    GameEvents.RaiseHitResolved(target, result, ability.DamageType);
                    LogHitResult(user, target, result, hit + 1, ability.HitCount);

                    if (result.IsEffective)
                    {
                        int finalDamage = ApplyDamageModifiers(result.Damage, target);
                        target.CurrentHP = Mathf.Max(0, target.CurrentHP - finalDamage);
                        GameEvents.RaiseDamageDealt(target, finalDamage, ability.DamageType);
                    }
                }

                CheckDefeated(target);
            }
        }

        private static void ExecuteMark(BattleCharacter user, AbilityData ability, List<BattleCharacter> targets)
        {
            foreach (BattleCharacter target in targets)
            {
                if (!target.IsAlive) continue;

                GameEvents.RaiseAbilityUsed(user, ability, target);

                // Deal damage
                for (int hit = 0; hit < ability.HitCount; hit++)
                {
                    HitResult result = ResolveHit(user, ability, target);
                    GameEvents.RaiseHitResolved(target, result, ability.DamageType);
                    LogHitResult(user, target, result, hit + 1, ability.HitCount);

                    if (result.IsEffective)
                    {
                        int finalDamage = ApplyDamageModifiers(result.Damage, target);
                        target.CurrentHP = Mathf.Max(0, target.CurrentHP - finalDamage);
                        GameEvents.RaiseDamageDealt(target, finalDamage, ability.DamageType);
                    }
                }

                // Apply mark (even if damage missed — the mark still lands)
                if (target.IsAlive)
                {
                    var effect = new StatusEffectInstance(StatusEffect.Mark, GameplayConfig.MarkDuration, 0, user);
                    target.AddEffect(effect);
                    GameEvents.RaiseStatusEffectApplied(target, StatusEffect.Mark, 0);
                    Log($"{target.Data.Name} is marked! All attacks deal bonus damage.");
                }

                CheckDefeated(target);
            }
        }

        private static void ExecuteWordOfProtection(BattleCharacter user, AbilityData ability, List<BattleCharacter> targets)
        {
            foreach (BattleCharacter target in targets)
            {
                if (!target.IsAlive) continue;

                GameEvents.RaiseAbilityUsed(user, ability, target);

                int shieldValue = ability.BasePower;
                var effect = new StatusEffectInstance(StatusEffect.Shield, -1, shieldValue, user);
                target.AddEffect(effect);

                GameEvents.RaiseStatusEffectApplied(target, StatusEffect.Shield, shieldValue);
                Log($"{user.Data.Name} casts {ability.Name} on {target.Data.Name}! Shield: {shieldValue}");
            }
        }

        // --- Core Damage/Healing ---

        private static void ExecuteDamage(BattleCharacter user, AbilityData ability, List<BattleCharacter> targets)
        {
            foreach (BattleCharacter target in targets)
            {
                if (!target.IsAlive) continue;

                GameEvents.RaiseAbilityUsed(user, ability, target);
                Log($"{user.Data.Name} uses {ability.Name} on {target.Data.Name}!");

                for (int hit = 0; hit < ability.HitCount; hit++)
                {
                    HitResult result = ResolveHit(user, ability, target);
                    GameEvents.RaiseHitResolved(target, result, ability.DamageType);
                    LogHitResult(user, target, result, hit + 1, ability.HitCount);

                    if (result.IsEffective)
                    {
                        int finalDamage = ApplyDamageModifiers(result.Damage, target);
                        target.CurrentHP = Mathf.Max(0, target.CurrentHP - finalDamage);
                        GameEvents.RaiseDamageDealt(target, finalDamage, ability.DamageType);
                    }
                }

                CheckDefeated(target);
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

        // --- Damage Pipeline Helpers ---

        private static int ApplyDamageModifiers(int baseDamage, BattleCharacter target)
        {
            // Mark bonus
            float markMult = StatusEffectProcessor.GetMarkBonus(target);
            int damage = Mathf.RoundToInt(baseDamage * markMult);

            // Shield absorption
            damage = StatusEffectProcessor.AbsorbDamage(target, damage);

            return damage;
        }

        private static void CheckDefeated(BattleCharacter target)
        {
            if (!target.IsAlive)
            {
                Log($"{target.Data.Name} was defeated!");
                GameEvents.RaiseCharacterDefeated(target);
            }
        }

        // --- Hit Resolution ---

        private static HitResult ResolveHit(BattleCharacter user, AbilityData ability, BattleCharacter target)
        {
            float accuracy = StatCalculator.CalculateAccuracy(user.EffectiveStats.Dexterity);
            if (Random.value > accuracy)
                return HitResult.Miss();

            float dodgeChance = StatCalculator.CalculateDodgeChance(target.EffectiveStats.Dexterity);
            if (Random.value < dodgeChance)
                return HitResult.Dodge();

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
                Log($"{prefix}{user.Data.Name} missed!");
            else if (result.Dodged)
                Log($"{prefix}{target.Data.Name} dodged!");
            else if (result.IsCrit)
                Log($"{prefix}CRITICAL! {result.Damage} damage!");
            else
                Log($"{prefix}{result.Damage} damage!");
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

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

            // Track once-per-battle usage
            if (ability.OncePerBattle)
                user.MarkUsedOnce(ability.Tag);

            // Track last spell element for Elementalist (excluding EnergyBolt itself)
            if (ability.DamageType == DamageType.Magical && ability.Element != Element.None
                && ability.Tag != AbilityTag.EnergyBolt)
            {
                user.LastSpellElement = ability.Element;
            }

            // Route by tag
            switch (ability.Tag)
            {
                // --- Generic ---
                case AbilityTag.Swap:
                    ExecuteSwap(user, targets);
                    return;
                case AbilityTag.Anticipate:
                    ExecuteAnticipate(user);
                    return;
                case AbilityTag.React:
                    ExecuteReact(user);
                    return;
                case AbilityTag.Taunt:
                    ExecuteTaunt(user);
                    return;
                case AbilityTag.Hide:
                    ExecuteHide(user);
                    return;
                case AbilityTag.Pass:
                    ExecutePass(user);
                    return;

                // --- Warrior ---
                case AbilityTag.CrushArmor:
                    WarriorAbilityHandler.ExecuteCrushArmor(user, ability, targets);
                    return;
                case AbilityTag.Bulwark:
                    WarriorAbilityHandler.ExecuteBulwark(user);
                    return;
                case AbilityTag.StanceDefensive:
                    WarriorAbilityHandler.ExecuteStance(user, StatusEffect.StanceDefensive, "Defensive Stance");
                    return;
                case AbilityTag.StanceBrawling:
                    WarriorAbilityHandler.ExecuteStance(user, StatusEffect.StanceBrawling, "Brawling Stance");
                    return;
                case AbilityTag.StanceBerserker:
                    WarriorAbilityHandler.ExecuteStance(user, StatusEffect.StanceBerserker, "Berserker Stance");
                    return;
                case AbilityTag.Cleave:
                    WarriorAbilityHandler.ExecuteCleave(user, ability, targets);
                    return;
                case AbilityTag.SecondWind:
                    WarriorAbilityHandler.ExecuteSecondWind(user);
                    return;
                case AbilityTag.BlockAbility:
                    WarriorAbilityHandler.ExecuteBlock(user);
                    return;
                case AbilityTag.Bodyguard:
                    WarriorAbilityHandler.ExecuteBodyguard(user, targets);
                    return;
                case AbilityTag.Bladedance:
                    WarriorAbilityHandler.ExecuteBladedance(user, ability, targets);
                    return;

                // --- Rogue ---
                case AbilityTag.SuckerPunch:
                    RogueAbilityHandler.ExecuteSuckerPunch(user, ability, targets);
                    return;
                case AbilityTag.Ambush:
                    RogueAbilityHandler.ExecuteAmbush(user, ability, targets);
                    return;
                case AbilityTag.Vanish:
                    RogueAbilityHandler.ExecuteVanish(user);
                    return;
                case AbilityTag.Envenom:
                    RogueAbilityHandler.ExecuteEnvenom(user);
                    return;
                case AbilityTag.UltimateReflexes:
                    RogueAbilityHandler.ExecuteUltimateReflexes(user);
                    return;
                case AbilityTag.DaggerThrow:
                    RogueAbilityHandler.ExecuteDaggerThrow(user, ability, targets);
                    return;
                case AbilityTag.Assassination:
                    RogueAbilityHandler.ExecuteAssassination(user, ability, targets);
                    return;
                case AbilityTag.PowderBomb:
                    RogueAbilityHandler.ExecutePowderBomb(user, ability, targets);
                    return;
                case AbilityTag.Caltrops:
                    RogueAbilityHandler.ExecuteCaltrops(user);
                    return;

                // --- Elementalist ---
                case AbilityTag.EnergyBolt:
                    ElementalistAbilityHandler.ExecuteEnergyBolt(user, ability, targets);
                    return;
                case AbilityTag.Ignite:
                    ElementalistAbilityHandler.ExecuteIgnite(user, ability, targets);
                    return;
                case AbilityTag.Earthquake:
                    ExecuteDamage(user, ability, targets);
                    return;
                case AbilityTag.SteamBeam:
                    ElementalistAbilityHandler.ExecuteSteamBeam(user, ability, targets);
                    return;
                case AbilityTag.WaveCrash:
                    ElementalistAbilityHandler.ExecuteWaveCrash(user, ability, targets);
                    return;
                case AbilityTag.Levitate:
                    ElementalistAbilityHandler.ExecuteLevitate(user, ability, targets);
                    return;
                case AbilityTag.SealOfElements:
                    ElementalistAbilityHandler.ExecuteSealOfElements(user, ability, targets);
                    return;
                case AbilityTag.ArcaneBurst:
                    ElementalistAbilityHandler.ExecuteArcaneBurst(user, ability, targets);
                    return;
                case AbilityTag.Splinters:
                    ElementalistAbilityHandler.ExecuteSplinters(user, ability, targets);
                    return;
                case AbilityTag.Invisibility:
                    ElementalistAbilityHandler.ExecuteInvisibility(user, ability, targets);
                    return;
                case AbilityTag.Meltdown:
                    ElementalistAbilityHandler.ExecuteMeltdown(user, ability, targets);
                    return;
                case AbilityTag.ElementalArmor:
                    ElementalistAbilityHandler.ExecuteElementalArmor(user, ability, targets);
                    return;
                case AbilityTag.ImbueStaff:
                    ElementalistAbilityHandler.ExecuteImbueStaff(user);
                    return;

                // --- Warlock ---
                case AbilityTag.Ritual:
                    ExecuteRitual(user, ability);
                    return;
                case AbilityTag.Terror:
                    WarlockAbilityHandler.ExecuteTerror(user, ability, targets);
                    return;
                case AbilityTag.Curse:
                    WarlockAbilityHandler.ExecuteCurse(user, ability, targets);
                    return;
                case AbilityTag.Hex:
                    WarlockAbilityHandler.ExecuteHex(user, ability, targets);
                    return;
                case AbilityTag.Consume:
                    WarlockAbilityHandler.ExecuteConsume(user, ability, targets);
                    return;
                case AbilityTag.MassConfusion:
                    WarlockAbilityHandler.ExecuteMassConfusion(user, ability, targets);
                    return;
                case AbilityTag.CorpseExplosion:
                    WarlockAbilityHandler.ExecuteCorpseExplosion(user, ability, targets);
                    return;
                case AbilityTag.LeechLife:
                    WarlockAbilityHandler.ExecuteLeechLife(user, ability, targets);
                    return;

                // --- Priest ---
                case AbilityTag.WordOfProtection:
                    ExecuteWordOfProtection(user, ability, targets);
                    return;

                // --- Ranger ---
                case AbilityTag.Mark:
                    ExecuteMark(user, ability, targets);
                    return;
            }

            // Default: damage or healing
            if (IsHealingAbility(ability))
            {
                ExecuteHealing(user, ability, targets);
            }
            else if (ability.BasePower > 0 || ability.IsWeaponAttack)
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

        private static void ExecuteReact(BattleCharacter user)
        {
            user.Priority = Priority.Negative;
            var effect = new StatusEffectInstance(StatusEffect.React, -1, 0, user);
            user.AddEffect(effect);

            GameEvents.RaiseAbilityUsed(user, null, user);
            GameEvents.RaiseStatusEffectApplied(user, StatusEffect.React, 0);
            Log($"{user.Data.Name} reacts! Will move last next turn.");
        }

        private static void ExecuteTaunt(BattleCharacter user)
        {
            var effect = new StatusEffectInstance(StatusEffect.Taunt, GameplayConfig.TauntDuration, 0, user);
            user.AddEffect(effect);

            GameEvents.RaiseAbilityUsed(user, null, user);
            GameEvents.RaiseStatusEffectApplied(user, StatusEffect.Taunt, 0);
            Log($"{user.Data.Name} taunts! Drawing enemy attention for {GameplayConfig.TauntDuration} turns.");
        }

        private static void ExecuteHide(BattleCharacter user)
        {
            var effect = new StatusEffectInstance(StatusEffect.Hide, -1, 0, user);
            user.AddEffect(effect);

            GameEvents.RaiseAbilityUsed(user, null, user);
            GameEvents.RaiseStatusEffectApplied(user, StatusEffect.Hide, 0);
            Log($"{user.Data.Name} hides! Less likely to be targeted.");
        }

        private static void ExecutePass(BattleCharacter user)
        {
            user.LongActionsRemaining = 0;
            user.ShortActionsRemaining = 0;

            GameEvents.RaiseAbilityUsed(user, null, user);
            Log($"{user.Data.Name} passes.");
        }

        private static void ExecuteRitual(BattleCharacter user, AbilityData ability)
        {
            int manaGain = Mathf.RoundToInt(ability.HPCost * GameplayConfig.RitualManaPerHP);
            user.CurrentMana = Mathf.Min(user.CurrentMana + manaGain, user.MaxMana);

            GameEvents.RaiseAbilityUsed(user, ability, user);
            Log($"{user.Data.Name} performs a dark ritual! Gains {manaGain} mana.");
        }

        private static void ExecuteMark(BattleCharacter user, AbilityData ability, List<BattleCharacter> targets)
        {
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
                        int finalDamage = ApplyDamageModifiers(result.Damage, user, target);
                        target.CurrentHP = Mathf.Max(0, target.CurrentHP - finalDamage);
                        GameEvents.RaiseDamageDealt(target, finalDamage, ability.DamageType);
                    }
                }

                if (target.IsAlive)
                {
                    var effect = new StatusEffectInstance(StatusEffect.Mark, GameplayConfig.MarkDuration, 0, user);
                    target.AddEffect(effect);
                    GameEvents.RaiseStatusEffectApplied(target, StatusEffect.Mark, 0);
                    Log($"{target.Data.Name} is marked! Attacks have boosted accuracy.");
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

        public static void ExecuteDamage(BattleCharacter user, AbilityData ability, List<BattleCharacter> targets)
        {
            foreach (BattleCharacter target in targets)
            {
                if (!target.IsAlive) continue;

                GameEvents.RaiseAbilityUsed(user, ability, target);
                Log($"{user.Data.Name} uses {ability.Name} on {target.Data.Name}!");

                int totalDamageDealt = 0;
                for (int hit = 0; hit < ability.HitCount; hit++)
                {
                    HitResult result = ResolveHit(user, ability, target);
                    GameEvents.RaiseHitResolved(target, result, ability.DamageType);
                    LogHitResult(user, target, result, hit + 1, ability.HitCount);

                    if (result.IsEffective)
                    {
                        int finalDamage = ApplyDamageModifiers(result.Damage, user, target);
                        target.CurrentHP = Mathf.Max(0, target.CurrentHP - finalDamage);
                        GameEvents.RaiseDamageDealt(target, finalDamage, ability.DamageType);
                        totalDamageDealt += finalDamage;
                    }
                }

                if (totalDamageDealt > 0)
                    ProcessPostHitEffects(user, target, totalDamageDealt, ability.DamageType);

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

                // Poison impairs healing
                if (target.HasEffect(StatusEffect.Poison))
                {
                    healAmount = Mathf.RoundToInt(healAmount * (1f - GameplayConfig.PoisonHealingReduction));
                }

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

        private static int ApplyDamageModifiers(int baseDamage, BattleCharacter user, BattleCharacter target)
        {
            int damage = baseDamage;

            // Imbue bonus damage
            if (user != null && user.HasEffect(StatusEffect.Imbue))
                damage += GameplayConfig.ImbueBonusDamage;

            float markMult = StatusEffectProcessor.GetMarkBonus(target);
            damage = Mathf.RoundToInt(damage * markMult);
            damage = StatusEffectProcessor.AbsorbDamage(target, damage);
            return damage;
        }

        /// <summary>
        /// Called after damage is dealt to a target. Handles stance triggers and Envenom.
        /// Custom ability handlers should call this after dealing damage.
        /// </summary>
        public static void ProcessPostHitEffects(BattleCharacter attacker, BattleCharacter target, int damage, DamageType damageType)
        {
            if (!target.IsAlive) return;

            // Stance triggers (physical attacks only)
            if (damageType == DamageType.Physical)
                WarriorAbilityHandler.ProcessStanceTriggers(attacker, target, damage);

            // Envenom: apply poison on physical hit
            if (attacker.HasEffect(StatusEffect.Envenom) && damageType == DamageType.Physical && target.IsAlive)
            {
                var poison = new StatusEffectInstance(StatusEffect.Poison, GameplayConfig.PoisonDuration, 0, attacker);
                target.AddEffect(poison);
                GameEvents.RaiseStatusEffectApplied(target, StatusEffect.Poison, 0);
                Log($"{target.Data.Name} is poisoned by envenomed weapon!");
            }
        }

        public static void CheckDefeated(BattleCharacter target)
        {
            if (!target.IsAlive)
            {
                Log($"{target.Data.Name} was defeated!");
                GameEvents.RaiseCharacterDefeated(target);
            }
        }

        // --- Hit Resolution ---

        public static HitResult ResolveHit(BattleCharacter user, AbilityData ability, BattleCharacter target,
            float bonusCrit = 0f)
        {
            // Ultimate Reflexes: auto-dodge
            if (target.HasEffect(StatusEffect.UltimateReflexes))
                return HitResult.Dodge();

            // Hit chance: single contest
            float hitChance = StatCalculator.CalculateHitChance(
                user.EffectiveStats.Dexterity, target.EffectiveStats.Dexterity);
            if (Random.value > hitChance)
                return HitResult.Miss();

            // Block check (physical only)
            if (ability.DamageType == DamageType.Physical)
            {
                if (CheckBlock(target))
                    return HitResult.Block();
            }

            // Damage: three-way branch
            int damage;
            if (ability.IsWeaponAttack)
            {
                int weaponDmg = user.Data.GetWeaponDamage();
                float armorPen = user.Data.GetArmorPenetration() + ability.AbilityArmorPen;
                damage = StatCalculator.CalculateWeaponDamage(
                    weaponDmg, user.EffectiveStats.Strength, ability.DamageMultiplier,
                    target.EffectiveStats.Armor, armorPen);
            }
            else if (ability.DamageType == DamageType.Physical)
            {
                float armorPen = user.Data.GetArmorPenetration() + ability.AbilityArmorPen;
                damage = StatCalculator.CalculatePhysicalSpellDamage(
                    ability.BasePower, user.EffectiveStats.Willpower,
                    target.EffectiveStats.Armor, armorPen);
            }
            else
            {
                int magicPen = user.Data.GetMagicPenetration() + ability.AbilityMagicPen;
                damage = StatCalculator.CalculateSpellDamage(
                    ability.BasePower, user.EffectiveStats.Willpower,
                    target.EffectiveStats.MagicResist, magicPen);
            }

            // Crit
            float critChance = StatCalculator.CalculateCritChance(user.EffectiveStats.Dexterity) + bonusCrit;
            bool isCrit = Random.value < critChance;
            if (isCrit)
            {
                damage = Mathf.RoundToInt(damage * GameplayConfig.CritDamageMultiplier);
            }

            return HitResult.Hit(damage, isCrit);
        }

        private static bool CheckBlock(BattleCharacter target)
        {
            // Block status effect = guaranteed block
            if (target.HasEffect(StatusEffect.Block))
            {
                target.RemoveEffect(StatusEffect.Block);
                GameEvents.RaiseStatusEffectRemoved(target, StatusEffect.Block);
                return true;
            }

            // Berserker stance cannot block
            if (target.HasEffect(StatusEffect.StanceBerserker))
                return false;

            // Single additive roll: equipment block + stance bonus
            float blockChance = target.Data.GetBaseBlockChance();
            if (target.HasEffect(StatusEffect.StanceDefensive))
                blockChance += GameplayConfig.DefensiveStanceBlockBonus;

            if (blockChance > 0 && Random.value < blockChance)
            {
                // Defensive Stance costs energy when block triggers
                if (target.HasEffect(StatusEffect.StanceDefensive))
                {
                    target.CurrentEnergy -= GameplayConfig.StanceEnergyCostPerTrigger;
                    if (target.CurrentEnergy <= 0)
                    {
                        target.CurrentEnergy = 0;
                        target.RemoveEffect(StatusEffect.StanceDefensive);
                        GameEvents.RaiseStatusEffectRemoved(target, StatusEffect.StanceDefensive);
                        Log($"{target.Data.Name}'s Defensive Stance fades (no energy)!");
                    }
                }
                return true;
            }

            return false;
        }

        private static void LogHitResult(BattleCharacter user, BattleCharacter target,
            HitResult result, int hitNumber, int totalHits)
        {
            string prefix = totalHits > 1 ? $"Hit {hitNumber}: " : "";

            if (result.Missed)
                Log($"{prefix}{user.Data.Name} missed!");
            else if (result.Dodged)
                Log($"{prefix}{target.Data.Name} dodged!");
            else if (result.Blocked)
                Log($"{prefix}{target.Data.Name} blocked!");
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

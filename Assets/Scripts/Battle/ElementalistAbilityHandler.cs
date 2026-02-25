using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PixelWarriors
{
    public static class ElementalistAbilityHandler
    {
        public static void ExecuteEnergyBolt(BattleCharacter user, AbilityData ability, List<BattleCharacter> targets)
        {
            Element element = user.LastSpellElement;
            Log($"{user.Data.Name}'s Energy Bolt takes on {element} element!");

            // Arcane: refund mana cost
            if (element == Element.Arcane)
            {
                user.CurrentMana = Mathf.Min(user.CurrentMana + ability.ManaCost, user.MaxMana);
                Log("Arcane resonance! Mana refunded.");
            }

            foreach (BattleCharacter target in targets)
            {
                if (!target.IsAlive) continue;

                GameEvents.RaiseAbilityUsed(user, ability, target);

                // Earth: physical damage + bonus
                DamageType dmgType = element == Element.Earth ? DamageType.Physical : DamageType.Magical;

                HitResult result = ActionExecutor.ResolveHit(user, ability, target);
                GameEvents.RaiseHitResolved(target, result, dmgType);

                if (result.IsEffective)
                {
                    int damage = result.Damage;

                    // Earth bonus damage
                    if (element == Element.Earth)
                        damage = Mathf.RoundToInt(damage * 1.3f);

                    target.CurrentHP = Mathf.Max(0, target.CurrentHP - damage);
                    GameEvents.RaiseDamageDealt(target, damage, dmgType);
                    Log($"{damage} damage!");

                    // Fire: apply Burn 2
                    if (element == Element.Fire && target.IsAlive)
                    {
                        var burn = new StatusEffectInstance(StatusEffect.Burn, GameplayConfig.BurnDuration, 2, user);
                        target.AddEffect(burn);
                        GameEvents.RaiseStatusEffectApplied(target, StatusEffect.Burn, 2);
                        Log($"{target.Data.Name} is set on fire!");
                    }

                    // Water: apply Chilled 1
                    if (element == Element.Water && target.IsAlive)
                    {
                        var chill = new StatusEffectInstance(StatusEffect.Chilled, 1, 0, user);
                        target.AddEffect(chill);
                        GameEvents.RaiseStatusEffectApplied(target, StatusEffect.Chilled, 1);
                        Log($"{target.Data.Name} is chilled!");
                    }

                    // Air: bounce to random enemy
                    if (element == Element.Air && target.IsAlive)
                    {
                        List<BattleCharacter> foes = targets.Where(t => t.IsAlive && t != target).ToList();
                        if (foes.Count > 0)
                        {
                            BattleCharacter bounceTarget = foes[Random.Range(0, foes.Count)];
                            int bounceDamage = Mathf.Max(1, Mathf.RoundToInt(damage * 0.5f));
                            bounceTarget.CurrentHP = Mathf.Max(0, bounceTarget.CurrentHP - bounceDamage);
                            GameEvents.RaiseDamageDealt(bounceTarget, bounceDamage, dmgType);
                            Log($"Bolt bounces to {bounceTarget.Data.Name} for {bounceDamage}!");
                            ActionExecutor.CheckDefeated(bounceTarget);
                        }
                    }
                }
                else
                {
                    LogMissOrDodge(user, target, result);
                }

                ActionExecutor.CheckDefeated(target);
            }
        }

        public static void ExecuteIgnite(BattleCharacter user, AbilityData ability, List<BattleCharacter> targets)
        {
            user.LastSpellElement = Element.Fire;

            foreach (BattleCharacter target in targets)
            {
                if (!target.IsAlive) continue;

                GameEvents.RaiseAbilityUsed(user, ability, target);

                var burn = new StatusEffectInstance(StatusEffect.Burn, GameplayConfig.BurnDuration, 2, user);
                target.AddEffect(burn);
                GameEvents.RaiseStatusEffectApplied(target, StatusEffect.Burn, 2);
                Log($"{user.Data.Name} ignites {target.Data.Name}! Fire 2 applied.");
            }
        }

        public static void ExecuteSteamBeam(BattleCharacter user, AbilityData ability, List<BattleCharacter> targets)
        {
            user.LastSpellElement = Element.Water;

            foreach (BattleCharacter target in targets)
            {
                if (!target.IsAlive) continue;

                GameEvents.RaiseAbilityUsed(user, ability, target);
                Log($"{user.Data.Name} fires a steam beam at {target.Data.Name}!");

                HitResult result = ActionExecutor.ResolveHit(user, ability, target);
                GameEvents.RaiseHitResolved(target, result, ability.DamageType);

                if (result.IsEffective)
                {
                    target.CurrentHP = Mathf.Max(0, target.CurrentHP - result.Damage);
                    GameEvents.RaiseDamageDealt(target, result.Damage, ability.DamageType);
                    Log($"{result.Damage} damage!");

                    if (target.IsAlive)
                    {
                        var debuff = new StatusEffectInstance(StatusEffect.SteamBeamDebuff, 2, 0, user);
                        target.AddEffect(debuff);
                        GameEvents.RaiseStatusEffectApplied(target, StatusEffect.SteamBeamDebuff, 0);
                        Log($"{target.Data.Name} takes increased damage!");
                    }
                }
                else
                {
                    LogMissOrDodge(user, target, result);
                }

                ActionExecutor.CheckDefeated(target);
            }
        }

        public static void ExecuteWaveCrash(BattleCharacter user, AbilityData ability, List<BattleCharacter> targets)
        {
            user.LastSpellElement = Element.Water;

            foreach (BattleCharacter target in targets)
            {
                if (!target.IsAlive) continue;

                GameEvents.RaiseAbilityUsed(user, ability, target);
                Log($"{user.Data.Name} crashes a wave into {target.Data.Name}!");

                HitResult result = ActionExecutor.ResolveHit(user, ability, target);
                GameEvents.RaiseHitResolved(target, result, ability.DamageType);

                if (result.IsEffective)
                {
                    target.CurrentHP = Mathf.Max(0, target.CurrentHP - result.Damage);
                    GameEvents.RaiseDamageDealt(target, result.Damage, ability.DamageType);
                    Log($"{result.Damage} damage!");

                    // Swap target to back row
                    if (target.IsAlive && target.Row == GridRow.Front)
                    {
                        target.Row = GridRow.Back;
                        GameEvents.RaisePositionSwapped(target, target);
                        Log($"{target.Data.Name} is pushed to the back row!");
                    }
                }
                else
                {
                    LogMissOrDodge(user, target, result);
                }

                ActionExecutor.CheckDefeated(target);
            }
        }

        public static void ExecuteLevitate(BattleCharacter user, AbilityData ability, List<BattleCharacter> targets)
        {
            user.LastSpellElement = Element.Air;

            foreach (BattleCharacter target in targets)
            {
                if (!target.IsAlive) continue;

                GameEvents.RaiseAbilityUsed(user, ability, target);

                var effect = new StatusEffectInstance(StatusEffect.Levitate, 3, 0, user);
                target.AddEffect(effect);
                GameEvents.RaiseStatusEffectApplied(target, StatusEffect.Levitate, 0);
                Log($"{target.Data.Name} is levitated! Lower aggro, close skills hit any target.");
            }
        }

        public static void ExecuteSealOfElements(BattleCharacter user, AbilityData ability, List<BattleCharacter> targets)
        {
            Element element = user.LastSpellElement;
            Log($"{user.Data.Name} casts Seal of Elements ({element})!");

            foreach (BattleCharacter target in targets)
            {
                if (!target.IsAlive) continue;

                GameEvents.RaiseAbilityUsed(user, ability, target);

                switch (element)
                {
                    case Element.Fire:
                        // Attacks against this enemy replenish mana
                        var fireSeal = new StatusEffectInstance(StatusEffect.Mark, 3, 0, user);
                        target.AddEffect(fireSeal);
                        GameEvents.RaiseStatusEffectApplied(target, StatusEffect.Mark, 0);
                        Log($"{target.Data.Name} is fire-sealed! Attacks replenish mana.");
                        break;

                    case Element.Water:
                        // Enemy is silenced for 2 turns
                        var silence = new StatusEffectInstance(StatusEffect.Silence, 2, 0, user);
                        target.AddEffect(silence);
                        GameEvents.RaiseStatusEffectApplied(target, StatusEffect.Silence, 0);
                        Log($"{target.Data.Name} is water-sealed! Silenced for 2 turns.");
                        break;

                    case Element.Earth:
                        // Target must choose frontline target, damage decreased
                        var earthSeal = new StatusEffectInstance(StatusEffect.SteamBeamDebuff, 2, 0, user);
                        target.AddEffect(earthSeal);
                        GameEvents.RaiseStatusEffectApplied(target, StatusEffect.SteamBeamDebuff, 0);
                        Log($"{target.Data.Name} is earth-sealed! Weakened for 2 turns.");
                        break;

                    case Element.Air:
                        // Attacks against this enemy replenish energy
                        var airSeal = new StatusEffectInstance(StatusEffect.Mark, 3, 0, user);
                        target.AddEffect(airSeal);
                        GameEvents.RaiseStatusEffectApplied(target, StatusEffect.Mark, 0);
                        Log($"{target.Data.Name} is air-sealed! Attacks replenish energy.");
                        break;

                    default:
                        // Arcane: increased magic damage for 3 turns
                        var arcaneSeal = new StatusEffectInstance(StatusEffect.SteamBeamDebuff, 3, 0, user);
                        target.AddEffect(arcaneSeal);
                        GameEvents.RaiseStatusEffectApplied(target, StatusEffect.SteamBeamDebuff, 0);
                        Log($"{target.Data.Name} is arcane-sealed! Takes more magic damage.");
                        break;
                }
            }
        }

        public static void ExecuteArcaneBurst(BattleCharacter user, AbilityData ability, List<BattleCharacter> targets)
        {
            foreach (BattleCharacter target in targets)
            {
                if (!target.IsAlive) continue;

                GameEvents.RaiseAbilityUsed(user, ability, target);

                // Bonus damage based on target's mana pool
                int bonusDamage = Mathf.RoundToInt(target.MaxMana * 0.5f);
                int totalPower = ability.BasePower + bonusDamage;

                int magicPen = user.Data.GetMagicPenetration() + ability.AbilityMagicPen;
                int damage = StatCalculator.CalculateSpellDamage(
                    totalPower, user.EffectiveStats.Willpower, target.EffectiveStats.MagicResist, magicPen);
                target.CurrentHP = Mathf.Max(0, target.CurrentHP - damage);
                GameEvents.RaiseDamageDealt(target, damage, ability.DamageType);
                Log($"{user.Data.Name} fires Arcane Burst at {target.Data.Name}! {damage} damage!");

                ActionExecutor.CheckDefeated(target);
            }
        }

        public static void ExecuteSplinters(BattleCharacter user, AbilityData ability, List<BattleCharacter> targets)
        {
            user.LastSpellElement = Element.Earth;

            foreach (BattleCharacter target in targets)
            {
                if (!target.IsAlive) continue;

                GameEvents.RaiseAbilityUsed(user, ability, target);

                HitResult result = ActionExecutor.ResolveHit(user, ability, target);
                GameEvents.RaiseHitResolved(target, result, ability.DamageType);

                if (result.IsEffective)
                {
                    target.CurrentHP = Mathf.Max(0, target.CurrentHP - result.Damage);
                    GameEvents.RaiseDamageDealt(target, result.Damage, ability.DamageType);
                    Log($"{result.Damage} damage!");

                    // Chance to apply bleed
                    if (target.IsAlive && Random.value < 0.4f)
                    {
                        var bleed = new StatusEffectInstance(StatusEffect.Bleed, GameplayConfig.BleedDuration, 0, user);
                        target.AddEffect(bleed);
                        GameEvents.RaiseStatusEffectApplied(target, StatusEffect.Bleed, 0);
                        Log($"{target.Data.Name} is bleeding!");
                    }
                }
                else
                {
                    LogMissOrDodge(user, target, result);
                }

                ActionExecutor.CheckDefeated(target);
            }
        }

        public static void ExecuteInvisibility(BattleCharacter user, AbilityData ability, List<BattleCharacter> targets)
        {
            foreach (BattleCharacter target in targets)
            {
                if (!target.IsAlive) continue;

                GameEvents.RaiseAbilityUsed(user, ability, target);

                var effect = new StatusEffectInstance(StatusEffect.Conceal, -1, 0, user);
                target.AddEffect(effect);
                GameEvents.RaiseStatusEffectApplied(target, StatusEffect.Conceal, 0);
                Log($"{user.Data.Name} makes {target.Data.Name} invisible!");
            }
        }

        public static void ExecuteMeltdown(BattleCharacter user, AbilityData ability, List<BattleCharacter> targets)
        {
            user.LastSpellElement = Element.Fire;

            foreach (BattleCharacter target in targets)
            {
                if (!target.IsAlive) continue;

                GameEvents.RaiseAbilityUsed(user, ability, target);

                // Bonus damage based on target's armor
                int bonusDamage = target.EffectiveStats.Armor;
                int totalPower = ability.BasePower + bonusDamage;

                int magicPen = user.Data.GetMagicPenetration() + ability.AbilityMagicPen;
                int damage = StatCalculator.CalculateSpellDamage(
                    totalPower, user.EffectiveStats.Willpower, target.EffectiveStats.MagicResist, magicPen);
                target.CurrentHP = Mathf.Max(0, target.CurrentHP - damage);
                GameEvents.RaiseDamageDealt(target, damage, ability.DamageType);
                Log($"{user.Data.Name} melts {target.Data.Name}'s defenses! {damage} damage!");

                ActionExecutor.CheckDefeated(target);
            }
        }

        public static void ExecuteElementalArmor(BattleCharacter user, AbilityData ability, List<BattleCharacter> targets)
        {
            Element element = user.LastSpellElement;

            foreach (BattleCharacter target in targets)
            {
                if (!target.IsAlive) continue;

                GameEvents.RaiseAbilityUsed(user, ability, target);

                var effect = new StatusEffectInstance(StatusEffect.ElementalArmor, 3, (int)element, user);
                target.AddEffect(effect);
                GameEvents.RaiseStatusEffectApplied(target, StatusEffect.ElementalArmor, (int)element);
                Log($"{user.Data.Name} grants {target.Data.Name} elemental armor ({element})!");
            }
        }

        public static void ExecuteImbueStaff(BattleCharacter user)
        {
            var effect = new StatusEffectInstance(StatusEffect.Imbue, 3, 0, user);
            user.AddEffect(effect);
            GameEvents.RaiseAbilityUsed(user, null, user);
            GameEvents.RaiseStatusEffectApplied(user, StatusEffect.Imbue, 0);
            Log($"{user.Data.Name} imbues their staff! Attacks deal bonus damage for 3 turns.");
        }

        public static void ExecuteChainLightning(BattleCharacter user, AbilityData ability,
            List<BattleCharacter> targets, List<BattleCharacter> allEnemies)
        {
            user.LastSpellElement = Element.Air;

            foreach (BattleCharacter target in targets)
            {
                if (!target.IsAlive) continue;

                GameEvents.RaiseAbilityUsed(user, ability, target);
                Log($"{user.Data.Name} casts Chain Lightning at {target.Data.Name}!");

                HitResult result = ActionExecutor.ResolveHit(user, ability, target);
                GameEvents.RaiseHitResolved(target, result, ability.DamageType);

                if (result.IsEffective)
                {
                    target.CurrentHP = Mathf.Max(0, target.CurrentHP - result.Damage);
                    GameEvents.RaiseDamageDealt(target, result.Damage, ability.DamageType);
                    Log($"{result.Damage} damage!");

                    // Bounce to up to 2 other enemies at 50% damage
                    List<BattleCharacter> bounceTargets = allEnemies
                        .Where(e => e.IsAlive && e != target).ToList();

                    int bounces = Mathf.Min(GameplayConfig.ChainLightningMaxBounces, bounceTargets.Count);
                    int bounceDamage = Mathf.Max(1,
                        Mathf.RoundToInt(result.Damage * GameplayConfig.ChainLightningBounceMultiplier));

                    for (int i = 0; i < bounces; i++)
                    {
                        int idx = Random.Range(0, bounceTargets.Count);
                        BattleCharacter bounceTarget = bounceTargets[idx];
                        bounceTargets.RemoveAt(idx);

                        bounceTarget.CurrentHP = Mathf.Max(0, bounceTarget.CurrentHP - bounceDamage);
                        GameEvents.RaiseDamageDealt(bounceTarget, bounceDamage, ability.DamageType);
                        Log($"Lightning bounces to {bounceTarget.Data.Name} for {bounceDamage}!");
                        ActionExecutor.CheckDefeated(bounceTarget);
                    }
                }
                else
                {
                    LogMissOrDodge(user, target, result);
                }

                ActionExecutor.CheckDefeated(target);
            }
        }

        public static void ExecuteFrozenTomb(BattleCharacter user, AbilityData ability, List<BattleCharacter> targets)
        {
            user.LastSpellElement = Element.Water;

            foreach (BattleCharacter target in targets)
            {
                if (!target.IsAlive) continue;

                GameEvents.RaiseAbilityUsed(user, ability, target);

                if (!ActionExecutor.RollSpellHit(user, ability, target))
                {
                    Log($"{target.Data.Name} resists the Frozen Tomb!");
                    continue;
                }

                var effect = new StatusEffectInstance(StatusEffect.FrozenTomb,
                    GameplayConfig.FrozenTombDuration, 0, user);
                target.AddEffect(effect);
                GameEvents.RaiseStatusEffectApplied(target, StatusEffect.FrozenTomb, 0);
                Log($"{user.Data.Name} entombs {target.Data.Name} in ice! Stunned but immune.");
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

using System.Collections.Generic;
using UnityEngine;

namespace PixelWarriors
{
    public static class PriestAbilityHandler
    {
        public static void ExecutePrayerOfMending(BattleCharacter user, AbilityData ability, List<BattleCharacter> targets)
        {
            foreach (BattleCharacter target in targets)
            {
                if (!target.IsAlive) continue;

                GameEvents.RaiseAbilityUsed(user, ability, target);

                int healPerTurn = GameplayConfig.RegenerationHealPerTurn;

                // Faith passive boosts the HoT value
                if (PassiveProcessor.HasPassive(user, "Faith"))
                    healPerTurn = Mathf.RoundToInt(healPerTurn * (1f + GameplayConfig.FaithHealingBonus));

                var effect = new StatusEffectInstance(StatusEffect.Regeneration,
                    GameplayConfig.RegenerationDuration, healPerTurn, user);
                target.AddEffect(effect);

                GameEvents.RaiseStatusEffectApplied(target, StatusEffect.Regeneration, 0);
                GameEvents.RaiseCombatLogMessage(
                    $"{user.Data.Name} casts Prayer of Mending on {target.Data.Name}! Healing {healPerTurn}/turn.");

                // Devotion: also grant shield
                if (PassiveProcessor.HasPassive(user, "Devotion"))
                {
                    var shield = new StatusEffectInstance(StatusEffect.Shield, -1,
                        GameplayConfig.DevotionShieldValue, user);
                    target.AddEffect(shield);
                    GameEvents.RaiseStatusEffectApplied(target, StatusEffect.Shield,
                        GameplayConfig.DevotionShieldValue);
                }
            }
        }

        public static void ExecuteHolyWard(BattleCharacter user, AbilityData ability, List<BattleCharacter> targets)
        {
            GameEvents.RaiseCombatLogMessage($"{user.Data.Name} casts Holy Ward!");

            foreach (BattleCharacter target in targets)
            {
                if (!target.IsAlive) continue;

                GameEvents.RaiseAbilityUsed(user, ability, target);

                int shieldValue = ability.BasePower;

                // Faith passive boosts shield value
                if (PassiveProcessor.HasPassive(user, "Faith"))
                    shieldValue = Mathf.RoundToInt(shieldValue * (1f + GameplayConfig.FaithHealingBonus));

                var effect = new StatusEffectInstance(StatusEffect.Shield, -1, shieldValue, user);
                target.AddEffect(effect);
                GameEvents.RaiseStatusEffectApplied(target, StatusEffect.Shield, shieldValue);
                GameEvents.RaiseCombatLogMessage($"  {target.Data.Name} gains Shield: {shieldValue}");
            }
        }

        public static void ExecutePurify(BattleCharacter user, AbilityData ability, List<BattleCharacter> targets)
        {
            foreach (BattleCharacter target in targets)
            {
                if (!target.IsAlive) continue;

                GameEvents.RaiseAbilityUsed(user, ability, target);

                int removed = 0;
                for (int i = target.StatusEffects.Count - 1; i >= 0; i--)
                {
                    StatusEffectInstance effect = target.StatusEffects[i];
                    if (IsNegativeStatusEffect(effect.Type))
                    {
                        target.StatusEffects.RemoveAt(i);
                        GameEvents.RaiseStatusEffectRemoved(target, effect.Type);
                        removed++;
                    }
                }

                if (removed > 0)
                    GameEvents.RaiseCombatLogMessage(
                        $"{user.Data.Name} purifies {target.Data.Name}! {removed} effect(s) removed.");
                else
                    GameEvents.RaiseCombatLogMessage(
                        $"{user.Data.Name} purifies {target.Data.Name}! No effects to remove.");
            }
        }

        public static void ExecuteResurrect(BattleCharacter user, AbilityData ability, List<BattleCharacter> targets)
        {
            foreach (BattleCharacter target in targets)
            {
                if (target.IsAlive) continue; // Should only target dead allies

                GameEvents.RaiseAbilityUsed(user, ability, target);

                int reviveHP = Mathf.Max(1, Mathf.RoundToInt(target.MaxHP * GameplayConfig.ResurrectHPPercent));
                target.CurrentHP = reviveHP;

                // Clear all status effects on revived character
                target.StatusEffects.Clear();

                GameEvents.RaiseHealingReceived(target, reviveHP);
                GameEvents.RaiseCombatLogMessage(
                    $"{user.Data.Name} resurrects {target.Data.Name} with {reviveHP} HP!");
            }
        }

        public static void ExecuteBlessing(BattleCharacter user, AbilityData ability, List<BattleCharacter> targets)
        {
            foreach (BattleCharacter target in targets)
            {
                if (!target.IsAlive) continue;

                GameEvents.RaiseAbilityUsed(user, ability, target);

                var effect = new StatusEffectInstance(StatusEffect.Blessing,
                    GameplayConfig.BlessingDuration, 0, user);
                target.AddEffect(effect);

                GameEvents.RaiseStatusEffectApplied(target, StatusEffect.Blessing, 0);
                GameEvents.RaiseCombatLogMessage(
                    $"{user.Data.Name} blesses {target.Data.Name}! +{Mathf.RoundToInt(GameplayConfig.BlessingDamageBonus * 100)}% damage.");
            }
        }

        public static void ExecuteDivineIntervention(BattleCharacter user, AbilityData ability, List<BattleCharacter> targets)
        {
            foreach (BattleCharacter target in targets)
            {
                if (!target.IsAlive) continue;

                GameEvents.RaiseAbilityUsed(user, ability, target);

                var effect = new StatusEffectInstance(StatusEffect.DivineIntervention, 1, 0, user);
                target.AddEffect(effect);

                GameEvents.RaiseStatusEffectApplied(target, StatusEffect.DivineIntervention, 0);
                GameEvents.RaiseCombatLogMessage(
                    $"{user.Data.Name} casts Divine Intervention on {target.Data.Name}! Immune to damage!");
            }
        }

        private static bool IsNegativeStatusEffect(StatusEffect type)
        {
            return type == StatusEffect.Bleed || type == StatusEffect.Poison ||
                   type == StatusEffect.Burn || type == StatusEffect.Stun ||
                   type == StatusEffect.Silence || type == StatusEffect.Terror ||
                   type == StatusEffect.Chilled || type == StatusEffect.Confusion ||
                   type == StatusEffect.Mark || type == StatusEffect.Pin ||
                   type == StatusEffect.SteamBeamDebuff || type == StatusEffect.DrainSoul ||
                   type == StatusEffect.SoulLink || type == StatusEffect.CorpseExplosion ||
                   type == StatusEffect.LeechLife || type == StatusEffect.FrozenTomb;
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PixelWarriors
{
    public static class PassiveProcessor
    {
        public static void OnBattleStart(BattleCharacter character)
        {
            // Strategist: start battle concealed
            if (HasPassive(character, "Strategist"))
            {
                character.AddEffect(new StatusEffectInstance(StatusEffect.Conceal, -1, 0, character));
                GameEvents.RaiseStatusEffectApplied(character, StatusEffect.Conceal, 0);
                GameEvents.RaiseCombatLogMessage($"{character.Data.Name} starts concealed!");
            }
        }

        public static void OnTurnStart(BattleCharacter character)
        {
            // Enrage: +1 long action if below 30% HP
            if (HasPassive(character, "Enrage") &&
                character.CurrentHP < character.MaxHP * 0.30f)
            {
                character.LongActionsRemaining += 1;
                GameEvents.RaiseCombatLogMessage($"{character.Data.Name} is enraged! +1 action!");
            }
        }

        public static void OnDamageTaken(BattleCharacter target, int damage, List<BattleCharacter> allCharacters)
        {
            if (!target.IsAlive) return;

            // Escape Plan: become concealed below 50% HP (once per battle)
            if (HasPassive(target, "Escape Plan") &&
                !target.HasUsedOnce(AbilityTag.None) &&
                target.CurrentHP < target.MaxHP * 0.50f)
            {
                // Use a special tracking — mark via the passive name
                target.MarkUsedOnce(AbilityTag.Hide); // Reuse as once-per-battle flag
                target.AddEffect(new StatusEffectInstance(StatusEffect.Conceal, -1, 0, target));
                GameEvents.RaiseStatusEffectApplied(target, StatusEffect.Conceal, 0);
                GameEvents.RaiseCombatLogMessage($"{target.Data.Name}'s escape plan triggers! Concealed!");
            }

            // Dark Pact: gain mana when taking damage
            if (HasPassive(target, "Dark Pact"))
            {
                int manaGain = Mathf.Max(1, Mathf.RoundToInt(damage * 0.2f));
                target.CurrentMana = Mathf.Min(target.CurrentMana + manaGain, target.MaxMana);
                GameEvents.RaiseCombatLogMessage($"{target.Data.Name} gains {manaGain} mana from pain!");
            }

            // Stance triggers
            // (handled in WarriorAbilityHandler.ProcessStanceTriggers, called from damage pipeline)
        }

        public static void OnCharacterDefeated(BattleCharacter defeated, List<BattleCharacter> allCharacters)
        {
            // Soul Harvest: recover HP, energy, and mana when any character dies
            foreach (BattleCharacter character in allCharacters)
            {
                if (!character.IsAlive) continue;
                if (character == defeated) continue;
                if (!HasPassive(character, "Soul Harvest")) continue;

                int hpGain = Mathf.Max(1, Mathf.RoundToInt(character.MaxHP * 0.10f));
                int energyGain = Mathf.Max(1, Mathf.RoundToInt(character.MaxEnergy * 0.15f));
                int manaGain = Mathf.Max(1, Mathf.RoundToInt(character.MaxMana * 0.15f));

                character.CurrentHP = Mathf.Min(character.CurrentHP + hpGain, character.MaxHP);
                character.CurrentEnergy = Mathf.Min(character.CurrentEnergy + energyGain, character.MaxEnergy);
                character.CurrentMana = Mathf.Min(character.CurrentMana + manaGain, character.MaxMana);

                GameEvents.RaiseHealingReceived(character, hpGain);
                GameEvents.RaiseCombatLogMessage(
                    $"{character.Data.Name} harvests the fallen soul! +{hpGain} HP, +{energyGain} EN, +{manaGain} MP");
            }

            // Corpse Explosion: if the defeated character was marked, deal AoE
            StatusEffectInstance corpseExplosion = defeated.GetEffect(StatusEffect.CorpseExplosion);
            if (corpseExplosion != null)
            {
                int explosionDamage = corpseExplosion.Value;
                BattleCharacter source = corpseExplosion.Source;
                GameEvents.RaiseCombatLogMessage($"{defeated.Data.Name} explodes!");

                // Find all living enemies of the source
                foreach (BattleCharacter target in allCharacters)
                {
                    if (!target.IsAlive) continue;
                    if (target == defeated) continue;
                    if (source != null && target.Side == source.Side) continue;

                    target.CurrentHP = Mathf.Max(0, target.CurrentHP - explosionDamage);
                    GameEvents.RaiseDamageDealt(target, explosionDamage, DamageType.Magical);
                    GameEvents.RaiseCombatLogMessage($"{target.Data.Name} takes {explosionDamage} explosion damage!");
                    ActionExecutor.CheckDefeated(target);
                }
            }
        }

        private static bool HasPassive(BattleCharacter character, string passiveName)
        {
            return character.Data.Passives.Any(p => p.Name == passiveName);
        }
    }
}

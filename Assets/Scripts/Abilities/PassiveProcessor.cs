using System.Collections.Generic;
using UnityEngine;

namespace PixelWarriors
{
    public static class PassiveProcessor
    {
        public static void OnBattleStart(BattleCharacter character)
        {
            if (character.HasPassiveEffect(PassiveEffect.Strategist))
            {
                character.AddEffect(new StatusEffectInstance(StatusEffect.Conceal, -1, 0, character));
                GameEvents.RaiseStatusEffectApplied(character, StatusEffect.Conceal, 0);
                GameEvents.RaiseCombatLogMessage($"{character.Data.Name} starts concealed!");
            }
        }

        public static void OnTurnStart(BattleCharacter character)
        {
            if (character.HasPassiveEffect(PassiveEffect.Enrage) &&
                character.CurrentHP < character.MaxHP * 0.30f)
            {
                character.LongActionsRemaining += 1;
                GameEvents.RaiseCombatLogMessage($"{character.Data.Name} is enraged! +1 action!");
            }

            if (character.HasPassiveEffect(PassiveEffect.Survivalist))
            {
                int gain = GameplayConfig.SurvivalistEnergyRegen;
                int prev = character.CurrentEnergy;
                character.CurrentEnergy = Mathf.Min(character.CurrentEnergy + gain, character.MaxEnergy);
                int actual = character.CurrentEnergy - prev;
                if (actual > 0)
                    GameEvents.RaiseCombatLogMessage($"{character.Data.Name} recovers {actual} energy (Survivalist).");
            }

            if (character.HasPassiveEffect(PassiveEffect.InnerLight))
            {
                int gain = GameplayConfig.InnerLightManaRegen;
                int prev = character.CurrentMana;
                character.CurrentMana = Mathf.Min(character.CurrentMana + gain, character.MaxMana);
                int actual = character.CurrentMana - prev;
                if (actual > 0)
                    GameEvents.RaiseCombatLogMessage($"{character.Data.Name} recovers {actual} mana (Inner Light).");
            }
        }

        public static void OnDamageTaken(BattleCharacter target, int damage, List<BattleCharacter> allCharacters)
        {
            if (!target.IsAlive) return;

            if (target.HasPassiveEffect(PassiveEffect.EscapePlan) &&
                !target.HasUsedOnce(AbilityTag.EscapePlan) &&
                target.CurrentHP < target.MaxHP * 0.50f)
            {
                target.MarkUsedOnce(AbilityTag.EscapePlan);
                target.AddEffect(new StatusEffectInstance(StatusEffect.Conceal, -1, 0, target));
                GameEvents.RaiseStatusEffectApplied(target, StatusEffect.Conceal, 0);
                GameEvents.RaiseCombatLogMessage($"{target.Data.Name}'s escape plan triggers! Concealed!");
            }

            if (target.HasPassiveEffect(PassiveEffect.DarkPact))
            {
                int manaGain = Mathf.Max(1, Mathf.RoundToInt(damage * 0.2f));
                target.CurrentMana = Mathf.Min(target.CurrentMana + manaGain, target.MaxMana);
                GameEvents.RaiseCombatLogMessage($"{target.Data.Name} gains {manaGain} mana from pain!");
            }
        }

        public static void OnCharacterDefeated(BattleCharacter defeated, List<BattleCharacter> allCharacters)
        {
            foreach (BattleCharacter character in allCharacters)
            {
                if (!character.IsAlive) continue;
                if (character == defeated) continue;
                if (!character.HasPassiveEffect(PassiveEffect.SoulHarvest)) continue;

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

            StatusEffectInstance corpseExplosion = defeated.GetEffect(StatusEffect.CorpseExplosion);
            if (corpseExplosion != null)
            {
                int explosionDamage = corpseExplosion.Value;
                BattleCharacter source = corpseExplosion.Source;
                GameEvents.RaiseCombatLogMessage($"{defeated.Data.Name} explodes!");

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
    }
}

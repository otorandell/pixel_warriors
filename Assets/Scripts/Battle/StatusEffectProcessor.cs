using UnityEngine;

namespace PixelWarriors
{
    public static class StatusEffectProcessor
    {
        public static void ProcessTurnStart(BattleCharacter character)
        {
            // Anticipate: grant positive priority (already set when applied)
            if (character.HasEffect(StatusEffect.Anticipate))
            {
                character.ShortActionsRemaining = Mathf.Max(0, character.ShortActionsRemaining - 1);
                character.RemoveEffect(StatusEffect.Anticipate);
                GameEvents.RaiseStatusEffectRemoved(character, StatusEffect.Anticipate);
            }

            // Prepare: bonus regen then remove
            if (character.HasEffect(StatusEffect.Prepare))
            {
                int bonusEnergy = Mathf.Max(1, Mathf.RoundToInt(character.MaxEnergy * GameplayConfig.PrepareRegenBonus));
                int bonusMana = Mathf.Max(1, Mathf.RoundToInt(character.MaxMana * GameplayConfig.PrepareRegenBonus));
                character.CurrentEnergy = Mathf.Min(character.CurrentEnergy + bonusEnergy, character.MaxEnergy);
                character.CurrentMana = Mathf.Min(character.CurrentMana + bonusMana, character.MaxMana);
                character.RemoveEffect(StatusEffect.Prepare);
                GameEvents.RaiseStatusEffectRemoved(character, StatusEffect.Prepare);
            }

            // Protect/Hide: removed at turn start (lasted since last turn)
            if (character.HasEffect(StatusEffect.Protect))
            {
                character.RemoveEffect(StatusEffect.Protect);
                GameEvents.RaiseStatusEffectRemoved(character, StatusEffect.Protect);
            }

            if (character.HasEffect(StatusEffect.Hide))
            {
                character.RemoveEffect(StatusEffect.Hide);
                GameEvents.RaiseStatusEffectRemoved(character, StatusEffect.Hide);
            }
        }

        public static void ProcessTurnEnd(BattleCharacter character)
        {
            for (int i = character.StatusEffects.Count - 1; i >= 0; i--)
            {
                StatusEffectInstance effect = character.StatusEffects[i];
                if (effect.RemainingTurns > 0)
                {
                    effect.RemainingTurns--;
                    if (effect.IsExpired)
                    {
                        character.StatusEffects.RemoveAt(i);
                        GameEvents.RaiseStatusEffectRemoved(character, effect.Type);
                    }
                }
            }
        }

        public static int AbsorbDamage(BattleCharacter target, int damage)
        {
            StatusEffectInstance shield = target.GetEffect(StatusEffect.Shield);
            if (shield == null) return damage;

            if (damage <= shield.Value)
            {
                shield.Value -= damage;
                return 0;
            }

            int remaining = damage - shield.Value;
            target.RemoveEffect(StatusEffect.Shield);
            GameEvents.RaiseStatusEffectRemoved(target, StatusEffect.Shield);
            return remaining;
        }

        public static float GetMarkBonus(BattleCharacter target)
        {
            if (target.HasEffect(StatusEffect.Mark))
                return 1f + GameplayConfig.MarkDamageBonus;
            return 1f;
        }

        public static float GetAggroModifier(BattleCharacter character)
        {
            if (character.HasEffect(StatusEffect.Protect))
                return GameplayConfig.ProtectAggroMultiplier;
            if (character.HasEffect(StatusEffect.Hide))
                return GameplayConfig.HideAggroMultiplier;
            return 1f;
        }
    }
}

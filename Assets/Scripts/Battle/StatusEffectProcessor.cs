using UnityEngine;

namespace PixelWarriors
{
    public static class StatusEffectProcessor
    {
        public static void ProcessTurnStart(BattleCharacter character)
        {
            // Stun: zero all actions, character skips turn naturally
            if (character.HasEffect(StatusEffect.Stun))
            {
                character.LongActionsRemaining = 0;
                character.ShortActionsRemaining = 0;
                character.RemoveEffect(StatusEffect.Stun);
                GameEvents.RaiseStatusEffectRemoved(character, StatusEffect.Stun);
                GameEvents.RaiseCombatLogMessage($"{character.Data.Name} is stunned and cannot act!");
            }

            // Chilled: act last this turn
            if (character.HasEffect(StatusEffect.Chilled))
            {
                character.Priority = Priority.Negative;
            }

            // Anticipate: grant positive priority (already set when applied), -1 short action
            if (character.HasEffect(StatusEffect.Anticipate))
            {
                character.ShortActionsRemaining = Mathf.Max(0, character.ShortActionsRemaining - 1);
                character.RemoveEffect(StatusEffect.Anticipate);
                GameEvents.RaiseStatusEffectRemoved(character, StatusEffect.Anticipate);
            }

            // React: set negative priority (already set when applied), then remove
            if (character.HasEffect(StatusEffect.React))
            {
                character.RemoveEffect(StatusEffect.React);
                GameEvents.RaiseStatusEffectRemoved(character, StatusEffect.React);
            }

            // Hide: removed at turn start (lasted since last turn)
            if (character.HasEffect(StatusEffect.Hide))
            {
                character.RemoveEffect(StatusEffect.Hide);
                GameEvents.RaiseStatusEffectRemoved(character, StatusEffect.Hide);
            }
        }

        public static void ProcessTurnEnd(BattleCharacter character)
        {
            // Process DoTs before duration ticking
            ProcessBleed(character);
            ProcessPoison(character);
            ProcessBurn(character);
            ProcessLeechLife(character);

            // Tick durations
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

            // Stance energy check: cancel stance if energy depleted
            CheckStanceEnergy(character, StatusEffect.StanceDefensive);
            CheckStanceEnergy(character, StatusEffect.StanceBrawling);
        }

        private static void ProcessBleed(BattleCharacter character)
        {
            var bleeds = character.GetAllEffects(StatusEffect.Bleed);
            if (bleeds.Count == 0) return;

            int totalDamage = bleeds.Count * GameplayConfig.BleedDamagePerStack;
            character.CurrentHP = Mathf.Max(0, character.CurrentHP - totalDamage);
            GameEvents.RaiseDamageDealt(character, totalDamage, DamageType.Physical);
            GameEvents.RaiseCombatLogMessage(
                $"{character.Data.Name} takes {totalDamage} bleed damage! ({bleeds.Count} stacks)");

            // Tick bleed stacks individually
            for (int i = bleeds.Count - 1; i >= 0; i--)
            {
                bleeds[i].RemainingTurns--;
                if (bleeds[i].IsExpired)
                {
                    character.StatusEffects.Remove(bleeds[i]);
                    GameEvents.RaiseStatusEffectRemoved(character, StatusEffect.Bleed);
                }
            }

            if (!character.IsAlive) GameEvents.RaiseCharacterDefeated(character);
        }

        private static void ProcessPoison(BattleCharacter character)
        {
            if (!character.HasEffect(StatusEffect.Poison)) return;

            character.CurrentHP = Mathf.Max(0, character.CurrentHP - GameplayConfig.PoisonDamagePerTurn);
            GameEvents.RaiseDamageDealt(character, GameplayConfig.PoisonDamagePerTurn, DamageType.Physical);
            GameEvents.RaiseCombatLogMessage(
                $"{character.Data.Name} takes {GameplayConfig.PoisonDamagePerTurn} poison damage!");

            if (!character.IsAlive) GameEvents.RaiseCharacterDefeated(character);
        }

        private static void ProcessBurn(BattleCharacter character)
        {
            if (!character.HasEffect(StatusEffect.Burn)) return;

            character.CurrentHP = Mathf.Max(0, character.CurrentHP - GameplayConfig.BurnDamagePerTurn);
            GameEvents.RaiseDamageDealt(character, GameplayConfig.BurnDamagePerTurn, DamageType.Magical);
            GameEvents.RaiseCombatLogMessage(
                $"{character.Data.Name} takes {GameplayConfig.BurnDamagePerTurn} burn damage!");

            if (!character.IsAlive) GameEvents.RaiseCharacterDefeated(character);
        }

        private static void ProcessLeechLife(BattleCharacter character)
        {
            StatusEffectInstance leech = character.GetEffect(StatusEffect.LeechLife);
            if (leech == null) return;

            int drain = leech.Value;
            character.CurrentHP = Mathf.Max(0, character.CurrentHP - drain);
            GameEvents.RaiseDamageDealt(character, drain, DamageType.Magical);
            GameEvents.RaiseCombatLogMessage($"{character.Data.Name} is drained for {drain} HP!");

            // Heal the source
            if (leech.Source != null && leech.Source.IsAlive)
            {
                leech.Source.CurrentHP = Mathf.Min(leech.Source.CurrentHP + drain, leech.Source.MaxHP);
                GameEvents.RaiseHealingReceived(leech.Source, drain);
            }

            if (!character.IsAlive) GameEvents.RaiseCharacterDefeated(character);
        }

        private static void CheckStanceEnergy(BattleCharacter character, StatusEffect stanceType)
        {
            if (character.HasEffect(stanceType) && character.CurrentEnergy <= 0)
            {
                character.RemoveEffect(stanceType);
                GameEvents.RaiseStatusEffectRemoved(character, stanceType);
                GameEvents.RaiseCombatLogMessage($"{character.Data.Name}'s stance fades (no energy)!");
            }
        }

        // --- Modifier Queries ---

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
            if (character.HasEffect(StatusEffect.Conceal))
                return GameplayConfig.ConcealAggroMultiplier;
            if (character.HasEffect(StatusEffect.Taunt))
                return GameplayConfig.TauntAggroMultiplier;
            if (character.HasEffect(StatusEffect.Levitate))
                return GameplayConfig.HideAggroMultiplier;
            if (character.HasEffect(StatusEffect.Hide))
                return GameplayConfig.HideAggroMultiplier;
            return 1f;
        }
    }
}

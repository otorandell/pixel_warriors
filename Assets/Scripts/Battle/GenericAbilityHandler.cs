using System.Collections.Generic;
using UnityEngine;

namespace PixelWarriors
{
    public static class GenericAbilityHandler
    {
        public static void ExecuteSwap(BattleCharacter user, List<BattleCharacter> targets)
        {
            if (targets.Count == 0) return;
            BattleCharacter target = targets[0];

            if (user.HasEffect(StatusEffect.Pin))
            {
                Log($"{user.Data.Name} is pinned and cannot swap!");
                return;
            }
            if (target.HasEffect(StatusEffect.Pin))
            {
                Log($"{target.Data.Name} is pinned and cannot swap!");
                return;
            }

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

        public static void ExecuteReposition(BattleCharacter user)
        {
            if (user.HasEffect(StatusEffect.Pin))
            {
                Log($"{user.Data.Name} is pinned and cannot reposition!");
                return;
            }

            GridRow newRow = user.Row == GridRow.Front ? GridRow.Back : GridRow.Front;
            user.Row = newRow;

            GameEvents.RaiseAbilityUsed(user, null, user);
            GameEvents.RaisePositionSwapped(user, user);
            Log($"{user.Data.Name} moves to the {newRow.ToString().ToLower()} row!");
        }

        public static void ExecuteAnticipate(BattleCharacter user)
        {
            user.Priority = Priority.Positive;
            var effect = new StatusEffectInstance(StatusEffect.Anticipate, -1, 0, user);
            user.AddEffect(effect);

            GameEvents.RaiseAbilityUsed(user, null, user);
            GameEvents.RaiseStatusEffectApplied(user, StatusEffect.Anticipate, 0);
            Log($"{user.Data.Name} anticipates! Will act first next turn.");
        }

        public static void ExecuteReact(BattleCharacter user)
        {
            user.Priority = Priority.Negative;
            var effect = new StatusEffectInstance(StatusEffect.React, -1, 0, user);
            user.AddEffect(effect);

            GameEvents.RaiseAbilityUsed(user, null, user);
            GameEvents.RaiseStatusEffectApplied(user, StatusEffect.React, 0);
            Log($"{user.Data.Name} reacts! Will move last next turn.");
        }

        public static void ExecuteTaunt(BattleCharacter user)
        {
            var effect = new StatusEffectInstance(StatusEffect.Taunt, GameplayConfig.TauntDuration, 0, user);
            user.AddEffect(effect);

            GameEvents.RaiseAbilityUsed(user, null, user);
            GameEvents.RaiseStatusEffectApplied(user, StatusEffect.Taunt, 0);
            Log($"{user.Data.Name} taunts! Drawing enemy attention for {GameplayConfig.TauntDuration} turns.");
        }

        public static void ExecuteHide(BattleCharacter user)
        {
            var effect = new StatusEffectInstance(StatusEffect.Hide, -1, 0, user);
            user.AddEffect(effect);

            GameEvents.RaiseAbilityUsed(user, null, user);
            GameEvents.RaiseStatusEffectApplied(user, StatusEffect.Hide, 0);
            Log($"{user.Data.Name} hides! Less likely to be targeted.");
        }

        public static void ExecutePass(BattleCharacter user)
        {
            user.LongActionsRemaining = 0;
            user.ShortActionsRemaining = 0;

            GameEvents.RaiseAbilityUsed(user, null, user);
            Log($"{user.Data.Name} passes.");
        }

        private static void Log(string message) => GameEvents.RaiseCombatLogMessage(message);
    }
}

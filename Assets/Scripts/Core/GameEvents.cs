using System;

namespace PixelWarriors
{
    public static class GameEvents
    {
        // --- Battle Flow ---
        public static event Action OnBattleStarted;
        public static event Action OnBattleEnded;
        public static event Action<BattleState> OnBattleStateChanged;

        // --- Turn ---
        public static event Action<BattleCharacter> OnTurnStarted;
        public static event Action<BattleCharacter> OnTurnEnded;

        // --- Actions ---
        public static event Action<BattleCharacter, AbilityData, BattleCharacter> OnAbilityUsed;
        public static event Action<BattleCharacter, int, DamageType> OnDamageDealt;
        public static event Action<BattleCharacter, int> OnHealingReceived;
        public static event Action<BattleCharacter> OnCharacterDefeated;

        // --- UI ---
        public static event Action<BattleCharacter> OnCharacterSelected;
        public static event Action<AbilityData> OnAbilitySelected;
        public static event Action<BattleCharacter> OnTargetSelected;
        public static event Action<string> OnCombatLogMessage;

        public static void RaiseBattleStarted() => OnBattleStarted?.Invoke();
        public static void RaiseBattleEnded() => OnBattleEnded?.Invoke();
        public static void RaiseBattleStateChanged(BattleState state) => OnBattleStateChanged?.Invoke(state);
        public static void RaiseTurnStarted(BattleCharacter character) => OnTurnStarted?.Invoke(character);
        public static void RaiseTurnEnded(BattleCharacter character) => OnTurnEnded?.Invoke(character);

        public static void RaiseAbilityUsed(BattleCharacter user, AbilityData ability, BattleCharacter target)
            => OnAbilityUsed?.Invoke(user, ability, target);

        public static void RaiseDamageDealt(BattleCharacter target, int amount, DamageType type)
            => OnDamageDealt?.Invoke(target, amount, type);

        public static void RaiseHealingReceived(BattleCharacter target, int amount)
            => OnHealingReceived?.Invoke(target, amount);

        public static void RaiseCharacterDefeated(BattleCharacter character)
            => OnCharacterDefeated?.Invoke(character);

        public static void RaiseCharacterSelected(BattleCharacter character)
            => OnCharacterSelected?.Invoke(character);

        public static void RaiseAbilitySelected(AbilityData ability)
            => OnAbilitySelected?.Invoke(ability);

        public static void RaiseTargetSelected(BattleCharacter target)
            => OnTargetSelected?.Invoke(target);

        public static void RaiseCombatLogMessage(string message)
            => OnCombatLogMessage?.Invoke(message);
    }
}

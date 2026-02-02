using System;
using System.Collections.Generic;

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

        // --- Turn Order ---
        public static event Action<int, BattleCharacter, List<BattleCharacter>> OnTurnOrderUpdated;

        // --- Detail Popups ---
        public static event Action<BattleCharacter> OnCharacterDetailRequested;
        public static event Action<AbilityData> OnAbilityDetailRequested;

        // --- Staging / Confirmation ---
        public static event Action OnActionConfirmed;
        public static event Action OnActionCancelled;
        public static event Action<PlayerInputPhase> OnPlayerInputPhaseChanged;
        public static event Action<string> OnStagedActionChanged;

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

        public static void RaiseActionConfirmed() => OnActionConfirmed?.Invoke();
        public static void RaiseActionCancelled() => OnActionCancelled?.Invoke();

        public static void RaisePlayerInputPhaseChanged(PlayerInputPhase phase)
            => OnPlayerInputPhaseChanged?.Invoke(phase);

        public static void RaiseStagedActionChanged(string description)
            => OnStagedActionChanged?.Invoke(description);

        public static void RaiseTurnOrderUpdated(int roundNumber, BattleCharacter active, List<BattleCharacter> turnOrder)
            => OnTurnOrderUpdated?.Invoke(roundNumber, active, turnOrder);

        public static void RaiseCharacterDetailRequested(BattleCharacter character)
            => OnCharacterDetailRequested?.Invoke(character);

        public static void RaiseAbilityDetailRequested(AbilityData ability)
            => OnAbilityDetailRequested?.Invoke(ability);
    }
}

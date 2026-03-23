using System.Collections.Generic;
using UnityEngine;

namespace PixelWarriors
{
    public static class ConsumableAbilityHandler
    {
        private static readonly StatusEffect[] NegativeStatuses =
        {
            StatusEffect.Bleed, StatusEffect.Poison, StatusEffect.Burn,
            StatusEffect.Chilled, StatusEffect.Stun, StatusEffect.Silence,
            StatusEffect.Terror, StatusEffect.Confusion, StatusEffect.Mark,
            StatusEffect.SteamBeamDebuff, StatusEffect.Pin
        };

        public static void ExecuteHeal(BattleCharacter user, AbilityData ability, List<BattleCharacter> targets)
        {
            foreach (BattleCharacter target in targets)
            {
                if (!target.IsAlive) continue;
                GameEvents.RaiseAbilityUsed(user, ability, target);

                int healAmount = ability.BasePower;
                if (target.HasEffect(StatusEffect.Poison))
                    healAmount = Mathf.RoundToInt(healAmount * (1f - GameplayConfig.PoisonHealingReduction));

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

        public static void ExecuteEnergyRestore(BattleCharacter user, AbilityData ability, List<BattleCharacter> targets)
        {
            foreach (BattleCharacter target in targets)
            {
                if (!target.IsAlive) continue;
                GameEvents.RaiseAbilityUsed(user, ability, target);

                int previousEN = target.CurrentEnergy;
                target.CurrentEnergy = Mathf.Min(target.CurrentEnergy + ability.BasePower, target.MaxEnergy);
                int actualRestore = target.CurrentEnergy - previousEN;

                Log($"{user.Data.Name} uses {ability.Name} on {target.Data.Name}! Restores {actualRestore} Energy!");
            }
        }

        public static void ExecuteManaRestore(BattleCharacter user, AbilityData ability, List<BattleCharacter> targets)
        {
            foreach (BattleCharacter target in targets)
            {
                if (!target.IsAlive) continue;
                GameEvents.RaiseAbilityUsed(user, ability, target);

                int previousMP = target.CurrentMana;
                target.CurrentMana = Mathf.Min(target.CurrentMana + ability.BasePower, target.MaxMana);
                int actualRestore = target.CurrentMana - previousMP;

                Log($"{user.Data.Name} uses {ability.Name} on {target.Data.Name}! Restores {actualRestore} Mana!");
            }
        }

        public static void ExecuteAntidote(BattleCharacter user, AbilityData ability, List<BattleCharacter> targets)
        {
            foreach (BattleCharacter target in targets)
            {
                if (!target.IsAlive) continue;
                GameEvents.RaiseAbilityUsed(user, ability, target);

                int removed = 0;
                foreach (StatusEffect effect in NegativeStatuses)
                {
                    if (target.HasEffect(effect))
                    {
                        target.RemoveEffect(effect);
                        GameEvents.RaiseStatusEffectRemoved(target, effect);
                        removed++;
                    }
                }

                Log(removed > 0
                    ? $"{user.Data.Name} uses {ability.Name} on {target.Data.Name}! Cleansed {removed} effect(s)!"
                    : $"{user.Data.Name} uses {ability.Name} on {target.Data.Name}! No effects to cleanse.");
            }
        }

        public static void ExecuteBandage(BattleCharacter user, AbilityData ability, List<BattleCharacter> targets)
        {
            foreach (BattleCharacter target in targets)
            {
                if (!target.IsAlive) continue;
                GameEvents.RaiseAbilityUsed(user, ability, target);

                if (target.HasEffect(StatusEffect.Bleed))
                {
                    target.RemoveEffect(StatusEffect.Bleed);
                    GameEvents.RaiseStatusEffectRemoved(target, StatusEffect.Bleed);
                    Log($"{target.Data.Name}'s bleeding is stopped!");
                }

                int healAmount = ability.BasePower;
                int previousHP = target.CurrentHP;
                target.CurrentHP = Mathf.Min(target.CurrentHP + healAmount, target.MaxHP);
                int actualHeal = target.CurrentHP - previousHP;

                if (actualHeal > 0)
                {
                    Log($"{user.Data.Name} uses {ability.Name} on {target.Data.Name}! Heals {actualHeal} HP!");
                    GameEvents.RaiseHealingReceived(target, actualHeal);
                }
            }
        }

        public static void ExecuteSmokeBomb(BattleCharacter user, AbilityData ability, List<BattleCharacter> targets)
        {
            GameEvents.RaiseAbilityUsed(user, ability, user);
            Log($"{user.Data.Name} throws a Smoke Bomb!");

            foreach (BattleCharacter target in targets)
            {
                if (!target.IsAlive) continue;
                var effect = new StatusEffectInstance(StatusEffect.Hide, -1, 0, user);
                target.AddEffect(effect);
                GameEvents.RaiseStatusEffectApplied(target, StatusEffect.Hide, 0);
            }
            Log("All allies are hidden!");
        }

        private static void Log(string message) => GameEvents.RaiseCombatLogMessage(message);
    }
}

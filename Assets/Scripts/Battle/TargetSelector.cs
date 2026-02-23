using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PixelWarriors
{
    public static class TargetSelector
    {
        public static List<BattleCharacter> GetValidTargets(
            BattleCharacter user, AbilityData ability,
            List<BattleCharacter> players, List<BattleCharacter> enemies)
        {
            List<BattleCharacter> allies = user.Side == TeamSide.Player ? players : enemies;
            List<BattleCharacter> foes = user.Side == TeamSide.Player ? enemies : players;

            List<BattleCharacter> result = ability.TargetType switch
            {
                TargetType.SingleEnemy => foes.Where(c => c.IsAlive).ToList(),
                TargetType.SingleAlly => allies.Where(c => c.IsAlive).ToList(),
                TargetType.Self => new List<BattleCharacter> { user },
                TargetType.AllEnemies => foes.Where(c => c.IsAlive).ToList(),
                TargetType.AllAllies => allies.Where(c => c.IsAlive).ToList(),
                TargetType.All => players.Concat(enemies).Where(c => c.IsAlive).ToList(),
                _ => new List<BattleCharacter>()
            };

            if (ability.ExcludeSelf)
            {
                result.Remove(user);
            }

            // Resolve Weapon range to Close/Reach based on equipped weapon
            AbilityRange resolvedRange = ability.Range;
            if (resolvedRange == AbilityRange.Weapon)
            {
                WeaponType mainWeapon = user.Data.Equipment[(int)EquipmentSlot.Hand1]?.WeaponType ?? WeaponType.None;
                resolvedRange = EquipmentData.GetRangeForWeapon(mainWeapon);
            }

            // Close range: frontline blocking (unless user has Levitate)
            if (resolvedRange == AbilityRange.Close && !user.HasEffect(StatusEffect.Levitate))
            {
                bool isOffensive = ability.TargetType == TargetType.SingleEnemy ||
                                   ability.TargetType == TargetType.AllEnemies;
                if (isOffensive)
                {
                    List<BattleCharacter> foesFull = user.Side == TeamSide.Player ? enemies : players;
                    int frontlineCount = foesFull.Count(f => f.IsAlive && f.Row == GridRow.Front);

                    result = result.Where(c =>
                    {
                        if (c.Row == GridRow.Front) return true;
                        if (frontlineCount <= 1)
                        {
                            // 0-1 frontliners: any frontliner blocks ALL backline targets
                            return frontlineCount == 0;
                        }
                        // 2+ frontliners: column-based blocking
                        bool frontBlocked = foesFull.Any(f => f.IsAlive && f.Row == GridRow.Front && f.Column == c.Column);
                        return !frontBlocked;
                    }).ToList();
                }
            }

            return result;
        }

        public static bool RequiresManualTargetSelection(TargetType targetType)
        {
            return targetType == TargetType.SingleEnemy || targetType == TargetType.SingleAlly;
        }

        public static float GetAggroPercent(BattleCharacter character, List<BattleCharacter> team)
        {
            if (!character.IsAlive) return 0f;

            float myWeight = GetBaseWeight(character) * StatusEffectProcessor.GetAggroModifier(character);

            float totalWeight = 0f;
            foreach (BattleCharacter c in team)
            {
                if (!c.IsAlive) continue;
                totalWeight += GetBaseWeight(c) * StatusEffectProcessor.GetAggroModifier(c);
            }

            return totalWeight > 0f ? myWeight / totalWeight : 0f;
        }

        public static BattleCharacter SelectAggroTarget(List<BattleCharacter> potentialTargets)
        {
            if (potentialTargets.Count == 0) return null;
            if (potentialTargets.Count == 1) return potentialTargets[0];

            float totalWeight = 0f;
            float[] weights = new float[potentialTargets.Count];

            for (int i = 0; i < potentialTargets.Count; i++)
            {
                BattleCharacter target = potentialTargets[i];
                float weight = GetBaseWeight(target) * StatusEffectProcessor.GetAggroModifier(target);
                weights[i] = weight;
                totalWeight += weight;
            }

            // Concealment fallback: if all weights are 0, distribute equally
            if (totalWeight <= 0f)
            {
                return potentialTargets[Random.Range(0, potentialTargets.Count)];
            }

            float roll = Random.value * totalWeight;
            float cumulative = 0f;

            for (int i = 0; i < weights.Length; i++)
            {
                cumulative += weights[i];
                if (roll <= cumulative)
                {
                    return potentialTargets[i];
                }
            }

            return potentialTargets[potentialTargets.Count - 1];
        }

        private static float GetBaseWeight(BattleCharacter character)
        {
            return character.Row == GridRow.Front
                ? GameplayConfig.FrontlineBaseAggro
                : GameplayConfig.BacklineBaseAggro;
        }
    }
}

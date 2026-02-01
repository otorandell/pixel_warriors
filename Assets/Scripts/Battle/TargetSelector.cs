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

            return ability.TargetType switch
            {
                TargetType.SingleEnemy => foes.Where(c => c.IsAlive).ToList(),
                TargetType.SingleAlly => allies.Where(c => c.IsAlive).ToList(),
                TargetType.Self => new List<BattleCharacter> { user },
                TargetType.AllEnemies => foes.Where(c => c.IsAlive).ToList(),
                TargetType.AllAllies => allies.Where(c => c.IsAlive).ToList(),
                TargetType.All => players.Concat(enemies).Where(c => c.IsAlive).ToList(),
                _ => new List<BattleCharacter>()
            };
        }

        public static bool RequiresManualTargetSelection(TargetType targetType)
        {
            return targetType == TargetType.SingleEnemy || targetType == TargetType.SingleAlly;
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
                float weight = target.Row == GridRow.Front
                    ? GameplayConfig.FrontlineBaseAggro
                    : GameplayConfig.BacklineBaseAggro;

                weights[i] = weight;
                totalWeight += weight;
            }

            // Normalize and roll
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
    }
}

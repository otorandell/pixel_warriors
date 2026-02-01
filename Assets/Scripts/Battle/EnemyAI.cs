using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PixelWarriors
{
    public static class EnemyAI
    {
        public static void DecideAction(
            BattleCharacter enemy,
            List<BattleCharacter> players,
            List<BattleCharacter> enemies,
            out AbilityData chosenAbility,
            out List<BattleCharacter> chosenTargets)
        {
            chosenAbility = null;
            chosenTargets = null;

            List<AbilityData> usable = enemy.Data.Abilities
                .Where(a => !a.IsPassive && enemy.CanUseAbility(a))
                .ToList();

            if (usable.Count == 0) return;

            chosenAbility = usable[Random.Range(0, usable.Count)];

            List<BattleCharacter> validTargets = TargetSelector.GetValidTargets(
                enemy, chosenAbility, players, enemies);

            if (validTargets.Count == 0)
            {
                chosenAbility = null;
                return;
            }

            if (TargetSelector.RequiresManualTargetSelection(chosenAbility.TargetType))
            {
                BattleCharacter target = TargetSelector.SelectAggroTarget(validTargets);
                chosenTargets = new List<BattleCharacter> { target };
            }
            else
            {
                chosenTargets = validTargets;
            }
        }
    }
}

using System.Collections.Generic;
using System.Linq;

namespace PixelWarriors
{
    public static class TurnOrderCalculator
    {
        public static Queue<BattleCharacter> CalculateTurnOrder(List<BattleCharacter> allCharacters)
        {
            List<BattleCharacter> living = allCharacters.Where(c => c.IsAlive).ToList();

            List<BattleCharacter> positive = living
                .Where(c => c.Priority == Priority.Positive)
                .OrderByDescending(c => c.EffectiveStats.Initiative)
                .ToList();

            List<BattleCharacter> normal = living
                .Where(c => c.Priority == Priority.Normal)
                .OrderByDescending(c => c.EffectiveStats.Initiative)
                .ToList();

            List<BattleCharacter> negative = living
                .Where(c => c.Priority == Priority.Negative)
                .OrderByDescending(c => c.EffectiveStats.Initiative)
                .ToList();

            Queue<BattleCharacter> queue = new Queue<BattleCharacter>();

            foreach (BattleCharacter c in positive) queue.Enqueue(c);
            foreach (BattleCharacter c in normal) queue.Enqueue(c);
            foreach (BattleCharacter c in negative) queue.Enqueue(c);

            return queue;
        }
    }
}

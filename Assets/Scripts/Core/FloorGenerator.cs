using System.Collections.Generic;
using UnityEngine;

namespace PixelWarriors
{
    public static class FloorGenerator
    {
        /// <summary>
        /// Generate the floor plan for an entire act. Called once when entering a new act.
        /// 19 normal floors + 1 boss floor = 20 total.
        /// Distribution: ~10-12 Battle, ~4-5 Bonus (Shop/Rest/Smithy), ~3-4 Events.
        /// </summary>
        public static List<FloorPlanEntry> GenerateActPlan(int act)
        {
            int normalFloors = RunConfig.BossFloor - 1; // 19
            List<FloorPlanEntry> plan = new(normalFloors + 1);

            // Determine bonus room counts for this act
            int restCount = Random.Range(1, 3);       // 1-2
            int shopCount = Random.Range(1, 3);        // 1-2
            int smithyCount = act >= 2 ? Random.Range(1, 3) : 0; // 0 in act 1, 1-2 after
            int eventCount = Random.Range(3, 5);       // 3-4
            int bonusTotal = restCount + shopCount + smithyCount;
            int battleCount = normalFloors - bonusTotal - eventCount;

            // Build the pool of floor types
            List<FloorCategory> categories = new(normalFloors);
            for (int i = 0; i < battleCount; i++) categories.Add(FloorCategory.Battle);
            for (int i = 0; i < bonusTotal; i++) categories.Add(FloorCategory.Bonus);
            for (int i = 0; i < eventCount; i++) categories.Add(FloorCategory.Event);

            // Pad or trim to exactly normalFloors
            while (categories.Count < normalFloors) categories.Add(FloorCategory.Battle);
            while (categories.Count > normalFloors) categories.RemoveAt(categories.Count - 1);

            // Shuffle
            Shuffle(categories);

            // Ensure floor 1 is always a Battle (gentle start)
            int firstBattle = categories.IndexOf(FloorCategory.Battle);
            if (firstBattle > 0)
            {
                (categories[0], categories[firstBattle]) = (categories[firstBattle], categories[0]);
            }

            // Build bonus type queue
            List<RoomType> bonusQueue = new();
            for (int i = 0; i < restCount; i++) bonusQueue.Add(RoomType.Rest);
            for (int i = 0; i < shopCount; i++) bonusQueue.Add(RoomType.Shop);
            for (int i = 0; i < smithyCount; i++) bonusQueue.Add(RoomType.Smithy);
            Shuffle(bonusQueue);

            // Determine elite battles (later floors more likely)
            int bonusIdx = 0;
            for (int i = 0; i < normalFloors; i++)
            {
                FloorPlanEntry entry = new() { Category = categories[i] };

                if (categories[i] == FloorCategory.Battle)
                {
                    // Elite chance increases with floor number
                    float eliteChance = Mathf.Lerp(0.05f, 0.35f, (float)i / normalFloors);
                    entry.IsElite = Random.value < eliteChance;
                }
                else if (categories[i] == FloorCategory.Bonus)
                {
                    entry.BonusType = bonusIdx < bonusQueue.Count
                        ? bonusQueue[bonusIdx++]
                        : RoomType.Shop;
                }

                plan.Add(entry);
            }

            // Floor 20: Boss
            plan.Add(new FloorPlanEntry { Category = FloorCategory.Boss });

            return plan;
        }

        /// <summary>
        /// Get the floor plan entry for the current floor (1-indexed).
        /// </summary>
        public static FloorPlanEntry GetCurrentFloor(RunData runData)
        {
            int index = runData.CurrentFloor - 1;
            if (index >= 0 && index < runData.FloorPlan.Count)
                return runData.FloorPlan[index];

            // Fallback
            return new FloorPlanEntry { Category = FloorCategory.Battle };
        }

        public static string GetRoomName(RoomType roomType)
        {
            return roomType switch
            {
                RoomType.Battle => "Battle",
                RoomType.EliteBattle => "Elite Battle",
                RoomType.BossBattle => "Boss Battle",
                RoomType.Shop => "Shop",
                RoomType.Rest => "Rest Site",
                RoomType.Event => "Mystery",
                RoomType.Smithy => "Smithy",
                _ => "Unknown"
            };
        }

        public static string GetRoomDescription(RoomType roomType)
        {
            return roomType switch
            {
                RoomType.Battle => "Fight a group of enemies.\nEarn gold and loot.",
                RoomType.EliteBattle => "Powerful foes await.\nGreater rewards.",
                RoomType.BossBattle => "A fearsome guardian\nblocks the way.",
                RoomType.Shop => "A merchant offers\nwares for sale.",
                RoomType.Rest => "A safe place to\nrecover your strength.",
                RoomType.Event => "Something unusual\nlies ahead...",
                RoomType.Smithy => "A forge where gear\ncan be improved.",
                _ => ""
            };
        }

        public static Color GetRoomColor(RoomType roomType)
        {
            return roomType switch
            {
                RoomType.Battle => UIStyleConfig.TextPrimary,
                RoomType.EliteBattle => UIStyleConfig.AccentYellow,
                RoomType.BossBattle => UIStyleConfig.AccentRed,
                RoomType.Shop => UIStyleConfig.AccentGreen,
                RoomType.Rest => UIStyleConfig.AccentCyan,
                RoomType.Event => UIStyleConfig.AccentMagenta,
                RoomType.Smithy => UIStyleConfig.AccentYellow,
                _ => UIStyleConfig.TextPrimary
            };
        }

        public static string GetRoomSymbol(RoomType roomType)
        {
            return roomType switch
            {
                RoomType.Battle => "X",
                RoomType.EliteBattle => "XX",
                RoomType.BossBattle => "!!!",
                RoomType.Shop => "$",
                RoomType.Rest => "+",
                RoomType.Event => "?",
                RoomType.Smithy => "&",
                _ => "?"
            };
        }

        private static void Shuffle<T>(List<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }
    }
}

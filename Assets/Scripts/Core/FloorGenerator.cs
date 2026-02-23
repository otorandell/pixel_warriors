using System.Collections.Generic;
using UnityEngine;

namespace PixelWarriors
{
    public static class FloorGenerator
    {
        public static List<RoomType> GenerateRoomChoices(RunData runData)
        {
            // Boss floor: no choice
            if (runData.IsBossFloor)
            {
                return new List<RoomType> { RoomType.BossBattle };
            }

            List<RoomType> choices = new();

            // Floor 4 guarantees a shop option
            if (runData.CurrentFloor == RunConfig.ShopGuaranteedFloor &&
                runData.PreviousRoom != RoomType.Shop)
            {
                choices.Add(RoomType.Shop);
            }

            // Fill remaining slots
            while (choices.Count < RunConfig.RoomChoicesPerFloor)
            {
                RoomType room = RollRoomType(runData);

                // No duplicates in the same choice set
                if (choices.Contains(room))
                    continue;

                // No two shops in a row
                if (room == RoomType.Shop && runData.PreviousRoom == RoomType.Shop)
                    continue;

                // Recruit only if party below max
                if (room == RoomType.Recruit && runData.Party.Count >= RunConfig.MaxPartySize)
                    continue;

                choices.Add(room);
            }

            return choices;
        }

        private static RoomType RollRoomType(RunData runData)
        {
            // Weight distribution shifts across the run
            // Early: lots of battles. Mid: more variety. Late: more elites.
            float progress = (float)runData.TotalFloor / (RunConfig.FloorsPerAct * RunConfig.ActCount);

            int wBattle = Mathf.RoundToInt(Mathf.Lerp(50, 25, progress));
            int wElite = Mathf.RoundToInt(Mathf.Lerp(5, 25, progress));
            int wShop = 15;
            int wRest = Mathf.RoundToInt(Mathf.Lerp(10, 15, progress));
            int wEvent = 15;
            int wRecruit = runData.Party.Count < RunConfig.MaxPartySize ? 10 : 0;

            int total = wBattle + wElite + wShop + wRest + wEvent + wRecruit;
            int roll = Random.Range(0, total);

            roll -= wBattle;  if (roll < 0) return RoomType.Battle;
            roll -= wElite;   if (roll < 0) return RoomType.EliteBattle;
            roll -= wShop;    if (roll < 0) return RoomType.Shop;
            roll -= wRest;    if (roll < 0) return RoomType.Rest;
            roll -= wEvent;   if (roll < 0) return RoomType.Event;
            return RoomType.Recruit;
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
                RoomType.Recruit => "Wanderer",
                _ => "Unknown"
            };
        }

        public static string GetRoomDescription(RoomType roomType)
        {
            return roomType switch
            {
                RoomType.Battle => "Fight a group of enemies.\nEarn XP and gold.",
                RoomType.EliteBattle => "Powerful foes await.\nGreater rewards.",
                RoomType.BossBattle => "A fearsome guardian\nblocks the way.",
                RoomType.Shop => "A merchant offers\nwares for sale.",
                RoomType.Rest => "A safe place to\nrecover your strength.",
                RoomType.Event => "Something unusual\nlies ahead...",
                RoomType.Recruit => "A wandering fighter\nseeks a party.",
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
                RoomType.Recruit => UIStyleConfig.AccentGreen,
                _ => UIStyleConfig.TextPrimary
            };
        }
    }
}

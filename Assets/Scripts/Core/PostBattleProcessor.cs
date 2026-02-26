using System.Collections.Generic;
using UnityEngine;

namespace PixelWarriors
{
    public struct PostBattleResult
    {
        public int XPPerCharacter;
        public int GoldEarned;
        public List<LevelingSystem.LevelUpResult> LevelUpResults;
        public List<CharacterData> FallenCharacters;
        public List<EquipmentData> LootDrops;
    }

    public static class PostBattleProcessor
    {
        public static PostBattleResult Process(RunData runData, List<BattleCharacter> players,
            RoomType roomType = RoomType.Battle)
        {
            PostBattleResult result = new()
            {
                LevelUpResults = new List<LevelingSystem.LevelUpResult>(),
                FallenCharacters = new List<CharacterData>(),
                LootDrops = new List<EquipmentData>()
            };

            // --- Sync battle state back to CharacterData ---
            foreach (BattleCharacter bc in players)
            {
                bc.SyncToData();
            }

            // --- Handle permadeath ---
            for (int i = runData.Party.Count - 1; i >= 0; i--)
            {
                BattleCharacter bc = players.Find(p => p.Data == runData.Party[i]);
                if (bc != null && !bc.IsAlive)
                {
                    result.FallenCharacters.Add(runData.Party[i]);
                    runData.Fallen.Add(runData.Party[i]);
                    runData.Party.RemoveAt(i);
                }
            }

            // --- Room type multipliers ---
            float xpMultiplier = roomType switch
            {
                RoomType.EliteBattle => RunConfig.EliteXPMultiplier,
                RoomType.BossBattle => RunConfig.BossXPMultiplier,
                _ => 1f
            };
            float goldMultiplier = roomType switch
            {
                RoomType.EliteBattle => RunConfig.EliteGoldMultiplier,
                RoomType.BossBattle => RunConfig.BossGoldMultiplier,
                _ => 1f
            };

            // --- Calculate XP ---
            int floor = runData.TotalFloor;
            int baseXP = RunConfig.BaseXPPerBattle + Random.Range(0, RunConfig.XPPerBattleVariance + 1);
            int xpEarned = Mathf.RoundToInt(baseXP * (1f + (floor - 1) * RunConfig.XPFloorScaling) * xpMultiplier);
            result.XPPerCharacter = xpEarned;

            // --- Calculate Gold ---
            int baseGold = RunConfig.BaseGoldPerBattle + Random.Range(0, RunConfig.GoldPerBattleVariance + 1);
            int goldEarned = Mathf.RoundToInt(baseGold * (1f + (floor - 1) * RunConfig.GoldFloorScaling) * goldMultiplier);
            result.GoldEarned = goldEarned;
            runData.Gold += goldEarned;

            // --- Generate loot ---
            result.LootDrops = LootGenerator.GenerateLoot(runData.CurrentAct, roomType, runData);

            // --- Award XP and process level ups ---
            foreach (CharacterData character in runData.Party)
            {
                LevelingSystem.LevelUpResult levelResult = LevelingSystem.AddXP(character, xpEarned);
                result.LevelUpResults.Add(levelResult);
            }

            // --- Partial heal surviving party ---
            foreach (CharacterData character in runData.Party)
            {
                CharacterStats total = character.GetTotalStats();
                int maxHP = StatCalculator.CalculateMaxHP(total);
                int missingHP = maxHP - character.CurrentHP;
                int healAmount = Mathf.RoundToInt(missingHP * RunConfig.HealBetweenBattlesPercent);
                character.CurrentHP = Mathf.Min(character.CurrentHP + healAmount, maxHP);
            }

            return result;
        }
    }
}

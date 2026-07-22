using System.Collections.Generic;
using UnityEngine;

namespace PixelWarriors
{
    public struct PostBattleResult
    {
        public int GoldEarned;
        public List<CharacterData> FallenCharacters;
        public List<EquipmentData> LootDrops;
        public bool IsBossBattle;
        public int MetalScrapsGained;
        public int LeatherScrapsGained;
        public GemType? GemGained;
    }

    public static class PostBattleProcessor
    {
        public static PostBattleResult Process(RunData runData, List<BattleCharacter> players,
            RoomType roomType = RoomType.Battle)
        {
            PostBattleResult result = new()
            {
                FallenCharacters = new List<CharacterData>(),
                LootDrops = new List<EquipmentData>(),
                IsBossBattle = roomType == RoomType.BossBattle
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

            // --- Room type gold multiplier ---
            float goldMultiplier = roomType switch
            {
                RoomType.EliteBattle => RunConfig.EliteGoldMultiplier,
                RoomType.BossBattle => RunConfig.BossGoldMultiplier,
                _ => 1f
            };

            // --- Calculate Gold ---
            int floor = runData.TotalFloor;
            int baseGold = RunConfig.BaseGoldPerBattle + Random.Range(0, RunConfig.GoldPerBattleVariance + 1);
            int goldEarned = Mathf.RoundToInt(baseGold * (1f + (floor - 1) * RunConfig.GoldFloorScaling) * goldMultiplier);
            result.GoldEarned = goldEarned;
            runData.Gold += goldEarned;

            // --- Generate loot ---
            result.LootDrops = LootGenerator.GenerateLoot(runData.CurrentAct, roomType, runData);

            // --- Generate material drops ---
            if (Random.value < SmithyConfig.MaterialDropChance)
            {
                int metalDrop = Random.Range(SmithyConfig.MaterialDropMin, SmithyConfig.MaterialDropMax + 1);
                int leatherDrop = Random.Range(SmithyConfig.MaterialDropMin, SmithyConfig.MaterialDropMax + 1);
                result.MetalScrapsGained = metalDrop;
                result.LeatherScrapsGained = leatherDrop;
                runData.MetalScraps += metalDrop;
                runData.LeatherScraps += leatherDrop;
            }

            // Gem drop: elite/boss only
            if ((roomType == RoomType.EliteBattle || roomType == RoomType.BossBattle)
                && Random.value < SmithyConfig.GemDropChance)
            {
                GemType gem = (GemType)Random.Range(0, 5);
                result.GemGained = gem;
                runData.Gems.Add(gem);
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

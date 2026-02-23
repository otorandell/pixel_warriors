using System.Collections.Generic;
using UnityEngine;

namespace PixelWarriors
{
    public static class EncounterGenerator
    {
        public static List<BattleCharacter> GenerateEncounter(RunData runData, RoomType roomType)
        {
            int partySize = runData.Party.Count;
            int floor = runData.TotalFloor;

            List<CharacterData> enemyData = roomType switch
            {
                RoomType.EliteBattle => GenerateEliteEncounter(floor),
                RoomType.BossBattle => GenerateBossEncounter(runData.CurrentAct),
                _ => GenerateNormalEncounter(floor, partySize)
            };

            // Apply floor scaling
            float floorMultiplier = 1f + (floor - 1) * RunConfig.FloorStatScaling;
            float typeMultiplier = roomType switch
            {
                RoomType.EliteBattle => RunConfig.EliteStatMultiplier,
                RoomType.BossBattle => RunConfig.BossStatMultiplier,
                _ => 1f
            };

            foreach (CharacterData data in enemyData)
            {
                ScaleStats(data, floorMultiplier * typeMultiplier);
            }

            // Place on grid
            List<BattleCharacter> enemies = new();
            GridRow[] rows = { GridRow.Front, GridRow.Front, GridRow.Back, GridRow.Back };
            GridColumn[] cols = { GridColumn.Left, GridColumn.Right, GridColumn.Left, GridColumn.Right };

            for (int i = 0; i < enemyData.Count && i < 4; i++)
            {
                enemies.Add(new BattleCharacter(
                    enemyData[i], TeamSide.Enemy, rows[i], cols[i]));
            }

            return enemies;
        }

        private static List<CharacterData> GenerateNormalEncounter(int floor, int partySize)
        {
            int count = Mathf.Max(RunConfig.MinEnemies, partySize);
            count = Mathf.Min(count, RunConfig.MaxEnemies);

            List<CharacterData> enemies = new();

            // Pool: frontliners for front row slots, backliners for back row slots
            for (int i = 0; i < count; i++)
            {
                bool isFrontRow = i < 2;
                EnemyType type = isFrontRow ? PickFrontliner(floor) : PickBackliner(floor);
                enemies.Add(EnemyDefinitions.CreateEnemy(type));
            }

            return enemies;
        }

        private static List<CharacterData> GenerateEliteEncounter(int floor)
        {
            // 1-2 strong enemies
            List<CharacterData> enemies = new();
            enemies.Add(EnemyDefinitions.CreateEnemy(EnemyType.Minotaur));

            // 50% chance of an add
            if (Random.value > 0.5f)
            {
                enemies.Add(EnemyDefinitions.CreateEnemy(PickFrontliner(floor)));
            }

            return enemies;
        }

        private static List<CharacterData> GenerateBossEncounter(int act)
        {
            // Boss + 0-2 adds based on act
            List<CharacterData> enemies = new();

            // Use Minotaur as boss for all acts for now (Phase D will add proper bosses)
            enemies.Add(EnemyDefinitions.CreateEnemy(EnemyType.Minotaur));
            enemies[0].Name = act switch
            {
                1 => "Goblin King",
                2 => "Minotaur Lord",
                3 => "The Lich",
                _ => "Boss"
            };

            // Act 2+: 1 add
            if (act >= 2)
                enemies.Add(EnemyDefinitions.CreateEnemy(PickFrontliner(act * RunConfig.FloorsPerAct)));

            // Act 3: 2nd add
            if (act >= 3)
                enemies.Add(EnemyDefinitions.CreateEnemy(PickBackliner(act * RunConfig.FloorsPerAct)));

            return enemies;
        }

        private static EnemyType PickFrontliner(int floor)
        {
            // For now only Ratman as frontliner. Phase D adds more.
            return EnemyType.Ratman;
        }

        private static EnemyType PickBackliner(int floor)
        {
            // For now only GoblinArcher as backliner. Phase D adds more.
            return EnemyType.GoblinArcher;
        }

        private static void ScaleStats(CharacterData data, float multiplier)
        {
            if (multiplier <= 1f) return;

            data.BaseStats = new CharacterStats(
                Mathf.RoundToInt(data.BaseStats.Endurance * multiplier),
                Mathf.RoundToInt(data.BaseStats.Stamina * multiplier),
                Mathf.RoundToInt(data.BaseStats.Intellect * multiplier),
                Mathf.RoundToInt(data.BaseStats.Strength * multiplier),
                Mathf.RoundToInt(data.BaseStats.Dexterity * multiplier),
                Mathf.RoundToInt(data.BaseStats.Willpower * multiplier),
                Mathf.RoundToInt(data.BaseStats.Armor * multiplier),
                Mathf.RoundToInt(data.BaseStats.MagicResist * multiplier),
                data.BaseStats.Initiative // Don't scale initiative
            );

            // Scale weapon damage too
            for (int i = 0; i < data.Equipment.Length; i++)
            {
                if (data.Equipment[i] != null && data.Equipment[i].BaseDamage > 0)
                {
                    data.Equipment[i].BaseDamage = Mathf.RoundToInt(
                        data.Equipment[i].BaseDamage * multiplier);
                }
            }
        }
    }
}

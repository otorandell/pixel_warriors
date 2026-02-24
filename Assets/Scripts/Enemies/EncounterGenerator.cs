using System.Collections.Generic;
using UnityEngine;

namespace PixelWarriors
{
    public static class EncounterGenerator
    {
        // Frontliner pools by act progression
        private static readonly EnemyType[][] FrontlinerPools =
        {
            new[] { EnemyType.Ratman, EnemyType.Skeleton, EnemyType.ZombieShambler, EnemyType.FungusCreeper },
            new[] { EnemyType.Spider, EnemyType.Bandit, EnemyType.OrcWarrior, EnemyType.StoneSentinel, EnemyType.BerserkerCultist },
            new[] { EnemyType.DarkKnight, EnemyType.AbyssalGolem, EnemyType.PlagueBringer, EnemyType.DeathCultist, EnemyType.ChainDevil }
        };

        // Backliner pools by act progression
        private static readonly EnemyType[][] BacklinerPools =
        {
            new[] { EnemyType.GoblinArcher, EnemyType.SwarmBat, EnemyType.TunnelRat },
            new[] { EnemyType.DarkMage, EnemyType.HerbalistShaman, EnemyType.CrossbowBandit, EnemyType.FireImp },
            new[] { EnemyType.ShadowAssassin, EnemyType.LichAcolyte, EnemyType.BloodMage, EnemyType.VoidSpeaker }
        };

        // Elite pools by act progression
        private static readonly EnemyType[][] ElitePools =
        {
            new[] { EnemyType.Minotaur, EnemyType.GiantSpider, EnemyType.BoneLord },
            new[] { EnemyType.OrcBrute, EnemyType.WyvernKnight, EnemyType.NecromancerAdept, EnemyType.BladeDancer },
            new[] { EnemyType.VampireLord, EnemyType.TwinWraith, EnemyType.DemonChampion }
        };

        // Boss pools by act (2 per act for randomization)
        private static readonly EnemyType[][] BossPools =
        {
            new[] { EnemyType.GoblinKing, EnemyType.CatacombGuardian },
            new[] { EnemyType.MinotaurLord, EnemyType.BanditWarlord },
            new[] { EnemyType.Lich, EnemyType.ArchDemon }
        };

        public static EncounterData GenerateEncounter(RunData runData, RoomType roomType)
        {
            int partySize = runData.Party.Count;
            int floor = runData.TotalFloor;
            int actIndex = Mathf.Clamp(runData.CurrentAct - 1, 0, 2);

            EncounterData encounter = roomType switch
            {
                RoomType.EliteBattle => GenerateEliteEncounter(actIndex),
                RoomType.BossBattle => GenerateBossEncounter(runData.CurrentAct, actIndex),
                _ => GenerateNormalEncounter(actIndex, partySize)
            };

            // Apply floor scaling to initial enemies
            float floorMultiplier = 1f + (floor - 1) * RunConfig.FloorStatScaling;
            float typeMultiplier = roomType switch
            {
                RoomType.EliteBattle => RunConfig.EliteStatMultiplier,
                RoomType.BossBattle => RunConfig.BossStatMultiplier,
                _ => 1f
            };

            float totalMultiplier = floorMultiplier * typeMultiplier;

            foreach (CharacterData data in encounter.InitialEnemies)
                ScaleStats(data, totalMultiplier);

            // Scale reinforcement waves too
            foreach (ReinforcementWave wave in encounter.Waves)
            {
                foreach (CharacterData data in wave.Enemies)
                    ScaleStats(data, totalMultiplier);
            }

            return encounter;
        }

        private static EncounterData GenerateNormalEncounter(int actIndex, int partySize)
        {
            int count = Mathf.Clamp(partySize, RunConfig.MinEnemies, RunConfig.MaxEnemies);
            EncounterData encounter = new();

            for (int i = 0; i < count; i++)
            {
                bool isFrontRow = i < 2;
                EnemyType type = isFrontRow
                    ? PickFromPool(FrontlinerPools[actIndex])
                    : PickFromPool(BacklinerPools[actIndex]);
                encounter.InitialEnemies.Add(EnemyDefinitions.CreateEnemy(type));
            }

            // Chance of one reinforcement wave
            if (Random.value < RunConfig.NormalReinforcementChance)
            {
                int waveSize = Random.Range(1, RunConfig.NormalReinforcementWaveSize + 1);
                ReinforcementWave wave = new()
                {
                    Trigger = ReinforcementTrigger.OnEnemyCount,
                    TriggerValue = 1,
                    AnnouncementText = "More enemies arrive!"
                };

                for (int i = 0; i < waveSize; i++)
                {
                    EnemyType type = Random.value < 0.6f
                        ? PickFromPool(FrontlinerPools[actIndex])
                        : PickFromPool(BacklinerPools[actIndex]);
                    wave.Enemies.Add(EnemyDefinitions.CreateEnemy(type));
                }

                encounter.Waves.Add(wave);
            }

            return encounter;
        }

        private static EncounterData GenerateEliteEncounter(int actIndex)
        {
            EncounterData encounter = new();

            // 1 elite + 0-1 regular adds
            EnemyType eliteType = PickFromPool(ElitePools[actIndex]);
            encounter.InitialEnemies.Add(EnemyDefinitions.CreateEnemy(eliteType));

            if (Random.value > 0.5f)
            {
                encounter.InitialEnemies.Add(EnemyDefinitions.CreateEnemy(
                    PickFromPool(FrontlinerPools[actIndex])));
            }

            // Wave 1: regular adds when most enemies are down
            ReinforcementWave wave1 = new()
            {
                Trigger = ReinforcementTrigger.OnEnemyCount,
                TriggerValue = 1,
                AnnouncementText = "The elite calls for backup!"
            };
            for (int i = 0; i < RunConfig.EliteReinforcementWaveSize; i++)
            {
                EnemyType type = Random.value < 0.5f
                    ? PickFromPool(FrontlinerPools[actIndex])
                    : PickFromPool(BacklinerPools[actIndex]);
                wave1.Enemies.Add(EnemyDefinitions.CreateEnemy(type));
            }
            encounter.Waves.Add(wave1);

            // 50% chance of a second wave
            if (Random.value < 0.5f)
            {
                ReinforcementWave wave2 = new()
                {
                    Trigger = ReinforcementTrigger.OnEnemyCount,
                    TriggerValue = 1,
                    AnnouncementText = "Even more enemies appear!"
                };
                int wave2Size = Random.Range(1, RunConfig.EliteReinforcementWaveSize + 1);
                for (int i = 0; i < wave2Size; i++)
                {
                    wave2.Enemies.Add(EnemyDefinitions.CreateEnemy(
                        PickFromPool(FrontlinerPools[actIndex])));
                }
                encounter.Waves.Add(wave2);
            }

            return encounter;
        }

        private static EncounterData GenerateBossEncounter(int act, int actIndex)
        {
            EncounterData encounter = new();

            // Boss per act (randomized from pool of 2)
            EnemyType bossType = PickFromPool(BossPools[actIndex]);
            CharacterData bossData = EnemyDefinitions.CreateEnemy(bossType);
            bossData.IsBoss = true;
            encounter.InitialEnemies.Add(bossData);

            // Act 2+: 1 frontline add
            if (act >= 2)
                encounter.InitialEnemies.Add(EnemyDefinitions.CreateEnemy(
                    PickFromPool(FrontlinerPools[actIndex])));

            // Wave 1: adds at 75% boss HP
            ReinforcementWave wave1 = new()
            {
                Trigger = ReinforcementTrigger.OnBossHPPercent,
                TriggerValue = 75,
                AnnouncementText = "The boss summons reinforcements!"
            };
            for (int i = 0; i < RunConfig.BossReinforcementWaveSize; i++)
            {
                EnemyType type = Random.value < 0.5f
                    ? PickFromPool(FrontlinerPools[actIndex])
                    : PickFromPool(BacklinerPools[actIndex]);
                wave1.Enemies.Add(EnemyDefinitions.CreateEnemy(type));
            }
            encounter.Waves.Add(wave1);

            // Wave 2: adds at 40% boss HP
            ReinforcementWave wave2 = new()
            {
                Trigger = ReinforcementTrigger.OnBossHPPercent,
                TriggerValue = 40,
                AnnouncementText = "The boss is enraged! More enemies pour in!"
            };
            for (int i = 0; i < RunConfig.BossReinforcementWaveSize; i++)
            {
                wave2.Enemies.Add(EnemyDefinitions.CreateEnemy(
                    PickFromPool(FrontlinerPools[actIndex])));
            }
            encounter.Waves.Add(wave2);

            // Act 3: extra wave at 20%
            if (act >= 3)
            {
                ReinforcementWave wave3 = new()
                {
                    Trigger = ReinforcementTrigger.OnBossHPPercent,
                    TriggerValue = 20,
                    AnnouncementText = "A final desperate wave attacks!"
                };
                wave3.Enemies.Add(EnemyDefinitions.CreateEnemy(
                    PickFromPool(BacklinerPools[actIndex])));
                wave3.Enemies.Add(EnemyDefinitions.CreateEnemy(
                    PickFromPool(FrontlinerPools[actIndex])));
                encounter.Waves.Add(wave3);
            }

            return encounter;
        }

        private static EnemyType PickFromPool(EnemyType[] pool)
        {
            return pool[Random.Range(0, pool.Length)];
        }

        public static void ScaleStats(CharacterData data, float multiplier)
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

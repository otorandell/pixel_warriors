using System.Collections.Generic;
using UnityEngine;

namespace PixelWarriors
{
    public static class EventOutcomes
    {
        private static readonly string[] StatNames =
            { "Endurance", "Stamina", "Intellect", "Strength", "Dexterity", "Willpower" };

        public static void HealPartyPercent(RunData run, float percent)
        {
            foreach (CharacterData c in run.Party)
            {
                int maxHP = StatCalculator.CalculateMaxHP(c.GetTotalStats());
                int missing = maxHP - c.CurrentHP;
                int heal = Mathf.RoundToInt(missing * percent);
                c.CurrentHP = Mathf.Min(c.CurrentHP + heal, maxHP);
            }
        }

        public static void HealCharacterFull(RunData run, int partyIndex)
        {
            if (partyIndex < 0 || partyIndex >= run.Party.Count) return;
            CharacterData c = run.Party[partyIndex];
            c.CurrentHP = StatCalculator.CalculateMaxHP(c.GetTotalStats());
        }

        public static void DamagePartyPercent(RunData run, float percent, int minDamage = 5)
        {
            foreach (CharacterData c in run.Party)
            {
                int maxHP = StatCalculator.CalculateMaxHP(c.GetTotalStats());
                int dmg = Mathf.Max(minDamage, Mathf.RoundToInt(maxHP * percent));
                c.CurrentHP = Mathf.Max(1, c.CurrentHP - dmg);
            }
        }

        public static void DamageCharacter(RunData run, int partyIndex, int amount)
        {
            if (partyIndex < 0 || partyIndex >= run.Party.Count) return;
            run.Party[partyIndex].CurrentHP = Mathf.Max(1, run.Party[partyIndex].CurrentHP - amount);
        }

        public static void DamagePartyFlat(RunData run, int amount)
        {
            foreach (CharacterData c in run.Party)
                c.CurrentHP = Mathf.Max(1, c.CurrentHP - amount);
        }

        public static void GiveGold(RunData run, int amount)
        {
            run.Gold += amount;
        }

        public static void TakeGold(RunData run, int amount)
        {
            run.Gold = Mathf.Max(0, run.Gold - amount);
        }

        public static void GiveXPAll(RunData run, int amount)
        {
            foreach (CharacterData c in run.Party)
                LevelingSystem.AddXP(c, amount);
        }

        public static void GiveStatBoost(RunData run, int partyIndex, CharacterStats boost)
        {
            if (partyIndex < 0 || partyIndex >= run.Party.Count) return;
            CharacterData c = run.Party[partyIndex];
            c.BaseStats.Add(boost);
        }

        public static void GiveStatBoostAll(RunData run, CharacterStats boost)
        {
            foreach (CharacterData c in run.Party)
                c.BaseStats.Add(boost);
        }

        public static void GiveEquipment(RunData run, EquipmentData item)
        {
            if (run.Inventory.Count < LootConfig.MaxInventorySize)
                run.Inventory.Add(item);
            else
                Debug.Log("[Event] Inventory full — item lost.");
        }

        public static void GiveRandomEquipment(RunData run)
        {
            EquipmentData item = LootGenerator.GenerateProceduralItem(run.CurrentAct);
            GiveEquipment(run, item);
        }

        public static void GiveConsumable(RunData run, string id)
        {
            run.AddConsumable(id);
        }

        public static void GiveRandomConsumable(RunData run)
        {
            string[] pool = {
                "health_potion", "energy_potion", "mana_potion",
                "antidote", "bandages", "fire_bomb", "smoke_bomb"
            };
            run.AddConsumable(pool[Random.Range(0, pool.Length)]);
        }

        public static int RandomPartyIndex(RunData run)
        {
            if (run.Party.Count == 0) return -1;
            return Random.Range(0, run.Party.Count);
        }

        /// <summary>
        /// Gives +amount to a random offensive stat (END/STA/INT/STR/DEX/WIL).
        /// Returns the stat name for display.
        /// </summary>
        public static string GiveRandomStat(RunData run, int partyIndex, int amount = 2)
        {
            if (partyIndex < 0 || partyIndex >= run.Party.Count) return "";
            int stat = Random.Range(0, 6);
            CharacterStats boost = stat switch
            {
                0 => new CharacterStats(amount, 0, 0, 0, 0, 0, 0, 0, 0),
                1 => new CharacterStats(0, amount, 0, 0, 0, 0, 0, 0, 0),
                2 => new CharacterStats(0, 0, amount, 0, 0, 0, 0, 0, 0),
                3 => new CharacterStats(0, 0, 0, amount, 0, 0, 0, 0, 0),
                4 => new CharacterStats(0, 0, 0, 0, amount, 0, 0, 0, 0),
                _ => new CharacterStats(0, 0, 0, 0, 0, amount, 0, 0, 0),
            };
            run.Party[partyIndex].BaseStats.Add(boost);
            return StatNames[stat];
        }

        /// <summary>
        /// Removes a random stat point. Returns the stat name for display.
        /// </summary>
        public static string TakeRandomStat(RunData run, int partyIndex, int amount = 1)
        {
            return GiveRandomStat(run, partyIndex, -amount);
        }

        public static int FindClassIndex(RunData run, CharacterClass cls)
        {
            for (int i = 0; i < run.Party.Count; i++)
            {
                if (run.Party[i].Class == cls)
                    return i;
            }
            return -1;
        }

        public static void RestoreResources(RunData run)
        {
            foreach (CharacterData c in run.Party)
            {
                CharacterStats total = c.GetTotalStats();
                c.CurrentEnergy = StatCalculator.CalculateMaxEnergy(total);
                c.CurrentMana = StatCalculator.CalculateMaxMana(total);
            }
        }

        public static void HealAndRestoreAll(RunData run, float healPercent)
        {
            HealPartyPercent(run, healPercent);
            RestoreResources(run);
        }
    }
}

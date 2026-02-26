using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PixelWarriors
{
    public static class ShopGenerator
    {
        public static ShopStock GenerateShopStock(RunData runData)
        {
            ShopStock stock = new ShopStock();

            // Equipment: procedural items via LootGenerator
            for (int i = 0; i < ShopConfig.EquipmentSlots; i++)
            {
                EquipmentData item = LootGenerator.GenerateProceduralItem(runData.CurrentAct);
                stock.Equipment.Add(new ShopEquipmentEntry
                {
                    Item = item,
                    Price = CalculateEquipmentPrice(item),
                    Sold = false
                });
            }

            // Consumables: weighted random from catalog, no duplicate types
            List<ConsumableData> pool = ConsumableCatalog.GetAll()
                .Where(c => c.UsableInBattle && c.MinAct <= runData.CurrentAct && c.Category != ConsumableCategory.Book)
                .ToList();

            ShuffleList(pool);
            int consumableCount = Mathf.Min(ShopConfig.ConsumableSlots, pool.Count);

            for (int i = 0; i < consumableCount; i++)
            {
                int stockQty = pool[i].Category switch
                {
                    ConsumableCategory.Potion => Random.Range(2, 5),
                    ConsumableCategory.Bomb => Random.Range(1, 3),
                    ConsumableCategory.Scroll => Random.Range(1, 2),
                    _ => Random.Range(1, 3)
                };

                stock.Consumables.Add(new ShopConsumableEntry
                {
                    Consumable = pool[i],
                    Stock = stockQty,
                    Price = pool[i].BuyPrice
                });
            }

            // Book: 20% chance
            if (Random.value < ShopConfig.BookChance)
            {
                List<ConsumableData> books = ConsumableCatalog.GetAll()
                    .Where(c => c.Category == ConsumableCategory.Book && c.MinAct <= runData.CurrentAct)
                    .ToList();

                if (books.Count > 0)
                {
                    ConsumableData book = books[Random.Range(0, books.Count)];
                    stock.Book = new ShopBookEntry
                    {
                        Book = book,
                        Price = book.BuyPrice,
                        Sold = false
                    };
                }
            }

            return stock;
        }

        public static int CalculateEquipmentPrice(EquipmentData item)
        {
            int actIndex = Mathf.Clamp(item.ActLevel - 1, 0, 2);
            float actMult = ShopConfig.ActPriceMultiplier[actIndex];

            int slotBase = item.Slot switch
            {
                EquipmentSlot.Hand1 => ShopConfig.BaseWeaponPrice,
                EquipmentSlot.Offhand => ShopConfig.BaseWeaponPrice,
                EquipmentSlot.Head => ShopConfig.BaseArmorPrice,
                EquipmentSlot.Body => ShopConfig.BaseArmorPrice,
                _ => ShopConfig.BaseTrinketPrice
            };

            int totalStatPoints = SumStatPoints(item.StatModifiers);
            int weaponValue = item.BaseDamage * ShopConfig.WeaponDamageValue;
            int basePrice = slotBase + (totalStatPoints * ShopConfig.StatPointValue) + weaponValue;

            if (item.IsUnique)
                basePrice += ShopConfig.UniqueMarkup;

            return Mathf.Max(5, Mathf.RoundToInt(basePrice * actMult));
        }

        public static int CalculateEquipmentSellPrice(EquipmentData item)
        {
            return Mathf.Max(1, Mathf.RoundToInt(CalculateEquipmentPrice(item) * ShopConfig.SellBackPercent));
        }

        public static int CalculateRerollCost(int act)
        {
            return ShopConfig.RerollCostPerAct * act;
        }

        private static int SumStatPoints(CharacterStats stats)
        {
            return Mathf.Abs(stats.Endurance) + Mathf.Abs(stats.Stamina)
                + Mathf.Abs(stats.Intellect) + Mathf.Abs(stats.Strength)
                + Mathf.Abs(stats.Dexterity) + Mathf.Abs(stats.Willpower)
                + Mathf.Abs(stats.Armor) + Mathf.Abs(stats.MagicResist)
                + Mathf.Abs(stats.Initiative);
        }

        private static void ShuffleList<T>(List<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }
    }
}

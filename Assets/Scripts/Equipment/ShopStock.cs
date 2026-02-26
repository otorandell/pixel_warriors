using System.Collections.Generic;

namespace PixelWarriors
{
    public class ShopStock
    {
        public List<ShopEquipmentEntry> Equipment = new();
        public List<ShopConsumableEntry> Consumables = new();
        public ShopBookEntry Book;
    }

    public class ShopEquipmentEntry
    {
        public EquipmentData Item;
        public int Price;
        public bool Sold;
    }

    public class ShopConsumableEntry
    {
        public ConsumableData Consumable;
        public int Stock;
        public int Price;
    }

    public class ShopBookEntry
    {
        public ConsumableData Book;
        public int Price;
        public bool Sold;
    }
}

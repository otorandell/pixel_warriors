namespace PixelWarriors
{
    public static class ShopConfig
    {
        public const int EquipmentSlots = 4;
        public const int ConsumableSlots = 5;
        public const float BookChance = 0.20f;
        public const int RerollCostPerAct = 15;
        public const float SellBackPercent = 0.50f;

        // Equipment pricing: base per slot + stat scaling
        public const int BaseWeaponPrice = 20;
        public const int BaseArmorPrice = 18;
        public const int BaseTrinketPrice = 15;
        public const int StatPointValue = 3;
        public const int WeaponDamageValue = 2;
        public const int UniqueMarkup = 15;

        // Act price multipliers (index = act - 1)
        public static readonly float[] ActPriceMultiplier = { 1.0f, 1.5f, 2.2f };
    }
}

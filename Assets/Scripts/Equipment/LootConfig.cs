namespace PixelWarriors
{
    public static class LootConfig
    {
        // --- Drop Counts ---
        public const int NormalBattleDrops = 1;
        public const int EliteBattleDrops = 2;
        public const int BossBattleDrops = 2;

        // --- Unique Drop Chance ---
        public const float UniqueDropChance = 0.15f;
        public const float EliteUniqueDropBonus = 0.10f;
        public const float BossUniqueDropBonus = 0.15f;

        // --- Stat Ranges Per Act (index 0=Act1, 1=Act2, 2=Act3) ---
        public static readonly int[] PrimaryStatMin = { 1, 3, 5 };
        public static readonly int[] PrimaryStatMax = { 3, 5, 8 };
        public static readonly int[] SecondaryStatMin = { 0, 1, 2 };
        public static readonly int[] SecondaryStatMax = { 1, 3, 4 };

        // --- Weapon Damage Per Act ---
        public static readonly int[] WeaponDamageMin = { 3, 5, 8 };
        public static readonly int[] WeaponDamageMax = { 6, 8, 12 };

        // --- Armor Stat Ranges ---
        public static readonly int[] ArmorStatMin = { 1, 2, 4 };
        public static readonly int[] ArmorStatMax = { 2, 4, 6 };

        // --- Block Chance (Shields) ---
        public static readonly float[] BlockChanceMin = { 0.10f, 0.12f, 0.15f };
        public static readonly float[] BlockChanceMax = { 0.15f, 0.20f, 0.25f };

        // --- Inventory ---
        public const int MaxInventorySize = 20;

        // --- Name Prefixes Per Act ---
        public static readonly string[][] ActPrefixes =
        {
            new[] { "Worn", "Old", "Rusty", "Crude" },
            new[] { "Sturdy", "Fine", "Tempered", "Forged" },
            new[] { "Master", "Elite", "Ancient", "Arcane" }
        };

        // --- Slot Drop Weights ---
        public const int WeaponWeight = 25;
        public const int OffhandWeight = 15;
        public const int HeadWeight = 15;
        public const int BodyWeight = 20;
        public const int TrinketWeight = 25;
    }
}

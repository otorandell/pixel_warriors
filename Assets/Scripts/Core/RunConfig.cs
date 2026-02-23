namespace PixelWarriors
{
    public static class RunConfig
    {
        // --- Run Structure ---
        public const int FloorsPerAct = 7;
        public const int ActCount = 3;

        // --- Party ---
        public const int MaxPartySize = 4;
        public const int StartingPartySize = 2;

        // --- Economy ---
        public const int StartingGold = 50;
        public const int BaseGoldPerBattle = 15;
        public const int GoldPerBattleVariance = 10;
        public const float GoldFloorScaling = 0.10f;

        // --- XP ---
        public const int BaseXPPerBattle = 30;
        public const int XPPerBattleVariance = 20;
        public const float XPFloorScaling = 0.08f;

        // --- Healing ---
        public const float HealBetweenBattlesPercent = 0.30f;
        public const float RestHealPercent = 0.50f;

        // --- Floor Generation ---
        public const int ShopGuaranteedFloor = 4;
        public const int RoomChoicesPerFloor = 2;

        // --- Encounter Sizing ---
        public const int MinEnemies = 2;
        public const int MaxEnemies = 4;
        public const float EliteStatMultiplier = 1.4f;
        public const float BossStatMultiplier = 2.0f;
        public const float FloorStatScaling = 0.08f; // +8% per floor

        // --- XP/Gold multipliers by room type ---
        public const float EliteXPMultiplier = 1.8f;
        public const float BossXPMultiplier = 2.5f;
        public const float EliteGoldMultiplier = 1.8f;
        public const float BossGoldMultiplier = 3.0f;
    }
}

namespace PixelWarriors
{
    public static class RunConfig
    {
        // --- Run Structure ---
        public const int FloorsPerAct = 20;
        public const int ActCount = 4;
        public const int BossFloor = 20;

        // --- Party ---
        public const int MaxPartySize = 4;
        public const int StartingPartySize = 4;

        // --- Economy ---
        public const int StartingGold = 50;
        public const int BaseGoldPerBattle = 15;
        public const int GoldPerBattleVariance = 10;
        public const float GoldFloorScaling = 0.05f;

        // --- Healing ---
        public const float HealBetweenBattlesPercent = 0.30f;
        public const float RestHealPercent = 0.50f;

        // --- Encounter Sizing ---
        public const int MinEnemies = 2;
        public const int MaxEnemies = 4;
        public const float EliteStatMultiplier = 1.4f;
        public const float BossStatMultiplier = 2.0f;
        public const float FloorStatScaling = 0.04f;

        // --- Reinforcements ---
        public const float NormalReinforcementChance = 0.40f;
        public const int NormalReinforcementWaveSize = 2;
        public const int EliteReinforcementWaveSize = 2;
        public const int BossReinforcementWaveSize = 2;

        // --- Gold multipliers by room type ---
        public const float EliteGoldMultiplier = 1.8f;
        public const float BossGoldMultiplier = 3.0f;

        // --- Floor Generation ---
        public const int ShopGuaranteedFloor = 5;
        public const int RoomChoicesPerFloor = 2;

        // --- Leveling ---
        public const int BossLevelUps = 2;
    }
}

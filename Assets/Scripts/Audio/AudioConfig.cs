namespace PixelWarriors
{
    public static class AudioConfig
    {
        // --- General ---
        public const int SampleRate = 44100;
        public const float MasterVolume = 0.5f;
        public const int AudioSourcePoolSize = 6;

        // --- UI Sounds ---
        public const float UIClickVolume = 0.3f;
        public const float UIConfirmVolume = 0.4f;
        public const float UICancelVolume = 0.3f;
        public const float UIPopupVolume = 0.25f;
        public const float TabClickVolume = 0.2f;

        // --- Combat Sounds ---
        public const float PhysicalHitVolume = 0.5f;
        public const float MagicalHitVolume = 0.5f;
        public const float CritHitVolume = 0.6f;
        public const float MissVolume = 0.25f;
        public const float DodgeVolume = 0.3f;
        public const float HealVolume = 0.4f;
        public const float DefeatedVolume = 0.5f;

        // --- Battle Flow Sounds ---
        public const float BattleStartVolume = 0.5f;
        public const float VictoryVolume = 0.6f;
        public const float BattleDefeatVolume = 0.5f;
        public const float TurnNotifyVolume = 0.2f;

        // --- Music ---
        public const float MusicVolume = 0.7f;
        public const string BattleThemePath = "Music/BattleTheme";

        // --- Pitch Variation ---
        public const float PitchVariationMin = 0.9f;
        public const float PitchVariationMax = 1.1f;
    }
}

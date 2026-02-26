using DG.Tweening;

namespace PixelWarriors
{
    public static class AnimationConfig
    {
        // --- Floating Text ---
        public const float FloatingTextDuration = 0.8f;
        public const float FloatingTextRiseDistance = 60f;
        public const float FloatingTextFadeDelay = 0.4f;       // Start fading at this point
        public const float FloatingTextFadeDuration = 0.4f;     // Fade over this duration
        public const float FloatingTextFontSize = 14f;
        public const float FloatingTextCritFontSize = 18f;
        public const float FloatingTextSpawnOffsetY = 20f;
        public const float FloatingTextStaggerDelay = 0.1f;     // Stagger multi-hit numbers
        public const int FloatingTextPoolSize = 16;

        // --- Attacker Lunge ---
        public const float LungePushDuration = 0.12f;
        public const float LungeReturnDuration = 0.10f;
        public const float LungeDistance = 25f;
        public static readonly Ease LungePushEase = Ease.OutQuad;
        public static readonly Ease LungeReturnEase = Ease.InQuad;

        // --- Receiver Shake ---
        public const float ShakeDuration = 0.25f;
        public const float ShakeStrength = 8f;
        public const float CritShakeStrength = 16f;
        public const int ShakeVibrato = 15;
        public const float ShakeRandomness = 90f;

        // --- Screen Shake (Crit) ---
        public const float ScreenShakeDuration = 0.2f;
        public const float ScreenShakeStrength = 6f;
        public const int ScreenShakeVibrato = 12;

        // --- Turn Transition ---
        public const float TurnPulseScale = 1.03f;
        public const float TurnPulseDuration = 0.15f;
        public static readonly Ease TurnPulseEase = Ease.OutBack;

        // --- Hit Visual Delay ---
        // Damage numbers and receiver shake are delayed to appear after lunge peak
        public const float HitVisualDelay = 0.12f;
    }
}

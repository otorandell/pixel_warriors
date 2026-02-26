using DG.Tweening;

namespace PixelWarriors
{
    public static class AnimationConfig
    {
        // --- Floating Text ---
        public const float FloatingTextDuration = 0.8f;
        public const float FloatingTextRiseDistance = 60f;
        public const float FloatingTextFadeDelay = 0.4f;
        public const float FloatingTextFadeDuration = 0.4f;
        public const float FloatingTextFontSize = 14f;
        public const float FloatingTextCritFontSize = 18f;
        public const float FloatingTextSpawnOffsetY = 20f;
        public const float FloatingTextStaggerDelay = 0.1f;
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
        public const float HitVisualDelay = 0.12f;

        // --- Hit Squeeze/Stretch ---
        public const float SqueezeX = 0.85f;       // Squash horizontally
        public const float SqueezeY = 1.15f;        // Stretch vertically
        public const float SqueezeDuration = 0.08f;  // Squash in
        public const float SqueezeReturnDuration = 0.15f; // Spring back
        public static readonly Ease SqueezeEase = Ease.OutQuad;
        public static readonly Ease SqueezeReturnEase = Ease.OutBack;

        // --- Color Flash (on damage) ---
        public const float ColorFlashDuration = 0.15f;

        // --- Death: Shrink + Fade ---
        public const float DeathShrinkDuration = 0.4f;
        public const float DeathFadeDuration = 0.3f;
        public const float DeathShrinkScale = 0f;
        public static readonly Ease DeathShrinkEase = Ease.InBack;

        // --- Heal Pulse ---
        public const float HealPulseScale = 1.04f;
        public const float HealPulseDuration = 0.2f;
        public const float HealBorderFlashDuration = 0.3f;

        // --- Shield Shimmer ---
        public const float ShieldShimmerDuration = 0.3f;
        public const int ShieldShimmerLoops = 2;

        // --- Buff Glow ---
        public const float BuffGlowDuration = 0.3f;

        // --- Stun Wobble ---
        public const float StunWobbleAngle = 3f;
        public const float StunWobbleDuration = 0.5f;

        // --- Low HP Flicker ---
        public const float LowHPThreshold = 0.20f;
        public const float LowHPFlickerMinAlpha = 0.5f;
        public const float LowHPFlickerDuration = 0.4f;
    }
}

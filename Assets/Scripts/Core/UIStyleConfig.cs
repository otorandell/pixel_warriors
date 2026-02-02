using UnityEngine;

namespace PixelWarriors
{
    public static class UIStyleConfig
    {
        // --- Colors ---
        public static readonly Color Background = Color.black;
        public static readonly Color PanelBorder = Color.white;
        public static readonly Color PanelBackground = new Color(0.05f, 0.05f, 0.05f, 1f);
        public static readonly Color TextPrimary = Color.white;
        public static readonly Color TextDimmed = new Color(0.6f, 0.6f, 0.6f, 1f);

        // Terminal accent colors
        public static readonly Color AccentCyan = new Color(0f, 1f, 1f, 1f);
        public static readonly Color AccentMagenta = new Color(1f, 0f, 1f, 1f);
        public static readonly Color AccentGreen = new Color(0f, 1f, 0f, 1f);
        public static readonly Color AccentYellow = new Color(1f, 1f, 0f, 1f);
        public static readonly Color AccentRed = new Color(1f, 0.2f, 0.2f, 1f);

        // Resource bar colors
        public static readonly Color HPBarColor = AccentRed;
        public static readonly Color EnergyBarColor = AccentYellow;
        public static readonly Color ManaBarColor = AccentCyan;
        public static readonly Color HPBarBackground = new Color(0.3f, 0.05f, 0.05f, 1f);
        public static readonly Color EnergyBarBackground = new Color(0.3f, 0.3f, 0.05f, 1f);
        public static readonly Color ManaBarBackground = new Color(0.05f, 0.2f, 0.3f, 1f);

        // Tab colors
        public static readonly Color TabActive = PanelBorder;
        public static readonly Color TabInactive = TextDimmed;

        // Highlight colors
        public static readonly Color TargetHighlight = AccentCyan;
        public static readonly Color ActiveTurnHighlight = AccentGreen;
        public static readonly Color StagedHighlight = AccentYellow;

        // --- Font Sizes ---
        public const float FontSizeLarge = 24f;
        public const float FontSizeMedium = 18f;
        public const float FontSizeSmall = 14f;
        public const float FontSizeTiny = 10f;

        // --- Spacing ---
        public const float PanelPadding = 8f;
        public const float ElementSpacing = 4f;
        public const float BarHeight = 6f;
        public const float BarSpacing = 2f;

        // --- Border ---
        public const float BorderWidth = 2f;
        public const float BorderRadius = 4f;

        // --- Layout Ratios ---
        public const float BattleGridWidthRatio = 0.45f;   // Left side
        public const float AbilityPanelWidthRatio = 0.55f;  // Right side
        public const float CombatLogHeightRatio = 0.15f;    // Bottom strip (legacy, unused)
        public const float TurnInfoHeightRatio = 0.07f;       // Top turn info panel
        public const float BottomAreaHeightRatio = 0.20f;      // Bottom area (log + selection)
        public const float BottomSelectionWidthRatio = 0.45f;  // Right side of bottom area
        public const float SelectionButtonWidthRatio = 0.15f;  // Cancel/Confirm button width
        public const float AbilityButtonHeight = 40f;          // Fixed height for scrollable ability buttons

        // --- Popup ---
        public const float PopupWidthRatio = 0.70f;
        public const float PopupHeightRatio = 0.75f;
        public static readonly Color PopupBlockerColor = new Color(0f, 0f, 0f, 0.6f);
        public const float PopupPadding = 12f;

        // --- Long Press ---
        public const float LongPressThreshold = 0.4f;
    }
}

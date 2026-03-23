using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace PixelWarriors
{
    /// <summary>
    /// Maps StatusEffect → TMP sprite tag with tint color.
    /// Icons are white sprites tinted at display time via rich text.
    /// Usage: IconConfig.GetStatusSprite(StatusEffect.Bleed) → "<sprite name="bleed" tint=1>"
    /// </summary>
    public static class IconConfig
    {
        private static TMP_SpriteAsset _cachedSpriteAsset;

        // Sprite names (must match PixelIconGenerator.IconNames)
        public const string Heart = "heart";
        public const string Shield = "shield";
        public const string Sword = "sword";
        public const string Skull = "skull";
        public const string Lightning = "lightning";
        public const string Mana = "mana";
        public const string Flame = "flame";
        public const string Snowflake = "snowflake";
        public const string Bleed = "bleed";
        public const string Poison = "poison";
        public const string Crosshair = "crosshair";
        public const string Eye = "eye";
        public const string Stun = "stun";
        public const string Silence = "silence";
        public const string Terror = "terror";
        public const string Chain = "chain";
        public const string ArrowUp = "arrow_up";
        public const string ArrowDown = "arrow_down";
        public const string Cloak = "cloak";
        public const string Fist = "fist";
        public const string Star = "star";
        public const string Halo = "halo";
        public const string Pin = "pin";
        public const string Trap = "trap";
        public const string Scope = "scope";
        public const string Helmet = "helmet";
        public const string IceBlock = "ice_block";
        public const string Orb = "orb";
        public const string Regen = "regen";
        public const string Imbue = "imbue";
        public const string Caltrops = "caltrops";
        public const string Dodge = "dodge";

        private static readonly Dictionary<StatusEffect, (string sprite, Color color)> StatusIconMap = new()
        {
            { StatusEffect.Shield,             (Shield,    UIStyleConfig.AccentCyan) },
            { StatusEffect.Mark,               (Crosshair, UIStyleConfig.AccentRed) },
            { StatusEffect.Taunt,              (Fist,      UIStyleConfig.AccentYellow) },
            { StatusEffect.Hide,               (Eye,       UIStyleConfig.TextDimmed) },
            { StatusEffect.Conceal,            (Cloak,     UIStyleConfig.TextDimmed) },
            { StatusEffect.Bleed,              (Bleed,     UIStyleConfig.AccentRed) },
            { StatusEffect.Poison,             (Poison,    UIStyleConfig.AccentGreen) },
            { StatusEffect.Burn,               (Flame,     new Color(1f, 0.5f, 0f)) },
            { StatusEffect.Chilled,            (Snowflake, UIStyleConfig.AccentCyan) },
            { StatusEffect.Stun,               (Stun,      UIStyleConfig.AccentYellow) },
            { StatusEffect.Silence,            (Silence,   UIStyleConfig.AccentMagenta) },
            { StatusEffect.Terror,             (Terror,    UIStyleConfig.AccentMagenta) },
            { StatusEffect.StanceDefensive,    (Shield,    UIStyleConfig.AccentCyan) },
            { StatusEffect.StanceBrawling,     (Fist,      UIStyleConfig.AccentYellow) },
            { StatusEffect.StanceBerserker,    (Flame,     UIStyleConfig.AccentRed) },
            { StatusEffect.Block,              (Shield,    UIStyleConfig.TextPrimary) },
            { StatusEffect.Regeneration,       (Regen,     UIStyleConfig.AccentGreen) },
            { StatusEffect.Blessing,           (Star,      UIStyleConfig.AccentYellow) },
            { StatusEffect.DivineIntervention, (Halo,      UIStyleConfig.TextPrimary) },
            { StatusEffect.Pin,                (Pin,       UIStyleConfig.TextDimmed) },
            { StatusEffect.HuntersFocus,       (Scope,     UIStyleConfig.AccentYellow) },
            { StatusEffect.IronWill,           (Helmet,    UIStyleConfig.TextPrimary) },
            { StatusEffect.FrozenTomb,         (IceBlock,  UIStyleConfig.AccentCyan) },
            { StatusEffect.SoulLink,           (Chain,     UIStyleConfig.AccentMagenta) },
            { StatusEffect.DrainSoul,          (Orb,       UIStyleConfig.AccentMagenta) },
            { StatusEffect.Trap,               (Trap,      UIStyleConfig.AccentYellow) },
            { StatusEffect.Envenom,            (Poison,    UIStyleConfig.AccentGreen) },
            { StatusEffect.Imbue,              (Imbue,     UIStyleConfig.AccentCyan) },
            { StatusEffect.UltimateReflexes,   (Dodge,     UIStyleConfig.AccentGreen) },
            { StatusEffect.Caltrops,           (Caltrops,  UIStyleConfig.AccentYellow) },
            { StatusEffect.Levitate,           (ArrowUp,   UIStyleConfig.AccentCyan) },
            { StatusEffect.Confusion,          (Stun,      UIStyleConfig.AccentMagenta) },
            { StatusEffect.CorpseExplosion,    (Skull,     UIStyleConfig.AccentRed) },
            { StatusEffect.LeechLife,          (Heart,     UIStyleConfig.AccentMagenta) },
            { StatusEffect.Bodyguard,          (Shield,    UIStyleConfig.AccentYellow) },
            { StatusEffect.ElementalArmor,     (Shield,    UIStyleConfig.AccentCyan) },
        };

        public static TMP_SpriteAsset GetSpriteAsset()
        {
            if (_cachedSpriteAsset == null)
            {
                _cachedSpriteAsset = Resources.Load<TMP_SpriteAsset>("Fonts/StatusIcons Sprite Asset");
                if (_cachedSpriteAsset == null)
                {
                    Debug.LogWarning("[IconConfig] StatusIcons Sprite Asset not found at Resources/Fonts/. " +
                                     "Run Tools > Generate Status Icons Sprite Sheet, then Tools > Create Status Icons TMP Sprite Asset.");
                }
            }
            return _cachedSpriteAsset;
        }

        /// <summary>
        /// Returns the TMP rich text tag for a status effect icon with color tint.
        /// Example: "<color=#FF3333><sprite name="bleed" tint=1></color>"
        /// </summary>
        public static string GetStatusSprite(StatusEffect effect)
        {
            if (!StatusIconMap.TryGetValue(effect, out var entry)) return "";
            string hex = ColorUtility.ToHtmlStringRGB(entry.color);
            return $"<color=#{hex}><sprite name=\"{entry.sprite}\" tint=1></color>";
        }

        /// <summary>
        /// Returns a colored sprite tag for a named icon.
        /// </summary>
        public static string Sprite(string spriteName, Color color)
        {
            string hex = ColorUtility.ToHtmlStringRGB(color);
            return $"<color=#{hex}><sprite name=\"{spriteName}\" tint=1></color>";
        }

        /// <summary>
        /// Returns a white (untinted) sprite tag.
        /// </summary>
        public static string Sprite(string spriteName)
        {
            return $"<sprite name=\"{spriteName}\">";
        }

        /// <summary>
        /// Shortcut for common resource icons used across UI.
        /// </summary>
        public static string HPIcon => Sprite(Heart, UIStyleConfig.HPBarColor);
        public static string EnergyIcon => Sprite(Lightning, UIStyleConfig.EnergyBarColor);
        public static string ManaIcon => Sprite(Mana, UIStyleConfig.ManaBarColor);
        public static string SwordIcon => Sprite(Sword, UIStyleConfig.TextPrimary);
        public static string SkullIcon => Sprite(Skull, UIStyleConfig.AccentRed);
    }
}

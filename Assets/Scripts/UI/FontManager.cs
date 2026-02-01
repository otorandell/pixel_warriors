using TMPro;
using UnityEngine;

namespace PixelWarriors
{
    public static class FontManager
    {
        private static TMP_FontAsset _cachedFont;

        public static TMP_FontAsset GetFont()
        {
            if (_cachedFont == null)
            {
                _cachedFont = Resources.Load<TMP_FontAsset>("Fonts/PressStart2P-Regular");

                if (_cachedFont == null)
                {
                    Debug.LogWarning("[FontManager] TMP font asset not found at Resources/Fonts/PressStart2P-Regular SDF. Using TMP default.");
                    _cachedFont = TMP_Settings.defaultFontAsset;
                }
            }

            return _cachedFont;
        }
    }
}

using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PixelWarriors
{
    public static class PanelBuilder
    {
        public static RectTransform CreatePanel(string name, Transform parent)
        {
            GameObject go = new GameObject(name);
            RectTransform rect = go.AddComponent<RectTransform>();
            rect.SetParent(parent, false);

            Image bg = go.AddComponent<Image>();
            bg.color = UIStyleConfig.PanelBackground;

            AddBorder(go);

            return rect;
        }

        public static RectTransform CreateContainer(string name, Transform parent)
        {
            GameObject go = new GameObject(name);
            RectTransform rect = go.AddComponent<RectTransform>();
            rect.SetParent(parent, false);
            return rect;
        }

        public static TextMeshProUGUI CreateText(string name, Transform parent, string content,
            float fontSize = -1, TextAlignmentOptions alignment = TextAlignmentOptions.MidlineLeft,
            Color? color = null)
        {
            GameObject go = new GameObject(name);
            RectTransform rect = go.AddComponent<RectTransform>();
            rect.SetParent(parent, false);

            TextMeshProUGUI text = go.AddComponent<TextMeshProUGUI>();
            text.text = content;
            text.font = FontManager.GetFont();
            text.fontSize = fontSize < 0 ? UIStyleConfig.FontSizeSmall : fontSize;
            text.alignment = alignment;
            text.color = color ?? UIStyleConfig.TextPrimary;
            text.overflowMode = TextOverflowModes.Overflow;
            text.textWrappingMode = TextWrappingModes.NoWrap;

            return text;
        }

        public static Image CreateBar(string name, Transform parent, Color fillColor, Color bgColor)
        {
            // Background
            GameObject bgGo = new GameObject(name + "_BG");
            RectTransform bgRect = bgGo.AddComponent<RectTransform>();
            bgRect.SetParent(parent, false);
            Image bgImage = bgGo.AddComponent<Image>();
            bgImage.color = bgColor;

            // Fill
            GameObject fillGo = new GameObject(name + "_Fill");
            RectTransform fillRect = fillGo.AddComponent<RectTransform>();
            fillRect.SetParent(bgRect, false);
            fillRect.anchorMin = Vector2.zero;
            fillRect.anchorMax = Vector2.one;
            fillRect.offsetMin = Vector2.zero;
            fillRect.offsetMax = Vector2.zero;
            fillRect.pivot = new Vector2(0f, 0.5f);

            Image fillImage = fillGo.AddComponent<Image>();
            fillImage.color = fillColor;

            return fillImage;
        }

        public static Button CreateButton(string name, Transform parent, string label,
            Color? textColor = null, float fontSize = -1)
        {
            GameObject go = new GameObject(name);
            RectTransform rect = go.AddComponent<RectTransform>();
            rect.SetParent(parent, false);

            Image bg = go.AddComponent<Image>();
            bg.color = UIStyleConfig.PanelBackground;

            Button button = go.AddComponent<Button>();
            ColorBlock colors = button.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = UIStyleConfig.AccentCyan;
            colors.pressedColor = UIStyleConfig.AccentGreen;
            colors.disabledColor = UIStyleConfig.TextDimmed;
            button.colors = colors;

            CreateText(name + "_Label", rect, label,
                fontSize < 0 ? UIStyleConfig.FontSizeSmall : fontSize,
                TextAlignmentOptions.Center,
                textColor ?? UIStyleConfig.TextPrimary);

            // Fill the button
            RectTransform labelRect = go.transform.GetChild(0).GetComponent<RectTransform>();
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = Vector2.zero;
            labelRect.offsetMax = Vector2.zero;

            AddBorder(go);

            return button;
        }

        private static void AddBorder(GameObject go)
        {
            // Top border
            CreateBorderEdge(go.transform, "Border_Top",
                new Vector2(0, 1), new Vector2(1, 1),
                new Vector2(0, 0), new Vector2(0, UIStyleConfig.BorderWidth));

            // Bottom border
            CreateBorderEdge(go.transform, "Border_Bottom",
                new Vector2(0, 0), new Vector2(1, 0),
                new Vector2(0, -UIStyleConfig.BorderWidth), new Vector2(0, 0));

            // Left border
            CreateBorderEdge(go.transform, "Border_Left",
                new Vector2(0, 0), new Vector2(0, 1),
                new Vector2(-UIStyleConfig.BorderWidth, 0), new Vector2(0, 0));

            // Right border
            CreateBorderEdge(go.transform, "Border_Right",
                new Vector2(1, 0), new Vector2(1, 1),
                new Vector2(0, 0), new Vector2(UIStyleConfig.BorderWidth, 0));
        }

        private static void CreateBorderEdge(Transform parent, string name,
            Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax)
        {
            GameObject edge = new GameObject(name);
            RectTransform rect = edge.AddComponent<RectTransform>();
            rect.SetParent(parent, false);
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = offsetMin;
            rect.offsetMax = offsetMax;

            Image img = edge.AddComponent<Image>();
            img.color = UIStyleConfig.PanelBorder;
            img.raycastTarget = false;
        }

        public static void SetFill(RectTransform rect, float padding = 0f)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = new Vector2(padding, padding);
            rect.offsetMax = new Vector2(-padding, -padding);
        }

        public static void SetAnchored(RectTransform rect,
            float anchorMinX, float anchorMinY, float anchorMaxX, float anchorMaxY,
            float offsetMinX = 0, float offsetMinY = 0, float offsetMaxX = 0, float offsetMaxY = 0)
        {
            rect.anchorMin = new Vector2(anchorMinX, anchorMinY);
            rect.anchorMax = new Vector2(anchorMaxX, anchorMaxY);
            rect.offsetMin = new Vector2(offsetMinX, offsetMinY);
            rect.offsetMax = new Vector2(offsetMaxX, offsetMaxY);
        }
    }
}

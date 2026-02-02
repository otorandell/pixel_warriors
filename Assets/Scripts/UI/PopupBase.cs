using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PixelWarriors
{
    public class PopupBase
    {
        protected RectTransform _root;
        protected RectTransform _contentArea;
        protected readonly List<GameObject> _contentChildren = new();

        public bool IsVisible => _root != null && _root.gameObject.activeSelf;

        public void Build(Transform canvasTransform)
        {
            // Root fills canvas, hidden by default
            GameObject rootGo = new GameObject(GetType().Name);
            _root = rootGo.AddComponent<RectTransform>();
            _root.SetParent(canvasTransform, false);
            PanelBuilder.SetFill(_root);
            _root.gameObject.SetActive(false);

            // Blocker (full-screen semi-transparent, catches taps to dismiss)
            GameObject blockerGo = new GameObject("Blocker");
            RectTransform blockerRect = blockerGo.AddComponent<RectTransform>();
            blockerRect.SetParent(_root, false);
            PanelBuilder.SetFill(blockerRect);
            Image blockerImage = blockerGo.AddComponent<Image>();
            blockerImage.color = UIStyleConfig.PopupBlockerColor;
            Button blockerButton = blockerGo.AddComponent<Button>();
            blockerButton.transition = Selectable.Transition.None;
            blockerButton.onClick.AddListener(Hide);

            // Popup panel (centered)
            float w = UIStyleConfig.PopupWidthRatio;
            float h = UIStyleConfig.PopupHeightRatio;
            float xMin = (1f - w) / 2f;
            float yMin = (1f - h) / 2f;

            RectTransform popupPanel = PanelBuilder.CreatePanel("PopupPanel", _root);
            PanelBuilder.SetAnchored(popupPanel, xMin, yMin, xMin + w, yMin + h);

            // Popup also dismisses on tap
            Button popupButton = popupPanel.gameObject.AddComponent<Button>();
            popupButton.transition = Selectable.Transition.None;
            popupButton.onClick.AddListener(Hide);

            // Content area (padded interior)
            float padding = UIStyleConfig.PopupPadding;
            _contentArea = PanelBuilder.CreateContainer("Content", popupPanel);
            PanelBuilder.SetFill(_contentArea, padding);
        }

        public void Hide()
        {
            if (_root != null)
            {
                _root.gameObject.SetActive(false);
            }
        }

        protected void Show()
        {
            if (_root != null)
            {
                _root.SetAsLastSibling();
                _root.gameObject.SetActive(true);
            }
        }

        protected void ClearContent()
        {
            foreach (GameObject child in _contentChildren)
            {
                Object.Destroy(child);
            }
            _contentChildren.Clear();
        }

        protected TextMeshProUGUI AddText(string content, float fontSize, Color color,
            float anchorMinY, float anchorMaxY)
        {
            TextMeshProUGUI text = PanelBuilder.CreateText("PopupText", _contentArea, content,
                fontSize, TextAlignmentOptions.TopLeft, color);
            RectTransform rect = text.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(rect, 0, anchorMinY, 1, anchorMaxY);
            _contentChildren.Add(text.gameObject);
            return text;
        }
    }
}

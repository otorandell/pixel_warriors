using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace PixelWarriors
{
    public class ScreenManager
    {
        private Canvas _canvas;
        private IScreen _currentScreen;

        public Transform CanvasParent => _canvas.transform;

        public ScreenManager()
        {
            EnsureEventSystem();
            BuildCanvas();
        }

        public void ShowScreen(IScreen screen)
        {
            _currentScreen?.Hide();
            _currentScreen = screen;
            _currentScreen.Show();
        }

        public void TransitionTo(IScreen newScreen)
        {
            if (_currentScreen != null)
            {
                _currentScreen.Hide();
                _currentScreen.Destroy();
            }

            _currentScreen = newScreen;
            _currentScreen.Build(CanvasParent);
            _currentScreen.Show();
        }

        public void HideAll()
        {
            _currentScreen?.Hide();
            _currentScreen = null;
        }

        private void EnsureEventSystem()
        {
            if (Object.FindAnyObjectByType<EventSystem>() != null) return;

            GameObject esGo = new GameObject("EventSystem");
            esGo.AddComponent<EventSystem>();
            esGo.AddComponent<InputSystemUIInputModule>();
        }

        private void BuildCanvas()
        {
            GameObject canvasGo = new GameObject("GameCanvas");
            _canvas = canvasGo.AddComponent<Canvas>();
            _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            _canvas.sortingOrder = 0;

            CanvasScaler scaler = canvasGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(960, 540);
            scaler.matchWidthOrHeight = 0.5f;

            canvasGo.AddComponent<GraphicRaycaster>();

            // Background
            GameObject bgGo = new GameObject("Background");
            RectTransform bgRect = bgGo.AddComponent<RectTransform>();
            bgRect.SetParent(_canvas.transform, false);
            PanelBuilder.SetFill(bgRect);
            Image bgImage = bgGo.AddComponent<Image>();
            bgImage.color = UIStyleConfig.Background;
            bgImage.raycastTarget = false;
        }
    }
}

using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PixelWarriors
{
    public class LongPressHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        public event Action OnLongPress;
        public bool WasLongPress { get; private set; }

        private float _pointerDownTime;
        private bool _isPointerDown;
        private bool _longPressTriggered;

        public void OnPointerDown(PointerEventData eventData)
        {
            _pointerDownTime = Time.unscaledTime;
            _isPointerDown = true;
            _longPressTriggered = false;
            WasLongPress = false;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _isPointerDown = false;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _isPointerDown = false;
        }

        private void Update()
        {
            if (!_isPointerDown || _longPressTriggered) return;

            if (Time.unscaledTime - _pointerDownTime >= UIStyleConfig.LongPressThreshold)
            {
                _longPressTriggered = true;
                WasLongPress = true;
                OnLongPress?.Invoke();
            }
        }
    }
}

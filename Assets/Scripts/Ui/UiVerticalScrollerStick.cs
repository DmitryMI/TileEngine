using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Ui
{
    [RequireComponent(typeof(RectTransform))]
    class UiVerticalScrollerStick : UiElement, IDragable
    {
        private Vector2 _prevMousePos;
        private bool _isActive;

        private UiVerticalScroller _parentScroller;
        private RectTransform _rectTransform;
        private RectTransform _parentRectTransform;

        private float _currentY;
        private float _halfHeight;
        private float _maxY;
        private float _minY;

        private void Start()
        {
            _rectTransform = GetComponent<RectTransform>();

            FindParentScroller();

            if (_parentScroller != null)
                _parentRectTransform = _parentScroller.RectTransformComponent;
        }

        public void SetParentScroller(UiVerticalScroller parent)
        {
            _parentScroller = parent;
            if (_parentScroller != null)
                _parentRectTransform = _parentScroller.RectTransformComponent;
        }

        private void FindParentScroller()
        {
            _parentScroller = GetComponentInParent<UiVerticalScroller>();
        }

        public void OnDragStart(IPointerDataProvider pointerDataProvider)
        {
            _prevMousePos = pointerDataProvider.MouseScreenPosition;
        }

        public void ForceValue(float value)
        {
            float y = (_maxY - _minY) * value;
            _currentY = y;
        }

        public void OnDragContinue(IPointerDataProvider pointerDataProvider)
        {
            float deltaY = pointerDataProvider.MouseScreenPosition.y - _prevMousePos.y;
            float shift = deltaY * _parentScroller.Multiplier;

            MoveStick(shift);

            _prevMousePos = pointerDataProvider.MouseScreenPosition;
        }

        public void OnDragEnd(IDropReceiver receiver, IPointerDataProvider pointerDataProvider)
        {

        }

        public override void SetActive()
        {
            _isActive = true;
        }

        private void MoveStick(float shift)
        {
            _currentY += shift;
        }

        private void LateUpdate()
        {
            _halfHeight = _rectTransform.sizeDelta.y / 2f;

            _minY = _halfHeight;
            _maxY = _parentRectTransform.anchoredPosition.y + _parentRectTransform.rect.size.y - _halfHeight;
            
            if (_currentY > _maxY)
                _currentY = _maxY;
            if (_currentY < _minY)
                _currentY = _minY;

            Vector2 anchoredPos = _rectTransform.anchoredPosition;
            
            anchoredPos.y = _currentY;

            _rectTransform.anchoredPosition = anchoredPos;

            _parentScroller.ScrollValue = (_currentY - _minY) / (_maxY - _minY);
            
            _isActive = false;
        }
    }
}

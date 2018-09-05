using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Ui
{
    class HideableInventoryPanel : UiElement
    {
        [SerializeField] private Vector2 SizeDifference = new Vector2(1f, 1f);

        [SerializeField] private Vector2 _size;

        [SerializeField]
        private List<GameObject> _children;

        private RectTransform _rectTransform;
        private Image _renderer;

        private bool _isCollapsing;
        private bool _isUnrolling;


        public void Hide()
        {
            ShowHideChildren(false);

            _isCollapsing = true;
            _isUnrolling = false;
            StartCoroutine(CollapseDelayed());
        }

        public void HideImmidiately()
        {
            ShowHideChildren(false);
            _renderer.enabled = false;
            _isCollapsing = false;
            _isUnrolling = false;

            _rectTransform.sizeDelta = Vector2.zero;
            _rectTransform.anchoredPosition = Vector2.zero;
        }

        public void Show()
        {
            _isCollapsing = false;
            _isUnrolling = true;
            StartCoroutine(UnrollDelayed());
        }

        private void Start()
        {
            _rectTransform = GetComponent<RectTransform>();
            _renderer = GetComponent<Image>();
        }

        private void ShowHideChildren(bool active)
        {
            foreach (var child in _children)
            {
                child.SetActive(active);
            }
        }

        private IEnumerator CollapseDelayed()
        {
            while (_rectTransform.sizeDelta != Vector2.zero && _isCollapsing)
            {
                Vector2 anchoredPos = _rectTransform.anchoredPosition;
                Vector2 size = _rectTransform.sizeDelta;
                size -= SizeDifference;
                anchoredPos -= SizeDifference / 2;

                if (size.x < 1 || size.y < 1)
                {
                    break;
                }

                _rectTransform.sizeDelta = size;
                _rectTransform.anchoredPosition = anchoredPos;

                yield return new WaitForEndOfFrame();
            }

            if (_isCollapsing)
            {
                _renderer.enabled = false;
                _isCollapsing = false;
            }
        }

        private IEnumerator UnrollDelayed()
        {
            _renderer.enabled = true;

            while (_isUnrolling)
            {
                Vector2 anchoredPos = _rectTransform.anchoredPosition;
                Vector2 size = _rectTransform.sizeDelta;
                size += SizeDifference;
                anchoredPos += SizeDifference / 2;

                if (size.x >= _size.x || size.y >= _size.y)
                {
                    break;
                }

                _rectTransform.sizeDelta = size;
                _rectTransform.anchoredPosition = anchoredPos;

                yield return new WaitForEndOfFrame();
            }

            if (_isUnrolling)
            {
                _isUnrolling = false;
                ShowHideChildren(true);
            }
        }
    }
}

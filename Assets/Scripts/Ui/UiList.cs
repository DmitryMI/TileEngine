using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.UIElements.StyleEnums;

namespace Assets.Scripts.Ui
{
    public class UiList : UiElement
    {
        [Tooltip("Children of this transfrom will be scrolled")]
        [SerializeField] private Transform _contentGroup;

        [SerializeField] private float _elementDistance;
        [SerializeField] private float _leftOffset;
        [SerializeField] private bool _startFromTop = true;
        [SerializeField] private bool _scrollInverse;

        private IScroller _scrollerUi;

        private List<RectTransform> _elementList;
        private RectTransform _rectTransform;

        public void Add(RectTransform element)
        {
            AddElementToBottom(element);
        }

        public void Remove(RectTransform element)
        {
            RemoveElement(element);
        }

        private void Start()
        {
            _scrollerUi = GetComponentInChildren<IScroller>();

            if (_scrollerUi == null)
            {
                Debug.LogWarning("No scrollers found");
            }

            _elementList = new List<RectTransform>();

            _rectTransform = GetComponent<RectTransform>();

            FindChildren();

            if (!_scrollInverse)
                _scrollerUi.ScrollValue = 1 - _scrollerUi.ScrollValue;
        }

        private void FindChildren()
        {
            for (int i = 0; i < _contentGroup.childCount; i++)
            {
                Transform childTransform = _contentGroup.GetChild(i);
                RectTransform childRectTransform = childTransform.gameObject.GetComponent<RectTransform>();
                if (childRectTransform != null)
                {
                    AddElementToBottom(childRectTransform);
                }
            }
        }
        

        private void AddElementToBottom(RectTransform element)
        {
            if (_elementList == null)
            {
                _elementList = new List<RectTransform>();
            }
            element.transform.SetParent(_contentGroup, false);

            element.anchorMin = Vector2.zero;
            element.anchorMax = Vector2.zero;

            _elementList.Add(element);
        }

        private void RemoveElement(RectTransform element)
        {
            if(_elementList == null)
                return;

            element.transform.SetParent(null);
            _elementList.Remove(element);
        }

        private void ProcessPlacement()
        {
            if(_elementList.Count == 0)
                return;

            float startX = _elementList[0].rect.width / 2 + _leftOffset;
            float startY = _elementList[0].rect.height / 2;

            _elementList[0].anchoredPosition = new Vector2(startX, startY);
            
            for (int i = 1; i < _elementList.Count; i++)
            {
                float x = _elementList[i].rect.width / 2 + _leftOffset;

                float y = 
                    _elementList[i - 1].anchoredPosition.y +
                    _elementList[i - 1].rect.height / 2 +
                    _elementDistance +
                    _elementList[i].rect.height / 2;

                _elementList[i].anchoredPosition = new Vector2(x, y);
            }

            float highestPos = _elementList[0].anchoredPosition.y;
            float lowestPos = _elementList.Last().anchoredPosition.y;

            if (highestPos < lowestPos)
            {
                float tmp = highestPos;
                highestPos = lowestPos;
                lowestPos = tmp;
            }

            if (_startFromTop)
            {
                lowestPos -= _elementList.Last().rect.height;
            }
            else
            {
                highestPos += _elementList.Last().rect.height;
            }

            float scrollerValue = _scrollerUi.ScrollValue;

            if (_scrollInverse)
                scrollerValue = 1 - scrollerValue;

            float hight = scrollerValue * (highestPos - lowestPos - _rectTransform.rect.height);
            
            for (int i = 0; i < _elementList.Count; i++)
            {
                Vector2 anchoredPosition = _elementList[i].anchoredPosition;

                if (_startFromTop)
                    anchoredPosition.y -= hight;
                else
                    anchoredPosition.y += hight;

                _elementList[i].anchoredPosition = anchoredPosition;
            }


            if (_startFromTop)
            {
                for (int i = 0; i < _elementList.Count; i++)
                {
                    Vector2 anchoredPosition = _elementList[i].anchoredPosition;

                    anchoredPosition.y = _rectTransform.rect.height - anchoredPosition.y;

                    _elementList[i].anchoredPosition = anchoredPosition;
                }
            }
        }

        protected override void Update()
        {
            base.Update();

            ProcessPlacement();
        }
    }
}

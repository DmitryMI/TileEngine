using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Ui;
using UnityEngine;

namespace Assets._MapEditor.Scripts
{
    class CategoryButton : UiElement, IUiList
    {
        [SerializeField]
        protected float ElementDistance;

        [SerializeField] protected float LeftOffset;
        protected List<RectTransform> ElementList;

        [SerializeField] protected Transform ContentGroup;

        [SerializeField] protected RectTransform HeadRectTransform;

        protected RectTransform RectTransform;
        protected RectTransform ContentGroupRectTransform;

        protected virtual void Start()
        {
            RectTransform = GetComponent<RectTransform>();
            ContentGroupRectTransform = ContentGroup.gameObject.GetComponent<RectTransform>();
            FindChildren();
        }

        protected override void Update()
        {
            ProcessPlacement();
        }

        protected void ProcessPlacement()
        {
            if (ElementList == null || ElementList.Count == 0)
                return;

            float startX = ElementList[0].rect.width / 2 + LeftOffset;
            float startY = ElementList[0].rect.height / 2;

            ElementList[0].anchoredPosition = new Vector2(startX, startY);

            for (int i = 1; i < ElementList.Count; i++)
            {
                float x = ElementList[i].rect.width / 2 + LeftOffset;

                float y =
                    ElementList[i - 1].anchoredPosition.y +
                    ElementList[i - 1].rect.height / 2 +
                    ElementDistance +
                    ElementList[i].rect.height / 2;

                ElementList[i].anchoredPosition = new Vector2(x, y);
            }


            for (int i = 0; i < ElementList.Count; i++)
            {
                Vector2 anchoredPosition = ElementList[i].anchoredPosition;

                anchoredPosition.y = ContentGroupRectTransform.rect.height - anchoredPosition.y;

                ElementList[i].anchoredPosition = anchoredPosition;
            }


            float highestPos = ElementList[0].anchoredPosition.y;
            float lowestPos = ElementList.Last().anchoredPosition.y;

            if (highestPos < lowestPos)
            {
                float tmp = highestPos;
                highestPos = lowestPos;
                lowestPos = tmp;
            }

            highestPos += ElementList[0].rect.height / 2;
            lowestPos -= ElementList.Last().rect.height / 2;

            Vector2 size = RectTransform.sizeDelta;
            size.y = highestPos - lowestPos + HeadRectTransform.rect.height;

            RectTransform.sizeDelta = size;
        }

        private void FindChildren()
        {
            for (int i = 0; i < ContentGroup.childCount; i++)
            {
                Transform childTransform = ContentGroup.GetChild(i);
                RectTransform childRectTransform = childTransform.gameObject.GetComponent<RectTransform>();
                if (childRectTransform != null)
                {
                    AddElementToBottom(childRectTransform);
                }
            }
        }


        private void AddElementToBottom(RectTransform element)
        {
            if (ElementList == null)
            {
                ElementList = new List<RectTransform>();
            }
            element.transform.SetParent(ContentGroup, false);

            element.anchorMin = Vector2.zero;
            element.anchorMax = Vector2.zero;

            ElementList.Add(element);
        }

        private void RemoveElement(RectTransform element)
        {
            if (ElementList == null)
                return;

            element.transform.SetParent(null);
            ElementList.Remove(element);
        }

        public void Add(RectTransform element)
        {
            AddElementToBottom(element);
        }

        public void Remove(RectTransform element)
        {
            RemoveElement(element);
        }

        public void Clear()
        {
            foreach (var element in ElementList)
            {
                RemoveElement(element);
                Destroy(element.gameObject);
            }
        }
    }
}

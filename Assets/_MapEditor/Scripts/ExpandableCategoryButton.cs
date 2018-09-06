using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Ui;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._MapEditor.Scripts
{
    class ExpandableCategoryButton : CategoryButton, IUiList
    {
        [SerializeField] private Image _image;
        [SerializeField] private Text _text;
        [SerializeField] private Vector2 _collapsedSize;
        [SerializeField] private Vector2 _headDefaultMinAnchor;

        [SerializeField]
        private bool _expanded = true;
        private Vector2 _prevSize;

        public void SetText(string text)
        {
            _text.text = text;
        }

        public void Expand()
        {
            _expanded = true;
            ShowContents(true);
        }

        public void Collapse()
        {
            _expanded = false;
            ShowContents(false);
        }

        public override void Click()
        {
            _expanded = !_expanded;
            ShowContents(_expanded);
        }

        protected override void Update()
        {
            if(_expanded)
                ProcessPlacement();
        }

        private void ShowContents(bool expanded)
        {
            if (RectTransform == null)
            {
                RectTransform = GetComponent<RectTransform>();
            }

            if (!expanded)
            {
                ContentGroup.gameObject.SetActive(false);
                _prevSize = RectTransform.rect.size;

                RectTransform.sizeDelta = _collapsedSize;
                HeadRectTransform.anchorMin = Vector2.zero;
            }
            else
            {
                HeadRectTransform.anchorMin = _headDefaultMinAnchor;
                RectTransform.sizeDelta = _prevSize;
                ContentGroup.gameObject.SetActive(true);
            }
        }

        public RectTransform[] GetContentList()
        {
            return ElementList.ToArray();
        }
    }
}

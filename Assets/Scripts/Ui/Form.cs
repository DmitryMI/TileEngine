using Assets._MapEditor.Scripts;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts.Ui
{

    [RequireComponent(typeof(Image))]
    public class Form : UiElement, IDragable
    {
        private CustomInputModule _inputModule;
        private IPointerDataProvider _pointerDataProvider;
        private Vector2 _dragCursorPrevPos;
        private RectTransform _rectTransform;

        [SerializeField] private Vector2 _headAnchorMin;
        [SerializeField] private Vector2 _headAnchorMax;

        [SerializeField] private RectTransform _headRectTransform;

        [SerializeField] private FormButton _collapseButton;

        [SerializeField]
        private bool _replaceable;
        [SerializeField]
        private bool _resizeableByUser;
        [SerializeField]
        private bool _collapsed;

        private Vector2 _prevSize;

        protected virtual void Start()
        {
            _inputModule = (CustomInputModule)EventSystem.current.currentInputModule;
            _rectTransform = GetComponent<RectTransform>();

            if (_collapseButton != null)
            {
                _collapseButton.SetParentForm(this);
                _collapseButton.SetAction(CollapseButtonClick);
            }
        }

        private void CollapseButtonClick()
        {
            _collapsed = !_collapsed;
            if(!_collapsed)
                Unroll();
            else
            {
                Collapse();
            }
        }

        private void Unroll()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                child.gameObject.SetActive(true);
            }

            if (_rectTransform == null)
            {
                _rectTransform = GetComponent<RectTransform>();
            }

            Vector2 pos = _rectTransform.anchoredPosition;
            pos.y -= _prevSize.y / 2 - _headRectTransform.rect.height / 2;

            _headRectTransform.anchorMin = _headAnchorMin;

            _rectTransform.sizeDelta = _prevSize;
            _rectTransform.anchoredPosition = pos;
        }

        private void Collapse()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                if(child.gameObject.name != "Head")
                    child.gameObject.SetActive(false);
            }

            if (_rectTransform == null)
            {
                _rectTransform = GetComponent<RectTransform>();
            }

            _prevSize = _rectTransform.sizeDelta;

            Vector2 size = _headRectTransform.rect.size;
            Vector2 pos = _rectTransform.anchoredPosition;
            pos.y += _rectTransform.rect.height / 2 - _headRectTransform.rect.height / 2;

            _headRectTransform.anchorMin = Vector2.zero;
            _rectTransform.sizeDelta = size;
            _rectTransform.anchoredPosition = pos;
        }

        public bool Collapsed
        {
            get { return _collapsed; }
            set
            {
                bool old = _collapsed;
                _collapsed = value;
                if(_collapsed)
                    Collapse();
                else if (old)
                    Unroll();
            }
        }

        public bool ResizeableByUser
        {
            get { return _resizeableByUser; }
            set { _resizeableByUser = value; }
        }

        public bool ReplaceableByUser
        {
            get {return _replaceable; }
            set { _replaceable = value; }
        }

        public void OnDragStart(IPointerDataProvider pointerDataProvider)
        {
            _dragCursorPrevPos = pointerDataProvider.MouseScreenPosition;
        }

        public void OnDragContinue(IPointerDataProvider pointerDataProvider)
        {
            Vector2 delta = pointerDataProvider.MouseScreenPosition - _dragCursorPrevPos;
            _dragCursorPrevPos = pointerDataProvider.MouseScreenPosition;
            _rectTransform.anchoredPosition += delta;
        }

        public void OnDragEnd(IDropReceiver receiver, IPointerDataProvider pointerDataProvider)
        {
            
        }

        protected void Close()
        {
            Destroy(this.gameObject);
        }
    }
}

using Assets.Scripts;
using Assets.Scripts.Ui;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace Assets._MapEditor.Scripts
{

    [RequireComponent(typeof(Image))]
    public class Form : UiElement, IDragable
    {
        private CustomInputModule _inputModule;
        private IPointerDataProvider _pointerDataProvider;
        private Vector2 _dragCursorPrevPos;
        private RectTransform _rectTransform;

        private void Start()
        {
            _inputModule = (CustomInputModule)EventSystem.current.currentInputModule;
            _rectTransform = GetComponent<RectTransform>();
        }

        protected override void Update()
        {
            base.Update();
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
    }
}

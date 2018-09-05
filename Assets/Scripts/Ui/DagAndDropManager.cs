using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Controllers;
using Assets.Scripts.Objects;
using UnityEngine;

namespace Assets.Scripts.Ui
{
    class DagAndDropManager : MonoBehaviour
    {
        [SerializeField]
        private float _dragMouseDelta;
        private IDragable _currentDragable;
        private IPointerDataProvider _pointerDataProvider;

        private Vector2 _prevMousePosition;
        

        private void Start()
        {
            FindPointerDataProvider();
        }

        private void FindPointerDataProvider()
        {
            MonoBehaviour[] controllers = FindObjectsOfType<MonoBehaviour>();

            foreach (var controller in controllers)
            {
                IPointerDataProvider provider = controller as IPointerDataProvider;
                if (provider != null)
                    _pointerDataProvider = provider;
            }
        }

        private void FixedUpdate()
        {
            Vector2 mouseDelta = _pointerDataProvider.MouseScreenPosition - _prevMousePosition;
            

            if (_pointerDataProvider.CurrentLmbState == true && _pointerDataProvider.PrevLmbState == false && mouseDelta.magnitude > _dragMouseDelta)
            {
                OnMouseHoldStart();
            }

            if (_pointerDataProvider.CurrentLmbState && _pointerDataProvider.PrevLmbState)
            {
                OnMouseHoldContinue();
            }

            if (_pointerDataProvider.CurrentLmbState == false && _pointerDataProvider.PrevLmbState)
            {
                OnMouseHoldFinish();
            }

            _prevMousePosition = _pointerDataProvider.MouseScreenPosition;
        }

        private void OnMouseHoldStart()
        {
            Debug.Log("MouseHolding started");
            UiElement underCursor = _pointerDataProvider.UnderCursorUiElement;
            IDragable dragable = underCursor as IDragable;
            if (dragable != null)
            {
                dragable.OnDragStart(_pointerDataProvider);
                _currentDragable = dragable;
            }
        }

        private void OnMouseHoldContinue()
        {
            _currentDragable?.OnDragContinue(_pointerDataProvider);
        }

        private void OnMouseHoldFinish()
        {
            UiElement underCursorUi = _pointerDataProvider.UnderCursorUiElement;
            TileObject underCursorTileObject = _pointerDataProvider.UnderCursorTileObject;

            IDropReceiver receiver = underCursorUi as IDropReceiver;
            if(receiver == null)
                receiver = underCursorTileObject as IDropReceiver;

            if (receiver != null)
                _currentDragable?.OnDragEnd(receiver, _pointerDataProvider);
        }
    }
}

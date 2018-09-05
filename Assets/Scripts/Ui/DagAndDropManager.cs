using System;
using System.Collections;
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
        [SerializeField] private float _minMouseOffset;
        private IDragable _currentDragable;
        private IPointerDataProvider _pointerDataProvider;

        private bool mousePressed;

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

        void Update()
        {
            mousePressed = _pointerDataProvider.CurrentLmbState;
            bool prevMousePressed = _pointerDataProvider.PrevLmbState;

            if (!prevMousePressed && mousePressed)
            {
                StartCoroutine(WaitForMouse(Input.mousePosition));
            }

            if (!mousePressed && _currentDragable != null)
                EndDrag();

            if (mousePressed && _currentDragable != null && prevMousePressed)
            {
                ContinueDrag();
            }
        }

        IEnumerator WaitForMouse(Vector2 curMousePos)
        {
            while (mousePressed)
            {
                if (Vector2.Distance(curMousePos, Input.mousePosition) >= _minMouseOffset)
                {
                    StartDrag();
                    break;
                }

                yield return new WaitForEndOfFrame();
            }
        }

        void StartDrag()
        {
            _currentDragable = FindDragable();
            _currentDragable?.OnDragStart(_pointerDataProvider);
        }

        void EndDrag()
        {
            if(_currentDragable != null)
                _currentDragable.OnDragEnd(FindDropReceiver(), _pointerDataProvider);
            _currentDragable = null;
        }

        void ContinueDrag()
        {
            _currentDragable.OnDragContinue(_pointerDataProvider);
        }

        IDropReceiver FindDropReceiver()
        {
            UiElement ui = _pointerDataProvider.UnderCursorUiElement;
            IDropReceiver result = ui as IDropReceiver;
            if (result != null)
                return result;

            TileObject to = _pointerDataProvider.UnderCursorTileObject;
            result = to as IDropReceiver;

            return result;
        }

        IDragable FindDragable()
        {
            UiElement ui = _pointerDataProvider.UnderCursorUiElement;
            IDragable result = ui as IDragable;
            if (result != null)
                return result;

            TileObject to = _pointerDataProvider.UnderCursorTileObject;
            result = to as IDragable;

            return result;
        }
    }
}

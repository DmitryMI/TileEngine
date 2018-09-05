using System.Collections.Generic;
using Assets.Scripts;
using Assets.Scripts.Objects;
using Assets.Scripts.Ui;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets._MapEditor.Scripts
{
    public class EditorController : MonoBehaviour
    {
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private ElementButton _buttonPrefab;

        private MapManager _mapManager;
        private List<PrefabData> _loadedPrefabs;

        private TileObject _objectUnderCursor;
        private UiElement _uiElementUnderCursor;
        private Vector2 _mouseWorldPosition;
        private Vector2Int _mouseCell;

        private bool _prevLmbPressed;
        private Grid _grid;

        void Start()
        {
            _mapManager = new MapManager();


        }

        private void FixedUpdate()
        {
            UpdateMouseWorldPosition();
            UpdateObjectUnderCursor();
            UpdateMouseCell();
            UpdateUiElementUnderCursor();
        }

        private void LateUpdate()
        {
            ProcessMouseClick();
        }

        public void UpdateMouseWorldPosition()
        {
            Vector2 mousePos = Input.mousePosition;
            _mouseWorldPosition = Camera.main.ScreenToWorldPoint(mousePos);
        }

        private void UpdateMouseCell()
        {
            Vector2 mousePos = _mouseWorldPosition;// - new Vector2(_grid.cellSize.x, _grid.cellSize.y);
            Vector2 cellSize = _grid.cellSize;

            float cellRawX = (mousePos.x / cellSize.x);
            float cellRawY = (mousePos.y / cellSize.y);

            int cellX = (int)cellRawX;
            int cellY = (int)cellRawY;

            float remainderX = cellRawX - cellX;
            float remainderY = cellRawY - cellY;

            if (remainderX > cellSize.x / 2)
            {
                cellX += 1;
            }

            if (remainderY > cellSize.y / 2)
            {
                cellY += 1;
            }

            Debug.DrawRay(Vector2.zero, mousePos, Color.blue);
            Debug.DrawRay(Vector2.zero, _grid.CellToWorld(new Vector3Int(cellX, cellY, 0)), Color.red);

            _mouseCell = new Vector2Int(cellX, cellY);
        }

        private void UpdateUiElementUnderCursor()
        {
            PointerEventData pointerEventData = ((CustomInputModule)EventSystem.current.currentInputModule).GetPointerData();

            GameObject obj = pointerEventData.pointerCurrentRaycast.gameObject;

            if (obj != null)
            {
                UiElement ui = obj.GetComponent<UiElement>();
                _uiElementUnderCursor = ui;
            }
            else
            {
                _uiElementUnderCursor = null;
            }

        }

        private void UpdateObjectUnderCursor()
        {
            Vector2 mousePos = _mouseWorldPosition;

            RaycastHit2D[] hits = Physics2D.RaycastAll(mousePos, Vector2.zero, 0);

            TileObject result = null;
            int maxSortingLayer = 0;
            int maxSortingOrder = 0;

            foreach (RaycastHit2D hit in hits)
            {
                GameObject go = hit.transform.gameObject;
                TileObject to = go.GetComponent<TileObject>();

                SpriteRenderer spriteRenderer = go.GetComponent<SpriteRenderer>();

                if (spriteRenderer == null)
                    continue;

                int layerValue = SortingLayer.GetLayerValueFromID(spriteRenderer.sortingLayerID);

                if (layerValue > maxSortingLayer || result == null)
                {
                    result = to;
                    maxSortingLayer = layerValue;
                    maxSortingOrder = 0;
                }
                else if (spriteRenderer.sortingOrder > maxSortingOrder)
                {
                    maxSortingOrder = spriteRenderer.sortingOrder;
                    result = to;
                }

            }

            _objectUnderCursor = result;
        }

        public TileObject UnderCursorTileObject
        {
            get { return _objectUnderCursor; }
        }

        public UiElement UnderCursorUiElement
        {
            get { return _uiElementUnderCursor; }
        }

        public Vector2Int UnderCursorCell
        {
            get { return _mouseCell; }
        }

        public Vector2 MouseWorldPosition
        {
            get { return _mouseWorldPosition; }
        }

        private void ProcessMouseClick()
        {
            bool currentMouseState = Input.GetMouseButton(0);

            if (!_prevLmbPressed && currentMouseState)
            {
                DoUiElementClick();
                DoObjectClick();
            }

            _prevLmbPressed = currentMouseState;
        }

        private void DoUiElementClick()
        {
            if(_uiElementUnderCursor)
                _uiElementUnderCursor.Click();
        }

        private void DoObjectClick()
        {
            if (_objectUnderCursor)
            {
                // TODO Object click
            }
        }

    }
}


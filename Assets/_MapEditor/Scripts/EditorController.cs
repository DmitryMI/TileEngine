using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using Assets.Scripts.Controllers;
using Assets.Scripts.Objects;
using Assets.Scripts.Ui;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets._MapEditor.Scripts
{
    public class EditorController : MonoBehaviour, IPointerDataProvider, IServerDataProvider
    {
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private ElementButton _buttonPrefab;
        [SerializeField] private Vector2Int _mapSize;
        [SerializeField] private float _maxClickTime;
        [SerializeField] private float _maxClickDelta;

        [SerializeField] private Form _elementsForm;

        private MapManager _mapManager;
        private List<PrefabData> _loadedPrefabs;

        private TileObject _objectUnderCursor;
        private UiElement _uiElementUnderCursor;
        private Vector2 _mouseWorldPosition;
        private Vector2 _mousePrevScreenPosition;
        private Vector2Int _mouseCell;

        private bool _prevLmbPressed;
        private int _pressFrame;
        private Grid _grid;

        void Start()
        {
            _mapManager = new MapManager();
            _grid = FindObjectOfType<Grid>();

            Controller[] controllers = FindObjectsOfType<Controller>();
            foreach (Controller controller in controllers)
            {
                controller.RegistrateDataProvider(this);
                controller.OnGameLoaded(this);
            }

            _elementsForm.Collapsed = false;
        }

        private void FixedUpdate()
        {
            UpdateMouseWorldPosition();
            UpdateObjectUnderCursor();
            UpdateMouseCell();
            UpdateUiElementUnderCursor();
            UpdateMouseScreen();
        }

        private void LateUpdate()
        {
            ProcessMouseState();
        }

        public void UpdateMouseWorldPosition()
        {
            Vector2 mousePos = Input.mousePosition;
            _mouseWorldPosition = Camera.main.ScreenToWorldPoint(mousePos);
        }

        public void UpdateMouseScreen()
        {
            MouseScreenDelta = MouseScreenPosition - _mousePrevScreenPosition;
            _mousePrevScreenPosition = MouseScreenPosition;
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

            _mouseCell = new Vector2Int(cellX, cellY);
        }

        private void UpdateUiElementUnderCursor()
        {
            PointerEventData pointerEventData = FindObjectOfType<CustomInputModule>().GetPointerData();

            if(pointerEventData == null)
                return;

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

        public Vector2 MouseScreenPosition
        {
            get { return Input.mousePosition; }
        }

        public Vector2 MouseScreenDelta { get; private set; }

        public bool PrevLmbState { get { return _prevLmbPressed; } }
        public bool CurrentLmbState { get { return Input.GetAxis("Fire1") > 0; } }

        private void ProcessMouseState()
        {

            if (CurrentLmbState && !PrevLmbState)
            {
                StartCoroutine(WaitForMouse(MouseScreenPosition));
            }

            _prevLmbPressed = CurrentLmbState;
        }

        IEnumerator WaitForMouse(Vector2 curMousePos)
        {
            bool noClick = false;
            while (CurrentLmbState)
            {
                if (Vector2.Distance(curMousePos, Input.mousePosition) >= _maxClickDelta)
                {
                    noClick = true;
                    break;
                }

                yield return new WaitForEndOfFrame();
            }

            if (!noClick)
            {
                DoObjectClick();
                DoUiElementClick();
            }
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

        public Vector2Int MapSize { get { return _mapSize; } }
        public void RequestLoadingFinished()
        {
            //throw new System.NotImplementedException();
        }

        public bool Ready { get { return true; } }

        public bool IsCellInBounds(Vector2Int cell)
        {
            return IsCellInBounds(cell.x, cell.y);
        }

        public bool IsCellInBounds(int x, int y)
        {
            return x >= 0 && y >= 0 && x < _mapSize.x && y < _mapSize.y;
        }
    }
}


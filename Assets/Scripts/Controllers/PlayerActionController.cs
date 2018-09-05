using System;
using System.Collections.Generic;
using Assets.Scripts.Objects;
using Assets.Scripts.Objects.Item;
using Assets.Scripts.Objects.Mob;
using Assets.Scripts.Ui;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts.Controllers
{
    public class PlayerActionController : Controller
    {
        [SerializeField] private Text _descriptiveText;

        private VisionController _visionController;
        private Grid _grid;

        private TileObject _objectUnderCursor;
        private UiElement _uiElementUnderCursor;
        private Vector2 _mouseWorldPosition;
        private Vector2Int _mouseCell;
        private Player _localPlayer;

        [SerializeField]
        private SlotEnum _activeHand;

        private bool _prevLmbPressed;

        private static PlayerActionController _currentController;

        public static PlayerActionController Current
        {
            get { return _currentController; }
        }

        public override void OnGameLoaded(ServerController controller)
        {
            WasLoaded = true;

            _visionController = FindObjectOfType<VisionController>();
            _grid = FindObjectOfType<Grid>();
            _currentController = this;

            FindLocalPlayer();
        }

        public void UpdateMouseWorldPosition()
        {
            Vector2 mousePos = Input.mousePosition;
            _mouseWorldPosition =  Camera.main.ScreenToWorldPoint(mousePos);
        }

        private void UpdateMouseCell()
        {
            Vector2 mousePos = _mouseWorldPosition;// - new Vector2(_grid.cellSize.x, _grid.cellSize.y);
            Vector2 cellSize = _grid.cellSize;

            float cellRawX = (mousePos.x / cellSize.x);
            float cellRawY = (mousePos.y / cellSize.y);

            int cellX = (int) cellRawX;
            int cellY = (int) cellRawY;

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

            _mouseCell =  new Vector2Int(cellX, cellY);
        }

        private void FindLocalPlayer()
        {
            Player[] players = FindObjectsOfType<Player>();

            foreach (var p in players)
            {
                if (p.isLocalPlayer)
                    _localPlayer = p;
            }
        }

        private void UpdateUiElementUnderCursor()
        {
            PointerEventData pointerEventData = ((CustomInputModule) EventSystem.current.currentInputModule).GetPointerData();

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
            Vector2 mousePos =_mouseWorldPosition;

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

        public Player LocalPlayer
        {
            get { return _localPlayer; }
        }

        public bool CheckVisibilityUnderCursor()
        {
            Vector2Int cell = _mouseCell;
            VisionMask visionMask = _visionController.GetMask(cell.x, cell.y);
            if (visionMask == null)
                return false;
            return visionMask.IsVisible();
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

        public SlotEnum ActiveHand
        {
            get { return _activeHand; }
            set { _activeHand = value; }
        }

        private void PrintDescriptiveText()
        {
            if(_descriptiveText == null)
                return;

            if (_objectUnderCursor == null)
                _descriptiveText.text = "";

            if (CheckVisibilityUnderCursor())
            {
                if (_objectUnderCursor == null)
                {
                    _descriptiveText.text = "Nothing here??";
                }
                else
                {
                    string descName = _objectUnderCursor.DescriptiveName;
                    _descriptiveText.text = descName;
                }
            }
            else
            {
                _descriptiveText.text = "This area is hidden in the dark";
            }
        }

        private void Start()
        {
            if(_descriptiveText == null)
                Debug.LogWarning("Descriptive text is not attached!");
        }

        private void FixedUpdate()
        {
            if (WasLoaded)
            {
                UpdateMouseWorldPosition();
                UpdateObjectUnderCursor();
                UpdateMouseCell();
                UpdateUiElementUnderCursor();
            }
        }

        private void LateUpdate()
        {
            if (WasLoaded)
            {
                PrintDescriptiveText();
                ProcessMouseClick();
            }
        }

        private void ProcessMouseClick()
        {
            bool currentMouseState = Input.GetMouseButton(0);
            if (!_prevLmbPressed && currentMouseState)
            {

                if (_localPlayer == null)
                    FindLocalPlayer();

                if (_localPlayer == null)
                {
                    Debug.LogError("Local player was not found!");
                    return;
                }

                ClickOnTileObject();
                ClickOnUiElement();
            }

            _prevLmbPressed = currentMouseState;
        }

        private void ClickOnTileObject()
        {
            if (CheckVisibilityUnderCursor())
            {
                TileObject to = _objectUnderCursor;

                var item = to as IPlayerInteractable;
                if (item != null && to.IsNeighbour(_localPlayer))
                {
                    ((IPlayerInteractable) to).ApplyItemClient(_localPlayer.GetItemBySlot(ActiveHand));
                }
            }
        }

        private void ClickOnUiElement()
        {
            if(_uiElementUnderCursor != null)
                _uiElementUnderCursor.Click();
        }
        
    }
}

﻿using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.GameMechanics;
using Assets.Scripts.GameMechanics.Actions;
using Assets.Scripts.Objects;
using Assets.Scripts.Objects.Item;
using Assets.Scripts.Objects.Mob;
using Assets.Scripts.Objects.Mob.Humanoids;
using Assets.Scripts.Ui;
using Assets.Scripts._Legacy;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts.Controllers
{
    public class PlayerActionController : Controller, IPointerDataProvider, IExclusiveActionManager
    {
        [SerializeField] private Text _descriptiveText;
        [SerializeField] private float _maxClickDelta = 0.1f;

        private VisionController _visionController;
        private Grid _grid;

        private TileObject _objectUnderCursor;
        private UiElement _uiElementUnderCursor;
        private Vector2 _mouseWorldPosition;
        private Vector2Int _mouseCell;
        private Vector2 _mouseCellOffset = new Vector2();
        private Mob _localPlayerMob;
        private Vector2 _mousePrevScreenPosition;
        private ExclusiveActionManager _actionManager = new ExclusiveActionManager();

        [SerializeField]
        private SlotEnum _activeHand;

        [SerializeField] private Intent _intent = Intent.Help;
        [SerializeField] private ImpactLimb _impactTarget = ImpactLimb.Chest;

        private bool _prevLmbPressed;

        private RaycastHit2D[] _raycastBuffer = new RaycastHit2D[32];

        private static PlayerActionController _currentController;

        private Func<bool> _abortConditionChecker;

        public static PlayerActionController Current
        {
            get { return _currentController; }
        }


        public override void OnGameLoaded(IServerDataProvider controller)
        {
            WasLoaded = true;

            _visionController = FindObjectOfType<VisionController>();

            if (_visionController == null)
            {
                Debug.LogError("VisionController not found!");
                Debug.Break();
            }

            _grid = FindObjectOfType<Grid>();
            _currentController = this;

            /*FindLocalPlayer();

            _visionController.SetViewerPosition(_localPlayerMob);*/
        }

        public void SetLocalPlayer(Mob mob)
        {
            _localPlayerMob = mob;

            _visionController.SetViewerPosition(mob);
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

            // TODO Check if is correct
            _mouseCellOffset.x = _mouseWorldPosition.x - (cellX * _grid.cellSize.x);
            _mouseCellOffset.y = _mouseWorldPosition.y - (cellY * _grid.cellSize.y);
        }

        private void FindLocalPlayer()
        {
            Mob[] players = FindObjectsOfType<Mob>();

            foreach (var p in players)
            {
                if (p.isLocalPlayer)
                    _localPlayerMob = p;
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

            //RaycastHit2D[] hits = Physics2D.RaycastAll(mousePos, Vector2.zero, 0);
            
            int count = Physics2D.RaycastNonAlloc(mousePos, Vector2.zero, _raycastBuffer, 0, -1);

            
            TileObject result = null;
            int maxSortingLayer = 0;
            int maxSortingOrder = 0;

            //foreach (RaycastHit2D hit in hits)
            for(int i = 0; i < count; i++)
            {
                RaycastHit2D hit = _raycastBuffer[i];
                
                GameObject go = hit.collider.gameObject;

                //Debug.Log("Raycast hit: " + go.name);

                TileObject to = go.GetComponent<TileObject>();

                if (to == null)
                {
                    IChildCollider child = go.GetComponent<IChildCollider>();
                    GameObject parentGo = child?.Parent;
                    to = parentGo?.GetComponent<TileObject>();
                }

                SpriteRenderer spriteRenderer = to?.Renderer;
                if (spriteRenderer == null)
                    spriteRenderer = go.GetComponent<SpriteRenderer>();

                if (spriteRenderer == null)
                    continue;

                int layerValue = SortingLayer.GetLayerValueFromID(spriteRenderer.sortingLayerID);

                if (layerValue > maxSortingLayer || result == null)
                {
                    result = to;
                    maxSortingLayer = layerValue;
                    maxSortingOrder = 0;
                }
                else if (layerValue == maxSortingLayer && spriteRenderer.sortingOrder > maxSortingOrder)
                {
                    maxSortingOrder = spriteRenderer.sortingOrder;
                    result = to;
                }

            }

            _objectUnderCursor = result;
        }

        public Mob LocalPlayerMob
        {
            get { return _localPlayerMob; }
        }

        public bool CheckVisibilityUnderCursor()
        {
            /*Vector2Int cell = _mouseCell;
            VisionMask visionMask = _visionController.GetMask(cell.x, cell.y);
            if (visionMask == null)
                return true;

            return visionMask.IsVisible();*/
            return _visionController.IsCellVisible(_mouseCell.x, _mouseCell.y);
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

        public bool PrevLmbState { get { return _prevLmbPressed; } }
        public bool CurrentLmbState { get { return Input.GetAxis("Fire1") > 0; } }

        public SlotEnum ActiveHand
        {
            get { return _activeHand; }
            set { _activeHand = value; }
        }

        public Item ActiveHandItem => (LocalPlayerMob as Humanoid)?.GetItemBySlot(ActiveHand);

        public Intent Intent
        {
            get => _intent;
            set => _intent = value;
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
            _intent = Intent.Help;
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

                ProcessPlayerControl();
            }

        }

        private void LateUpdate()
        {
            if (WasLoaded)
            {
                PrintDescriptiveText();
                ProcessMouseState();
            }
        }

        private void ProcessMouseState()
        {
            if (_localPlayerMob == null)
                FindLocalPlayer();

            if (_localPlayerMob == null)
            {
                //Debug.LogError("Local player was not found!");
                return;
            }

            if(CurrentLmbState && !PrevLmbState)
                StartCoroutine(WaitForMouse(MouseScreenPosition));

            _prevLmbPressed = CurrentLmbState;
        }

        private void ProcessPlayerControl()
        {
            float vertical = Input.GetAxisRaw("Vertical");
            float horizontal = Input.GetAxisRaw("Horizontal");

            if (vertical > 0)
            {
                _localPlayerMob.DoMove(Direction.Forward);
            }
            if (vertical < 0)
            {
                _localPlayerMob.DoMove(Direction.Backward);
            }
            if (horizontal > 0)
            {
                _localPlayerMob.DoMove(Direction.Right);
            }
            if (horizontal < 0)
            {
                _localPlayerMob.DoMove(Direction.Left);
            }
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
                ClickOnTileObject();
                ClickOnUiElement();
            }
        }

        private void ClickOnTileObject()
        {
            Direction wantRotation = Utils.GetDirection(LocalPlayerMob.X, LocalPlayerMob.Y, _mouseCell.x, _mouseCell.y);
            LocalPlayerMob.SetRotation(wantRotation);

            TileObject to = _objectUnderCursor;
            if(to == null)
                return;
            

            Humanoid playerHumanoid = _localPlayerMob as Humanoid;
            if (playerHumanoid != null)
            {
                Item activeItem = ActiveHandItem;

                bool neighbour = to.IsNeighbour(_localPlayerMob);
                if (neighbour)
                {
                    if (CheckVisibilityUnderCursor())
                    {
                        if (to is IPlayerApplicable applicable)
                        {
                            applicable.ApplyItemClient(activeItem, _intent);
                        }
                        else if (to is IPlayerImpactable impactable)
                        {
                            impactable.ImpactItemClient(playerHumanoid, activeItem, _intent, _impactTarget);
                        }
                    }
                }
                else
                {
                    activeItem.ItemTargetPointClient(_mouseCell, _mouseCellOffset);
                }
            }
            else
            {
                if(_objectUnderCursor == null)
                    _localPlayerMob.DoPointAction(_mouseCell.x, _mouseCell.y);
                else
                    _localPlayerMob.DoTargetAction(_objectUnderCursor);
            }
        }

        private void ClickOnUiElement()
        {
            if(_uiElementUnderCursor != null)
                _uiElementUnderCursor.Click();
        }

        public IDelayedAction StartAction(Action<object> action, Func<bool> abortConditionChecker, Action<object> abortHandler, object args,
            object abortHandlerArgs, float delay)
        {
            // TODO Draw progress bar
            _abortConditionChecker = abortConditionChecker;
            return _actionManager.StartAction(action, AbortConditionChecker, abortHandler, args, abortHandlerArgs, delay);
        }

        public void AbortAction()
        {
            // TODO Remove progress bar
            _actionManager.AbortAction();
        }

        private bool AbortConditionChecker()
        {
            // TODO Update progress bar
            return _abortConditionChecker();
        }

        public bool IsActionPending => _actionManager.IsActionPending;
    }
}

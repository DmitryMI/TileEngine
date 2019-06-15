﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Assets.Scripts.Controllers;
using Assets.Scripts.Controllers.Atmos;
using Assets.Scripts.GameMechanics;
using Assets.Scripts.Objects.Equipment.Power;
using Assets.Scripts._Legacy;
using UnityEngine;
using UnityEngine.Networking;
using Debug = UnityEngine.Debug;
using VisionController = Assets.Scripts.Controllers.VisionController;

namespace Assets.Scripts.Objects
{
    public abstract class TileObject : NetworkBehaviour
    {
        [SerializeField] protected int TypeId;

        [SerializeField] protected float CellOffsetComparisonTolerance = 0.001f;

        [SerializeField]
        private Vector2Int _cell;

        [SerializeField]
        private Vector2 _cellOffset;

        [SerializeField] protected Direction Rotation;

        [SerializeField] protected int LayersNeededCount = 1;

        public virtual int LayersNeeded => LayersNeededCount;

        protected WalkController WalkController;
        protected VisionController VisionController;
        protected ServerController ServerController;
        protected AtmosController AtmosController;
        protected TileController TileController;
        protected ICellPositionProvider PositionProvider;
        protected AudioSource AudioSource;
        protected SpriteRenderer SpriteRenderer;

        public event Action OnCellChanged;

        [SyncVar]
        [SerializeField]
        protected int SortingStartIndex;

        private int _prevSortingStartIndex;
        private bool _cellChanged;

        protected virtual void OnSortingOrderChange()
        {
            if(SpriteRenderer == null)
                return;

            //if(Renderer.sortingOrder != SortingStartIndex)
                Renderer.sortingOrder = SortingStartIndex;
        }

        public WalkController GetWalkController()
        {
            EnsureControllers();
            return WalkController;
        }

        public VisionController GetVisionController()
        {
            EnsureControllers();
            return VisionController;
        }

        public ServerController GetServerController()
        {
            EnsureControllers();
            return ServerController;
        }

        public SpriteRenderer Renderer => SpriteRenderer;

        public AtmosController GetAtmosController()
        {
            EnsureControllers();
            return AtmosController;
        }

        public TileController GetTileController()
        {
            EnsureControllers();
            return TileController;
        }

        /// <summary>
        /// True - object does not block light and vision
        /// </summary>
        protected abstract bool Transparent { get; }

        /// <summary>
        /// True - object does not block mobs' movement
        /// </summary>
        protected abstract bool CanWalkThrough { get; }

        /// <summary>
        /// True - object lets gases pass through
        /// </summary>
        protected abstract bool PassesGas { get; }

        /// <summary>
        /// This name can be changed under some circumstances (e.g. renaming with labeler, person's proper name).
        /// </summary>
        public abstract string DescriptiveName { get; }

        protected Grid Grid;

        private bool _transformChanged = true;

        protected virtual void Start()
        {
            AudioSource = GetComponent<AudioSource>();
            SpriteRenderer = GetComponent<SpriteRenderer>();

            Grid = FindObjectOfType<Grid>();
            EnsureControllers();


            TileController = FindObjectOfType<TileController>();
            StartCoroutine(WaitForServerController());

            PositionProvider = new CellPositionProvider(this);

            if (LayersNeededCount == 0)
            {
                LayersNeededCount = 1;
                Debug.LogWarning(name + "LayersNeededCount must be greater than zero.");
            }
        }

        IEnumerator WaitForServerController()
        {
            while(!ServerController.Ready)
                yield return new WaitForEndOfFrame();

            TileController = FindObjectOfType<TileController>();
            TileController.AddObject(Cell.x, Cell.y, this);

            _transformChanged = true;
        }
        
        protected virtual void Update()
        {
            Sync();

            UpdateTransfrom();
            UpdateControllers();

            if (_prevSortingStartIndex != SortingStartIndex)
            {
                //Debug.Log("Sorting order changed: " + name);
                OnSortingOrderChange();
            }

            _prevSortingStartIndex = SortingStartIndex;
        }

        protected virtual void LateUpdate()
        {
            _cellChanged = false;
        }

        protected virtual void Sync()
        {
            if (isLocalPlayer || (!isLocalPlayer && isServer))
            {
                if (_transformChanged)
                {
                    CmdSendTransformData(_cell.x, _cell.y, _cellOffset, Rotation);
                    _transformChanged = false;
                }
            }
        }

        public virtual void ForceSync()
        {
            _transformChanged = true;
            Sync();
        }

        [Command]
        private void CmdSendTransformData(int x, int y, Vector2 offset, Direction direction)
        {
            _cell = new Vector2Int(x, y);
            _cellOffset = offset;
            Rotation = direction;

            RpcReceiveTransformData(x, y, offset, direction);
        }

        [ClientRpc]
        private void RpcReceiveTransformData(int x, int y, Vector2 offset, Direction direction)
        {
            if (!isLocalPlayer)
            {
                if (!isServer)
                {
                    _cell = new Vector2Int(x, y);
                    _cellOffset = offset;
                    Rotation = direction;
                }
            }
        }

        [ClientRpc]
        public void RpcForceTransformation(int x, int y, Vector2 offset)
        {
            _cell = new Vector2Int(x, y);
            _cellOffset = offset;
        }

        private void UpdateControllers()
        {
            if (!CanWalkThrough)
            {
                WalkController.SetBlock(_cell.x, _cell.y);
            }
            if (!Transparent)
            {
                VisionController.SetBlock(_cell.x, _cell.y);
            }
            if (!PassesGas)
            {
                if(isServer)
                    AtmosController.SetBlock(_cell.x, _cell.y);
            }
        }

        protected bool EnsureControllers()
        {
            if(VisionController == null)
                VisionController = FindObjectOfType<VisionController>();
            if(ServerController == null)
                ServerController = FindObjectOfType<ServerController>();
            if(WalkController == null)
                WalkController = FindObjectOfType<WalkController>();
            if(AtmosController == null)
                AtmosController = FindObjectOfType<AtmosController>();

            if (!VisionController.IsReady)
                return false;
            if (!WalkController.IsReady)
                return false;

            if (isServer)
            {
                if (!AtmosController.IsReady)
                    return false;
            }

            return true;
        }

        private void UpdateTransfrom()
        {
            Vector3 position = Grid.CellToWorld(new Vector3Int(_cell.x, _cell.y, 0)) + new Vector3(_cellOffset.x, _cellOffset.y);
            position.z = 0;
            if (Vector3.SqrMagnitude(transform.position - position) > 0.25f)
            {
                transform.position = position;
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, position, 0.5f);
            }
        }

        public Vector2Int Cell
        {
            get { return _cell; }
            set
            {
                Vector2Int oldCell = _cell;
                _cell = value;
                
                if (!oldCell.Equals(_cell))
                {
                    _cellChanged = true;
                    _transformChanged = true;

                    if (isServer)
                    {
                        TileController.Current.RemoveObject(oldCell.x, oldCell.y, this);
                        TileController.Current.AddObject(_cell.x, _cell.y, this);
                    }

                    OnCellChanged?.Invoke();
                }
            }
        }

        public Vector2 CellOffset
        {
            get { return _cellOffset; }
            set
            {
                float delta = (value - _cellOffset).magnitude;

                if (delta > CellOffsetComparisonTolerance)
                {
                    _cellOffset = value;
                    _transformChanged = true;

                    if (GetType() == typeof(LocalPowerController) && isServer)
                    {
                        Debug.Log("NEW VALUE: " + _cellOffset);
                    }
                }
            }
        }

        public virtual void SetRotation(Direction rotation)
        {
            Rotation = rotation;
            _transformChanged = true;
        }

        public int SortingOrder
        {
            get
            {
                if (SpriteRenderer != null)
                    return SpriteRenderer.sortingOrder;
                else
                    return 0;
            }
        }

        public int Id
        {
            get { return TypeId; }
        }

        public int SortingStart
        {
            get { return SortingStartIndex; }
            set { SortingStartIndex = value; }
        }

        public virtual string ToMap()
        {
            string mapData = "";
            mapData += Cell.x + " " + Cell.y + " " + CellOffset.x + " " + CellOffset.y;

            return mapData;
        }

        public virtual bool FromMap(string mapData)
        {
            string[] units = mapData.Split(' ');

            bool ok = true;

            try
            {

                _cell.x = Int32.Parse(units[0]);
                _cell.y = Int32.Parse(units[1]);
                _cellOffset.x = (float) Double.Parse(units[2]);
                _cellOffset.y = (float) Double.Parse(units[3]);
            }
            catch (FormatException ex)
            {
                Debug.Log(ex.Message);
                ok = false;
            }

            return ok;
        }

        public virtual bool IsNeighbour(TileObject other)
        {
            Vector2Int cellA = Cell;
            Vector2Int cellB = other.Cell;

            int dx = Mathf.Abs(cellA.x - cellB.x);
            int dy = Mathf.Abs(cellA.y - cellB.y);

            return dx <= 1 && dy <= 1;
        }

        public class CellPositionProvider : ICellPositionProvider
        {
            private readonly TileObject _tileObject;
            public int X => _tileObject.Cell.x;
            public int Y => _tileObject.Cell.x;

            public CellPositionProvider(TileObject obj)
            {
                _tileObject = obj;
            }
        }

        [Server]
        public void PlaySoundOn(AudioClip clip)
        {
            if (AudioSource != null)
            {
                int id = AudioManager.Instance.GetId(clip);
                RpcPlaySoundOn(id);
            }
            else
            {
                Debug.Log("Object does not support sound playing");
            }
        }

        [ClientRpc]
        private void RpcPlaySoundOn(int audioClipId)
        {
            if (AudioSource != null)
            {
                AudioClip clip = AudioManager.Instance.GetById(audioClipId);
                AudioSource.PlayOneShot(clip);
            }
            else
            {
                Debug.Log("Object does not support sound playing");
            }
        }
    }
}

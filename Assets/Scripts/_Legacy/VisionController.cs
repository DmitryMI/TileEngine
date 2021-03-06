﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Assets.Scripts.Controllers;
using Assets.Scripts.GameMechanics;
using UnityEngine;

namespace Assets.Scripts._Legacy
{
    [Obsolete("Do not use legacy controllers. VisionController is slow and does not work correct")]
    public class VisionController : Controller
    {
        class VisionMaskData
        {
            public float Brightness;
            public Color Color;
        }


        [SerializeField] private int _maxSpawnPerFrame = 64;
        [SerializeField] private VisionMask _maskPrefab;
        [SerializeField] private bool _enableVisionMasks = true;
        [SerializeField] private int _sightRange;
        [SerializeField] private bool _visionActive;
        [SerializeField] private Vector2Int _visionSourceCell;

        [SerializeField] private bool _ignoreLight;

        private int[,] _blockersCount;

        private VisionMask[,] _masks;
        private List<LightSourceInfo> _tempLightSourceInfos = new List<LightSourceInfo>();

        private List<VisionMask> _processedMasks = new List<VisionMask>();
        private List<float > _processedBrightness = new List<float>();

        private Camera _camera;
        private Grid _grid;
        private bool _prevEnableVisionMasks = true;
        private GameObject _visionMasksGroup;

        private Task _job;

        private static VisionController _current;
        public static VisionController Current
        {
            get { return _current; }
        }

        public override void OnGameLoaded(IServerDataProvider controller)
        {
            //WasLoaded = true;
            ServerController = controller;

            _current = this;

            _visionMasksGroup = GameObject.Find("VisionMasks");

            if (_visionMasksGroup == null)
                Debug.LogWarning("Grouper for vision masks was not found!");

            _blockersCount = new int[controller.MapSize.x, controller.MapSize.y];
            _masks = new VisionMask[controller.MapSize.x, controller.MapSize.y];

            if (_maskPrefab == null)
            {
                Debug.LogError("No vision mask prefab found!");
                return;
            }

            if (isClient)
            {
                SpawnVisionMasks();
            }
        }

        private void SpawnVisionMasks()
        {
            StartCoroutine(SpawnMasksFpsFriendly(_maxSpawnPerFrame));
        }

        private IEnumerator SpawnMasksFpsFriendly(int perFrameMaximum)
        {
            int count = 0;
            for (int x = 0; x < ServerController.MapSize.x; x++)
            {
                for (int y = 0; y < ServerController.MapSize.y; y++)
                {
                    GameObject mask = Instantiate(_maskPrefab.gameObject);
                    if (_visionMasksGroup != null)
                    {
                        mask.transform.parent = _visionMasksGroup.transform;
                    }
                    _masks[x, y] = mask.GetComponent<VisionMask>();
                    _masks[x, y].SetCell(x, y);
                    _masks[x, y].SetInvisible();

                    count++;

                    if (count >= perFrameMaximum)
                    {
                        count = 0;
                        yield return new WaitForEndOfFrame();
                    }
                }
            }

            WasLoaded = true;
        }

        public bool VisionProcessingEnabled
        {
            get { return _visionActive; }
            set { _visionActive = value; }
        }

        private void Start()
        {
            _camera = Camera.main;
            _grid = FindObjectOfType<Grid>();

            _prevEnableVisionMasks = !_enableVisionMasks;
        }

        private void Update()
        {
            if (!WasLoaded)
            {
                return;
            }

            //_tempLightSourceInfos.Clear();

            CheckVisionMasksEnabled();

            for (int i = 0; i < _blockersCount.GetLength(0); i++)
            for (int j = 0; j < _blockersCount.GetLength(1); j++)
            {
                _blockersCount[i, j] = 0;
                _masks[i, j].SetInvisible();
                _masks[i, j].SetLighting(0, Color.black);
            }
        }

        private void LateUpdate()
        {
            if (_visionActive && WasLoaded)
            {
                ProcessVisionBlockers();

                if (_job == null || _job.IsCompleted)
                {
                    SetLightSourcesAfterJob();

                    _job = new Task(SetLightSourcesJob);
                    _job.Start();
                }
            }
        }

        private void CheckVisionMasksEnabled()
        {
            if (_enableVisionMasks == true && _prevEnableVisionMasks == false)
            {
                SetAllMasksEnabled(true);
                _prevEnableVisionMasks = true;
            }

            if (_enableVisionMasks == false && _prevEnableVisionMasks == true)
            {
                SetAllMasksEnabled(false);
                _prevEnableVisionMasks = false;
            }
        }

        private void SetAllMasksEnabled(bool areEnabled)
        {
            if (!WasLoaded) return;

            for (int x = 0; x < ServerController.MapSize.x; x++)
            {
                for (int y = 0; y < ServerController.MapSize.y; y++)
                {
                    if (areEnabled)
                        _masks[x, y].Enable();
                    else
                        _masks[x, y].Disable();
                }
            }
        }

        public void SetCameraPosition(Vector2Int cell, Vector2 offset)
        {
            float camZ = _camera.transform.position.z;
            _camera.transform.position = _grid.CellToWorld(new Vector3Int(cell.x, cell.y, 0)) +
                                         new Vector3(offset.x, offset.y, camZ);
            _visionSourceCell = cell;
        }

        public void SetCameraPosition(Vector2 planeWorldPosition)
        {
            float camZ = _camera.transform.position.z;
            Vector3 pos = planeWorldPosition;
            pos.z = camZ;
            _camera.transform.position = pos;
            Vector3Int cell3 = _grid.WorldToCell(pos);
            _visionSourceCell = new Vector2Int(cell3.x, cell3.y);
        }

        public Vector2Int VisionSourceCell
        {
            get { return _visionSourceCell; }
            set { _visionSourceCell = value; }
        }

        public void SetBlock(int x, int y)
        {
            if (!WasLoaded)
                return;

            if (!ServerController.IsCellInBounds(new Vector2Int(x, y)))
                return;

            _blockersCount[x, y]++;
        }

        public bool IsTransperent(int x, int y)
        {
            if (ServerController.IsCellInBounds(new Vector2Int(x, y)))
                return false;

            return _blockersCount[x, y] == 0;
        }

        public bool IsVisibleFrom(Vector2Int source, Vector2Int dest)
        {
            int x0 = source.x;
            int y0 = source.y;

            int x1 = dest.x;
            int y1 = dest.y;

            float tmpX = x0;
            float tmpY = y0;
            int dx = (x1 - x0);
            int dy = (y1 - y0);
            int L = Mathf.Max(Mathf.Abs(dx), Mathf.Abs(dy)) + 1;

            float sx = (float) dx / L;
            float sy = (float) dy / L;

            int x = x0, y = y0;

            for (int i = 0; i < L; i++)
            {
                x = (int) Mathf.Round(tmpX);
                y = (int) Mathf.Round(tmpY);

                if (x < 0 || y < 0 || x >= ServerController.MapSize.x || y >= ServerController.MapSize.y)
                    continue;

                if (_blockersCount[x, y] > 0)
                    return false;

                tmpX += sx;
                tmpY += sy;
            }

            return true;
        }

        public VisionMask GetMask(int x, int y)
        {
            if (!WasLoaded)
                return null;

            if (!ServerController.IsCellInBounds(new Vector2Int(x, y)))
                return null;
            return _masks[x, y];
        }

        private void ProcessVisionBlockers()
        {
            // Cells near player are always visible;
            for (int x = _visionSourceCell.x - 1; x <= _visionSourceCell.x + 1; x++)
            {
                for (int y = _visionSourceCell.y - 1; y <= _visionSourceCell.y + 1; y++)
                {
                    VisionMask mask = GetMask(x, y);
                    if (mask != null)
                    {
                        mask.SetVisible();

                        if (_ignoreLight)
                            mask.SetLighting(1, Color.black);
                    }
                }
            }

            for (int x = _visionSourceCell.x - _sightRange; x <= _visionSourceCell.x + _sightRange; x++)
            {
                for (int y = _visionSourceCell.y - _sightRange; y <= _visionSourceCell.y + _sightRange; y++)
                {
                    bool result = IsVisibleFrom(_visionSourceCell, new Vector2Int(x, y));

                    if (!ServerController.IsCellInBounds(new Vector2Int(x, y)))
                        continue;

                    VisionMask mask = _masks[x, y];
                    if (mask != null && result)
                    {
                        mask.SetVisible();

                        if (_ignoreLight)
                            mask.SetLighting(1, Color.black);
                    }

                }
            }
        }

        public void SetLightSource(LightSourceInfo info)
        {
            if (!WasLoaded)
                return;

            if (_job == null || _job.Status != TaskStatus.Running)
            {
                if (!_tempLightSourceInfos.Contains(info))
                {
                    _tempLightSourceInfos.Add(info);
                }
            }
        }

        private void SetLightSourcesJob()
        {
            foreach(var info in _tempLightSourceInfos)
            {
                // Cells near light source are always visible;
                for (int x = info.X - 1; x <= info.X + 1; x++)
                {
                    for (int y = info.Y - 1; y <= info.Y + 1; y++)
                    {
                        VisionMask mask = GetMask(x, y);
                        if (mask != null)
                        {
                            float range = Vector2Int.Distance(new Vector2Int(info.X, info.Y), new Vector2Int(x, y));
                            float intensity = info.CalculateIntensityRange(range);

                            if (intensity < 0)
                                intensity = 0;

                            float brightness = intensity;

                            _processedBrightness.Add(brightness);
                            _processedMasks.Add(mask);
                        }
                    }
                }

                for (int x = (int)Mathf.Round(info.X - info.Range); x <= info.X + info.Range; x++)
                {
                    for (int y = (int)Mathf.Round(info.Y - info.Range); y <= info.Y + info.Range; y++)
                    {
                        bool result = IsVisibleFrom(new Vector2Int(info.X, info.Y), new Vector2Int(x, y));

                        VisionMask mask = GetMask(x, y);
                        if (mask != null && result)
                        {

                            //float brightness = mask.GetBrightness() + info.CalculateIntensity(new Vector2Int(x, y));
                            float range = Vector2Int.Distance(new Vector2Int(info.X, info.Y), new Vector2Int(x, y));
                            float intensity = info.CalculateIntensityRange(range);

                            if (intensity < 0)
                                intensity = 0;

                            float brightness = intensity;

                            _processedBrightness.Add(brightness);
                            _processedMasks.Add(mask);

                            //mask.SetLighting(brightness, mask.GetColor());
                        }
                    }
                }
            }

            _tempLightSourceInfos.Clear();
        }

        private void SetLightSourcesAfterJob()
        {
            for (int i = 0; i < _processedMasks.Count; i++)
            {
                _processedMasks[i].SetLighting(_processedBrightness[i], Color.black);
            }
            _processedMasks.Clear();
            _processedBrightness.Clear();
        }
        
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.GameMechanics;
using Assets.Scripts.Objects.Mob;
using Assets.Scripts._Legacy;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Controllers
{
    class TileMaskVisionController : VisionController
    {
        private Hashtable _permanentLightSources = new Hashtable();
        private List<ILightInfo> _tempLightSources = new List<ILightInfo>();
        private Camera _camera;

        private TileMask[,] _tileMaskMatrix;
        [SerializeField]
        private TileMask _tileMaskPrefab;
        private IServerDataProvider _serverDataProvider;
        private int _prevViewerX = 0;
        private int _prevViewerY = 0;
        private int _prevHeight, _prevWidth;

        public override bool IsCellVisible(int x, int y)
        {
            throw new NotImplementedException();
        }

        public override bool VisionProcessingEnabled { get; }

        [Obsolete("Try to avoid calling this function. Better use 3D-lighting system via LightBlocker spawning")]
        public override void SetBlock(int x, int y)
        {
            return;
        }

        [Obsolete("VisionMask is not present in the game.")]
        public override VisionMask GetMask(int x, int y)
        {
            return null;
        }

        public override void SetLightForOneFrame(ILightInfo info)
        {
            _tempLightSources.Add(info);
        }

        public override int SetLightContinuous(ILightInfo info)
        {
            return AddLightSource(info);
        }

        public override void RemoveLightById(int id)
        {
            RemoveLight(id);
        }

        public override void OnGameLoaded(IServerDataProvider controller)
        {
            _current = this;
            _serverDataProvider = controller;

            CreateTileMasks();

            WasLoaded = true;
        }

        public override void SetViewerPosition(IPositionProvider viewPositionProvider)
        {
            base.SetViewerPosition(viewPositionProvider);

            _prevViewerX = viewPositionProvider.X;
            _prevViewerY = viewPositionProvider.Y;
        }

        protected void LateUpdate()
        {
            if(IsReady)
                RefreshLighting();

            SetCameraView();
        }

        protected void SetCameraView()
        {
            if (ViewerPositionProvider == null)
            {
                Debug.LogWarning("No position provider found!");
                return;
            }

            float x = ViewerPositionProvider.X + ViewerPositionProvider.OffsetX;
            float y = ViewerPositionProvider.Y + ViewerPositionProvider.OffsetY;
            Vector3 position = Camera.main.transform.position;
            position.x = x;
            position.y = y;
            Camera.main.transform.position = position;
        }

        private void RefreshLighting()
        {
            _tempLightSources.Clear();
        }

        private int AddLightSource(ILightInfo info)
        {
            int id;
            do
            {
                id = Random.Range(1, Int32.MaxValue);
            } while (_permanentLightSources.ContainsKey(id));

            _permanentLightSources.Add(id, info);

            return id;
        }

        private void RemoveLight(int id)
        {
            _permanentLightSources.Remove(id);
        }

        private void SetCell(TileMask mask, int x, int y)
        {
            Vector3 position = Grid.CellToWorld(new Vector3Int(x, y, 0));
            mask.transform.position = position;

            mask.X = x;
            mask.Y = y;
        }

        private void CreateTileMasks()
        {
            Grid = FindObjectOfType<Grid>();

            GameObject group = GameObject.Find("TileMasks");
            
            int height = _serverDataProvider.MapSize.y;
            int width = _serverDataProvider.MapSize.x;

            if(_tileMaskMatrix != null)
                for (int i = 0; i < _tileMaskMatrix.GetLength(0); i++)
                {
                    for (int j = 0; j < _tileMaskMatrix.GetLength(1); j++)
                    {
                        Destroy(_tileMaskMatrix[i, j]);
                    }
                }

            _tileMaskMatrix = new TileMask[height,width];

            for (int i = 0; i < _tileMaskMatrix.GetLength(0); i++)
            {
                for (int j = 0; j < _tileMaskMatrix.GetLength(1); j++)
                {
                    GameObject tileMaskGo = Instantiate(_tileMaskPrefab.gameObject, group.transform);
                    TileMask tileMask = tileMaskGo.GetComponent<TileMask>();

                    tileMask.SetColor(Color.black);
                    tileMask.SetAlpha(0.5f);
                    SetCell(tileMask, j, i);
                    _tileMaskMatrix[i, j] = tileMask;
                    
                    tileMask.SetActive(false);
                }
            }
        }

        private void SetActiveMasksOnCamera(int width, int height, bool active)
        {
            int viewerX = ViewerPositionProvider.X;
            int viewerY = ViewerPositionProvider.Y;

            int yStart = viewerY - height;
            int yEnd = viewerY + height;

            if (yStart < 0)
                yStart = 0;
            
            if (yEnd >= _tileMaskMatrix.GetLength(0))
                yEnd = _tileMaskMatrix.GetLength(0) - 1;

            int xStart = viewerX - width;
            int xEnd = viewerX + width;
            if (xStart < 0)
                xStart = 0;
            if (xEnd >= _tileMaskMatrix.GetLength(1))
                xEnd = _tileMaskMatrix.GetLength(1) - 1;

            for (int y = yStart; y <= yEnd; y++)
            {
                for (int x = xStart; x <= xEnd; x++)
                {
                    _tileMaskMatrix[y, x].SetActive(active);
                }
            }
        }

        private void Update()
        {
            if(ViewerPositionProvider == null)
                return;
            
            int viewerX = ViewerPositionProvider.X;
            int viewerY = ViewerPositionProvider.Y;

            Camera mainCamera = Camera.main;
            float verticalSize = mainCamera.orthographicSize;
            float horizontalSize = mainCamera.aspect * verticalSize;

            int height = Mathf.CeilToInt(verticalSize);
            int width = Mathf.CeilToInt(horizontalSize);

            if (WasLoaded)
            {
               

                int viewerDeltaX = viewerX - _prevViewerX;
                int viewerDeltaY = viewerY - _prevViewerY;

                #region Activating

                int addLineLeftBound = viewerX - width;
                int addLineRightBound = viewerX + width;
                int addLineCount = Mathf.Abs(viewerDeltaY);
                int addLineY;
                int lineYShift;
                if (viewerDeltaY < 0) // Camera goes upwards
                {
                    addLineY = viewerY - height;
                    lineYShift = 1;
                }
                else // Camera goes downwards
                {
                    addLineY = viewerY + height;
                    lineYShift = -1;
                }

                for (int i = 0; i < addLineCount; i++)
                {
                    int y = addLineY + i * lineYShift;
                    if (viewerY - height > y || y > viewerY + height)
                        continue;
                    LineSetActive(addLineLeftBound, addLineRightBound, addLineY + i * lineYShift, true);
                }


                int addColHighBound = viewerY - height;
                int addColLowBound = viewerY + height;
                int addColCount = Mathf.Abs(viewerDeltaX);

                int addColX;
                int colXShift;
                if (viewerDeltaX < 0) // Camera goes leftwards
                {
                    addColX = viewerX - width;
                    colXShift = 1;
                }
                else // Camera goes rightwards
                {
                    addColX = viewerX + width;
                    colXShift = -1;
                }

                for (int i = 0; i < addColCount; i++)
                {
                    int x = addColX + colXShift * i;
                    if (viewerX - width > x || x > viewerX + width)
                        continue;
                    ColSetActive(addColX + colXShift * i, addColHighBound, addColLowBound, true);
                }

                #endregion

                #region Deactivating

                int removeLineLeftBound = _prevViewerX - width;
                int removeLineRightBound = _prevViewerX + width;
                int removeLineCount = Mathf.Abs(viewerDeltaY);
                int removeLineY;
                int removeLineYShift;
                if (viewerDeltaY >= 0) // Camera goes upwards
                {
                    removeLineY = _prevViewerY - height;
                    removeLineYShift = 1;
                }
                else // Camera goes downwards
                {
                    removeLineY = _prevViewerY + height;
                    removeLineYShift = -1;
                }

                for (int i = 0; i < removeLineCount; i++)
                {
                    int y = removeLineY + i * removeLineYShift;
                    /*if (_prevViewerX - height > y || y > _prevViewerX + height)
                        continue;*/
                    LineSetActive(removeLineLeftBound, removeLineRightBound, removeLineY + i * removeLineYShift, false);
                }


                int removeColHighBound = _prevViewerY - height;
                int removeColLowBound = _prevViewerY + height;
                int removeColCount = Mathf.Abs(viewerDeltaX);

                int removeColX;
                int removeColXShift;
                if (viewerDeltaX >= 0) // Camera goes leftwards
                {
                    removeColX = _prevViewerX - width;
                    removeColXShift = 1;
                }
                else // Camera goes rightwards
                {
                    removeColX = _prevViewerX + width;
                    removeColXShift = -1;
                }

                for (int i = 0; i < removeColCount; i++)
                {
                    int x = removeColX + removeColXShift * i;
                    /*if (_prevViewerX - width > x || x > _prevViewerX + width)
                        continue;*/
                    ColSetActive(removeColX + removeColXShift * i, removeColHighBound, removeColLowBound, false);
                }

                #endregion

                if (height != _prevHeight || width != _prevWidth)
                {
                    SetActiveMasksOnCamera(_prevWidth, _prevHeight, false);
                    SetActiveMasksOnCamera(width, height, true);
                }
            }

            _prevViewerX = viewerX;
            _prevViewerY = viewerY;
            _prevWidth = width;
            _prevHeight = height;
        }

        private void LineSetActive(int leftBound, int rightBound, int y, bool active)
        {
            string message = active ? "Activating" : "Deactivating";
            string formatString = $"{message} line: {leftBound} : {rightBound}, y = {y}";
            Debug.Log(formatString);

            if (leftBound < 0)
                leftBound = 0;
            if(leftBound >= _tileMaskMatrix.GetLength(1))
                return;

            if(y < 0)
                return;

            if(y >= _tileMaskMatrix.GetLength(0))
                return;

            for (int x = leftBound; x <= rightBound; x++)
            {
                _tileMaskMatrix[y, x].SetActive(active);
            }
        }

        private void ColSetActive(int x, int highBound, int lowBound, bool active)
        {
            string message = active ? "Activating" : "Deactivating" ;
            string formatString = $"{message} col: {highBound} : {lowBound}, x = {x}";
            Debug.Log(formatString);

            if (highBound < 0)
                highBound = 0;

            if (lowBound >= _tileMaskMatrix.GetLength(0))
                return;

            if (x < 0)
                return;

            if (x >= _tileMaskMatrix.GetLength(1))
                return;

            for (int y = highBound; y <= lowBound; y++)
            {
                _tileMaskMatrix[y, x].SetActive(active);
            }
        }
    }
}

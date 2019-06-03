using Assets.Scripts._Legacy;
using UnityEngine;

namespace Assets.Scripts.Controllers
{
    public class WalkController : Controller
    {
        [SerializeField] private bool _displayWalkBlockers;

        private int[,] _walkBlockerCount;
        private VisionController _visionController;
        

        public override void OnGameLoaded(IServerDataProvider controller)
        {
            WasLoaded = true;
            ServerController = controller;
            _walkBlockerCount = new int[controller.MapSize.x + 1, controller.MapSize.y + 1];
        }

        public bool CanPass(Vector2Int cell)
        {
            if(ServerController.IsCellInBounds(cell))
                return _walkBlockerCount[cell.x, cell.y] == 0;
            return false;
        }

        public bool CanPass(int x, int y)
        {
            if (!WasLoaded)
                return false;

            return _walkBlockerCount[x, y] == 0;
        }

        public void SetBlock(int x, int y)
        {
            if (!WasLoaded)
                return;

            _walkBlockerCount[x, y]++;
        }

        
        // Must be invoked before any TileObject's UpdateControllers
        private void Update()
        {
            if (!WasLoaded)
            {
                //Debug.LogError("Update on WalkController invoked before OnGameLoaded()");
                return;
            }

            if (_displayWalkBlockers)
            {
                if (_visionController == null)
                    _visionController = FindObjectOfType<VisionController>();

                for (int x = 0; x < ServerController.MapSize.x; x++)
                {
                    for (int y = 0; y < ServerController.MapSize.y; y++)
                    {
                        Color color = Color.red;
                        float brightness = 1;
                        if (_walkBlockerCount[x, y] <= 0)
                        {
                            brightness = 1;
                        }
                        if (_walkBlockerCount[x, y] == 1)
                        {
                            brightness = 0.9f;
                        }
                        if (_walkBlockerCount[x, y] > 1)
                        {
                            brightness = 0.2f;
                        }
                        _visionController.GetMask(x, y)?.SetLighting(brightness, color);
                    }
                }
            }

            for(int i = 0; i < _walkBlockerCount.GetLength(0); i++)
            for (int j = 0; j < _walkBlockerCount.GetLength(1); j++)
                _walkBlockerCount[i, j] = 0;
        }
    }
}

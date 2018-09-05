using System.IO;
using Assets.Scripts.Objects;
using Assets.Scripts.Objects.Mob;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Controllers
{
    public class ServerController : NetworkBehaviour
    {
        [SerializeField] [SyncVar] private int _mapSizeX;
        [SerializeField] [SyncVar] private int _mapSizeY;
        [SerializeField] private bool _shouldClearScene;
        [SerializeField] private bool _debugSaveScene;

        private MapManager _mapManager;
        private int _notFinished;

        private static ServerController _instance;

        public static ServerController Current => _instance;

        private void Start()
        {
            _instance = this;

            if (isServer)
            {
                _notFinished = 0;

                _mapManager = new MapManager();

                ApplyFixers();

                if (_debugSaveScene)
                {
                    _mapManager.SaveScene("SavedScene.temap");
                    Debug.Log("Scene saved!");
                }

                if (_shouldClearScene)
                    ClearScene();

                if (!LoadMap())
                {
                    Debug.LogError("Map was not loaded correctly!");
                }
            }
            else
            {
                //ClearScene();
            }

            LaunchGameplay();
        }

        private bool LoadMap()
        {
            bool ok = _mapManager.LoadMapToScene("SavedScene.temap");
            return ok;
        }

        private void ClearScene()
        {
            TileObject[] tos = FindObjectsOfType<TileObject>();
            foreach (var to in tos)
            {
                //Network.Destroy(to.gameObject);
                if(isServer)
                    NetworkServer.Destroy(to.gameObject);
                Destroy(to.gameObject);
            }
        }

        [Server]
        private void ApplyFixers()
        {
            TileObject[] tos = FindObjectsOfType<TileObject>();

            foreach (var to in tos)
            {
                Fixer fixer = to.gameObject.GetComponent<Fixer>();
                if (fixer != null)
                    fixer.ApplySavedTransformations();
            }
        }

        private void LaunchGameplay()
        {
            Controller[] controllers = FindObjectsOfType<Controller>();
            foreach (var controller in controllers)
            {
                controller.OnGameLoaded(this);
                Debug.Log("Controller loading started: " + controller.name);
                _notFinished++;
            }
        }

        public void RequestLoadingFinished()
        {
            _notFinished--;
        }

        public bool GameReady
        {
            get { return _notFinished == 0; }
        }

        [Server]
        public void SpawnPlayer(Player player)
        {
            //player.Cell = new Vector2Int(5, 5);
            SpawnPoint[] spawners = FindObjectsOfType<SpawnPoint>();
            int rand = Random.Range(0, spawners.Length);

            if (spawners.Length == 0)
            {
                Debug.LogWarning("No spawners found!");
                Vector2Int cell = Vector2Int.zero;
                player.RpcForceTransformation(cell.x, cell.y, Vector2.zero);
            }
            else
            {
                Vector2Int cell = spawners[rand].Cell;
                player.RpcForceTransformation(cell.x, cell.y, Vector2.zero);

                Debug.Log("Player spawned: " + cell);
            }
        }

        [Command]
        public void CmdRequestTransformUpdate()
        {
            TileObject[] tos = FindObjectsOfType<TileObject>();
            foreach(var to in tos)
                to.ForceSync();
        }

        public void RequestTransformUpdate()
        {
            TileObject[] tos = FindObjectsOfType<TileObject>();
            foreach (var to in tos)
                to.ForceSync();
        }

        public bool IsCellInBounds(Vector2Int cell)
        {
            return cell.x >= 0 && cell.y >= 0 && cell.x < _mapSizeX && cell.y < _mapSizeY;
        }

        public bool IsCellInBounds(int x, int y)
        {
            return x >= 0 && y >= 0 && x < _mapSizeX && y < _mapSizeY;
        }

        public Vector2Int MapSize
        {
            get { return new Vector2Int(_mapSizeX, _mapSizeY); }
        }
    }
}

using System.IO;
using Assets.Scripts.Objects;
using Assets.Scripts.Objects.Mob;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Assets.Scripts.Controllers
{
    public class ServerController : NetworkBehaviour, IServerDataProvider
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

                
                if (Application.isEditor && _debugSaveScene)
                {
                    ApplyFixers();
                    _mapManager.SaveScene("SavedScene.temap");
                    Debug.Log("Scene saved!");
                }

                if (_shouldClearScene)
                    ClearScene();

                if (!LoadMap())
                {
                    Debug.LogError("Map was not loaded correctly!");
                }
                else
                {
                    Debug.Log("Map loaded successfully: ");
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
            TileController.Current = GameObject.FindObjectOfType<TileController>();

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
                controller.RegistrateDataProvider(this);
                controller.OnGameLoaded(this);
                Debug.Log("Controller loading started: " + controller.name);
                _notFinished++;
            }
        }

        public void RequestLoadingFinished()
        {
            _notFinished--;
        }

        public bool Ready
        {
            get { return _notFinished == 0; }
        }

        [Server]
        public void SpawnPlayer(Player player)
        {
            Text output = GameObject.Find("OutputText").GetComponent<Text>();

            SpawnPoint[] spawners = FindObjectsOfType<SpawnPoint>();

            output.text += "\nSpawners count: " + spawners.Length;

            int rand = Random.Range(0, spawners.Length);
            output.text += "\nChosen spawner: " + rand;

            if (spawners.Length == 0)
            {
                output.text += "\nNO SPAWNERS FOUND!";
                Vector2Int cell = Vector2Int.zero;
                //player.RpcForceTransformation(cell.x, cell.y, Vector2.zero);
                RpcSetPlayerSpawned(player.gameObject, cell.x, cell.y, Vector2.zero);
            }
            else
            {
                Vector2Int cell = spawners[rand].Cell;
                RpcSetPlayerSpawned(player.gameObject, cell.x, cell.y, Vector2.zero);

                output.text += "\nPlayer spawned on point: " + cell.x + " " + cell.y;
            }
        }

        [ClientRpc]
        private void RpcSetPlayerSpawned(GameObject playerGo, int x, int y, Vector2 ofset)
        {
            Player player = playerGo.GetComponent<Player>();
            player.Cell = new Vector2Int(x, y);
            player.CellOffset = ofset;
            player.Spawned = true;
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

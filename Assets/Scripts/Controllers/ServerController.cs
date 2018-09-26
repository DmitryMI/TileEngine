using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Assets.Scripts.Objects;
using Assets.Scripts.Objects.Mob;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Controllers
{
    public class ServerController : NetworkBehaviour, IServerDataProvider
    {
        [SerializeField] [SyncVar] private int _mapSizeX;
        [SerializeField] [SyncVar] private int _mapSizeY;
        [SerializeField] private bool _shouldClearScene;
        [SerializeField] private bool _debugSaveScene;

        //private MapManager _mapManager;
        private int _notFinished = Int32.MaxValue;

        private bool _gameReady = false;

        private static ServerController _instance;

        public static ServerController Current => _instance;

        private void Start()
        {
            _instance = this;

            LaunchGameplay();
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
            if (TileController.Current == null)
            {
                TileController controller = GameObject.FindObjectOfType<TileController>();
                TileController.Current = controller;
            }

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

            _notFinished = controllers.Length;

            foreach (var controller in controllers)
            {
                controller.RegistrateDataProvider(this);
                controller.OnGameLoaded(this);
                Debug.Log("Controller loading started: " + controller.name);
            }
        }

        public void ReportLoadingFinished()
        {
            _notFinished--;

            if (_notFinished == 0)
            {
                _gameReady = true;
                ApplyFixers();
                Debug.Log("All controllers loaded!");
            }
        }

        public bool Ready
        {
            get { return _gameReady; }
        }

        [Server]
        public void SpawnPlayer(Player player)
        {
            StartCoroutine(WaitForLoadingFinished(SpawnPlayerImmediately, player));
        }

        IEnumerator WaitForLoadingFinished<T>(Action<T> action, T value)
        {
            while (_notFinished > 0)
            {
                //Debug.Log("WAITING FOR CONTROLLERS. Left: " + _notFinished);
                yield return new WaitForEndOfFrame();
            }

            //Debug.Log("ACTION!");
            action(value);
        }

        IEnumerator WaitForLoadingFinished<T1, T2>(Action<T1, T2> action, T1 value1, T2 value2)
        {
            while (_notFinished > 0)
            {
                //Debug.Log("WAITING FOR CONTROLLERS. Left: " + _notFinished);
                yield return new WaitForEndOfFrame();
            }

            //Debug.Log("ACTION!");
            action(value1, value2);
        }

        IEnumerator WaitForLoadingFinished<T1, T2, T3>(Action<T1, T2, T3> action, T1 value1, T2 value2, T3 value3)
        {
            while (_notFinished > 0)
            {
                //Debug.Log("WAITING FOR CONTROLLERS. Left: " + _notFinished);
                yield return new WaitForEndOfFrame();
            }

            //Debug.Log("ACTION!");
            action(value1, value2, value3);
        }

        private void SpawnPlayerImmediately(Player player)
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
        private void RpcSetPlayerSpawned(GameObject playerGo, int x, int y, Vector2 offset)
        {
            Player player = playerGo.GetComponent<Player>();
            StartCoroutine(WaitForLoadingFinished(SetPlayerSpawnedImmediately, player, new Vector2Int(x, y), offset));
        }

        private void SetPlayerSpawnedImmediately(Player player, Vector2Int cell, Vector2 offset)
        {
            player.Cell = cell;
            player.CellOffset = offset;
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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Objects;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Controllers
{
    public class TileController : Controller
    {

        private List<TileObject>[,] _objects;

        private Vector2Int _mapSize;
        private bool _allocationFinished;
        private Task _allocationTask;

        private static TileController _instance;

        public static TileController Current
        {
            get { return _instance; }
            set { _instance = value; }
        } 

        public override void OnGameLoaded(IServerDataProvider controller)
        {
            //WasLoaded = true;
            ServerController = controller;
            _mapSize = ServerController.MapSize;

            _instance = this;

            AllocateMemory();
        }

        

        [Server]
        public void AddObject(int x, int y, TileObject obj)
        {
            if(WasLoaded && ServerController.IsCellInBounds(x, y))
                _objects[x, y].Add(obj);

            if (!WasLoaded)
            {
                Debug.Log("Detected attempt to registrate object before launching TileController. This can cause malfunctions");
            }
        }

        [Server]
        public void RemoveObject(int x, int y, TileObject obj)
        {
            if (WasLoaded && ServerController.IsCellInBounds(x, y))
                _objects[x, y].Remove(obj);
        }

        [Server]
        public TileObject[] GetObjects(int x, int y)
        {
            if(WasLoaded && ServerController.IsCellInBounds(x, y))
                return _objects[x, y].ToArray();

            return null;
        }

        public T Find<T>(int x, int y) where T : TileObject
        {
            TileObject[] objects = GetObjects(x, y);

            if (objects == null)
                return null;

            foreach (var obj in objects)
            {
                var find = obj as T;
                if (find != null)
                    return find;
            }

            return null;
        }

        private IEnumerator WaitForAsyncAllocation()
        {
            while(_allocationTask.IsCompleted)
                yield return new WaitForEndOfFrame();

            WasLoaded = true;
        }

        private void AllocateMemory()
        {
            _allocationTask = new Task(AllocateMemoryAsync, TaskCreationOptions.LongRunning);
            _allocationTask.Start();

            StartCoroutine(WaitForAsyncAllocation());
        }

        private void AllocateMemoryAsync()
        {
            _objects = new List<TileObject>[_mapSize.x, _mapSize.y];

            for (int x = 0; x < _mapSize.x; x++)
            {
                for (int y = 0; y < _mapSize.x; y++)
                {
                    _objects[x, y] = new List<TileObject>(10);
                }
            }
        }
    }
}

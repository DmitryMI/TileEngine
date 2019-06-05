using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.GameMechanics;
using Assets.Scripts.Objects;
using Assets.Scripts.Objects.Item;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Controllers
{
    public class TileController : Controller
    {

        private List<TileObject>[,] _objects;

        private ItemStackCollection _itemStackCollection;

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

            //Debug.Log("Starting memoty allocation");
            AllocateMemory();
        }

        
        public void AddObject(int x, int y, TileObject obj)
        {
            if (WasLoaded && ServerController.IsCellInBounds(x, y))
            {
                _objects[x, y].Add(obj);
                if (isServer)
                {
                    Item item = obj as Item;
                    if (item)
                        _itemStackCollection.Add(item, new Vector2Int(x, y));
                }
            }

            if (!WasLoaded)
            {
                Debug.Log("Detected attempt to registrate object before launching TileController. This can cause malfunctions");
            }

            
        }

        public void RemoveObject(int x, int y, TileObject obj)
        {
            if (WasLoaded && ServerController.IsCellInBounds(x, y))
            {
                _objects[x, y].Remove(obj);

                Item item = obj as Item;
                if (item)
                    _itemStackCollection.Remove(item, new Vector2Int(x, y));
            }
        }

        public TileObject[] GetObjects(int x, int y)
        {
            if(WasLoaded && ServerController.IsCellInBounds(x, y))
                return _objects[x, y].ToArray();

            return null;
        }

        public T Find<T>(int x, int y) where T:class
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

        public void FindAll<T>(int x, int y, IList<T> list) where T: class 
        {
            TileObject[] objects = GetObjects(x, y);

            if (objects == null)
                return;

            foreach (var obj in objects)
            {
                var find = obj as T;
                if (find != null)
                {
                    list.Add(find);
                }
            }
        }

        private IEnumerator WaitForAsyncAllocation()
        {
            while (_allocationTask.IsCompleted)
            {
                Debug.Log("Waiting for task completion");
                yield return new WaitForEndOfFrame();
            }

            WasLoaded = true;
        }

        private void AllocateMemory()
        {
            if (isServer)
            {
                _itemStackCollection = new ItemStackCollection();
            }
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

        private void Update()
        {
            if (WasLoaded && ServerController.Ready && isServer)
            {
                SortItemStacks();
            }
        }

#pragma warning disable 618
        [Server]
#pragma warning restore 618
        private void SortItemStacks()
        {
            if (_itemStackCollection.Length > 0)
            {
                foreach (var stack in _itemStackCollection)
                {
                    SortItemStack(stack);
                }
            }
        }

#pragma warning disable 618
        [Server]
#pragma warning restore 618
        private void SortItemStack(ItemStack stack)
        {
            stack.SortStack();

            int currentSortingOrder = 0;

            for (int i = 0; i < stack.Length; i++)
            {
                currentSortingOrder += stack[i].LayersNeeded;
                stack[i].SpriteRenderer.sortingOrder = currentSortingOrder;
            }
        }
    }
}

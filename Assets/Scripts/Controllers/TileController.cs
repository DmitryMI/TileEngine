using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Objects;
using UnityEngine.Networking;

namespace Assets.Scripts.Controllers
{
    public class TileController : Controller
    {

        private List<TileObject>[,] _objects;

        public override void OnGameLoaded(ServerController controller)
        {
            WasLoaded = true;
            ServerController = controller;

            _objects = new List<TileObject>[controller.MapSize.x, controller.MapSize.y];

            for (int x = 0; x < controller.MapSize.x; x++)
            {
                for (int y = 0; y < controller.MapSize.x; y++)
                {
                    _objects[x, y] = new List<TileObject>(10);
                }
            }
        }

        [Server]
        public void AddObject(int x, int y, TileObject obj)
        {
            if(WasLoaded && ServerController.IsCellInBounds(x, y))
                _objects[x, y].Add(obj);
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
    }
}

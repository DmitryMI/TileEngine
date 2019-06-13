using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Controllers;
using Assets.Scripts.Objects;
using Assets.Scripts.Objects.Item;
using UnityEngine;

namespace Assets.Scripts.GameMechanics
{
    class StackCollection : IEnumerable<TileObjectStack>
    {
        private List<Vector2Int> _coordsList;
        private List<TileObjectStack> _itemStackList;

        public StackCollection()
        {
            _coordsList = new List<Vector2Int>();
            _itemStackList = new List<TileObjectStack>();
        }

        public StackCollection(int capacity)
        {
            _coordsList = new List<Vector2Int>(capacity);
            _itemStackList = new List<TileObjectStack>(capacity);
        }

        public void Add(TileObject item, Vector2Int cell)
        {
            int index = _coordsList.IndexOf(cell);

            if (index != -1)
            {
                _itemStackList[index].Add(item);
            }
            else
            {
                _coordsList.Add(cell);
                TileObjectStack stack = new TileObjectStack();
                stack.Add(item);
                _itemStackList.Add(stack);
            }
        }

        public void Remove(TileObject item, Vector2Int cell)
        {
            int index = _coordsList.IndexOf(cell);
            if (index != -1)
            {
                _itemStackList[index].Remove(item);
                if (_itemStackList[index].Length == 0)
                {
                    _coordsList.RemoveAt(index);
                    _itemStackList.RemoveAt(index);
                }
            }
        }

        public int Length => _coordsList.Count;

        public TileObjectStack this[int i] => _itemStackList[i];

        public IEnumerator<TileObjectStack> GetEnumerator()
        {
            return new StackCollectionEnumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}

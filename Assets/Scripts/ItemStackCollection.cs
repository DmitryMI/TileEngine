using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Controllers;
using Assets.Scripts.Objects.Item;
using UnityEngine;

namespace Assets.Scripts
{
    class ItemStackCollectionEnumerator : IEnumerator<ItemStack>
    {
        private int _current = 0;
        private ItemStackCollection _collection;
        public bool MoveNext()
        {
            if (_collection.Length - 1 == _current)
                return false;
            _current++;
            return true;
        }

        public void Reset()
        {
            _current = 0;
        }

        public ItemStack Current => _collection[_current];

        object IEnumerator.Current
        {
            get { return Current; }
        }

        public void Dispose()
        {
            
        }

        public ItemStackCollectionEnumerator(ItemStackCollection collection)
        {
            _collection = collection;
        }
    }

    class ItemStackCollection : IEnumerable<ItemStack>
    {
        private List<Vector2Int> _coordsList;
        private List<ItemStack> _itemStackList;

        public ItemStackCollection()
        {
            _coordsList = new List<Vector2Int>();
            _itemStackList = new List<ItemStack>();
        }

        public ItemStackCollection(int capacity)
        {
            _coordsList = new List<Vector2Int>(capacity);
            _itemStackList = new List<ItemStack>(capacity);
        }

        public void Add(Item item, Vector2Int cell)
        {
            int index = _coordsList.IndexOf(cell);

            if (index != -1)
            {
                _itemStackList[index].Add(item);
            }
            else
            {
                _coordsList.Add(cell);
                ItemStack stack = new ItemStack();
                stack.Add(item);
                _itemStackList.Add(stack);
            }
        }

        public void Remove(Item item, Vector2Int cell)
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

        public ItemStack this[int i] => _itemStackList[i];

        public IEnumerator<ItemStack> GetEnumerator()
        {
            return new ItemStackCollectionEnumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}

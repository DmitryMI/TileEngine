using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Objects;
using Assets.Scripts.Objects.Item;

namespace Assets.Scripts.Controllers
{
    class TileObjectStack
    {
        private List<TileObject> _itemList;

        private bool _isSortDirty;
        private TileObjectOrderComparator _comparator;

        public bool IsSortDirty => _isSortDirty;

        public void Add(TileObject item)
        {
            _itemList.Add(item);
            _isSortDirty = true;
        }

        public void Remove(TileObject item)
        {
            _itemList.Remove(item);
            _isSortDirty = true; // TODO Consider if it is needed
        }

        public TileObject this[int i]
        {
            get { return _itemList[i]; }
        }

        public int Length => _itemList.Count;

        public TileObjectStack()
        {
            _itemList = new List<TileObject>();
            _comparator = new TileObjectOrderComparator();
            _isSortDirty = true;
        }

        public void SortStack()
        {
            _itemList.Sort(_comparator);
            _isSortDirty = false;
        }

        class TileObjectOrderComparator : IComparer<TileObject>
        {
            public int Compare(TileObject x, TileObject y)
            {
                if (x == null || y == null)
                    return 0;
                return x.SortingStart - y.SortingStart;
            }
        }
    }
}

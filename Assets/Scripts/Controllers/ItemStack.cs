using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Objects.Item;

namespace Assets.Scripts.Controllers
{
    class ItemStack
    {
        private List<Item> _itemList;

        public void Add(Item item)
        {
            _itemList.Add(item);
        }

        public void Remove(Item item)
        {
            _itemList.Remove(item);
        }

        public Item this[int i]
        {
            get { return _itemList[i]; }
        }

        public int Length => _itemList.Count;

        public ItemStack()
        {
            _itemList = new List<Item>();
        }

        public void SortStack()
        {
            _itemList.Sort(new ItemOrderComparator());
        }

        class ItemOrderComparator : IComparer<Item>
        {
            public int Compare(Item x, Item y)
            {
                if (x == null || y == null)
                    return 0;
                return x.SortingOrder - y.SortingOrder;
            }
        }
    }
}

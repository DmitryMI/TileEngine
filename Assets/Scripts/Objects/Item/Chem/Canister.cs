using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Objects.Item.Chem
{
    class Canister : Item
    {
        [SerializeField] private VolumeFiller _filler;

        [SerializeField] private float _fillValue;
        [SerializeField] private Color _color;

        public override int LayersNeeded => 2;

        protected override void OnSortingOrderChange()
        {
            if (SpriteRenderer == null)
                return;

            Renderer.sortingOrder = SortingStartIndex + 1;
        }

        protected override void Update()
        {
            base.Update();

            if (Holder != null)
            {
                _filler.gameObject.SetActive(false);
            }
            else
            {
                _filler.gameObject.SetActive(true);
            }

            _filler.FillValue = _fillValue;
            _filler.ReagentsColor = _color;
            _filler.SetSortingOrder(SpriteRenderer.sortingOrder - 1);
        }
    }
}

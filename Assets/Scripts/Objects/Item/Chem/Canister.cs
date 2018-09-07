using System;
using System.Collections.Generic;
using System.Linq;
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

        protected override void Update()
        {
            base.Update();

            _filler.FillValue = _fillValue;
            _filler.ReagentsColor = _color;
        }
    }
}

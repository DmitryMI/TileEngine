using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Objects.Turf
{
    class MetalPlating : Turf
    {
        [SerializeField] private Sprite[] _variationList;

        [SerializeField] private int _variation;

        private SpriteRenderer _spriteRenderer;

        protected override void Start()
        {
            base.Start();
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        protected override void Update()
        {
            base.Update();

            if(_variationList.Length != 0 && _variation >= 0 && _variation < _variationList.Length)
                _spriteRenderer.sprite = _variationList[_variation];
        }

        public override string ToMap()
        {
            return base.ToMap() + " " + _variation;
        }

        public override string DescriptiveName
        {
            get { return "Metal plating"; }
        }

        protected override bool Transperent
        {
            get { return true; }
        }

        protected override bool PassesGas
        {
            get { return true; }
        }

        protected override bool CanWalkThrough
        {
            get { return true; }
        }

        public override bool FromMap(string mapData)
        {
            bool ok = base.FromMap(mapData);

            string[] units = mapData.Split(' ');

            try
            {
                _variation = Int32.Parse(units[4]);
            }
            catch (FormatException ex)
            {
                Debug.Log(ex.Message);
                ok = false;
            }

            return ok;
        }
    }
}


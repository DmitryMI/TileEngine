using System;
using System.Globalization;
using Assets.Scripts.GameMechanics;
using Assets.Scripts.Objects.Equipment.Power;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Objects.Turf
{
    public class TestLightFixture : Turf, IPowerConsumer
    {
        [SerializeField]
        [SyncVar]
        private float _range;

        [SerializeField]
        [SyncVar]
        private Color _color;

        [SerializeField] [SyncVar] private float _initialIntensity = 1f;
        [SerializeField] [SyncVar] private float _intensityDecrement = 0.05f;

        private float _power;

        [SyncVar] private bool _isLighting;

        protected override void Update ()
        {
            base.Update();

            if (isServer)
            {
                if (_power > 0)
                {
                    //UpdateVisionController();
                    _isLighting = true;
                    _power -= 25;
                }
                else
                {
                    _power = 0;
                    _isLighting = false;
                }
            }

            if (isClient)
            {
                if (_isLighting)
                {
                    UpdateVisionController();
                }
            }
        }

        private void UpdateVisionController()
        {
            LightSourceInfo info = new LightSourceInfo(Cell.x, Cell.y, _color, _range, _initialIntensity, _intensityDecrement);
            VisionController.SetLightForOneFrame(info);
        }

        public override string ToMap()
        {
            string baseMap = base.ToMap() + " ";
            baseMap += _range.ToString(CultureInfo.InvariantCulture) + " " + _color.r + " " + _color.g + " " + _color.b + " " + _initialIntensity;

            return baseMap;
        }

        public override bool FromMap(string mapData)
        {
            string[] units = mapData.Split(' ');

            bool ok = true;
            try
            {
                int x = Int32.Parse(units[0]);
                int y = Int32.Parse(units[1]);
                float offsetX = (float)Double.Parse(units[2]);
                float offsetY = (float)Double.Parse(units[3]);
                float range = (float)Double.Parse(units[4]);
                float r = (float)Double.Parse(units[5]);
                float g = (float)Double.Parse(units[6]);
                float b = (float)Double.Parse(units[7]);
                _initialIntensity = (float)Double.Parse(units[8]);

                Cell = new Vector2Int(x, y);
                CellOffset = new Vector2(offsetX, offsetY);
                _range = range;
                _color.r = r;
                _color.g = g;
                _color.b = b;

            }
            catch (FormatException ex)
            {
                Debug.Log(ex.Message);
                ok = false;
            }

            return ok;
        }

        public void SendPower(float power)
        {
            float delta = 100 - _power;
            if (delta > power)
                _power += power;
            else
            {
                _power += delta;
            }
        }

        public float AmountOfNeededPower
        {
            get { return 100f; }
        }
    }
}

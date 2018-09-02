using Assets.Scripts.Controllers.Atmos;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Assets.Scripts.Objects.Item
{
    public class AirAnalyzer : Item
    {
        [SerializeField] private Sprite _alertSprite;
        [SerializeField] private Sprite _alertAltSprite;

        [SerializeField] private float _minimumPressure;
        [SerializeField] private float _maximumPressure;
        [SerializeField] private float _minimumOxygenPart;
        [SerializeField] private float _maximumOxygenPart;

        [SyncVar] private bool _isOk;
        [SyncVar] private float _oxygenPart;
        [SyncVar] private float _pressure;
        [SyncVar] private float _temperature;


        private Animator _animator;

        public override string DescriptiveName
        {
            get { return "Air analyzer"; }
        }

        protected override void Start()
        {
            base.Start();

            _animator = GetComponent<Animator>();
        }

        [Server]
        private void CheckAtmos()
        {
            bool ok = true;

            Vector2Int cell;
            if (Holder == null)
                cell = Cell;
            else
                cell = Holder.GetComponent<TileObject>().Cell;

            Gas oxygen = AtmosController.GetGasByName("Oxygen");

            GasInfo[] gasInfos = AtmosController.GetGases(cell.x, cell.y);

            if (gasInfos == null)
            {
                return;
            }

            float summPressure = 0;
            float oxygenPressure = 0;

            foreach (var gasInfo in gasInfos)
            {
                summPressure += gasInfo.Pressure;
                if (gasInfo.GasId == oxygen.Id)
                {
                    oxygenPressure += gasInfo.Pressure;
                }
            }
            float oxygenPart;

            if (summPressure != 0)
                oxygenPart = oxygenPressure / summPressure;
            else
            {
                oxygenPart = 0;
            }

            if (oxygenPart < _minimumOxygenPart || oxygenPart > _maximumOxygenPart)
                ok = false;

            if (summPressure < _minimumPressure || summPressure > _maximumPressure)
                ok = false;

            //Debug.Log("Oxygen: " + oxygenPart * 100 + "%" + ", pressure: " + summPressure + "kPa");

            _oxygenPart = oxygenPart;
            _pressure = summPressure;

            if(gasInfos.Length > 0)
                _temperature = gasInfos[0].Temperature;

            _isOk = ok;
        }

        protected override void Update()
        {
            base.Update();

            if (isServer)
            {
                CheckAtmos();
            }

            _animator.SetBool("Ok", _isOk);
        }

        public override void ApplyItemClient(Item item)
        {
            // Item pick
            base.ApplyItemClient(item);

            if (item == this)
            {
                // TODO Fix stupid output
                GameObject.Find("OutputText").GetComponent<Text>().text +=
                    "\nPressure (kPa): " + _pressure + "\nTemperature (C): " + (_temperature - 273.15f) + "\nOxygen: " +
                    _oxygenPart * 100 + "%\n";
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Controllers.Atmos
{

    public class GasInfo
    {
        private float _pressure;
        private readonly int _gasId;
        private float _temperature;

        public GasInfo(int gasId)
        {
            _pressure = 0;
            _gasId = gasId;
            _temperature = 0;
        }

        public GasInfo(float pressure, int gasId)
        {
            _pressure = pressure;
            _gasId = gasId;
            _temperature = 0;
        }

        public GasInfo(float pressure, int gasId, float temperature)
        {
            _pressure = pressure;
            _gasId = gasId;
            _temperature = temperature;
        }

        public float Pressure
        {
            get { return _pressure; }
            set { _pressure = value; }
        }

        public int GasId
        {
            get { return _gasId; }
        }

        public float Temperature
        {
            get { return _temperature; }
            set { _temperature = value; }
        }

        public float TemperatureCelsium
        {
            get { return _temperature + 274.15f; }
        }


        public int CompareTo(object obj)
        {
            GasInfo other = (GasInfo) obj;
            return _gasId - other._gasId;
        }
    }
}

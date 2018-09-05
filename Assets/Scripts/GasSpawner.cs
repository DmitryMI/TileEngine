using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Assets.Scripts.Controllers.Atmos;
using Assets.Scripts.Objects;
using UnityEngine;

namespace Assets.Scripts
{
    public class GasSpawner : TileObject
    {
        [Tooltip("Format: <name1>-<part1>,<name2>-<part2>,...")]
        [SerializeField] private string _gasNames;
        [SerializeField] private float _temperatureCelsium;
        [SerializeField] private float _pressure;

        protected override bool Transperent
        {
            get { return true; }
        }

        protected override bool CanWalkThrough
        {
            get { return true; }
        }

        protected override bool PassesGas
        {
            get { return true; }
        }

        public override string DescriptiveName
        {
            get { return "Gas spawner"; }
        }

        public override bool FromMap(string data)
        {
            bool ok = base.FromMap(data);

            string[] units = data.Split(' ');

            try
            {
                string gasNames = units[4];
                float pressure = float.Parse(units[5]);
                float temperatureCelsium = float.Parse(units[6]);
                _gasNames = gasNames;
                _pressure = pressure;
                _temperatureCelsium = temperatureCelsium;
            }
            catch (FormatException ex)
            {
                ok = false;
                Debug.Log(ex.Message);
            }

            return ok;
        }

        public override string ToMap()
        {
            string res = base.ToMap() + " " + _gasNames + " " + _pressure + " " + _temperatureCelsium;
            return res;
        }

        protected override void Start()
        {
            base.Start();
            StartCoroutine(SpawnGasDelayed());
        }

        IEnumerator SpawnGasDelayed()
        {
            while (!AtmosController.IsReady)
            {
                yield return new WaitForEndOfFrame();
            }

            try
            {
                GasInfo[] gasInfos = ParseGasSettings(_gasNames, _pressure, _temperatureCelsium + 273.15f);
                foreach (var gasInfo in gasInfos)
                {
                    AtmosController.AddGas(Cell.x, Cell.y, gasInfo.GasId, 1f, gasInfo.Pressure, _temperatureCelsium + 273.15f);
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning(ex.Message);
            }
        }

        private GasInfo[] ParseGasSettings(string settings, float summPressure, float temp)
        {
            string[] gasSets = settings.Split(',');

            List<GasInfo> gasInfos = new List<GasInfo>();

            foreach (var gasSet in gasSets)
            {
                string[] namePart = gasSet.Split('-');
                string name = namePart[0];
                float part = float.Parse(namePart[1], CultureInfo.InvariantCulture);
                Gas gas = AtmosController.GetGasByName(name);
                int id = gas.Id;

                gasInfos.Add(new GasInfo(summPressure * part, id, temp));
            }

            return gasInfos.ToArray();
        }
        
    }
}

using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Controllers.Atmos
{
    public class AtmosController : Controller
    {
        [SerializeField] private bool _visualizeGas = false;
        [SerializeField] private Gas[] _gasList;
        
        private static AtmosController _current;
        public static AtmosController Current
        {
            get { return _current; }
        }

        public override void OnGameLoaded(ServerController controller)
        {
            WasLoaded = true;
            ServerController = controller;
            _current = this;

            
        }

        [Server]
        public GasInfo[] GetGases(int x, int y)
        {
            throw new NotImplementedException();
        }
        

        [Server]
        public void SetBlock(int x, int y)
        {
            throw new NotImplementedException();
        }

        [Server]
        public void AddGas(int x, int y, int gasId, float volume, float pressure,float temperature)
        {
            throw new NotImplementedException();
        }

        public Gas GetGasById(int gasId)
        {
            foreach (var gas in _gasList)
            {
                if (gas.Id == gasId)
                    return gas;
            }

            return new Gas();
        }

        public Gas GetGasByName(string gasName)
        {
            foreach (var gas in _gasList)
            {
                if (gas.Name == gasName)
                    return gas;
            }

            return new Gas();
        }
        
        private void LateUpdate()
        {

        }

        
    }
}

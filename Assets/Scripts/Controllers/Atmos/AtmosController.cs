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

        #region Static access
        private static AtmosController _current;
        public static AtmosController Current
        {
            get { return _current; }
        }
        #endregion

        private NativeArray<NativeArray<NativeList<GasInfo>>> _gasInfoMatrix;
        private NativeArray<NativeArray<int>> _gasBlockerMatrix;

        private JobHandle _jobHandle;

        public override void OnGameLoaded(ServerController controller)
        {
            WasLoaded = true;
            ServerController = controller;
            _current = this;

            InitiateMatrixes();

            Debug.Log("Atmos: Matrixes initiated");
        }

        private void TestNativeList()
        {
            NativeList<int> list = new NativeList<int>();

            Debug.Log(list);
            list.Add(1);
            Debug.Log(list);
            list.Add(2);
            Debug.Log(list);
            list.Add(3);
            Debug.Log(list);
            list.Add(4);
            Debug.Log(list);
            list.Add(5);
            Debug.Log(list);
            list.Add(6);
            Debug.Log(list);
            list.Remove(5);
            Debug.Log(list);
            list.Remove(6);
            Debug.Log(list);
            list.Remove(4);
            Debug.Log(list);
            list.Remove(5);
            Debug.Log(list);
            list.Remove(1);
            Debug.Log(list);
            list.Remove(3);
            Debug.Log(list);
            list.Remove(2);
            Debug.Log(list);

            list.Add(1);
            Debug.Log(list);
            list.Add(2);
        }

        private void InitiateMatrixes()
        {
            _gasBlockerMatrix = new NativeArray<NativeArray<int>>(ServerController.MapSize.x, Allocator.Persistent);
            _gasInfoMatrix = new NativeArray<NativeArray<NativeList<GasInfo>>>(ServerController.MapSize.x, Allocator.Persistent);
            for (int x = 0; x < ServerController.MapSize.x; x++)
            {
                for (int y = 0; y < ServerController.MapSize.y; y++)
                {
                    _gasBlockerMatrix[x] = new NativeArray<int>(ServerController.MapSize.y, Allocator.Persistent);
                    _gasInfoMatrix[x] = new NativeArray<NativeList<GasInfo>>();
                }
            }
        }

        [Server]
        public GasInfo[] GetGases(int x, int y)
        {
            //throw new NotImplementedException();
            return null;
        }
        

        [Server]
        public void SetBlock(int x, int y)
        {
            //throw new NotImplementedException();
        }

        [Server]
        public void AddGas(int x, int y, int gasId, float volume, float pressure,float temperature)
        {
            //throw new NotImplementedException();
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
            if (_jobHandle.IsCompleted)
            {
                ProcessAtmos();
            }
        }

        private void ProcessAtmos()
        {
            AtmosJob job = new AtmosJob()
            {
                GasInfoMatrix = _gasInfoMatrix,
                MapWidth = ServerController.MapSize.x,
                MapHeight = ServerController.MapSize.y,
                GasBlockerMatrix = _gasBlockerMatrix
            };

            _jobHandle = job.Schedule(0, ServerController.MapSize.x * ServerController.MapSize.y);
        }

        struct AtmosJob : IJobParallelFor
        {
            public int MapWidth;
            public int MapHeight;

            public NativeArray<NativeArray<NativeList<GasInfo>>> GasInfoMatrix;
            public NativeArray<NativeArray<int>> GasBlockerMatrix;

            public void Execute(int index)
            {
                int x = index / MapWidth;
                int y = index - x;

                ProcessCell(x, y);
            }

            private void ProcessCell(int x, int y)
            {
                
            }

        }
        
    }
}

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

        private List<GasInfo>[,] _gasInfos;
        private int[,] _gasBlockers;

        private static AtmosController _current;
        public static AtmosController Current
        {
            get { return _current; }
        }
        #endregion
      

        public override void OnGameLoaded(ServerController controller)
        {
            WasLoaded = true;
            ServerController = controller;
            _current = this;

            InitiateMatrixes();

            Debug.Log("Atmos: Matrixes initiated");
        }

        

        private void InitiateMatrixes()
        {
            if (isServer)
            {

                Vector2Int mapSize = controller.MapSize;

                _gasInfos = new List<GasInfo>[mapSize.x, mapSize.y];
                _gasBlockers = new int[mapSize.x, mapSize.y];

                for (int i = 0; i < _gasInfos.GetLength(0); i++)
                {
                    for (int j = 0; j < _gasInfos.GetLength(1); j++)
                    {
                        _gasInfos[i, j] = new List<GasInfo>();
                    }
                }

                ResetGasBlockers();
            }
        }

        [Server]
        public GasInfo[] GetGases(int x, int y)
        {
            if (!WasLoaded)
                return null;

            if(ServerController.IsCellInBounds(x, y))
                return _gasInfos[x, y].ToArray();

            return null;
        }

        [Server]
        public float GetPressure(int x, int y)
        {
            List<GasInfo> gases = _gasInfos[x, y];

            float summ = 0;
            foreach (var gas in gases)
            {
                summ += gas.Pressure;
            }

            return summ;
        }

        [Server]
        public void SetBlock(int x, int y)
        {

            if (!WasLoaded)
                return;

            if (!ServerController.IsCellInBounds(new Vector2Int(x, y)))
                return;

            _gasBlockers[x, y]++;
        }

        [Server]
        public void AddGas(int x, int y, int gasId, float volume, float pressure,float temperature)
        {

            List<GasInfo> gasMixture = _gasInfos[x, y];
            GasInfo gasInfo = FindGas(gasMixture, gasId);

            if (gasInfo != null)
            {
                gasInfo.Pressure += pressure / volume; // TODO Volume?
            }
            else
            {
                gasMixture.Add(new GasInfo(pressure / volume, gasId, temperature));
            }
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

        private void ResetGasBlockers()
        {
            for (int i = 0; i < _gasInfos.GetLength(0); i++)
            {
                for (int j = 0; j < _gasInfos.GetLength(1); j++)
                {
                    _gasBlockers[i,j] = 0;
                }
            }
        }

        private void Update()
        {
            if (!WasLoaded)
            {
                return;
            }

            if (isServer)
            {
                for (int i = 0; i < _gasBlockers.GetLength(0); i++)
                for (int j = 0; j < _gasBlockers.GetLength(1); j++)
                {
                    _gasBlockers[i, j] = 0;
                }
            }
        }

        private void VisualizeGas()
        {
            VisionController visionController = VisionController.Current;
            if (visionController != null)
            {
                visionController.VisionProcessingEnabled = false;

                for (int x = 0; x < _gasInfos.GetLength(0); x++)
                {
                    for (int y = 0; y < _gasInfos.GetLength(1); y++)
                    {
                        VisionMask mask = visionController.GetMask(x, y);
                        mask.SetVisible();

                        float summPressure = GetPressure(x, y);

                        float brightness = (100 - summPressure) / 100;

                        Color color = new Color(0, 0, 0, 1);
                        List<GasInfo> mixture = _gasInfos[x, y];

                        foreach (GasInfo info in mixture)
                        {
                            Color gasColor = GetGasById(info.GasId).Color;
                            color += gasColor * info.Pressure / summPressure;
                        }

                        mask.SetLighting(brightness, color);
                    }
                }
            }
        }

        private GasInfo FindGas(List<GasInfo> mixture, int gasId)
        {
            foreach (var gasInfo in mixture)
            {
                if (gasInfo.GasId == gasId)
                    return gasInfo;
            }
            return null;
        }


        private void ProcessAtmos()
        {
            
            for (int x = 0; x < _gasInfos.GetLength(0); x++)
            {
                for (int y = 0; y < _gasInfos.GetLength(1); y++)
                {
                   ProcessCell(x, y);
                }
            }
        }

        private void ProcessCell(int x, int y)
        {
            List<Vector2Int> freeCells = new List<Vector2Int>(4);

            if (ServerController.IsCellInBounds(x - 1, y) && _gasBlockers[x - 1, y] == 0)
                freeCells.Add(new Vector2Int(x - 1, y));

            if (ServerController.IsCellInBounds(x, y + 1) && _gasBlockers[x, y + 1] == 0)
                freeCells.Add(new Vector2Int(x, y + 1));

            if (ServerController.IsCellInBounds(x, y - 1) && _gasBlockers[x, y - 1] == 0)
                freeCells.Add(new Vector2Int(x, y - 1));

            if (ServerController.IsCellInBounds(x + 1, y) && _gasBlockers[x + 1, y] == 0)
                freeCells.Add(new Vector2Int(x + 1, y));

            List<GasInfo> gasMixture = _gasInfos[x, y];
            foreach (GasInfo gasInfo in gasMixture)
            {
                float summPressure = gasInfo.Pressure;

                List<GasInfo> neighbourGasInfos = new List<GasInfo>(4);

                foreach (var cell in freeCells)
                {
                    List<GasInfo> cellGasMixture = _gasInfos[cell.x, cell.y];
                    GasInfo cellGasInfo = FindGas(cellGasMixture, gasInfo.GasId);

                    if (cellGasInfo != null)
                    {
                        summPressure += cellGasInfo.Pressure;
                        neighbourGasInfos.Add(cellGasInfo);
                    }
                    else
                    {
                        GasInfo info = new GasInfo(0, gasInfo.GasId);
                        cellGasMixture.Add(info);
                        neighbourGasInfos.Add(info);
                    }
                }

                float average;
                if (_gasBlockers[x, y] == 0)
                {
                    average = summPressure / (freeCells.Count + 1);
                    gasInfo.Pressure = average;
                }
                else
                {
                    average = summPressure / (freeCells.Count);
                    gasInfo.Pressure = 0;
                }

                foreach (var cellGasInfo in neighbourGasInfos)
                {
                    cellGasInfo.Pressure = average;
                }
            }
        }
        
        private void LateUpdate()
        {
            if (WasLoaded && isServer)
            {
                ProcessAtmos();
            }

            if (_visualizeGas)
                VisualizeGas();
        }        
        
    }
}

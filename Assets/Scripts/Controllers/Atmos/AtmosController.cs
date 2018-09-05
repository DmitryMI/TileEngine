using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Controllers.Atmos
{
    public class AtmosController : Controller
    {
        #region Static access
        private static AtmosController _current;
        public static AtmosController Current
        {
            get { return _current; }
        }
        #endregion


        [SerializeField] private bool _visualizeGas = false;
        [SerializeField] private Gas[] _gasList;

        private List<GasInfo>[,] _gasInfos;
        private int[,] _gasBlockers;

        private List<GasAdderData> _gasAdderDatas;
        private List<BlockAdderData> _blockAdderDatas;
        private Vector2Int _mapSize;
        private Task _atmosTask;
        private bool _loopStarted = false;

        private Task _allocationTask;

        public override void OnGameLoaded(ServerController controller)
        {
            ServerController = controller;
            _current = this;
            _mapSize = ServerController.MapSize;

            InitiateMatrixes();
        }

        

        private void InitiateMatrixes()
        {
            if (isServer)
            {
                _allocationTask = new Task(AllocateMemoryAsync, TaskCreationOptions.LongRunning);
                _allocationTask.Start();
            
                StartCoroutine(WaitForCompletion());

                int capacity = ServerController.MapSize.x * ServerController.MapSize.y - 1;

                _gasAdderDatas = new List<GasAdderData>(capacity);
                _blockAdderDatas = new List<BlockAdderData>(capacity);

                ResetGasBlockers();
            }
        }

        private IEnumerator WaitForCompletion()
        {
            while (!_allocationTask.IsCompleted)
            {
                yield return new WaitForEndOfFrame();
            }

            Debug.Log("Atmos: Matrixes initiated");

            WasLoaded = true;
        }

        private void AllocateMemoryAsync()
        {
            _gasInfos = new List<GasInfo>[_mapSize.x, _mapSize.y];
            _gasBlockers = new int[_mapSize.x, _mapSize.y];

            for (int i = 0; i < _gasInfos.GetLength(0); i++)
            {
                for (int j = 0; j < _gasInfos.GetLength(1); j++)
                {
                    _gasInfos[i, j] = new List<GasInfo>();
                }
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

            if (gases != null)
            {
                foreach (var gas in gases)
                {
                    summ += gas.Pressure;
                }
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

            _blockAdderDatas.Add(new BlockAdderData(){X = x, Y = y});
        }

        [Server]
        public void AddGas(int x, int y, int gasId, float volume, float pressure, float temperature)
        {
            if (!ServerController.IsCellInBounds(new Vector2Int(x, y)))
                return;

            _gasAdderDatas.Add(new GasAdderData(){GasId = gasId, Pressure = pressure, Temperature =  temperature, X = x, Y = y, Volume = volume});
        }

        private void AddGasAfterJob()
        {
            foreach (var gasAdder in _gasAdderDatas)
            {
                int x = gasAdder.X;
                int y = gasAdder.Y;
                int gasId = gasAdder.GasId;
                float pressure = gasAdder.Pressure;
                float volume = gasAdder.Volume;
                float temperature = gasAdder.Temperature;

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

            _gasAdderDatas.Clear();
        }

        private void AddBlocksAfterJob()
        {
            foreach (var blockAdder in _blockAdderDatas)
            {
                _gasBlockers[blockAdder.X, blockAdder.Y]++;
            }

            _blockAdderDatas.Clear();
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
                //if (_atmosJobThread == null || !_atmosJobThread.IsAlive)
                if(!_loopStarted || _atmosTask.Status != TaskStatus.Running)
                {
                    for (int i = 0; i < _gasBlockers.GetLength(0); i++)
                    for (int j = 0; j < _gasBlockers.GetLength(1); j++)
                    {
                        _gasBlockers[i, j] = 0;
                    }
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
                        float summPressure = 0;
                        List<GasInfo> gases = _gasInfos[x, y];
                        if (gases != null)
                        {
                            foreach (var gas in gases)
                            {
                                summPressure += gas.Pressure;
                            }
                        }

                        float brightness = (100 - summPressure) / 100;

                        Color color = new Color(0, 0, 0, 1);
                        List<GasInfo> mixture = _gasInfos[x, y];

                        foreach (GasInfo info in mixture)
                        {
                            Color gasColor = GetGasById(info.GasId).Color;
                            color += gasColor * info.Pressure / summPressure;
                        }

                        if (mask != null)
                        {
                            mask.SetVisible();
                            mask.SetLighting(brightness, color);
                        }
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

        private void ProcessGasParallel(int index)
        {
            int x = index / _mapSize.x;
            int y = index - x * _mapSize.x;

            ProcessCell(x, y);
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
            for (var i = 0; i < gasMixture.Count; i++)
            {
                GasInfo gasInfo = gasMixture[i];
                float summPressure = gasInfo.Pressure;

                List<GasInfo> neighbourGasInfos = new List<GasInfo>(4);

                for (var j = 0; j < freeCells.Count; j++)
                {
                    var cell = freeCells[j];
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

                for (var k = 0; k < neighbourGasInfos.Count; k++)
                {
                    var cellGasInfo = neighbourGasInfos[k];
                    cellGasInfo.Pressure = average;
                }
            }
        }
        
        private void LateUpdate()
        {
            if (WasLoaded && isServer)
            {
                //if (_atmosJobThread == null || !_atmosJobThread.IsAlive)
                if(!_loopStarted || _atmosTask.Status != TaskStatus.Running)
                {
                    AddBlocksAfterJob();
                    AddGasAfterJob();

                    if (_visualizeGas)
                        VisualizeGas();

                    _atmosTask = new Task(ProcessAtmos, TaskCreationOptions.LongRunning);
                    _atmosTask.Start();
                    _loopStarted = true;
                }
            }
        }        
        
    }
}

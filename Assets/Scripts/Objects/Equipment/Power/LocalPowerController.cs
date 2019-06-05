using System;
using System.Collections.Generic;
using Assets.Scripts.GameMechanics;
using Assets.Scripts.Ui;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Objects.Equipment.Power
{
    public class LocalPowerController : Equipment, IPowerConsumer, IWallPlaceable, IPlayerInteractable, INetworkDataReceiver
    {
        [SerializeField] [SyncVar] private Direction _wallPressDirection;
        [SerializeField] private float _pressedOffset;

        [SerializeField] private SpriteRenderer _screenSpriteRenderer;

        [SerializeField] private List<Vector2Int> _connectedCellsRelative;
        [SerializeField] private LpcTerminal _terminalPrefab;

        private LpcTerminal _terminalInstance;

        public Direction WallPressDirection => _wallPressDirection;

        protected override bool Transparent => true;
        protected override bool PassesGas => true;
        protected override bool CanWalkThrough => true;

        [SerializeField]
        private float _powerStored = 500f;

        [SerializeField]
        private float _powerCapacity = 1000f;

        [SyncVar] private bool _charging;

        [SerializeField] [SyncVar] private bool _ligtingEnabled = true;
        [SerializeField] [SyncVar] private bool _equipmentEnabled = true;
        [SerializeField] [SyncVar] private bool _lifeSupportEnabled = true;
        [SerializeField] [SyncVar] private bool _containmentEnabled = true;

        public bool CheckEnabled(PowerablePriority priority)
        {
            switch (priority)
            {
                case PowerablePriority.LifeSupport:
                    return _lifeSupportEnabled;
                case PowerablePriority.Containment:
                    return _containmentEnabled;
                case PowerablePriority.Lighting:
                    return _ligtingEnabled;
                case PowerablePriority.Equipment:
                    return _equipmentEnabled;
                default:
                    throw new ArgumentOutOfRangeException(nameof(priority), priority, null);
            }
        }

        protected override void Update()
        {
            base.Update();

            if (_charging)
            {
                _screenSpriteRenderer.color = Color.green;
            }
            else
            {
                _screenSpriteRenderer.color = Color.red;
            }

            if (isServer)
            {
                SetOffset();
                ElectrifyArea();
            }
        }

        [Server]
        private void ElectrifyArea()
        {
            foreach (var cellRelative in _connectedCellsRelative)
            {
                Vector2Int cell = Cell + cellRelative;
                List<IWirelessPowerable> powerables = new List<IWirelessPowerable>();
                TileController.FindAll(cell.x, cell.y, powerables);
                foreach (var powerable in powerables)
                {
                    //if(powerable.Priority == PowerablePriority.Lighting && _ligtingEnabled)
                    if (ShouldSendPower(powerable))
                    {
                        float powerNeeded = powerable.PowerNeeded;
                        if (_powerStored >= powerNeeded)
                        {
                            _powerStored -= powerNeeded;
                            powerable.Electrify();
                        }

                        Debug.DrawRay(transform.position, powerable.gameObject.transform.position - transform.position,
                            Color.yellow);
                    }
                }
            }
        }

        private bool ShouldSendPower(IWirelessPowerable powerable)
        {
            if (powerable.Priority == PowerablePriority.Lighting && _ligtingEnabled)
                return true;
            if (powerable.Priority == PowerablePriority.Containment && _containmentEnabled)
                return true;
            if (powerable.Priority == PowerablePriority.LifeSupport && _lifeSupportEnabled)
                return true;

            return powerable.Priority == PowerablePriority.Equipment && _equipmentEnabled;
        }

        protected void LateUpdate()
        {
            if (isServer)
            {
                _charging = false;
            }
        }

        [Server]
        private void SetOffset()
        {
            Vector2 cellOffset = Vector2.zero;

            switch (_wallPressDirection)
            {
                case Direction.Forward:
                    cellOffset.y = _pressedOffset;
                    break;
                case Direction.Backward:
                    cellOffset.y = -_pressedOffset;
                    break;
                case Direction.Left:
                    cellOffset.x = -_pressedOffset;
                    break;
                case Direction.Right:
                    cellOffset.x = _pressedOffset;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            CellOffset = cellOffset;
        }

        public void SendPower(float power)
        {
            _powerStored += power;

            if (_powerStored > _powerCapacity)
            {
                _powerStored = _powerCapacity;
            }

            if(power >= CalculateNeededPower())
                _charging = true;
        }

        public float AmountOfNeededPower => CalculateNeededPower();

        private float CalculateNeededPower()
        {
            return Mathf.Max(10, _powerCapacity - _powerStored);
        }

        // Interaction

        public void OnTerminalDestruction()
        {
            _terminalInstance = null;
        }

        public float StoreState => _powerStored / _powerCapacity;
        public float PowerStored => _powerStored;
        public bool ChargeState => _charging;

        public void ApplyItemClient(Item.Item item)
        {
            if (item == null)
            {
                // Show terminal
                if (_terminalInstance == null)
                {
                    GameObject terminalInstanceGo =
                        Instantiate(_terminalPrefab.gameObject, FindObjectOfType<Canvas>().transform);
                    _terminalInstance = terminalInstanceGo.GetComponent<LpcTerminal>();
                    _terminalInstance.SetInvoker(this);
                }
            }
            else
            {

                // TODO Tool interaction
                ApplyItemServer(item);
            }
        }

        public void ApplyItemServer(Item.Item item)
        {
            throw new NotImplementedException();
        }

        public void ReceiveData(INetworkDataReceiver sender, byte[] data)
        {
            if (data.Length == 1 && (LocalPowerController) sender == this)
            {
                Debug.Log("IsServer: " + isServer + ", Data received!");
                PowerablePriority priorityCommand = (PowerablePriority)data[0];

                switch (priorityCommand)
                {
                    case PowerablePriority.Lighting:
                        _ligtingEnabled = !_ligtingEnabled;
                        break;

                    case PowerablePriority.LifeSupport:
                        _lifeSupportEnabled = !_lifeSupportEnabled;
                        break;
                    case PowerablePriority.Containment:
                        _containmentEnabled = !_containmentEnabled;
                        break;
                    case PowerablePriority.Equipment:
                        _equipmentEnabled = !_equipmentEnabled;
                        break;
                    default:
                        Debug.LogWarning("Unknown command!");
                        break;
                }
            }
        }
    }
}

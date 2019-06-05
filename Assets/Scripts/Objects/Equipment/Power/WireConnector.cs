using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.GameMechanics;
using UnityEngine;

namespace Assets.Scripts.Objects.Equipment.Power
{
    class WireConnector : Equipment, IPowerSender, IPowerSpreader
    {
        [SerializeField] private Sprite _north;
        [SerializeField] private Sprite _south;
        [SerializeField] private Sprite _west;
        [SerializeField] private Sprite _east;

        [SerializeField] private Direction _connectionDirection;

        private SpriteRenderer _spriteRenderer;

        protected override bool Transparent
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
            get { return "Wire connector"; }
        }

        protected override void Start()
        {
            base.Start();
            
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        protected override void Update()
        {
            base.Update();

            UpdateSprite();
        }


        private void UpdateSprite()
        {
            switch (_connectionDirection)
            {
                case Direction.Forward:
                    _spriteRenderer.sprite = _north;
                    break;
                case Direction.Backward:
                    _spriteRenderer.sprite = _south;
                    break;
                case Direction.Left:
                    _spriteRenderer.sprite = _west;
                    break;
                case Direction.Right:
                    _spriteRenderer.sprite = _east;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private Wire FindWire(int x, int y)
        {
            TileObject[] objects = TileController.GetObjects(x, y);

            if (objects == null)
                return null;

            foreach (var obj in objects)
            {
                if(obj is Wire)
                    return obj as Wire;
            }

            return null;
        }

        public IPowerConsumer[] FindConsumers()
        {
            List<Wire> wires = new List<Wire>();
            Wire initialWire = null;

            switch (_connectionDirection)
            {
                case Direction.Forward:
                    initialWire = FindWire(Cell.x, Cell.y + 1);
                    break;

                case Direction.Backward:
                    initialWire = FindWire(Cell.x, Cell.y - 1);
                    break;

                case Direction.Left:
                    initialWire = FindWire(Cell.x - 1, Cell.y);
                    break;

                case Direction.Right:
                    initialWire = FindWire(Cell.x + 1, Cell.y);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (initialWire != null)
                return GetConsumers(initialWire, wires);

            return null;
        }

        public IPowerConsumer GetConnectedDevice()
        {
            TileObject[] objects = TileController.GetObjects(Cell.x, Cell.y);

            foreach (var obj in objects)
            {
                if(obj is IPowerConsumer)
                    return obj as IPowerConsumer;
            }

            return null;
        }

        private IPowerConsumer[] GetConsumers(Wire initialWire, List<Wire> wires)
        {
            List<Wire> connectedWires = initialWire.GetConnectedWires();
            connectedWires.Remove(initialWire);
            List<IPowerConsumer> result = new List<IPowerConsumer>();

            wires.Add(initialWire);
            foreach (var connectedWire in connectedWires)
            {
                if(wires.Contains(connectedWire))
                    continue;

                wires.Add(connectedWire);

                IPowerConsumer[] consumers = GetConsumers(connectedWire, wires);
                result.AddRange(consumers);
            }

            List<IPowerSpreader> connectors = initialWire.GetConnectors();
            foreach (var connector in connectors)
            {
                IPowerConsumer connectedDevice = connector.GetConnectedDevice();
                if(connectedDevice != null)
                    result.Add(connectedDevice);
            }

            return result.ToArray();
        }

        public override string ToMap()
        {
            string map = base.ToMap() + " " + (int) _connectionDirection;
            return map;
        }

        public override bool FromMap(string mapData)
        {
            if (!base.FromMap(mapData))
                return false;

            string[] units = mapData.Split(' ');

            try
            {
                int direction = Int32.Parse(units[4]);
                _connectionDirection = (Direction) direction;
            }
            catch (FormatException ex)
            {
                Debug.LogError(ex.Message);
                return false;
            }

            return true;
        }

    }
}

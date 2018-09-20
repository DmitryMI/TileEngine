using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Controllers;
using UnityEngine;

namespace Assets.Scripts.Objects.Equipment.Power
{
    class PowerSpreader : Equipment, IPowerSpreader
    {
        [SerializeField] private Direction _connectionDirection;

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
            get { return "LPC Connector"; }
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
                    //_spriteRenderer.sprite = _north;
                    transform.localRotation = Quaternion.Euler(0, 0, 90);
                    break;
                case Direction.Backward:
                    transform.localRotation = Quaternion.Euler(0, 0, -90);
                    break;
                case Direction.Left:
                    //_spriteRenderer.sprite = _west;
                    transform.localRotation = Quaternion.Euler(0, 0, 0);
                    break;
                case Direction.Right:
                    //_spriteRenderer.sprite = _east;
                    transform.localRotation = Quaternion.Euler(0, 0, 180);
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
                if (obj is Wire)
                    return obj as Wire;
            }

            return null;
        }

        /*public IPowerConsumer[] FindConsumers()
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
        */
        /*private IPowerConsumer[] GetConsumers(Wire initialWire, List<Wire> wires)
        {
            List<Wire> connectedWires = initialWire.GetConnectedWires();
            connectedWires.Remove(initialWire);
            List<IPowerConsumer> result = new List<IPowerConsumer>();

            wires.Add(initialWire);
            foreach (var connectedWire in connectedWires)
            {
                if (wires.Contains(connectedWire))
                    continue;

                wires.Add(connectedWire);

                IPowerConsumer[] consumers = GetConsumers(connectedWire, wires);
                result.AddRange(consumers);
            }

            List<IPowerSpreader> connectors = initialWire.GetConnectors();
            foreach (var connector in connectors)
            {
                IPowerConsumer connectedDevice = connector.GetConnectedDevice();
                if (connectedDevice != null)
                    result.Add(connectedDevice);
            }

            return result.ToArray();
        }
        */

        public IPowerConsumer GetConnectedDevice()
        {
            Vector2Int cell;
            switch (_connectionDirection)
            {
                case Direction.Forward:
                    cell = new Vector2Int(Cell.x, Cell.y + 1);
                    break;

                case Direction.Backward:
                    cell = new Vector2Int(Cell.x, Cell.y - 1);
                    break;

                case Direction.Left:
                    cell = new Vector2Int(Cell.x - 1, Cell.y);
                    break;

                case Direction.Right:
                    cell = new Vector2Int(Cell.x + 1, Cell.y);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }


            LocalPowerController lpc = TileController.Find<LocalPowerController>(cell.x, cell.y);
            return lpc;
        }
    }
}

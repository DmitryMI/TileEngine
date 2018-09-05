using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Objects.Equipment.Power
{
    class Wire : TileObject
    {
        [SerializeField] private Sprite _ns; //
        [SerializeField] private Sprite _we;
        [SerializeField] private Sprite _nsw;
        [SerializeField] private Sprite _nse;
        [SerializeField] private Sprite _nw;
        [SerializeField] private Sprite _ne;
        [SerializeField] private Sprite _sw;
        [SerializeField] private Sprite _se;
        [SerializeField] private Sprite _wen;
        [SerializeField] private Sprite _wes;
        [SerializeField] private Sprite _nesw;

        [SyncVar]
        [SerializeField] private bool _connectsNorth;
        [SyncVar]
        [SerializeField] private bool _connectsEast;
        [SyncVar]
        [SerializeField] private bool _connectsSouth;
        [SyncVar]
        [SerializeField] private bool _connectsWest;

        private SpriteRenderer _spriteRenderer;

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
            get { return "Mounted wire"; }
        }

        protected override void Start()
        {
            base.Start();

            _spriteRenderer = GetComponent<SpriteRenderer>();

            UpdateSprite();
        }

        protected override void Update()
        {
            base.Update();
            UpdateSprite();
        }

        private void UpdateSprite()
        {
            if (_connectsNorth && _connectsSouth && _connectsEast && _connectsWest)
            {
                _spriteRenderer.sprite = _nesw;
            }
            else if (_connectsWest && _connectsEast && _connectsSouth)
            {
                _spriteRenderer.sprite = _wes;
            }
            else if (_connectsNorth && _connectsSouth && _connectsEast)
            {
                _spriteRenderer.sprite = _nse;
            }
            else if (_connectsNorth && _connectsSouth && _connectsWest)
            {
                _spriteRenderer.sprite = _nsw;
            }
            else if (_connectsNorth && _connectsWest && _connectsEast)
            {
                _spriteRenderer.sprite = _wen;
            }
            else if (_connectsNorth && _connectsSouth)
            {
                _spriteRenderer.sprite = _ns;
            }
            else if (_connectsNorth && _connectsWest)
            {
                _spriteRenderer.sprite = _nw;
            }
            else if (_connectsNorth && _connectsEast)
            {
                _spriteRenderer.sprite = _ne;
            }
            else if (_connectsWest && _connectsEast)
            {
                _spriteRenderer.sprite = _we;
            }
            else if (_connectsWest && _connectsSouth)
            {
                _spriteRenderer.sprite = _sw;
            }
            else if (_connectsEast && _connectsSouth)
            {
                _spriteRenderer.sprite = _se;
            }


        }

    public List<Wire> GetConnectedWires()
        {
            List<Wire> wires = new List<Wire>();
            if (_connectsNorth)
            {
                Wire wire = TileController.Find<Wire>(Cell.x, Cell.y + 1);
                if(wire)
                    wires.Add(wire);
            }
            if (_connectsSouth)
            {
                Wire wire = TileController.Find<Wire>(Cell.x, Cell.y - 1);
                if (wire)
                    wires.Add(wire);
            }
            if (_connectsWest)
            {
                Wire wire = TileController.Find<Wire>(Cell.x - 1, Cell.y);
                if (wire)
                    wires.Add(wire);
            }
            if (_connectsEast)
            {
                Wire wire = TileController.Find<Wire>(Cell.x + 1, Cell.y);
                if (wire)
                    wires.Add(wire);
            }
            return wires;
        }

        public List<WireConnector> GetConnectors()
        {
            List<WireConnector> connectors = new List<WireConnector>();
            if (_connectsNorth)
            {
                WireConnector connector = TileController.Find<WireConnector>(Cell.x, Cell.y + 1);
                if (connector)
                    connectors.Add(connector);
            }
            if (_connectsSouth)
            {
                WireConnector connector = TileController.Find<WireConnector>(Cell.x, Cell.y - 1);
                if (connector)
                    connectors.Add(connector);
            }
            if (_connectsWest)
            {
                WireConnector connector = TileController.Find<WireConnector>(Cell.x - 1, Cell.y);
                if (connector)
                    connectors.Add(connector);
            }
            if (_connectsEast)
            {
                WireConnector connector = TileController.Find<WireConnector>(Cell.x + 1, Cell.y);
                if (connector)
                    connectors.Add(connector);
            }
            return connectors;
        }

        public override string ToMap()
        {
            string map = base.ToMap() + " " + _connectsNorth + " " + _connectsEast + " " + _connectsSouth + " " + _connectsWest;
            return map;
        }

        public override bool FromMap(string mapData)
        {
            if (!base.FromMap(mapData))
                return false;

            string[] units = mapData.Split(' ');

            try
            {
                bool north = Boolean.Parse(units[4]);
                bool east = Boolean.Parse(units[5]);
                bool south = Boolean.Parse(units[6]);
                bool west = Boolean.Parse(units[7]);

                _connectsNorth = north;
                _connectsSouth = south;
                _connectsEast = east;
                _connectsWest = west;

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

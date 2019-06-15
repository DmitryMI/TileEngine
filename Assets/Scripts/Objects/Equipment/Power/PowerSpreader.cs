using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Controllers;
using Assets.Scripts.GameMechanics;
using UnityEngine;

namespace Assets.Scripts.Objects.Equipment.Power
{
    class PowerSpreader : Equipment, IPowerSpreader
    {
        [SerializeField] private Direction _connectionDirection;

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

        public IPowerConsumer GetConnectedDevice()
        {
            Vector2Int cell = Cell;

            LocalPowerController lpc = TileController.Find<LocalPowerController>(cell.x, cell.y);

            if(CheckRotation(lpc))
                return lpc;

            return null;
        }

        private bool CheckRotation(LocalPowerController lpc)
        {
            return lpc?.WallPressDirection == _connectionDirection;
        }
    }
}

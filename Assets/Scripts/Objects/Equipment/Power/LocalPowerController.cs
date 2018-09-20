using System;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Objects.Equipment.Power
{
    public class LocalPowerController : Equipment, IPowerConsumer
    {
        [SerializeField] [SyncVar] private Direction _wallPressDirection;
        [SerializeField] private float _pressedOffset;

        [SerializeField] private SpriteRenderer _screenSpriteRenderer;


        public Direction WallPressDirection => _wallPressDirection;

        protected override bool Transperent => true;
        protected override bool PassesGas => true;
        protected override bool CanWalkThrough => true;

        private Vector2 _pressedCellOffset;

        [SerializeField]
        private float _powerStored = 500f;

        [SerializeField]
        private float _powerCapacity = 1000f;

        [SyncVar] private bool _charging;
        

        protected override void Update()
        {
            base.Update();

            if(isServer && ServerController.Ready)
                SetOffset();

            CellOffset = _pressedCellOffset;

            if (_charging)
            {
                _screenSpriteRenderer.color = Color.green;
            }
            else
            {
                _screenSpriteRenderer.color = Color.red;
            }
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

            _pressedCellOffset = cellOffset;
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
    }
}

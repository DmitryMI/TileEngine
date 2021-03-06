﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.GameMechanics;
using Assets.Scripts.Objects.Equipment.Power;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Objects.Equipment.Lighting
{
    class LightFixture : Equipment, IWallPlaceable, IWirelessPowerable
    {
        [SerializeField] private Sprite _onSprite;
        [SerializeField] private Sprite _offSprite;

        [SerializeField] [SyncVar] private Direction _wallPressDirection;
        [SerializeField] private float _pressedOffset;

        [SerializeField] private float _lightRange;

        public Direction WallPressDirection { get { return _wallPressDirection; } }

        [SyncVar]
        private bool _electrified;

        private int _prevElectroFrame;

        private SpriteRenderer _spriteRenderer;
        private LightSourceInfo _lightSourceInfo;
        private int _lightSourceId;

        // TODO Has light tube


        protected override void Start()
        {
            base.Start();

            _spriteRenderer = GetComponent<SpriteRenderer>();
            _lightSourceInfo = new LightSourceInfo(PositionProvider, Color.white, _lightRange, 1, 0.05f);

            VisionController.SetLightContinuous(_lightSourceInfo);
        }

        public void OnDestroy()
        {
            VisionController?.RemoveLightById(_lightSourceId);
        }

        protected override void Update()
        {
            base.Update();

            if(isServer)
                SetOffset();

            if(isServer)
                UpdateElectro();

            UpdateSprite();

            
        }

        protected void LateUpdate()
        {
            UpdateLightController(_electrified);
        }

        private void UpdateSprite()
        {
            switch (_wallPressDirection)
            {
                case Direction.Forward:
                    //_spriteRenderer.sprite = _north;
                    transform.localRotation = Quaternion.Euler(0, 0, 0);
                    break;
                case Direction.Backward:
                    transform.localRotation = Quaternion.Euler(0, 0, 0);
                    break;
                case Direction.Left:
                    //_spriteRenderer.sprite = _west;
                    transform.localRotation = Quaternion.Euler(0, 0, 90);
                    break;
                case Direction.Right:
                    //_spriteRenderer.sprite = _east;
                    transform.localRotation = Quaternion.Euler(0, 0, -90);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _spriteRenderer.sprite = _electrified ? _onSprite : _offSprite;
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

        public float PowerNeeded
        {
            // TODO Check if has light tube
            get { return 0.001f; }
        }

        public void Electrify()
        {
            _electrified = true;

            _prevElectroFrame = Time.frameCount;
        }

        public PowerablePriority Priority => PowerablePriority.Lighting;

        [Server]
        private void UpdateElectro()
        {
            if (Time.frameCount - _prevElectroFrame > 2)
            {
                _electrified = false;
            }
        }

        private void UpdateLightController(bool isOn)
        {
            
            if (isOn)
                _lightSourceInfo.Range = 0;
            else
                _lightSourceInfo.Range = _lightRange;
        }

        public override string DescriptiveName => "Light fixture";

        protected override bool PassesGas => true;
        protected override bool Transparent => true;
        protected override bool CanWalkThrough => true;
    }
}

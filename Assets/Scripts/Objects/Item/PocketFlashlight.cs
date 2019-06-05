using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Controllers;
using Assets.Scripts.GameMechanics;
using Assets.Scripts.Objects.Mob;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Objects.Item
{
    class PocketFlashlight : Item, ILightInfo
    {
        [SerializeField]
        private float _range;
        [SerializeField]
        private Color _color;

        [SerializeField] [SyncVar] private float _initialIntensity = 1f;

        [SerializeField] [SyncVar] private float _intensityDecrement = 0.05f;



        [SerializeField] private Sprite _onSprite;
        [SerializeField] private Sprite _offSprite;

        [SyncVar]
        [SerializeField] private bool _isOn;

        private SpriteRenderer _renderer;
        private int _lightSourceId;
        private bool _prevState;

        protected override void Start()
        {
            base.Start();

            _renderer = GetComponent<SpriteRenderer>();
            _prevState = _isOn;

            //_lightSourceInfo = new LightSourceInfo(PositionProvider, Color.white, _range, );

            _lightSourceId = VisionController.SetLightContinuous(this);
            OnLightingStateChange(_isOn);
        }

        protected override void Update()
        {
            base.Update();

            if (_isOn != _prevState)
            {
                OnLightingStateChange(_isOn);
            }

            _prevState = _isOn;
        }
        
        private void OnLightingStateChange(bool lightEnabled)
        {
            if (lightEnabled)
            {
                _renderer.sprite = _onSprite;
            }
            else
            {
                _renderer.sprite = _offSprite;
            }
        }

        private void OnDestroy()
        {
            VisionController?.RemoveLightById(_lightSourceId);
        }

        public override void ApplyItemServer(Item item)
        {
            if (item == this)
            {
                _isOn = !_isOn;
                OnLightingStateChange(_isOn);
            }
        }

        public override void ApplyItemClient(Item item)
        {
            Humanoid local = PlayerActionController.Current.LocalPlayerMob as Humanoid;
            if (item == null)
            {
                if(ItemHolder == null)
                    local.PickItem(this, PlayerActionController.Current.ActiveHand);
            }
            else
            {
                local.ApplyItem(this, item);
            }
        }

        public new ICellPositionProvider PositionProvider => base.PositionProvider;
        public Color LightColor => _color;

        public float Range
        {
            get
            {
                if (_isOn)
                    return _range;
                return 0;
            }
        }
        public float Decrement => _intensityDecrement;
        public float InitialIntensity => _initialIntensity;
    }
}

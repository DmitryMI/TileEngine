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
    class PocketFlashlight : Item
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

        protected override void Start()
        {
            base.Start();

            _renderer = GetComponent<SpriteRenderer>();
        }

        protected override void Update()
        {
            base.Update();

            UpdateLightController();
        }

        private void UpdateLightController()
        {
            if (_isOn)
            {
                LightSourceInfo info;

                if (Holder == null)
                {
                    info = new LightSourceInfo(Cell.x, Cell.y, _color, _range, _initialIntensity, _intensityDecrement);
                }
                else
                {
                    TileObject holdingTo = Holder.GetComponent<TileObject>();
                    info = new LightSourceInfo(holdingTo.Cell.x, holdingTo.Cell.y, _color, _range, _initialIntensity, _intensityDecrement);
                }

                VisionController.SetLightSource(info);

                _renderer.sprite = _onSprite;
            }
            else
                _renderer.sprite = _offSprite;
        }

        public override void ApplyItemServer(Item item)
        {
            if (item == this)
            {
                _isOn = !_isOn;
            }
        }

        public override void ApplyItemClient(Item item)
        {
            Player local = PlayerActionController.Current.LocalPlayer;
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
    }
}

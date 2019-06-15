using System;
using Assets.Scripts.Controllers;
using Assets.Scripts.GameMechanics;
using Assets.Scripts.Objects.Item;
using Assets.Scripts.Objects.Mob.Humanoids;
using UnityEngine;
using UnityEngine.Networking;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Objects.Turf
{
    public class MetalFloor : Turf, IPlayerApplicable
    {
        [SerializeField] private Sprite[] _variationList;

        [SerializeField] private int _variation;
        [SerializeField] private bool _randomizeVariation;

        private SpriteRenderer _spriteRenderer;

        protected override void Start()
        {
            base.Start();

            _spriteRenderer = GetComponent<SpriteRenderer>();

            if (_randomizeVariation)
            {
                _variation = Random.Range(0, _variationList.Length);
            }
        }

        protected override void Update()
        {
            base.Update();

            if (_variationList.Length != 0 && _variation >= 0 && _variation < _variationList.Length)
                _spriteRenderer.sprite = _variationList[_variation];
        }

        public override string ToMap()
        {
            return base.ToMap() + " " + _variation;
        }

        public override string DescriptiveName
        {
            get { return "Metal floor"; }
        }

        protected override bool Transparent
        {
            get { return true; }
        }

        protected override bool PassesGas
        {
            get { return true; }
        }

        protected override bool CanWalkThrough
        {
            get { return true; }
        }

        public override bool FromMap(string mapData)
        {
            bool ok = base.FromMap(mapData);

            string[] units = mapData.Split(' ');

            try
            {
                _variation = Int32.Parse(units[4]);
            }
            catch (FormatException ex)
            {
                Debug.Log(ex.Message);
                ok = false;
            }

            return ok;
        }

        public void ApplyItemClient(Item.Item item, Intent intent)
        {
            bool handled = false;
            if (item is IApplicationHandler handler)
            {
                handled = handler.OnApplicationClient(this, intent);
            }

            if (!handled)
            {
                (PlayerActionController.Current.LocalPlayerMob as Humanoid)?.ApplyItem(item, this, intent);
            }
        }

        public void ApplyItemServer(Item.Item item, Intent intent)
        {
            // Check if item has any custom behaviour
            bool handled = false;
            if (item is IApplicationHandler handler)
            {
                handled = handler.OnApplicationServer(this, intent);
            }

            if (!handled)
                // We need to process application by ourselves
                if (item is Crowbar)
                {
                    NetworkServer.Destroy(this.gameObject);
                    Destroy(this.gameObject);
                }
        }
    }
}


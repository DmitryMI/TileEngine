using System.Collections.Generic;
using Assets.Scripts.Controllers;
using Assets.Scripts.GameMechanics;
using Assets.Scripts.HumanAppearance;
using Assets.Scripts.Objects.Mob;
using Assets.Scripts.Objects.Mob.Humanoids;
using Assets.Scripts.Ui;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Objects.Item
{
    [RequireComponent(typeof(Collider2D))]
    public class Item : TileObject, IPlayerApplicable, IDisplayable
    {
        [SerializeField] private string _descriptiveName;

        [SerializeField]
        protected TileObject ItemHolder;
        

        protected SpriteRenderer Renderer;
        protected Collider2D Collider;

        

        

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

        protected override void Update()
        {
            base.Update();

            UpdateState();
            
        }

        protected override void Sync()
        {
            base.Sync();

            if (isServer)
            {
                RpcReceiveState(ItemHolder != null ? ItemHolder.gameObject : null);
            }
        }

        public override void ForceSync()
        {
            base.ForceSync();
            Sync();
        }

        [ClientRpc]
        private void RpcReceiveState(GameObject container)
        {
            if(!isServer)
                ItemHolder = container.GetComponent<TileObject>();
        }

        private void UpdateState()
        {
            if (ItemHolder != null)
            {
                transform.parent = ItemHolder.transform;
                transform.localPosition = Vector3.zero;
                
                if(Renderer)
                    Renderer.enabled = false;
                if(Collider)
                    Collider.enabled = false;
            }
            else
            {
                transform.parent = FindObjectOfType<Grid>().transform;

                if (Renderer)
                    Renderer.enabled = true;
                if (Collider)
                    Collider.enabled = true;
            }
        }

        

        public override string DescriptiveName
        {
            get { return _descriptiveName; }
        }

        public GameObject Holder
        {
            get { return ItemHolder?.gameObject; }
            set { ItemHolder = value?.GetComponent<TileObject>(); }
        }

        
        protected override void Start()
        {
            base.Start();

            PositionProvider = new ItemPositionProvider(this);

            Renderer = GetComponent<SpriteRenderer>();
            Collider = GetComponent<Collider2D>();
        }

        public virtual Sprite GetUiSprite()
        {
            return Renderer.sprite; // TODO Fix stupid variant
        }

        public virtual void ApplyItemClient(Item item, Intent intent)
        {
            Humanoid humanoid = PlayerActionController.Current.LocalPlayerMob as Humanoid;
            if (ItemHolder == null && item == null && humanoid != null)
                humanoid.PickItem(this, PlayerActionController.Current.ActiveHand);            
        }

        public virtual void ApplyItemServer(Item item, Intent intent)
        {
            if(item != null)
                Debug.Log("Item " + item.DescriptiveName + " was used on item " + _descriptiveName);
            else
                Debug.Log("Empty hand was used on item " + _descriptiveName);
        }

        public override bool IsNeighbour(TileObject other)
        {
            Vector2Int cellA;
            if (ItemHolder == null)
                cellA = Cell;
            else
                cellA = Holder.GetComponent<TileObject>().Cell;

            Vector2Int cellB = other.Cell;

            int dx = Mathf.Abs(cellA.x - cellB.x);
            int dy = Mathf.Abs(cellA.y - cellB.y);

            return dx <= 1 && dy <= 1;
        }

        public static bool CanBePlaced(Item item, SlotEnum slot)
        {
            // Empty space can be placed anywhere :)
            if (item == null)
                return true;

            if (slot == SlotEnum.LeftHand || slot == SlotEnum.RightHand)
                return true;

            var wearable = item as IWearable;
            return wearable?.AppropriateSlot == slot;
        }

        protected class ItemPositionProvider : ICellPositionProvider
        {
            private Item _item;

            public ItemPositionProvider(Item item)
            {
                _item = item;
            }

            private int GetX()
            {
                if (_item.Holder == null)
                    return _item.Cell.x;
                return _item.Holder.GetComponent<TileObject>().Cell.x;
            }

            private int GetY()
            {
                if (_item.Holder == null)
                    return _item.Cell.y;
                return _item.Holder.GetComponent<TileObject>().Cell.y;
            }

            public int X => GetX();
            public int Y => GetY();
        }
    }
}

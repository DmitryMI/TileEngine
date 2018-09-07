using System.Collections.Generic;
using Assets.Scripts.Controllers;
using Assets.Scripts.HumanAppearance;
using Assets.Scripts.Ui;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Objects.Item
{
    [RequireComponent(typeof(Collider2D))]
    public class Item : TileObject, IPlayerInteractable, IDisplayable
    {
        [SerializeField] private string _descriptiveName;

        [SerializeField]
        protected GameObject ItemHolder;

        [SerializeField] private int _layersNeeded = 1;
        [SerializeField] private int _sortingOrder = 0;

        protected SpriteRenderer Renderer;
        protected Collider2D Collider;

        public SpriteRenderer SpriteRenderer => Renderer;
        public virtual int LayersNeeded => _layersNeeded;

        public int SortingOrder => _sortingOrder;

        [SyncVar]
        private int _spriteRendererSortingOrder;

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

        protected override void Update()
        {
            base.Update();

            UpdateState();
            UpdateSpriteRenderer();
        }

        protected override void Sync()
        {
            base.Sync();

            if (isServer)
            {
                RpcReceiveState(ItemHolder);
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
                ItemHolder = container;
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

        private void UpdateSpriteRenderer()
        {
            if (isServer)
            {
                _spriteRendererSortingOrder = SpriteRenderer.sortingOrder;
            }
            else
            {
                SpriteRenderer.sortingOrder = _spriteRendererSortingOrder;
            }
        }

        public override string DescriptiveName
        {
            get { return _descriptiveName; }
        }

        public GameObject Holder
        {
            get { return ItemHolder; }
            set { ItemHolder = value; }
        }

        
        protected override void Start()
        {
            base.Start();

            Renderer = GetComponent<SpriteRenderer>();
            Collider = GetComponent<Collider2D>();
        }

        public virtual Sprite GetUiSprite()
        {
            return Renderer.sprite; // TODO Fix stupid variant
        }

        public virtual void ApplyItemClient(Item item)
        {
            if(ItemHolder == null && item == null)
                PlayerActionController.Current.LocalPlayer.PickItem(this, PlayerActionController.Current.ActiveHand);            
        }

        public virtual void ApplyItemServer(Item item)
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
    }
}

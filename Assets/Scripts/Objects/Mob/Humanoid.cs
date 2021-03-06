﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.GameMechanics;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Objects.Mob
{
    public abstract class Humanoid : Mob, IItemContainer
    {

        [SerializeField] [SyncVar] protected int HumanoidHairSetId;
        

        [SerializeField]
        [SyncVar]
        protected GameObject LeftHandItem;

        [SerializeField]
        [SyncVar]
        protected GameObject RightHandItem;

        [SerializeField]
        [SyncVar]
        protected GameObject CostumeItem;

        public int HairSetId => HumanoidHairSetId;

        protected override bool Transparent => true;

        protected override bool CanWalkThrough => true;

        protected override bool PassesGas => true;

        public bool BlocksLightFromInside => false;

        protected override void UpdateSprite()
        {
            base.UpdateSprite();

            if (IsLying)
            {
                transform.localRotation = Quaternion.Euler(0, 0, -90);
                Rotation = Direction.Forward;
            }
            else
            {
                transform.localRotation = Quaternion.identity;
            }
        }

        public Direction SpriteOrientation
        {
            get
            {
                if (!IsMobLying) return Rotation;
                else return Direction.Backward;
            }
        }

        //public override bool IsLying => IsMobLying;

        
        public Item.Item GetItemBySlot(SlotEnum slot)
        {
            Item.Item item = null;
            switch (slot)
            {
                case SlotEnum.LeftHand:

                    if (LeftHandItem != null)
                        item = LeftHandItem.GetComponent<Item.Item>();
                    break;

                case SlotEnum.RightHand:

                    if (RightHandItem != null)
                        item = RightHandItem.GetComponent<Item.Item>();
                    break;

                case SlotEnum.Back:
                    throw new NotImplementedException();
                case SlotEnum.Belt:
                    throw new NotImplementedException();
                case SlotEnum.Costume:
                    if (CostumeItem != null)
                        item = CostumeItem.GetComponent<Item.Item>();
                    break;
                case SlotEnum.Hardsuit:
                    break;
                case SlotEnum.Gloves:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(slot), slot, null);
            }

            return item;
        }

        [Server]
        private void SetItemBySlot(Item.Item item, SlotEnum slot)
        {
            GameObject itemGo = null;
            if (item != null)
                itemGo = item.gameObject;

            switch (slot)
            {
                case SlotEnum.LeftHand:
                    LeftHandItem = itemGo;
                    break;

                case SlotEnum.RightHand:
                    RightHandItem = itemGo;
                    break;

                case SlotEnum.Back:
                    throw new NotImplementedException();
                case SlotEnum.Belt:
                    throw new NotImplementedException();

                case SlotEnum.Costume:
                    CostumeItem = itemGo;
                    break;

                case SlotEnum.Hardsuit:
                    break;
                case SlotEnum.Gloves:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(slot), slot, null);
            }
        }

        // Interaction section
        public void PickItem(Item.Item item, SlotEnum slot)
        {
            CmdPickItem(item.gameObject, slot);
        }

        public void DropItem(SlotEnum slot, Vector2Int cell, Vector2 offset)
        {
            CmdDropItem(slot, cell.x, cell.y, offset);
        }

        // TODO Throw item

        public void MoveItem(SlotEnum source, SlotEnum destination)
        {
            CmdExchangeItem(source, destination);
        }

        public void ApplyItem(SlotEnum activeHand, IPlayerInteractable interactable)
        {
            CmdApplyItemSlot(activeHand, interactable.gameObject);
        }

        public void ApplyItem(Item.Item sourceItem, IPlayerInteractable interactable)
        {
            GameObject sourceGo = sourceItem?.gameObject;
            CmdApplyItem(sourceGo, interactable.gameObject);
        }

        // TODO Act on equipment

        // Server side of interaction

        [Command]
        private void CmdExchangeItem(SlotEnum source, SlotEnum dest)
        {
            Item.Item sourceItem = GetItemBySlot(source);
            Item.Item destItem = GetItemBySlot(dest);

            if (Item.Item.CanBePlaced(sourceItem, dest) && Item.Item.CanBePlaced(destItem, source))
            {

                SetItemBySlot(destItem, source);
                SetItemBySlot(sourceItem, dest);
            }
        }

        [Command]
        private void CmdApplyItemSlot(SlotEnum activeHand, GameObject interactableGo)
        {
            Item.Item activeItem = GetItemBySlot(activeHand);
            interactableGo.GetComponent<IPlayerInteractable>().ApplyItemServer(activeItem);
        }

        [Command]
        private void CmdApplyItem(GameObject sourceItemGo, GameObject interactableGo)
        {
            Item.Item sourceItem = sourceItemGo?.GetComponent<Item.Item>();
            IPlayerInteractable interactable = interactableGo.GetComponent<IPlayerInteractable>();

            interactable?.ApplyItemServer(sourceItem);
        }

        [Command]
        private void CmdPickItem(GameObject itemObject, SlotEnum slot)
        {
            Item.Item item = itemObject.GetComponent<Item.Item>();

            switch (slot)
            {
                case SlotEnum.LeftHand:

                    LeftHandItem = itemObject;
                    item.Holder = gameObject;
                    break;

                case SlotEnum.RightHand:

                    RightHandItem = itemObject;
                    item.Holder = gameObject;
                    break;

                case SlotEnum.Back:
                    break;
                case SlotEnum.Belt:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(slot), slot, null);
            }
        }

        [Command]
        private void CmdDropItem(SlotEnum slot, int cellX, int cellY, Vector2 position)
        {
            Item.Item item = null;

            switch (slot)
            {
                case SlotEnum.LeftHand:
                    if (LeftHandItem != null)
                        item = LeftHandItem.GetComponent<Item.Item>();
                    LeftHandItem = null;
                    break;
                case SlotEnum.RightHand:
                    if (RightHandItem != null)
                        item = RightHandItem.GetComponent<Item.Item>();
                    RightHandItem = null;
                    break;
                case SlotEnum.Back:
                    break;
                case SlotEnum.Belt:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(slot), slot, null);
            }

            if (item)
            {
                item.Holder = null;
                item.Cell = new Vector2Int(cellX, cellY);
                item.CellOffset = position;
            }
        }


        public void SendDataToServer(INetworkDataReceiver sender, INetworkDataReceiver receiver, byte[] data)
        {
            CmdSendByteArray(sender.gameObject, receiver.gameObject, data);
        }

        [Command]
        private void CmdSendByteArray(GameObject senderGo, GameObject receiverGo, byte[] data)
        {
            INetworkDataReceiver sender = senderGo.GetComponent<INetworkDataReceiver>();
            INetworkDataReceiver receiver = receiverGo.GetComponent<INetworkDataReceiver>();
            receiver.ReceiveData(sender, data);
        }


        [Obsolete("Consider removing this property. What's it's purpose?")]
        public bool Spawned { get; set; }
    }
}

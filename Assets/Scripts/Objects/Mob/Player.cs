using System;
using Assets.Scripts.Controllers;
using Assets.Scripts.HumanAppearance;
using UnityEngine;
using UnityEngine.Networking;
using Assets.Scripts.Objects.Item;

namespace Assets.Scripts.Objects.Mob
{
    public class Player : Mob, IItemContainer
    {
        [SerializeField]
        private float _moveSpeed;

        [SerializeField] [SyncVar] private int _hairSetId;

        private bool _transperent;
        private bool _canWalkThrough;
        private bool _canContainGas;

        private string _descriptiveName = "Unnamed human";

        

        // Carried Items
        [SerializeField] [SyncVar]
        private GameObject _leftHandItem;

        [SerializeField] [SyncVar]
        private GameObject _rightHandItem;

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
            get { return _descriptiveName; }
        }

        protected override void Update()
        {
            base.Update();

            if (isLocalPlayer)
            {
                UpdateCamera();
                ProcessPlayerControll();
            }
        }

        private void UpdateCamera()
        {
            //VisionController.SetCameraPosition(Cell, CellOffset);
            VisionController.SetCameraPosition(transform.position);
        }

        private void ProcessPlayerControll()
        {
            float vertical = Input.GetAxisRaw("Vertical");
            float horizontal = Input.GetAxisRaw("Horizontal");

            if (vertical > 0)
            {
                DoMove(Direction.Forward, _moveSpeed);
            }
            if (vertical < 0)
            {
                DoMove(Direction.Backward, _moveSpeed);
            }
            if (horizontal > 0)
            {
                DoMove(Direction.Right, _moveSpeed);
            }
            if (horizontal < 0)
            {
                DoMove(Direction.Left, _moveSpeed);
            }
        }
        public Item.Item GetItemBySlot(SlotEnum slot)
        {
            Item.Item item = null;
            switch (slot)
            {
                case SlotEnum.LeftHand:

                    if (_leftHandItem != null)
                        item = _leftHandItem.GetComponent<Item.Item>();
                    break;

                case SlotEnum.RightHand:

                    if(_rightHandItem != null)
                        item = _rightHandItem.GetComponent<Item.Item>();
                    break;

                case SlotEnum.Back:
                    throw new NotImplementedException();
                case SlotEnum.Belt:
                    throw new NotImplementedException();
                default:
                    throw new ArgumentOutOfRangeException("slot", slot, null);
            }

            return item;
        }

        public int HairSetId
        {
            get { return _hairSetId; }
        }

        public Direction SpriteOrientation
        {
            get { return Rotation; }
        }

        public bool BlocksLightFromInside()
        {
            return false;
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
            throw new NotImplementedException();
        }

        public void ApplyItem(SlotEnum activeHand, IPlayerInteractable interactable)
        {
            CmdApplyItemSlot(activeHand, interactable.gameObject);
        }

        public void ApplyItem(Item.Item sourceItem, IPlayerInteractable interactable)
        {
            CmdApplyItem(sourceItem.gameObject, interactable.gameObject);
        }

        // TODO Act on equipment

        // Server side of interaction

        [Command]
        private void CmdApplyItemSlot(SlotEnum activeHand, GameObject interactableGo)
        {
            Item.Item activeItem = GetItemBySlot(activeHand);
            interactableGo.GetComponent<IPlayerInteractable>().ApplyItemServer(activeItem);
        }

        [Command]
        private void CmdApplyItem(GameObject sourceItemGo, GameObject interactableGo)
        {
            Item.Item sourceItem = sourceItemGo.GetComponent<Item.Item>();
            IPlayerInteractable interactable = interactableGo.GetComponent<IPlayerInteractable>();

            if (interactable != null)
            {
                interactable.ApplyItemServer(sourceItem);
            }
        }

        [Command]
        private void CmdPickItem(GameObject itemObject, SlotEnum slot)
        {
            Item.Item item = itemObject.GetComponent<Item.Item>();

            switch (slot)
            {
                case SlotEnum.LeftHand:

                    _leftHandItem = itemObject;
                    item.Holder = gameObject;
                    break;

                case SlotEnum.RightHand:

                    _rightHandItem = itemObject;
                    item.Holder = gameObject;
                    break;

                case SlotEnum.Back:
                    break;
                case SlotEnum.Belt:
                    break;
                default:
                    throw new ArgumentOutOfRangeException("slot", slot, null);
            }
        }

        [Command]
        private void CmdDropItem(SlotEnum slot, int cellX, int cellY, Vector2 position)
        {
            Item.Item item = null;
        
            switch (slot)
            {
                case SlotEnum.LeftHand:
                    if(_leftHandItem != null)
                        item = _leftHandItem.GetComponent<Item.Item>();
                    _leftHandItem = null;
                    break;
                case SlotEnum.RightHand:
                    if (_rightHandItem != null)
                        item = _rightHandItem.GetComponent<Item.Item>();
                    _rightHandItem = null;
                    break;
                case SlotEnum.Back:
                    break;
                case SlotEnum.Belt:
                    break;
                default:
                    throw new ArgumentOutOfRangeException("slot", slot, null);
            }

            if (item)
            {
                item.Holder = null;
                item.Cell = new Vector2Int(cellX, cellY);
                item.CellOffset = position;
            }
        }

        #region ObjectsCommunication

        public void SendDataToServer(IReceiver sender, IReceiver receiver, byte[] data)
        {
            CmdSendByteArray(sender.gameObject, receiver.gameObject, data);
        }

        [Command]
        private void CmdSendByteArray(GameObject senderGo, GameObject receiverGo, byte[] data)
        {
            IReceiver sender = senderGo.GetComponent<IReceiver>();
            IReceiver receiver = receiverGo.GetComponent<IReceiver>();
            receiver.ReceiveData(sender, data);
        }

        #endregion
    }
}

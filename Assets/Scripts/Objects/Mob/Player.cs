using System;
using Assets.Scripts.Controllers;
using Assets.Scripts.HumanAppearance;
using UnityEngine;
using UnityEngine.Networking;
using Assets.Scripts.Objects.Item;

namespace Assets.Scripts.Objects.Mob
{
    public class Player : Mob, IItemContainer, IHumanoid
    {
        [SerializeField]
        private float _moveSpeed;

        [SerializeField] [SyncVar] private int _hairSetId;
        [SerializeField] [SyncVar] private bool _isLying = false;
        [SerializeField] [SyncVar] private HumanHealthData _healthData;


        private bool _transperent;
        private bool _canWalkThrough;
        private bool _canContainGas;


        private string _descriptiveName = "Unnamed human";
        private bool _spawned;

        // Carried Items
        [SerializeField] [SyncVar]
        private GameObject _leftHandItem;

        [SerializeField] [SyncVar]
        private GameObject _rightHandItem;

        [SerializeField]
        [SyncVar]
        private GameObject _costumeItem;


        protected override bool Transperent => true;

        protected override bool CanWalkThrough => true;

        protected override bool PassesGas => true;

        public override string DescriptiveName => _descriptiveName;

        public override bool IsLying => _isLying;

        protected override void Start()
        {
            base.Start();

            if(isServer)
                _healthData = new HumanHealthData();
        }


        protected override void Update()
        {
            base.Update();

            if (isLocalPlayer)
            {
                UpdateCamera();
                ProcessPlayerControll();
            }

            if (isServer)
            {
                UpdateHealth();
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

        public bool Spawned
        {
            get { return _spawned; }
            set { _spawned = value; }
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
                case SlotEnum.Costume:
                    if(_costumeItem != null)
                        item = _costumeItem.GetComponent<Item.Item>();
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
                    _leftHandItem = itemGo;
                    break;

                case SlotEnum.RightHand:
                    _rightHandItem = itemGo;
                    break;

                case SlotEnum.Back:
                    throw new NotImplementedException();
                case SlotEnum.Belt:
                    throw new NotImplementedException();

                case SlotEnum.Costume:
                    _costumeItem = itemGo;
                    break;

                case SlotEnum.Hardsuit:
                    break;
                case SlotEnum.Gloves:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(slot), slot, null);
            }
        }

        public int HairSetId => _hairSetId;

        public Direction SpriteOrientation
        {
            get
            {
                if (!_isLying) return Rotation;
                else return Direction.Backward;
            }
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
            CmdExchangeItem(source, destination);
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
            Item.Item sourceItem = sourceItemGo.GetComponent<Item.Item>();
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
                    throw new ArgumentOutOfRangeException(nameof(slot), slot, null);
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

        #region HealthSystem
        protected override void UpdateSprite()
        {
            base.UpdateSprite();

            if (_isLying)
            {
                transform.localRotation = Quaternion.Euler(0, 0, -90);
                Rotation = Direction.Forward;
            }
            else
            {
                transform.localRotation = Quaternion.identity;
            }
        }

        public HumanHealthData GetHealthDataCopy()
        {
            return _healthData;
        }

        public void SetHealthData(HumanHealthData data)
        {
            _healthData = data;
        }
        

        [Server]
        private void UpdateHealth()
        {
            Damage totalDamage = _healthData.TotalDamage;
            float health = 100 - totalDamage.Summ;

            Debug.Log("Health: " + health);

            if (health > 0)
            {
                _healthData.IsInCrit = false;
            }
            else if (health > -200f)
            {
                // TODO Critical state
                _healthData.IsInCrit = true;
            }
            else
            {
                _healthData.IsAlive = false;
            }

            if (_healthData.IsInCrit || _healthData.IsDead)
                _isLying = true;
            else
                _isLying = false;
            
            
        }
        #endregion
    }
}

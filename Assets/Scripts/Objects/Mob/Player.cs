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

        private Damage _totalDamage;

        [SerializeField][SyncVar] private bool _isInCrit = false;
        [SerializeField][SyncVar] private bool _isAlive = true;

        /*[SerializeField] private float _maxHeadDamage;
        [SerializeField] private float _maxNeckDamage;
        [SerializeField] private float _maxChestDamage;
        [SerializeField] private float _maxGroinDamage;
        [SerializeField] private float _maxArmDamage;
        [SerializeField] private float _maxWristDamage;
        [SerializeField] private float _maxLegDamage;
        [SerializeField] private float _maxFootDamage;*/ // TODO Implement damage thresold

        [SyncVar] private Damage _headDamage;
        [SyncVar] private Damage _neckDamage;
        [SyncVar] private Damage _chestDamage;
        [SyncVar] private Damage _groinDamage;
        [SyncVar] private Damage _leftArmDamage;
        [SyncVar] private Damage _rightArmDamage;
        [SyncVar] private Damage _leftLegDamage;
        [SyncVar] private Damage _rightLegDamage;
        [SyncVar] private Damage _leftFootDamage;
        [SyncVar] private Damage _rightFootDamage;
        [SyncVar] private Damage _leftWristDamage;
        [SyncVar] private Damage _rightWristDamage;

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


        
        public override bool IsAlive => _isAlive;
        public override bool IsInCrit => _isInCrit;
        public Damage HeadDamage {get { return _headDamage; }set { _headDamage = value; } }
        public Damage NeckDamage { get { return _neckDamage; } set { _neckDamage = value; } }
        public Damage ChestDamage { get { return _chestDamage; } set { _chestDamage = value; } }
        public Damage LeftArmDamage { get { return _leftArmDamage; } set { _leftArmDamage = value; } }
        public Damage RightArmDamage { get { return _rightArmDamage; } set { _rightArmDamage = value; } }
        public Damage LeftWristDamage { get { return _leftWristDamage; } set { _leftWristDamage = value; } }
        public Damage RightWristDamage { get { return _rightWristDamage; } set { _rightWristDamage = value; } }
        public Damage LeftLegDamage { get { return _leftLegDamage; } set { _leftLegDamage = value; } }
        public Damage RightLegDamage { get { return _rightLegDamage; } set { _rightLegDamage = value; } }
        public Damage LeftFootDamage { get { return _leftFootDamage; } set { _leftFootDamage = value; } }
        public Damage RightFootDamage { get { return _rightFootDamage; } set { _rightFootDamage = value; } }
        public Damage GroinDamage { get { return _groinDamage; } set { _groinDamage = value; } }

        

        public Damage TotalDamage => _totalDamage;

        [Server]
        private void UpdateHealth()
        {
            _totalDamage = CalculateTotalDamage();
            float health = 100 - _totalDamage.Summ;

            Debug.Log("Health: " + health);

            if (health > 0)
            {
                _isInCrit = false;
            }
            else if (health > -200f)
            {
                // TODO Critical state
                _isInCrit = true;
            }
            else
            {
                _isAlive = false;
            }

            if (_isInCrit || !_isAlive)
                _isLying = true;
            else
            {
                _isLying = false;
            }

        }

        private Damage CalculateTotalDamage()
        {
            Damage result = new Damage(0, 0, 0, 0);
            foreach (HumanoidImpactTarget target in Enum.GetValues(typeof(HumanoidImpactTarget)))
            {
                result += GetDamage(target);
            }

            return result;
        }

        private void DoDamage(Damage damage, HumanoidImpactTarget target)
        {
            switch (target)
            {
                case HumanoidImpactTarget.Head:
                    _headDamage += damage;
                    break;
                case HumanoidImpactTarget.Neck:
                    _neckDamage += damage;
                    break;
                case HumanoidImpactTarget.Chest:
                    _chestDamage += damage;
                    break;
                case HumanoidImpactTarget.Groin:
                    _groinDamage += damage;
                    break;
                case HumanoidImpactTarget.LeftArm:
                    _leftArmDamage += damage;
                    break;
                case HumanoidImpactTarget.RightArm:
                    _rightArmDamage += damage;
                    break;
                case HumanoidImpactTarget.LeftWrist:
                    _leftWristDamage += damage;
                    break;
                case HumanoidImpactTarget.RightWrist:
                    _rightWristDamage += damage;
                    break;
                case HumanoidImpactTarget.LeftLeg:
                    _leftLegDamage += damage;
                    break;
                case HumanoidImpactTarget.RightLeg:
                    _rightLegDamage += damage;
                    break;
                case HumanoidImpactTarget.LeftFoot:
                    _leftFootDamage += damage;
                    break;
                case HumanoidImpactTarget.RightFoot:
                    _rightFootDamage += damage;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(target), target, null);
            }
        }

        private Damage GetDamage(HumanoidImpactTarget target)
        {
            switch (target)
            {
                case HumanoidImpactTarget.Head:
                    return _headDamage;
                case HumanoidImpactTarget.Neck:
                    return _neckDamage;
                    
                case HumanoidImpactTarget.Chest:
                    return _chestDamage;
                    
                case HumanoidImpactTarget.Groin:
                    return _groinDamage;
                    
                case HumanoidImpactTarget.LeftArm:
                    return _leftArmDamage;
                    
                case HumanoidImpactTarget.RightArm:
                    return _rightArmDamage;
                    
                case HumanoidImpactTarget.LeftWrist:
                    return _leftWristDamage;
                    
                case HumanoidImpactTarget.RightWrist:
                    return _rightWristDamage;
                case HumanoidImpactTarget.LeftLeg:
                    return _leftLegDamage;
                    
                case HumanoidImpactTarget.RightLeg:
                    return _rightLegDamage;
                    
                case HumanoidImpactTarget.LeftFoot:
                    return _leftFootDamage;
                    
                case HumanoidImpactTarget.RightFoot:
                    return _rightFootDamage;
                    
                default:
                    throw new ArgumentOutOfRangeException(nameof(target), target, null);
            }
        }

        #endregion
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Controllers;
using Assets.Scripts.GameMechanics;
using Assets.Scripts.GameMechanics.Health;
using Assets.Scripts.Objects.Equipment.Doors;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Objects.Mob
{
    public abstract class Mob : TileObject, IPositionProvider, IPlayerImpactable
    {
        [SerializeField] protected Sprite FrontSprite;
        [SerializeField] protected Sprite BackSprite;
        [SerializeField] protected Sprite LeftSprite;
        [SerializeField] protected Sprite RightSprite;
        [SerializeField] protected Sprite LyingSprite;

        [SerializeField] [SyncVar] protected bool IsMobLying;
        [SyncVar] protected float ActualMoveSpeed;

        [SerializeField]
        protected float DefaultMoveSpeed;

        [SerializeField] protected MobHealth HealthData;

        private bool _movementFinished = true;

        public bool IsLying => IsMobLying;

        public MobHealth Health => HealthData;

        protected override void Start()
        {
            base.Start();
            SpriteRenderer = GetComponent<SpriteRenderer>();

            CreateHealthData();

            if (isServer)
            {
                HealthData.OnStart();
            }
        }

        protected override void Update()
        {
            base.Update();
            UpdateSprite();

            if (isServer)
            {
                HealthData.OnUpdate();
                HealthProcess();
                SendHealthDataToClients();
            }
        }

        [Server]
        protected void SendHealthDataToClients()
        {
            RpcReceiveHealthData(HealthData.NetHealthData);
        }
    
        [ClientRpc]
        protected void RpcReceiveHealthData(MobHealth.ClientData data)
        {
            if(isServer)
                return;
            
            if (HealthData == null)
            {
                CreateHealthData();
                HealthData.OnStart();
            }

            HealthData.NetHealthData = data;
        }

        [Server]
        protected abstract void CreateHealthData();

        public virtual void DoTargetAction(TileObject to)
        {

        }

        public virtual void DoPointAction(int x, int y)
        {

        }

        

        public void DoMove(Direction direction)
        {
            if (!EnsureControllers())
                return;

            float ms = ActualMoveSpeed;

            if(IsLying)
                return;

            if (!_movementFinished)
            {
                //Debug.Log("Movement not finished!");
                return;
            }

            Vector2Int nextCell = Cell;
            Vector2 shift;

            Rotation = direction;

            switch (direction)
            {
                case Direction.Forward:
                    
                    nextCell += Vector2Int.up;
                    shift = Vector2.up;
                    break;

                case Direction.Backward:
                    nextCell += Vector2Int.down;
                    shift = Vector2.down;
                    break;

                case Direction.Left:
                    nextCell += Vector2Int.left;
                    shift = Vector2.left;
                    break;

                case Direction.Right:
                    nextCell += Vector2Int.right;
                    shift = Vector2.right;
                    break;

                default:
                    throw new ArgumentOutOfRangeException("direction", direction, null);
            }

            if (WalkController.CanPass(nextCell))
            {
                _movementFinished = false;

                Cell = nextCell;
                Vector3Int cell = new Vector3Int(Cell.x, Cell.y, 0);
                Vector3 offset3d = Grid.CellToWorld(cell) - transform.position;
                Vector2 offset = new Vector2(offset3d.x, offset3d.y);
                CellOffset = -offset;

                StartCoroutine(AnimateMovement(shift * ms , 1 / ms));
            }
            else // Find a door
            {
                Door door = TileController.Find<Door>(nextCell.x, nextCell.y);

                if(door != null)
                    door.TryToPass();
            }
        }

        private IEnumerator AnimateMovement(Vector2 velocity, float time)
        {
            Vector2 velocityTimed = velocity * Time.deltaTime;
            float timePast = 0;
            //Debug.Log("Time of animation: " + time + ". CellOffset: " + CellOffset);

            Vector2 prevCellOffset = CellOffset - velocityTimed;
            
            while(true)
            {
                prevCellOffset = CellOffset;
                CellOffset += velocityTimed;

                if (CellOffset.magnitude >= prevCellOffset.magnitude)
                {
                    CellOffset = Vector2.zero;
                    break;
                }

                timePast += Time.deltaTime;
                
                yield return new WaitForEndOfFrame();
            }

            _movementFinished = true;
        }

        protected virtual void UpdateSprite()
        {
            if (IsMobLying)
            {
                SpriteRenderer.sprite = FrontSprite;
                transform.rotation = Quaternion.Euler(0, 0, 90);
                //SpriteRenderer.sortingOrder = SortingLayer.GetLayerValueFromName("MobLying");
                Renderer.sortingLayerName = "MobLying";
            }
            else
            {
                Renderer.sortingLayerName = "Mob";
                //SpriteRenderer.sortingOrder = SortingLayer.GetLayerValueFromName("Mob");

                transform.rotation = Quaternion.identity;
                switch (Rotation)
                {
                    case Direction.Forward:
                        SpriteRenderer.sprite = BackSprite;
                        break;

                    case Direction.Backward:
                        SpriteRenderer.sprite = FrontSprite;
                        break;

                    case Direction.Left:
                        SpriteRenderer.sprite = LeftSprite;
                        break;

                    case Direction.Right:
                        SpriteRenderer.sprite = RightSprite;
                        break;
                }
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



        public void ImpactItemAuthority(Mob impactSource, Item.Item sourceItem, IPlayerImpactable impactable, Intent intent, ImpactLimb iTarget)
        {
            GameObject itemGo = sourceItem?.gameObject;
            GameObject sourceGo = impactSource.gameObject;
            CmdImpactItem(sourceGo, itemGo, impactable.gameObject, intent, iTarget);
        }
        

        [Command]
        private void CmdImpactItem(GameObject impactSource, GameObject sourceItemGo, GameObject impactableGo, Intent intent, ImpactLimb iTarget)
        {
            Item.Item sourceItem = sourceItemGo?.GetComponent<Item.Item>();
            Mob sourceMob = impactSource?.GetComponent<Mob>();
            IPlayerImpactable impactable = impactableGo.GetComponent<IPlayerImpactable>();

            
            impactable?.ImpactItemServer(sourceMob, sourceItem, intent, iTarget);
        }

        public int X => Cell.x;
        public int Y => Cell.y;
        public float OffsetX => CellOffset.x;
        public float OffsetY => CellOffset.y;


        public virtual void ImpactItemClient(Mob impactSource, Item.Item item, Intent intent, ImpactLimb impactTarget)
        {
            if (isServer)
            {
                ImpactItemServer(impactSource, item, intent, impactTarget);
            }
            else
            {
                PlayerActionController.Current.LocalPlayerMob.ImpactItemAuthority(impactSource, item, this, intent, impactTarget);
            }
        }

        public virtual void ImpactItemServer(Mob impactSourceMob, Item.Item item, Intent intent, ImpactLimb impactTarget)
        {
            if (item == null)
            {
                IImpactHandler mobAsHandler = impactSourceMob as IImpactHandler;
                mobAsHandler?.OnImpact(this, intent, impactTarget);
            }
            else
            {
                IImpactHandler itemAsHandler = item as IImpactHandler;
                itemAsHandler?.OnImpact(this, intent, impactTarget);
            }
        }

        protected virtual void HealthProcess()
        {
            IsMobLying = !Health.CanStandOnLegs;
            ActualMoveSpeed = DefaultMoveSpeed * Health.SpeedMultiplier;
        }
    }
}

using System;
using System.Collections;
using Assets.Scripts.Controllers;
using Assets.Scripts.Objects.Equipment.Power;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Objects.Equipment.Doors
{
    public class DoublePartAirlock : Door, IPowerConsumer
    {
        [SerializeField] protected AnimationClip OpeningClip;
        [SerializeField] protected AnimationClip ClosingClip;
        [SerializeField] protected float DoorCloseDelay;

        [SerializeField] protected DoorPart _leftPart;
        [SerializeField] protected DoorPart _rightPart;

        [SerializeField] [SyncVar]
        protected DoorState State;

        protected Animator _animator;

        protected bool IsTransperent;
        protected bool DoPassGas;
        protected bool CanPassThrough;

        protected DoorState PrevDoorState;
        protected bool AnimationSwitchingBlocked = false;

        [SerializeField]
        private float _maxPowerStored = 100f;

        [SerializeField]
        private float _powerStored = 100f;

        [SyncVar]
        private bool _isPowered;

        private CoroutineController _prevCoroutinController;

        protected override void Start()
        {
            base.Start();

            _animator = GetComponent<Animator>();
            PrevDoorState = State;

            //_animator.SetBool("greenLighting", true);
        }

        protected override void Update()
        {
            base.Update();

            if (_powerStored > 0)
            {
                _isPowered = true;
                _powerStored -= 1.0f * Time.deltaTime;
            }
            else
            {
                _powerStored = 0;

                _isPowered = false;
            }

            switch (State)
            {
                case DoorState.Opened:
                    IsTransperent = true;
                    DoPassGas = true;
                    CanPassThrough = true;
                    break;
                case DoorState.Closed:
                    IsTransperent = false;
                    DoPassGas = false;
                    CanPassThrough = false;

                    _rightPart.EffectColor = Color.clear;
                    _leftPart.EffectColor = Color.clear;

                    break;
                case DoorState.Opening:
                    IsTransperent = true;
                    DoPassGas = true;
                    CanPassThrough = false;

                    if (_isPowered)
                    {
                        _rightPart.EffectColor = Color.green;
                        _leftPart.EffectColor = Color.green;
                    }
                    break;
                case DoorState.Closing:
                    IsTransperent = true;
                    DoPassGas = true;
                    CanPassThrough = false;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }


            if (State == DoorState.Opening && PrevDoorState != DoorState.Opening)
            {
                _animator.SetInteger("state", 2);
                StartCoroutine(WaitForAnimation(OpeningClip.length, SwitchToOpened));
                AnimationSwitchingBlocked = true;
            }

            if (State == DoorState.Closing && PrevDoorState != DoorState.Closing)
            {
                StartCoroutine(WaitForAnimation(ClosingClip.length, SwitchToClosed));
                AnimationSwitchingBlocked = true;
                _animator.SetInteger("state", 0);
            }

            PrevDoorState = State;
        }

        protected override bool Transperent => IsTransperent;
        protected override bool PassesGas => DoPassGas;
        protected override bool CanWalkThrough => CanPassThrough;

        [Server]
        public void ServerForceState(DoorState state)
        {
            State = state;
        }

        IEnumerator WaitForAnimation(float seconds, Action action)
        {
            yield return new WaitForSeconds(seconds);

            AnimationSwitchingBlocked = false;

            action();
        }

        void SwitchToClosed()
        {
            State = DoorState.Closed;
            _animator.SetInteger("state", 0);
        }

        void SwitchToOpened()
        {
            State = DoorState.Opened;
            _animator.SetInteger("state", 2);
        }

        public DoorState GetDoorState()
        {
            return State;
        }
        
        private void CallServerNoItem()
        {
            PlayerActionController.Current.LocalPlayer.ApplyItem(null, this);
        }

        public override void ApplyItemClient(Item.Item item)
        {
            Debug.Log("Door was pushed");
            PlayerActionController.Current.LocalPlayer.ApplyItem(item, this);
        }

        public override void ApplyItemServer(Item.Item item)
        {
            if (item == null)
            {
                if (_isPowered)
                {
                    if (State == DoorState.Closed)
                    {
                        State = DoorState.Opening;

                        if(_prevCoroutinController != null)
                            _prevCoroutinController.ShouldStop = true;

                        _prevCoroutinController = new CoroutineController();

                        StartCoroutine(CloseDoorDelayed(_prevCoroutinController));
                    }
                    if (State == DoorState.Opened)
                    {
                        State = DoorState.Closing;
                    }
                }
            }
        }

        public override void TryToPass()
        {
            CallServerNoItem();
        }

        private IEnumerator CloseDoorDelayed(CoroutineController controller)
        {
            yield return new WaitForSeconds(DoorCloseDelay);

            if (!controller.ShouldStop)
            {
                if (State == DoorState.Opened)
                    State = DoorState.Closing;

                Debug.Log("Coroutine!");
            }
        }

        public void SendPower(float power)
        {
            _powerStored += power;

            if (_powerStored > _maxPowerStored)
                _powerStored = _maxPowerStored;
        }

        public float AmountOfNeededPower => _maxPowerStored - _powerStored;

        class CoroutineController
        {
            public bool ShouldStop = false;
        }
    }
}

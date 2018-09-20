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

        protected override void Start()
        {
            base.Start();

            _animator = GetComponent<Animator>();
            PrevDoorState = State;

            _animator.SetBool("greenLighting", true);
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
                    break;
                case DoorState.Opening:
                    IsTransperent = true;
                    DoPassGas = true;
                    CanPassThrough = false;
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

                if (_isPowered)
                {
                    _rightPart.EffectColor = Color.green;
                    _leftPart.EffectColor = Color.green;
                }
            }

            if (State == DoorState.Closing && PrevDoorState != DoorState.Closing)
            {
                StartCoroutine(WaitForAnimation(ClosingClip.length, SwitchToClosed));
                AnimationSwitchingBlocked = true;
                _animator.SetInteger("state", 0);

                if (_isPowered)
                {
                    _rightPart.EffectColor = Color.green;
                    _leftPart.EffectColor = Color.green;
                }
            }

            if (State == DoorState.Closed)
            {
                _rightPart.EffectColor = Color.clear;
                _leftPart.EffectColor = Color.clear;
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
        
        private void CallServer()
        {
            PlayerActionController.Current.LocalPlayer.ApplyItem(null, this);
        }

        public override void ApplyItemClient(Item.Item item)
        {
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

                        StartCoroutine(CloseDoorDelayed());
                    }
                }
            }
        }

        public override void TryToPass()
        {
            CallServer();
        }

        private IEnumerator CloseDoorDelayed()
        {
            yield return new WaitForSeconds(DoorCloseDelay);

            State = DoorState.Closing;
        }

        public void SendPower(float power)
        {
            _powerStored += power;

            if (_powerStored > _maxPowerStored)
                _powerStored = _maxPowerStored;
        }

        public float AmountOfNeededPower => _maxPowerStored - _powerStored;
    }
}

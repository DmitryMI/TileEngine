using System;
using System.Collections;
using Assets.Scripts.Controllers;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Objects.Equipment.Doors
{
    public class DoublePartAirlock : Door
    {
        [SerializeField] protected AnimationClip OpeningClip;
        [SerializeField] protected AnimationClip ClosingClip;
        [SerializeField] protected float DoorCloseDelay;

        [SerializeField] [SyncVar]
        protected DoorState State;

        protected Animator _animator;

        protected bool IsTransperent;
        protected bool DoPassGas;
        protected bool CanPassThrough;

        protected DoorState PrevDoorState;
        protected bool AnimationSwitchingBlocked = false;

        protected override void Start()
        {
            base.Start();

            _animator = GetComponent<Animator>();
            PrevDoorState = State;
        }

        protected override void Update()
        {
            base.Update();

            if (!AnimationSwitchingBlocked)
            {
                _animator.SetInteger("state", (int) State);
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
                StartCoroutine(WaitForAnimation(OpeningClip.length, SwitchToOpened));
                AnimationSwitchingBlocked = true;
            }

            if (State == DoorState.Closing && PrevDoorState != DoorState.Closing)
            {
                StartCoroutine(WaitForAnimation(ClosingClip.length, SwitchToClosed));
                AnimationSwitchingBlocked = true;
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
        }

        void SwitchToOpened()
        {
            State = DoorState.Opened;
        }

        public DoorState GetDoorState()
        {
            return State;
        }
        
        private void CallServer()
        {
            PlayerActionController.Current.LocalPlayer.ApplyItem(null, this);
        }

        public override void ApplyItemServer(Item.Item item)
        {
            if (item == null)
            {
                if (State == DoorState.Closed)
                {
                    State = DoorState.Opening;

                    StartCoroutine(CloseDoorDelayed());
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
    }
}

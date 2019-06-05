using System;
using System.Collections;
using Assets.Scripts.Controllers;
using Assets.Scripts.Objects.Equipment.Power;
using Assets.Scripts.Objects.Mob;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Objects.Equipment.Doors
{
    public class DoublePartAirlock : Door
    {
        [SerializeField] protected AudioClip OpeningAudioClip;
        [SerializeField] protected AudioClip ClosingAudioClip;

        [SerializeField] protected AnimationClip OpeningClip;
        [SerializeField] protected AnimationClip ClosingClip;
        [SerializeField] protected float DoorCloseDelay;

        [SerializeField] protected bool HasWindow;

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

        [SyncVar]
        private bool _isPowered;

        private CoroutineController _prevCoroutinController;

        private AudioSource _audioSource;

        public override float PowerNeeded
        {
            get
            {
                if (State == DoorState.Opening || State == DoorState.Closing)
                    return 0.005f;
                else
                    return 0.001f;
            }
        }

        protected override void Start()
        {
            base.Start();

            _animator = GetComponent<Animator>();
            PrevDoorState = State;
            _audioSource = GetComponent<AudioSource>();

            //_animator.SetBool("greenLighting", true);
        }

        protected override void Update()
        {
            base.Update();

            if(isServer)
                _isPowered = Electrified;

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

            if (HasWindow)
                IsTransperent = true;

            if (State == DoorState.Opening && PrevDoorState != DoorState.Opening)
            {
                _animator.SetInteger("state", 2);
                StartCoroutine(WaitForAnimation(OpeningClip.length, SwitchToOpened));
                AnimationSwitchingBlocked = true;
                _audioSource.clip = OpeningAudioClip;
                _audioSource.Play();
            }

            if (State == DoorState.Closing && PrevDoorState != DoorState.Closing)
            {
                StartCoroutine(WaitForAnimation(ClosingClip.length, SwitchToClosed));
                AnimationSwitchingBlocked = true;
                _animator.SetInteger("state", 0);

                _audioSource.clip = ClosingAudioClip;
                _audioSource.Play();
            }

            PrevDoorState = State;
        }

        protected override bool Transparent => IsTransperent;
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
            Humanoid humanoid = PlayerActionController.Current.LocalPlayerMob as Humanoid;

            humanoid?.ApplyItem(null, this);
        }

        public override void ApplyItemClient(Item.Item item)
        {
            Humanoid humanoid = PlayerActionController.Current.LocalPlayerMob as Humanoid;
            if (humanoid != null)
            {
                Debug.Log("Door was pushed");
                humanoid.ApplyItem(item, this);
            }
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
            if(State == DoorState.Closed)
                CallServerNoItem();
        }

        private IEnumerator CloseDoorDelayed(CoroutineController controller)
        {
            yield return new WaitForSeconds(DoorCloseDelay);

            if (!controller.ShouldStop)
            {
                if (State == DoorState.Opened && Electrified)
                    State = DoorState.Closing;

                Debug.Log("Coroutine!");
            }
        }

        class CoroutineController
        {
            public bool ShouldStop = false;
        }
    }
}

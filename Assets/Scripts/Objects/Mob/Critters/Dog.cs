using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Objects.Mob.Critters
{
    class Dog : Mob
    {
        [SerializeField]
        protected string DogName;

        [SerializeField] protected AudioClip[] BarkSounds;

        protected AudioSource Source;

        protected override void Start()
        {
            base.Start();

            Source = GetComponent<AudioSource>();
        }

        protected override bool Transparent => true;
        protected override bool CanWalkThrough => true;
        protected override bool PassesGas => true;
        public override string DescriptiveName => DogName;
        public override bool IsLying { get; }

        
        public override void DoTargetAction(TileObject to)
        {
            if(isLocalPlayer)
                CmdDoBark(to.gameObject);
        }

        [Command]
        private void CmdDoBark(GameObject target)
        {
            RpcDoBark(target);
        }

        private void RpcDoBark(GameObject target)
        {
            if (!Source.isPlaying)
            {
                Source.PlayOneShot(Utils.GetRandom(BarkSounds));

                // TODO Dog barking at something
            }
        }
    }
}

using Assets.Scripts.GameMechanics.Health;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Objects.Mob.Critters
{
    class Dog : Animal
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

        public override string DescriptiveName => DogName;


        protected override void CreateHealthData()
        {
            HealthData = new DogHealth();
        }

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

        [ClientRpc]
        private void RpcDoBark(GameObject target)
        {
            Bark();
        }

        private void Bark()
        {
            if (!Source.isPlaying)
            {
                Source.PlayOneShot(Utils.GetRandom(BarkSounds));

                // TODO Dog barking at something
            }
        }
    }
}

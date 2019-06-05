using UnityEngine;

namespace Assets.Scripts.Objects.Mob.Critters
{
    class Dog : Mob
    {
        [SerializeField]
        protected string DogName;

        protected override bool Transparent => true;
        protected override bool CanWalkThrough => true;
        protected override bool PassesGas => true;
        public override string DescriptiveName => DogName;
        public override bool IsLying { get; }
    }
}

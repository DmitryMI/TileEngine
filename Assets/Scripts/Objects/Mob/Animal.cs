using Assets.Scripts.GameMechanics.Health;

namespace Assets.Scripts.Objects.Mob
{
    public abstract class Animal : Mob
    {
        protected override bool Transparent => true;
        protected override bool CanWalkThrough => true;
        protected override bool PassesGas => true;
    }
}

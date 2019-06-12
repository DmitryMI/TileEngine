using Assets.Scripts.GameMechanics.Chemistry;

namespace Assets.Scripts.Objects.FloorSplashes
{
    public abstract class FloorSplash : TileObject, ISubstanceContainer
    {
        protected override bool Transparent => true;
        protected override bool CanWalkThrough => true;
        protected override bool PassesGas => true;
        public abstract float RemainingVolume { get; }
        public abstract float MaximumVolume { get; }
        public abstract void TransferInto(SubstanceMixture mixture);
        public abstract void TransferToAnother(ISubstanceContainer container);
    }
}

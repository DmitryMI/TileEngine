using Assets.Scripts.Objects;

namespace Assets.Scripts.GameMechanics
{
    public class SpawnPoint : TileObject
    {
        protected override bool Transparent
        {
            get { return true; }
        }

        protected override bool CanWalkThrough
        {
            get { return true; }
        }

        protected override bool PassesGas
        {
            get { return true; }
        }

        public override string DescriptiveName
        {
            get { return "Spawn point"; }
        }
    }
}

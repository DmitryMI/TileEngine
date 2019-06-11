using Assets.Scripts.GameMechanics.Health;

namespace Assets.Scripts.Objects.Mob.Humanoids
{
    public class Human : Humanoid
    {
        public override string DescriptiveName => "Soulless human being";

        protected override void CreateHealthData()
        {
            HealthData = new HumanHealth();
        }
    }
}

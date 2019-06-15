using Assets.Scripts.Objects.Mob;

namespace Assets.Scripts.GameMechanics.Health
{
    abstract class AnimalHealth : VertebrateHealth
    {
        protected DamageBuffer LeftArm;
        protected DamageBuffer RightArm;
        protected DamageBuffer LeftLeg;
        protected DamageBuffer RightLeg;
        protected DamageBuffer Groin;
        

        protected void ModifyLeftArmDamage(DamageBuffer damage)
        {
            OverallDamage += damage;
            LeftArm += damage;
        }
        protected void ModifyRightArmDamage(DamageBuffer damage)
        {
            OverallDamage += damage;
            RightArm += damage;
        }
        protected void ModifyLeftLegDamage(DamageBuffer damage)
        {
            OverallDamage += damage;
            LeftLeg += damage;
        }
        protected void ModifyRightLegDamage(DamageBuffer damage)
        {
            OverallDamage += damage;
            RightLeg+= damage;
        }

        protected void ModifyGroinDamage(DamageBuffer damage)
        {
            OverallDamage += damage;
            Groin += damage;
        }

        protected override bool ModifyDamage(DamageBuffer damage, ImpactLimb impactTarget)
        {
            bool ok = base.ModifyDamage(damage, impactTarget);

            if (ok)
                return true;

            switch (impactTarget)
            {
                case ImpactLimb.LeftArm:
                    ModifyLeftArmDamage(damage);
                    return true;
                case ImpactLimb.RightArm:
                    ModifyRightArmDamage(damage);
                    return true;
                case ImpactLimb.LeftLeg:
                    ModifyLeftLegDamage(damage);
                    return true;
                case ImpactLimb.RightLeg:
                    ModifyRightLegDamage(damage);
                    return true;
                case ImpactLimb.Groin:
                    ModifyGroinDamage(damage);
                    return true;
            }

            return false;
        }

        public override bool SupportsImpactTarget(ImpactLimb impactTarget)
        {
            bool hasBase = base.SupportsImpactTarget(impactTarget);

            if (hasBase)
                return true;

            switch (impactTarget)
            {
                case ImpactLimb.LeftArm:
                    return true;
                case ImpactLimb.RightArm:
                    return true;
                case ImpactLimb.LeftLeg:
                    return true;
                case ImpactLimb.RightLeg:
                    return true;
                case ImpactLimb.Groin:
                    return true;
            }

            return false;
        }

        protected AnimalHealth(Mob owner) : base(owner)
        {
        }
    }
}

namespace Assets.Scripts.GameMechanics.Health
{
    class AnimalHealth : VertebrateHealth
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

        public override bool ModifyDamage(DamageBuffer damage, ImpactTarget impactTarget)
        {
            bool ok = base.ModifyDamage(damage, impactTarget);

            if (ok)
                return true;

            switch (impactTarget)
            {
                case ImpactTarget.LeftArm:
                    ModifyLeftArmDamage(damage);
                    return true;
                case ImpactTarget.RightArm:
                    ModifyRightArmDamage(damage);
                    return true;
                case ImpactTarget.LeftLeg:
                    ModifyLeftLegDamage(damage);
                    return true;
                case ImpactTarget.RightLeg:
                    ModifyRightLegDamage(damage);
                    return true;
                case ImpactTarget.Groin:
                    ModifyGroinDamage(damage);
                    return true;
            }

            return false;
        }

        public override bool SupportsImpactTarget(ImpactTarget impactTarget)
        {
            bool hasBase = base.SupportsImpactTarget(impactTarget);

            if (hasBase)
                return true;

            switch (impactTarget)
            {
                case ImpactTarget.LeftArm:
                    return true;
                case ImpactTarget.RightArm:
                    return true;
                case ImpactTarget.LeftLeg:
                    return true;
                case ImpactTarget.RightLeg:
                    return true;
                case ImpactTarget.Groin:
                    return true;
            }

            return false;
        }

    }
}

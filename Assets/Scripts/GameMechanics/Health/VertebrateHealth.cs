namespace Assets.Scripts.GameMechanics.Health
{
    class VertebrateHealth : MobHealth
    {
        protected DamageBuffer HeadDamage;

        protected void ModifyHeadDamage(DamageBuffer damage)
        {
            OverallDamage += damage;
            HeadDamage += damage;
        }

        public override bool ModifyDamage(DamageBuffer damage, ImpactLimb target)
        {
            bool ok = base.ModifyDamage(damage, target);

            if (ok)
                return true;

            switch (target)
            {
                case ImpactLimb.Head:
                    ModifyHeadDamage(damage);
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
                case ImpactLimb.Head:
                    return true;
            }

            return false;
        }
    }
}

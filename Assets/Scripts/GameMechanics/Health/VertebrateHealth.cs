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

        public override bool ModifyDamage(DamageBuffer damage, ImpactTarget target)
        {
            bool ok = base.ModifyDamage(damage, target);

            if (ok)
                return true;

            switch (target)
            {
                case ImpactTarget.Head:
                    ModifyHeadDamage(damage);
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
                case ImpactTarget.Head:
                    return true;
            }

            return false;
        }
    }
}

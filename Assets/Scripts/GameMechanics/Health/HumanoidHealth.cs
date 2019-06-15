using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Objects.Mob;
using UnityEngine;

namespace Assets.Scripts.GameMechanics.Health
{
    abstract class HumanoidHealth : AnimalHealth
    {
        protected DamageBuffer LeftWrist;
        protected DamageBuffer RightWrist;
        protected DamageBuffer LeftFoot;
        protected DamageBuffer RightFoot;

        protected virtual void ModifyLeftWristDamage(DamageBuffer damage)
        {
            OverallDamage += damage;
            LeftWrist += damage;
        }

        protected virtual void ModifyRightWristDamage(DamageBuffer damage)
        {
            OverallDamage += damage;
            RightWrist += damage;
        }

        protected virtual void ModifyLeftFootDamage(DamageBuffer damage)
        {
            OverallDamage += damage;
            LeftFoot += damage;
        }

        protected virtual void ModifyRightFootDamage(DamageBuffer damage)
        {
            OverallDamage += damage;
            RightFoot += damage;
        }

        protected override bool ModifyDamage(DamageBuffer damage, ImpactLimb impactTarget)
        {
            bool ok = base.ModifyDamage(damage, impactTarget);

            if (ok)
                return true;

            switch (impactTarget)
            {
                case ImpactLimb.LeftWrist:
                    ModifyLeftWristDamage(damage);
                    return true;
                case ImpactLimb.RightWrist:
                    ModifyRightWristDamage(damage);
                    return true;
                case ImpactLimb.LeftFoot:
                    ModifyLeftFootDamage(damage);
                    return true;
                case ImpactLimb.RightFoot:
                    ModifyRightFootDamage(damage);
                    return true;
            }

            return false;
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            // Blackout caused by damages
            // Color must be red

            float m = GlobalPreferences.Instance.HumanMaxHp;
            float l = GlobalPreferences.Instance.HumanSeverInjuryBorder;
            float a = 1 / (m - l);
            float b = -a * l;

            float x = OverallDamage.Summ;

            float damagePercentage = a * x + b;

            if (damagePercentage < 0)
                damagePercentage = 0;
            if (damagePercentage > 1)
                damagePercentage = 1;

            
            if (damagePercentage > 0)
            {
                NetHealthData.BlackoutColor = new Color(0.4f, 0, 0);
                int stage = (int) (BlackoutMasksNumber * damagePercentage);//Mathf.RoundToInt(BlackoutMasksNumber * damagePercentage);

                NetHealthData.BlackoutStage = stage;
            }
            else
            {
                NetHealthData.BlackoutStage = 0;
                NetHealthData.BlackoutColor = Color.white;
            }
        }


        public override bool SupportsImpactTarget(ImpactLimb impactTarget)
        {
            bool hasBase = base.SupportsImpactTarget(impactTarget);

            if (hasBase)
                return true;

            switch (impactTarget)
            {
                case ImpactLimb.LeftWrist:
                    return true;
                case ImpactLimb.RightWrist:
                    return true;
                case ImpactLimb.LeftFoot:
                    return true;
                case ImpactLimb.RightFoot:
                    return true;
            }

            return false;
        }

        protected HumanoidHealth(Mob owner) : base(owner)
        {
        }
    }
}

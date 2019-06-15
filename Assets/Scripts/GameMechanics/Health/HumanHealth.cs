using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Objects.Mob;
using UnityEngine.Networking;

namespace Assets.Scripts.GameMechanics.Health
{
    class HumanHealth : HumanoidHealth
    {
        public override void OnStart()
        {
            NutritionMax = GlobalPreferences.Instance.HumanMaxNutrition;
            NutritionDecrement = GlobalPreferences.Instance.HumanNutritionDecrement;
            NutritionCurrent = GlobalPreferences.Instance.HumanNutritionInitial;
        }

        public override bool IsCritical => OverallDamage.Summ >= GlobalPreferences.Instance.HumanMaxHp - GlobalPreferences.Instance.CriticalHealthPointsBorder;
        public override bool CanStandOnLegs => !IsCritical;

        public HumanHealth(Mob owner) : base(owner)
        {
        }

        public override bool DoBruteDamage(float amount, ImpactLimb impactLimb, BruteAttackType attackType)
        {
            bool ok = base.DoBruteDamage(amount, impactLimb, attackType);

            UnityEngine.Debug.Log("DoBruteDamage. IsServer: " + Owner.isServer);

            if (ok)
            {
                switch (impactLimb)
                {
                    case ImpactLimb.Head:
                        NetHealthData.HeadSkinDamage++;
                        break;
                    case ImpactLimb.Chest:
                        NetHealthData.ChestSkinDamage++;
                        break;
                    case ImpactLimb.LeftArm:
                        NetHealthData.LeftArmSkinDamage++;
                        break;
                    case ImpactLimb.RightArm:
                        NetHealthData.RightArmSkinDamage++;
                        break;
                    case ImpactLimb.LeftLeg:
                        NetHealthData.LeftLegSkinDamage++;
                        break;
                    case ImpactLimb.RightLeg:
                        NetHealthData.RightLegSkinDamage++;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(impactLimb), impactLimb, null);
                }
            }

            return ok;
        }

        public override float SpeedMultiplier
        {
            get
            {
                if (OverallDamage.Summ > GlobalPreferences.Instance.HumanSeverInjuryBorder)
                    return 0.5f;

                return 1.0f;
            }
        }
    }
}

using System;
using Assets.Scripts;
using Assets.Scripts.Controllers;
using Assets.Scripts.Controllers.Atmos;
using UnityEngine;

namespace Assets
{
    public class TestButtonScript : MonoBehaviour
    {
        [SerializeField]
        private HumanoidImpactTarget target;

        [SerializeField] private DamageType _damageType;

        public void TestButtonClick()
        {
            Damage damage = new Damage(_damageType, 20);

            switch (target)
            {
                case HumanoidImpactTarget.Head:
                    PlayerActionController.Current.LocalPlayer.HeadDamage += damage;
                    break;
                case HumanoidImpactTarget.Neck:
                    break;
                case HumanoidImpactTarget.Chest:
                    PlayerActionController.Current.LocalPlayer.ChestDamage += damage;
                    break;
                case HumanoidImpactTarget.Groin:
                    break;
                case HumanoidImpactTarget.LeftArm:
                    PlayerActionController.Current.LocalPlayer.LeftArmDamage += damage;
                    break;
                case HumanoidImpactTarget.RightArm:
                    break;
                case HumanoidImpactTarget.LeftWrist:
                    break;
                case HumanoidImpactTarget.RightWrist:
                    break;
                case HumanoidImpactTarget.LeftLeg:
                    break;
                case HumanoidImpactTarget.RightLeg:
                    break;
                case HumanoidImpactTarget.LeftFoot:
                    break;
                case HumanoidImpactTarget.RightFoot:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
        }
    }
}

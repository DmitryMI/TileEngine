using System;
using UnityEngine;

namespace Assets.Scripts.Objects.Mob
{
    [Serializable]
    public class HumanHealthData
    {
        [SerializeField]
        private readonly Damage[] _damageArray;

        [SerializeField]
        private bool _isInCrit;

        [SerializeField]
        private bool _isDead;

        [SerializeField] private bool _isUnconsious;

        public Damage HeadDamage { get { return _damageArray[0]; } set { _damageArray[0] = value; } }
        public Damage NeckDamage { get { return _damageArray[1]; } set { _damageArray[1] = value; } }
        public Damage ChestDamage { get { return _damageArray[2]; } set { _damageArray[3] = value; } }
        public Damage LeftArmDamage { get { return _damageArray[4]; } set { _damageArray[4] = value; } }
        public Damage RightArmDamage { get { return _damageArray[5]; } set { _damageArray[5] = value; } }
        public Damage LeftWristDamage { get { return _damageArray[6]; } set { _damageArray[6] = value; } }
        public Damage RightWristDamage { get { return _damageArray[7]; } set { _damageArray[7] = value; } }
        public Damage LeftLegDamage { get { return _damageArray[8]; } set { _damageArray[8] = value; } }
        public Damage RightLegDamage { get { return _damageArray[9]; } set { _damageArray[9] = value; } }
        public Damage LeftFootDamage { get { return _damageArray[10]; } set { _damageArray[10] = value; } }
        public Damage RightFootDamage { get { return _damageArray[11]; } set { _damageArray[11] = value; } }
        public Damage GroinDamage { get { return _damageArray[12]; } set { _damageArray[12] = value; } }


        public bool IsAlive { get { return !_isDead; } set { _isDead = !value; } }
        public bool IsInCrit { get { return _isInCrit; } set { _isInCrit = value; } }

        public bool IsConsious
        {
            get { return !_isUnconsious; }
            set { _isUnconsious = !value; }
        }

        public bool IsDead => _isDead;
        public bool IsUnconsious => _isUnconsious;

        public Damage TotalDamage => CalculateTotalDamage();

        private Damage CalculateTotalDamage()
        {
            Damage result = new Damage(0, 0, 0, 0);
            foreach (HumanoidImpactTarget target in Enum.GetValues(typeof(HumanoidImpactTarget)))
            {
                result += GetDamage(target);
            }

            return result;
        }

        public Damage GetDamage(HumanoidImpactTarget target)
        {
            switch (target)
            {
                case HumanoidImpactTarget.Head:
                    return HeadDamage;
                case HumanoidImpactTarget.Neck:
                    return NeckDamage;

                case HumanoidImpactTarget.Chest:
                    return ChestDamage;

                case HumanoidImpactTarget.Groin:
                    return GroinDamage;

                case HumanoidImpactTarget.LeftArm:
                    return LeftArmDamage;

                case HumanoidImpactTarget.RightArm:
                    return RightArmDamage;

                case HumanoidImpactTarget.LeftWrist:
                    return LeftWristDamage;

                case HumanoidImpactTarget.RightWrist:
                    return RightWristDamage;
                case HumanoidImpactTarget.LeftLeg:
                    return LeftLegDamage;

                case HumanoidImpactTarget.RightLeg:
                    return RightLegDamage;

                case HumanoidImpactTarget.LeftFoot:
                    return LeftFootDamage;

                case HumanoidImpactTarget.RightFoot:
                    return RightFootDamage;

                default:
                    throw new ArgumentOutOfRangeException(nameof(target), target, null);
            }
        }

        public void DoDamage(Damage damage, HumanoidImpactTarget target)
        {
            switch (target)
            {
                case HumanoidImpactTarget.Head:
                    HeadDamage += damage;
                    break;
                case HumanoidImpactTarget.Neck:
                    NeckDamage += damage;
                    break;
                case HumanoidImpactTarget.Chest:
                    ChestDamage += damage;
                    break;
                case HumanoidImpactTarget.Groin:
                    GroinDamage += damage;
                    break;
                case HumanoidImpactTarget.LeftArm:
                    LeftArmDamage += damage;
                    break;
                case HumanoidImpactTarget.RightArm:
                    RightArmDamage += damage;
                    break;
                case HumanoidImpactTarget.LeftWrist:
                    LeftWristDamage += damage;
                    break;
                case HumanoidImpactTarget.RightWrist:
                    RightWristDamage += damage;
                    break;
                case HumanoidImpactTarget.LeftLeg:
                    LeftLegDamage += damage;
                    break;
                case HumanoidImpactTarget.RightLeg:
                    RightLegDamage += damage;
                    break;
                case HumanoidImpactTarget.LeftFoot:
                    LeftFootDamage += damage;
                    break;
                case HumanoidImpactTarget.RightFoot:
                    RightFootDamage += damage;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(target), target, null);
            }
        }

        public HumanHealthData()
        {
            _isDead = false;

            _isInCrit = false;

            _damageArray = new Damage[13];
        }

    }
}

using System;
using Assets.Scripts.GameMechanics;
using UnityEngine;

namespace Assets.Scripts.Objects.Mob
{
    [Serializable]
    public class HumanHealthData
    {
        [SerializeField]
        private Damage[] _damageArray;

        [SerializeField]
        private bool _isInCrit;

        [SerializeField]
        private bool _isDead;

        [SerializeField] private bool _isUnconsious;

        public Damage HeadDamage { get { return _damageArray[0]; } set { _damageArray[0] = value; } }
        public Damage NeckDamage { get { return _damageArray[1]; } set { _damageArray[1] = value; } }
        public Damage ChestDamage { get { return _damageArray[2]; } set { _damageArray[2] = value; } }
        public Damage LeftArmDamage { get { return _damageArray[3]; } set { _damageArray[3] = value; } }
        public Damage RightArmDamage { get { return _damageArray[4]; } set { _damageArray[4] = value; } }
        public Damage LeftWristDamage { get { return _damageArray[5]; } set { _damageArray[5] = value; } }
        public Damage RightWristDamage { get { return _damageArray[6]; } set { _damageArray[6] = value; } }
        public Damage LeftLegDamage { get { return _damageArray[7]; } set { _damageArray[7] = value; } }
        public Damage RightLegDamage { get { return _damageArray[8]; } set { _damageArray[8] = value; } }
        public Damage LeftFootDamage { get { return _damageArray[9]; } set { _damageArray[9] = value; } }
        public Damage RightFootDamage { get { return _damageArray[10]; } set { _damageArray[10] = value; } }
        public Damage GroinDamage { get { return _damageArray[11]; } set { _damageArray[11] = value; } }


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
            int index = (int) target;

            if(index >= 0 && index < _damageArray.Length)
                return _damageArray[index];

            throw new ArgumentOutOfRangeException(nameof(target), $"{index} is not in range {0}..{_damageArray.Length}");
        }

        public void DoDamage(Damage damage, HumanoidImpactTarget target)
        {
            int index = (int)target;

            if (index >= 0 && index < _damageArray.Length)
                _damageArray[index] += damage;
            else
            {
                throw new ArgumentOutOfRangeException(nameof(target),
                    $"{index} is not in range {0}..{_damageArray.Length}");
            }
        }

        public HumanHealthData()
        {
            _isDead = false;

            _isInCrit = false;

            _damageArray = new Damage[12];
        }

    }
}

using System;
using Assets.Scripts.GameMechanics;
using Assets.Scripts.GameMechanics.Health;
using UnityEngine;

namespace Assets.Scripts.Objects.Mob
{
    [Obsolete("Use MobHealth and it's derivatives")]
    [Serializable]
    public class HumanHealthData
    {
        [SerializeField]
        private DamageBuffer[] _damageArray;

        [SerializeField]
        private bool _isInCrit;

        [SerializeField]
        private bool _isDead;

        [SerializeField] private bool _isUnconsious;

        public DamageBuffer HeadDamage { get { return _damageArray[0]; } set { _damageArray[0] = value; } }
        public DamageBuffer NeckDamage { get { return _damageArray[1]; } set { _damageArray[1] = value; } }
        public DamageBuffer ChestDamage { get { return _damageArray[2]; } set { _damageArray[2] = value; } }
        public DamageBuffer LeftArmDamage { get { return _damageArray[3]; } set { _damageArray[3] = value; } }
        public DamageBuffer RightArmDamage { get { return _damageArray[4]; } set { _damageArray[4] = value; } }
        public DamageBuffer LeftWristDamage { get { return _damageArray[5]; } set { _damageArray[5] = value; } }
        public DamageBuffer RightWristDamage { get { return _damageArray[6]; } set { _damageArray[6] = value; } }
        public DamageBuffer LeftLegDamage { get { return _damageArray[7]; } set { _damageArray[7] = value; } }
        public DamageBuffer RightLegDamage { get { return _damageArray[8]; } set { _damageArray[8] = value; } }
        public DamageBuffer LeftFootDamage { get { return _damageArray[9]; } set { _damageArray[9] = value; } }
        public DamageBuffer RightFootDamage { get { return _damageArray[10]; } set { _damageArray[10] = value; } }
        public DamageBuffer GroinDamage { get { return _damageArray[11]; } set { _damageArray[11] = value; } }


        public bool IsAlive { get { return !_isDead; } set { _isDead = !value; } }
        public bool IsInCrit { get { return _isInCrit; } set { _isInCrit = value; } }

        public bool IsConsious
        {
            get { return !_isUnconsious; }
            set { _isUnconsious = !value; }
        }

        public bool IsDead => _isDead;
        public bool IsUnconsious => _isUnconsious;

        public DamageBuffer TotalDamage => CalculateTotalDamage();

        private DamageBuffer CalculateTotalDamage()
        {
            DamageBuffer result = new DamageBuffer(0, 0, 0, 0);
            foreach (ImpactTarget target in Enum.GetValues(typeof(ImpactTarget)))
            {
                result += GetDamage(target);
            }

            return result;
        }

        public DamageBuffer GetDamage(ImpactTarget target)
        {
            int index = (int) target;

            if(index >= 0 && index < _damageArray.Length)
                return _damageArray[index];

            throw new ArgumentOutOfRangeException(nameof(target), $"{index} is not in range {0}..{_damageArray.Length}");
        }

        public void DoDamage(DamageBuffer damage, ImpactTarget target)
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

            _damageArray = new DamageBuffer[12];
        }

    }
}

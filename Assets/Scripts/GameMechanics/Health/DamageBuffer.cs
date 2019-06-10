using System;
using UnityEngine;

namespace Assets.Scripts.GameMechanics.Health
{
    [Serializable]
    public struct DamageBuffer
    {
        [SerializeField]
        private float _brute;
        [SerializeField]
        private float _burn;
        [SerializeField]
        private float _toxin;
        [SerializeField]
        private float _suffocation;

        public bool Equals(DamageBuffer other)
        {
            return Brute.Equals(other.Brute) && Burn.Equals(other.Burn) && Toxin.Equals(other.Toxin) && Suffocation.Equals(other.Suffocation);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is DamageBuffer && Equals((DamageBuffer) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Brute.GetHashCode();
                hashCode = (hashCode * 397) ^ Burn.GetHashCode();
                hashCode = (hashCode * 397) ^ Toxin.GetHashCode();
                hashCode = (hashCode * 397) ^ Suffocation.GetHashCode();
                return hashCode;
            }
        }

        private const float ComparisonTolerance = 0.000001f;

        public float Brute
        {
            get { return _brute; }
            set { _brute = value; }
        }

        public float Burn
        {
            get { return _burn; }
            set { _burn = value; }
        }

        public float Toxin
        {
            get { return _toxin; }
            set { _toxin = value; }
        }

        public float Suffocation
        {
            get { return _suffocation; }
            set { _suffocation = value; }
        }

        public DamageBuffer(float brute, float burn, float toxin, float suffocation)
        {
            _brute = brute;
            _burn = burn;
            _toxin = toxin;
            _suffocation = suffocation;
        }

        public DamageBuffer(DamageType damageType, float amount)
        {
            switch (damageType)
            {
                case DamageType.Brute:
                    _brute = amount;
                    _burn = 0;
                    _toxin = 0;
                    _suffocation = 0;
                    break;
                case DamageType.Burn:
                    _brute = 0;
                    _burn = amount;
                    _toxin = 0;
                    _suffocation = 0;
                    break;
                case DamageType.Toxin:
                    _brute = 0;
                    _burn = 0;
                    _toxin = amount;
                    _suffocation = 0;
                    break;
                case DamageType.Suffocation:
                    _brute = 0;
                    _burn = 0;
                    _toxin = 0;
                    _suffocation = amount;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(damageType), damageType, null);
            }
        }

        public static DamageBuffer operator +(DamageBuffer a, DamageBuffer b)
        {
            return new DamageBuffer(a.Brute + b.Brute, a.Burn + b.Burn, a.Toxin + b.Toxin, a.Suffocation + b.Suffocation);
        }

        public static DamageBuffer operator -(DamageBuffer a)
        {
            return new DamageBuffer(-a.Brute, -a.Burn, -a.Toxin, -a.Suffocation);
        }

        public static DamageBuffer operator -(DamageBuffer a, DamageBuffer b)
        {
            return new DamageBuffer(a.Brute - b.Brute, a.Burn - b.Burn, a.Toxin - b.Toxin, a.Suffocation - b.Suffocation);
        }

        public static DamageBuffer operator *(DamageBuffer a, float k)
        {
            return new DamageBuffer(a.Brute * k, a.Burn * k, a.Toxin * k, a.Suffocation * k);
        }

        public static DamageBuffer operator /(DamageBuffer a, float k)
        {
            return new DamageBuffer(a.Brute / k, a.Burn / k, a.Toxin / k, a.Suffocation / k);
        }

        public static bool operator ==(DamageBuffer a, DamageBuffer b)
        {
            bool result =
                Math.Abs(a.Burn - b.Burn) < ComparisonTolerance &&
                Math.Abs(a.Brute - b.Brute) < ComparisonTolerance &&
                Math.Abs(a.Toxin - b.Toxin) < ComparisonTolerance &&
                Math.Abs(a.Suffocation - b.Suffocation) < ComparisonTolerance;
            return result;
        }

        public static bool operator !=(DamageBuffer a, DamageBuffer b)
        {
            return !(a == b);
        }

        public float Summ => _brute + _burn + _toxin + _suffocation;
    }
}

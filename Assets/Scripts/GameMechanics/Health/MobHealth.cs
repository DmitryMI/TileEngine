using System;
using UnityEngine;

namespace Assets.Scripts.GameMechanics.Health
{
    [Serializable]
    public class MobHealth
    {
        [SerializeField]
        protected DamageBuffer OverallDamage;

        [SerializeField]
        protected DamageBuffer ChestDamage;

        protected virtual void ModifyChestDamage(DamageBuffer damage)
        {
            OverallDamage += damage;
            ChestDamage += damage;
        }

        /// <summary>
        /// Modifies damage of specified limp
        /// </summary>
        /// <param name="damage">Modifying damage buffer</param>
        /// <param name="impactTarget">Impact target</param>
        /// <returns>Returns true, if this MobHealth supports requested impact target</returns>
        public virtual bool ModifyDamage(DamageBuffer damage, ImpactTarget impactTarget)
        {
            switch (impactTarget)
            {
                case ImpactTarget.Chest:
                    ModifyChestDamage(damage);
                    return true;
            }

            return false;
        }

        public virtual bool SupportsImpactTarget(ImpactTarget impactTarget)
        {
            switch (impactTarget)
            {
                case ImpactTarget.Chest:
                    return true;
            }

            return false;
        }

        public DamageBuffer GetOverallDamage() => OverallDamage;
        public DamageBuffer GetChestDamage() => ChestDamage;
    }
}

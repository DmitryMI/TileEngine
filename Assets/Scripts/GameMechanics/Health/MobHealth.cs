using System;
using Assets.Scripts.Objects.Mob;
using UnityEngine;

namespace Assets.Scripts.GameMechanics.Health
{
    [Serializable]
    public abstract class MobHealth
    {
        // Network syncables
        [Serializable]
        public struct ClientData
        {
            [SerializeField] public float DamagePercentagePerception;
            [SerializeField] public float NutritionPercentagePerception;
        }

        public ClientData NetHealthData;


        protected DamageBuffer OverallDamage;

        protected DamageBuffer ChestDamage;


        protected float NutritionMax;

        protected float NutritionCurrent;

        protected float NutritionDecrement;

        protected Mob Owner;


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
        public virtual bool ModifyDamage(DamageBuffer damage, ImpactLimb impactTarget)
        {
            switch (impactTarget)
            {
                case ImpactLimb.Chest:
                    ModifyChestDamage(damage);
                    return true;
            }

            return false;
        }

        public virtual bool SupportsImpactTarget(ImpactLimb impactTarget)
        {
            switch (impactTarget)
            {
                case ImpactLimb.Chest:
                    return true;
            }

            return false;
        }

        public DamageBuffer GetOverallDamage() => OverallDamage;

        public virtual void OnUpdate()
        {
            NutritionCurrent -= NutritionDecrement * Time.deltaTime;
            if (NutritionCurrent <= 0)
                NutritionCurrent = 0;

            NetHealthData.DamagePercentagePerception = GetDamagePercentageByFeelings();
            NetHealthData.NutritionPercentagePerception = GetNutritionPercentageByFeelings();
        }

        public virtual void OnStart()
        {

        }

        public MobHealth(Mob owner)
        {
            Owner = owner;
        }

        public abstract bool IsCritical { get; }

        public abstract bool CanStandOnLegs { get; }

        protected virtual float GetDamagePercentageByFeelings() => 1.0f - OverallDamage.Summ / 100.0f;
        protected virtual float GetNutritionPercentageByFeelings() => NutritionCurrent / NutritionMax;
    }
}

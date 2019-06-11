using System;
using UnityEngine;

namespace Assets.Scripts.GameMechanics.Health
{
    [Serializable]
    public class MobHealth
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

        protected virtual float GetDamagePercentageByFeelings() => 1.0f - OverallDamage.Summ / 100.0f;
        protected virtual float GetNutritionPercentageByFeelings() => NutritionCurrent / NutritionMax;
    }
}

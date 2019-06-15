using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.GameMechanics;
using Assets.Scripts.GameMechanics.Chemistry;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Objects.Item.Food
{
    class FoodPiece : Item, IFoodConsumable, IImpactHandler
    {
        [SerializeField] protected float NutritionValue = 20.0f;
        [SerializeField] protected float MaximumReagentVolume = 10.0f;
        [SerializeField] protected float TransferAmount = 5.0f;

        protected ISubstanceContainer SubstanceContainer;

        protected override void Start()
        {
            base.Start();

            InitSubstanceContainer();
        }

        protected virtual void InitSubstanceContainer()
        {
            SubstanceContainer = new SimpleSubstanceContainer(MaximumReagentVolume, TransferAmount);
        }

        public virtual float RemainingVolume => SubstanceContainer.RemainingVolume;
        public virtual float MaximumVolume => SubstanceContainer.MaximumVolume;

        public void TransferInto(SubstanceMixture mixture)
        {
            SubstanceContainer.TransferInto(mixture);
        }

        public void TransferToAnother(ISubstanceContainer container)
        {
            Debug.LogWarning("Consider is hamburger can transfer anything to other containers");
            SubstanceContainer.TransferToAnother(container);
        }

        public SubstanceMixture Contents => SubstanceContainer.Contents;

        public virtual float Nutrition => NutritionValue;


        public void OnImpact(IPlayerImpactable target, Intent intent, ImpactLimb impactTarget)
        {
            Mob.Mob user = ItemHolder as Mob.Mob;

            if (user == null)
            {
                Debug.LogError("Can item be used if it has no holder?");
                return;
            }

            if (user.Equals(target))
            {
                // Holder wants to eat this piece of food
                user.Health.ModifyNutrition(Nutrition);
                
                user.Health.DigestiveSystem.TransferInto(SubstanceContainer.Contents);

                Debug.Log("Yummi yummi!");
                NetworkServer.Destroy(this.gameObject);
                Destroy(gameObject);
            }
            else
            {
                // TODO Holder wants to feed another mob
            }
        }
    }
}

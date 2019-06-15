using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.GameMechanics.Chemistry.Reactions;
using UnityEngine;

namespace Assets.Scripts.GameMechanics.Chemistry
{
    class SimpleSubstanceContainer : ISubstanceContainer
    {
        private float _maxVolume;
        private float _transferAmount;

        private SubstanceMixture Mixture = new SubstanceMixture();

        public SimpleSubstanceContainer(float maxVolume, float transferAmount)
        {
            _maxVolume = maxVolume;
            _transferAmount = transferAmount;
        }

        public void DoReactions()
        {
            Reaction.React(Mixture);
        }

        public GameObject gameObject { get; }
        public float RemainingVolume => _maxVolume - Mixture.Volume;
        public float MaximumVolume => _maxVolume;

        public SubstanceMixture Contents
        {
            get { return Mixture; }
        }


        public void TransferInto(SubstanceMixture incomingMixture)
        {
            float remainingVolume = RemainingVolume;

            SubstanceMixture concatinationMixture;

            if (remainingVolume >= incomingMixture.Volume)
            {
                concatinationMixture = incomingMixture.Clone() as SubstanceMixture;
                incomingMixture.Clear();
            }
            else
            {
                concatinationMixture = incomingMixture.SubtractVolume(remainingVolume);
            }

            Mixture.Concatinate(concatinationMixture);

            Debug.Log(gameObject?.name + ": New contents: " + Mixture);
        }

        public void TransferToAnother(ISubstanceContainer otherContainer)
        {
            Debug.Log(gameObject.name + ": transfering liquids to " + otherContainer.gameObject);

            float amount = Mathf.Min(_transferAmount, Mixture.Volume);

            if (Math.Abs(amount) < 0.0001f)
            {
                Debug.Log("Nothing to transfer!");
            }
            else
            {
                Debug.Log("Amount: " + amount);
                SubstanceMixture subtractedMixture = Mixture.SubtractVolume(amount);

                otherContainer.TransferInto(subtractedMixture);

                Mixture.Concatinate(subtractedMixture);
            }
        }
    }
}

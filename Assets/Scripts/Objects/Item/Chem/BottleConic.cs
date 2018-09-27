using System;
using System.Collections.Generic;
using Assets.Scripts.Controllers;
using Assets.Scripts.GameMechanics.Chemistry;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Objects.Item.Chem
{
    public class BottleConic : Item, ISubstanceContainer
    {
        [SerializeField] private VolumeFiller _filler;
        [SerializeField] [SyncVar] private Color _color;

        [SerializeField] private float _maxVolume;

        private SubstanceMixture _mixture = new SubstanceMixture();

        [SerializeField] private float _transferAmount = 5;

        [SerializeField] [SyncVar] private float _volume;
        
        protected override void Start()
        {
            base.Start();
        }

        protected override void Update()
        {
            base.Update();

            if(isServer)
                UpdateColorAndVolume();

            UpdateFiller();
        }

        private void UpdateFiller()
        {
            _filler.FillValue = _volume / _maxVolume;

            _filler.ReagentsColor = _color;

            if (Holder != null)
            {
                _filler.gameObject.SetActive(false);
            }
            else
            {
                _filler.gameObject.SetActive(true);
            }
        }

        [Server]
        private void UpdateColorAndVolume()
        {
            if (ChemistryController.Current == null)
                return;

            _color = ChemistryController.Current.GetSubtanceColor(_mixture);
            _volume = _mixture.Volume;
        }

        public override void ApplyItemClient(Item item)
        {
            if (item == null)
            {
                PlayerActionController.Current.LocalPlayer.PickItem(this, PlayerActionController.Current.ActiveHand);
            }
            else
            {
                //.ApplyItemClient(item);
                PlayerActionController.Current.LocalPlayer.ApplyItem(item, this);
            }
        }

        public override void ApplyItemServer(Item item)
        {
            ISubstanceContainer container = item as ISubstanceContainer;
            
            container?.TransferToAnother(this);
        }

        public override string DescriptiveName => "Laboratory conic bottle";

        private float GetRemainingVolume()
        {
            return _maxVolume - _mixture.Volume;
        }

        public float RemainingVolume => GetRemainingVolume();
        public float MaximumVolume => _maxVolume;

        [Server]
        public void TransferInto(SubstanceMixture incomingMixture)
        {
            float remainingVolume = GetRemainingVolume();

            SubstanceMixture concatinationMixture;

            if (remainingVolume >= incomingMixture.Volume)
            {
                concatinationMixture = incomingMixture.Clone() as SubstanceMixture;
                incomingMixture.Clear();
            }
            else
            {
                concatinationMixture = new SubstanceMixture();

                //foreach (var substanceInfo in incomingMixture)
                for(int i = 0; i < incomingMixture.Count; i++)
                {
                    SubstanceInfo substanceInfo = incomingMixture[i];
                    float partInMixture = incomingMixture.GetElementPart(i);
                    SubstanceInfo nInfo = substanceInfo;
                    nInfo.Volume *= partInMixture * remainingVolume;
                    substanceInfo.Volume -= nInfo.Volume;
                    concatinationMixture.Add(nInfo);

                    if(substanceInfo.Volume > 0)
                        incomingMixture[i] = substanceInfo;
                    else
                        incomingMixture.RemoveAt(i);
                }
            }

            _mixture.Concatinate(concatinationMixture);

            Debug.Log(gameObject.name + ": New contents: " + _mixture);
        }

        [Server]
        public void TransferToAnother(ISubstanceContainer otherContainer)
        {
            Debug.Log(gameObject.name + ": transfering liquids to " + otherContainer.gameObject);

            float amount = Mathf.Min(_transferAmount, _mixture.Volume);

            if (Math.Abs(amount) < 0.0001f)
            {
                Debug.Log("Nothing to transfer!");
            }
            else
            {

                SubstanceMixture subtractedMixture = _mixture.SubtractVolume(amount);

                otherContainer.TransferInto(subtractedMixture);

                _mixture.Concatinate(subtractedMixture);
            }
        }
    }
}

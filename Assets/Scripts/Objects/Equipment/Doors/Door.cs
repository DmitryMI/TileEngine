using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Objects.Equipment.Power;
using UnityEngine;

namespace Assets.Scripts.Objects.Equipment.Doors
{
    public abstract class Door : Equipment, IPlayerInteractable, IWirelessPowerable
    {
        protected bool Electrified = false;
        protected int PrevElectrificationFrame;

        public abstract void ApplyItemClient(Item.Item item);

        public abstract void ApplyItemServer(Item.Item item);

        public abstract void TryToPass();

        // Must be called before LateUpdate
        public abstract float PowerNeeded { get; }

        public void Electrify()
        {
            Electrified = true;
            PrevElectrificationFrame = Time.frameCount;
        }

        protected override void Update()
        {
            base.Update();

            if (Time.frameCount - PrevElectrificationFrame > 1)
            {
                Electrified = false;
            }
        }
        
    }
}

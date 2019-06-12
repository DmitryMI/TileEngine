using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.GameMechanics;
using Assets.Scripts.Objects.Equipment.Power;
using UnityEngine;

namespace Assets.Scripts.Objects.Equipment.Doors
{
    public abstract class Door : Equipment, IPlayerApplicable, IWirelessPowerable
    {
        protected bool Electrified = false;
        protected int PrevElectrificationFrame;

        public abstract void ApplyItemClient(Item.Item item, Intent intent);

        public abstract void ApplyItemServer(Item.Item item, Intent intent);

        public abstract void TryToPass();

        // Must be called before LateUpdate
        public abstract float PowerNeeded { get; }

        public void Electrify()
        {
            Electrified = true;
            PrevElectrificationFrame = Time.frameCount;
        }


        public PowerablePriority Priority => PowerablePriority.Containment;

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

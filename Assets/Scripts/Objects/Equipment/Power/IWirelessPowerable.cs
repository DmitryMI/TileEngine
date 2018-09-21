using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Objects.Equipment.Power
{
    interface IWirelessPowerable
    {
        // Must be called before LateUpdate

        float PowerNeeded { get; }
        void Electrify();

        GameObject gameObject { get; }
    }
}

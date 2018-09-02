using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Objects.Equipment.Power
{
    public interface IPowerConsumer
    {
        GameObject gameObject { get; }

        void SendPower(float power);

        float AmountOfNeededPower { get; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.GameMechanics
{
    public interface ILightInfo
    {

        ICellPositionProvider PositionProvider { get; }

        Color LightColor { get; }

        float Range { get; }

        float Decrement { get; }

        float InitialIntensity { get; }

    }
}

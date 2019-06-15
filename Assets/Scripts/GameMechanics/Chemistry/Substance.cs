using System;
using UnityEngine;

namespace Assets.Scripts.GameMechanics.Chemistry
{
    [Serializable]
    public struct Substance
    {
        public static Substance IncorrectSubstance = new Substance(){Id = SubstanceId.IncorrectSubstance};

        public SubstanceId Id;
        public string Name;
        public Color Color;
    }
}

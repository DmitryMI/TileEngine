using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.GameMechanics.Chemistry
{
    [Serializable]
    public struct Substance
    {
        public static Substance IncorrectSubstance = new Substance(){Id = -1};

        public int Id;
        public string Name;
        public Color Color;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.GameMechanics.Chemistry.Reactions
{
    struct Reagent
    {
        public string SubstanceName;
        public int Mole;

        public Reagent(string name, int mole)
        {
            SubstanceName = name;
            Mole = mole;
        }
    }
}

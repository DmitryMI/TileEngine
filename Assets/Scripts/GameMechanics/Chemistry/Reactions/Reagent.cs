using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.GameMechanics.Chemistry.Reactions
{
    struct Reagent
    {
        public SubstanceId SubstanceId;
        public int Mole;

        public Reagent(SubstanceId id, int mole)
        {
            SubstanceId = id;
            Mole = mole;
        }
    }
}

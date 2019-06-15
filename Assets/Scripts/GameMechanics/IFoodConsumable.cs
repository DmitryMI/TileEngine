using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.GameMechanics.Chemistry;

namespace Assets.Scripts.GameMechanics
{
    interface IFoodConsumable : ISubstanceContainer
    {
        float Nutrition { get; }
    }
}

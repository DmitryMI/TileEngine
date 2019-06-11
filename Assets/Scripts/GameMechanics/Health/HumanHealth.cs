using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.GameMechanics.Health
{
    class HumanHealth : HumanoidHealth
    {
        public override void OnStart()
        {
            NutritionMax = GlobalPreferences.Instance.HumanMaxNutrition;
            NutritionDecrement = GlobalPreferences.Instance.HumanNutritionDecrement;
            NutritionCurrent = GlobalPreferences.Instance.HumanNutritionInitial;
        }
    }
}

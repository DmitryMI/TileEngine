using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Objects.Mob;

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

        public override bool IsCritical => OverallDamage.Summ >= GlobalPreferences.Instance.HumanMaxHp - GlobalPreferences.Instance.CriticalHealthPointsBorder;
        public override bool CanStandOnLegs => !IsCritical;

        public HumanHealth(Mob owner) : base(owner)
        {
        }
    }
}

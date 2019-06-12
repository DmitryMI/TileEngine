using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.GameMechanics.Health
{
    class DogHealth : AnimalHealth
    {
        public override bool IsCritical => OverallDamage.Summ > 50.0f;
        public override bool CanStandOnLegs => !IsCritical;
    }
}

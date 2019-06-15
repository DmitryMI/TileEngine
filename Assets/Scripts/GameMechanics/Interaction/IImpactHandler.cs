using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.GameMechanics
{
    interface IImpactHandler
    {
        void OnImpact(IPlayerImpactable target, Intent intent, ImpactLimb impactTarget);
    }
}

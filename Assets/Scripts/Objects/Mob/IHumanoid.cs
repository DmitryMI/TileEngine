using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Objects.Mob
{
    interface IHumanoid
    {
        bool IsAlive { get; }

        Damage HeadDamage { get; set; }
        Damage NeckDamage { get; set; }
        Damage ChestDamage { get; set; }
        Damage LeftArmDamage { get; set; }
        Damage RightArmDamage { get; set; }
        Damage LeftWristDamage { get; set; }
        Damage RightWristDamage { get; set; }
        Damage LeftLegDamage { get; set; }
        Damage RightLegDamage { get; set; }
        Damage LeftFootDamage { get; set; }
        Damage RightFootDamage { get; set; }

        Damage TotalDamage { get; }
    }
}

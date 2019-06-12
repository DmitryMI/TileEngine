using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Objects.Item;
using Assets.Scripts.Objects.Mob;
using UnityEngine;

namespace Assets.Scripts.GameMechanics
{
    public interface IPlayerImpactable
    {
        // ReSharper disable once InconsistentNaming
        GameObject gameObject { get; }

        void ImpactItemClient(Mob impactSource, Item item, Intent intent, ImpactLimb impactTarget);

        void ImpactItemServer(Mob impactSource, Item item, Intent intent, ImpactLimb impactTarget);
    }
}

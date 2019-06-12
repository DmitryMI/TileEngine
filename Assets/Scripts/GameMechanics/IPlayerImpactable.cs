using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Objects.Item;
using UnityEngine;

namespace Assets.Scripts.GameMechanics
{
    interface IPlayerImpactable
    {
        // ReSharper disable once InconsistentNaming
        GameObject gameObject { get; }

        void ImpactItemClient(Item item, Intent intent, ImpactTarget impactTarget);

        void ImpactItemServer(Item item, Intent intent, ImpactTarget impactTarget);
    }
}

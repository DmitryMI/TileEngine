using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Objects.Item;
using Assets.Scripts.Objects.Mob;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts
{
    public interface IPlayerInteractable
    {
        // ReSharper disable once InconsistentNaming
        GameObject gameObject { get; }

        void ApplyItemClient(Item item);

        void ApplyItemServer(Item item);
    }
}

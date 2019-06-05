using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Objects.Mob;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.AI
{
#pragma warning disable 618
    abstract class AiController : NetworkBehaviour
#pragma warning restore 618
    {
        [SerializeField]
        protected Mob ControlledMob;

        protected virtual void Start()
        {
            if (!isServer)
                enabled = false;

            if (ControlledMob == null)
                ControlledMob = GetComponent<Mob>();
        }
    }
}

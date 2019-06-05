using System;
using Assets.Scripts.Controllers;
using Assets.Scripts.GameMechanics;
using Assets.Scripts.HumanAppearance;
using UnityEngine;
using UnityEngine.Networking;
using Assets.Scripts.Objects.Item;
using Assets.Scripts.Objects.Mob;

namespace Assets.Scripts._Legacy
{
    [Obsolete("Migrating to PlayerActionController")]
    public class Player : NetworkBehaviour
    {
        private string _descriptiveName = "Unnamed human";
        private bool _spawned;

        private Mob _controlledMob;

        

        private void Start()
        {
            _controlledMob = GetComponent<Mob>();

            if (!Spawned)
                _controlledMob.enabled = false;

            Assets.Scripts.Controllers.VisionController vc = _controlledMob.GetVisionController();

            Debug.LogWarning("Setting player's position...");
            vc.SetViewerPosition(_controlledMob);
        }


        private void Update()
        {
            if (!Spawned)
                _controlledMob.enabled = false;
            else
                _controlledMob.enabled = true;
        }


        

        public bool Spawned
        {
            get { return _spawned; }
            set { _spawned = value; }
        }
        



    }
}

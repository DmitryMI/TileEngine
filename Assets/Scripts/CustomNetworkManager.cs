using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Controllers;
using Assets.Scripts.Objects.Mob;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts
{
#pragma warning disable 618
    class CustomNetworkManager : NetworkManager
#pragma warning restore 618
    {

        private ServerController _serverController;

        private void Start()
        {
            _serverController = FindObjectOfType<ServerController>();
        }

#pragma warning disable 618
        public override void OnServerAddPlayer([NotNull] NetworkConnection conn, short playerControllerId)
#pragma warning restore 618
        {
            if (conn == null) throw new ArgumentNullException(nameof(conn));
            Debug.Log("Server is starting to add players: ");

            base.OnServerAddPlayer(conn, playerControllerId);

#pragma warning disable 618
            PlayerController playerController = conn.playerControllers[0];
#pragma warning restore 618

            if (_serverController == null)
            {
                _serverController = FindObjectOfType<ServerController>();
            }

            _serverController.SpawnPlayer(playerController.gameObject);

            _serverController.RequestTransformUpdate();
        }
    }
}

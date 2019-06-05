using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Controllers;
using Assets.Scripts.Objects.Mob;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts
{
    class CustomNetworkManager : NetworkManager
    {

        private ServerController _serverController;

        private void Start()
        {
            _serverController = FindObjectOfType<ServerController>();
        }

        public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
        {
            Debug.Log("Server is starting to add players: ");

            base.OnServerAddPlayer(conn, playerControllerId);

            PlayerController playerController = conn.playerControllers[0];

            if (_serverController == null)
            {
                _serverController = FindObjectOfType<ServerController>();
            }

            _serverController.SpawnPlayer(playerController.gameObject);

            _serverController.RequestTransformUpdate();
        }
    }
}

﻿using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Controllers
{
    public abstract class Controller : NetworkBehaviour, ILoadable
    {
        [SerializeField]
        private bool _wasLoaded;
        protected bool WasLoaded
        {
            get { return _wasLoaded; }
            set
            {
                _wasLoaded = value;
                if (_wasLoaded)
                {
                    Debug.Log(this.GetType().Name + " finished loading!");
                    ServerController.ReportLoadingFinished();
                }
            }
        }

        protected IServerDataProvider ServerController;

        public void RegistrateDataProvider(IServerDataProvider provider)
        {
            ServerController = provider;
        }

        public abstract void OnGameLoaded(IServerDataProvider controller);

        public virtual bool IsReady
        {
            get { return WasLoaded; }
        }
    }
}

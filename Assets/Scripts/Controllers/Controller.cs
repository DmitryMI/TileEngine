using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Controllers
{
    public abstract class Controller : NetworkBehaviour
    {
        private bool _wasLoaded;
        protected bool WasLoaded
        {
            get { return _wasLoaded; }
            set
            {
                _wasLoaded = value;
                if(_wasLoaded)
                    ServerController.Current.RequestLoadingFinished();
            }
        }

        protected ServerController ServerController;

        public abstract void OnGameLoaded(ServerController controller);

        public virtual bool IsReady
        {
            get { return WasLoaded; }
        }
    }
}

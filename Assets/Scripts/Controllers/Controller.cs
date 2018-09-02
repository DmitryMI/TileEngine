using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Controllers
{
    public abstract class Controller : NetworkBehaviour
    {
        protected bool WasLoaded = false;
        protected ServerController ServerController;

        public abstract void OnGameLoaded(ServerController controller);

        public virtual bool IsReady
        {
            get { return WasLoaded; }
        }
    }
}

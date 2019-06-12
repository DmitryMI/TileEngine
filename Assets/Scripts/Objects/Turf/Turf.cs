using Assets.Scripts.Objects.FloorSplashes;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Objects.Turf
{
    public abstract class Turf : TileObject
    {
        public override string DescriptiveName => "N/A";

        protected FloorSplash _splash;

        public void AddSplash(FloorSplash splash)
        {
            _splash = splash;
        }

        protected virtual void OnDestroy()
        {
            if(_splash != null)
                NetworkServer.Destroy(_splash.gameObject);
        }
    }
}

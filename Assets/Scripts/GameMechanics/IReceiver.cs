using UnityEngine;

namespace Assets.Scripts.GameMechanics
{
    public interface INetworkDataReceiver
    {
        // ReSharper disable once InconsistentNaming
        GameObject gameObject { get; }
        void ReceiveData(INetworkDataReceiver sender, byte[] data);
    }
}

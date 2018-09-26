using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    public interface INetworkDataReceiver
    {
        // ReSharper disable once InconsistentNaming
        GameObject gameObject { get; }
        void ReceiveData(INetworkDataReceiver sender, byte[] data);
    }
}

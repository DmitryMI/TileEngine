using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    public interface IReceiver
    {
        // ReSharper disable once InconsistentNaming
        GameObject gameObject { get; }
        void ReceiveData(IReceiver sender, object data);
    }
}

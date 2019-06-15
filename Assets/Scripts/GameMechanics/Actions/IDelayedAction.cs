using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.GameMechanics.Actions
{
    public interface IDelayedAction
    {
        void Abort();
        IEnumerator Coroutine { get; }
    }
}

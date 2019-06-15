using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.GameMechanics.Actions
{
    public interface IExclusiveActionManager
    {
        IDelayedAction StartAction(Action<object> action, Func<bool> abortConditionChecker, Action<object> abortHandler,
            object args, object abortHandlerArgs, float delay);

        void AbortAction();

        bool IsActionPending { get; }
    }
}

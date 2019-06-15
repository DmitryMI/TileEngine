using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.GameMechanics.Actions
{
    public class ExclusiveActionManager : IExclusiveActionManager
    {
        private IDelayedAction _delayedAction = null;
        private Action<object> _abortHandler;
        private object _abortHandlerArgs;
        

        public ExclusiveActionManager()
        {

        }

        public IDelayedAction StartAction(Action<object> action, Func<bool> abortConditionChecker, Action<object> abortHandler, object args, object abortHandlerArgs, float delay)
        {
            AbortAction();

            _abortHandlerArgs = abortHandlerArgs;
            _abortHandler = abortHandler;
            _delayedAction = new DelayedAction(action, abortConditionChecker, FinishReportHandler, args, delay);
            return _delayedAction;
        }

        public void AbortAction()
        {
            if (_delayedAction != null)
            {
                _delayedAction.Abort();
                _abortHandler?.Invoke(_abortHandlerArgs);
            }
        }

        private void FinishReportHandler(bool wasAborted)
        {
            _delayedAction = null;
        }

        public bool IsActionPending => _delayedAction != null;
    }
}

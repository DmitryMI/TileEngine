using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.GameMechanics.Actions
{
    class DelayedAction : IDelayedAction
    {
        private Action<object> _action;
        private Action<bool> _finishReport;
        private Func<bool> _abortConditionChecker;
        private object _args;
        private float _timeLeft;
        private bool _abort;

        public DelayedAction(Action<object> action, Func<bool> abortConditionChecker, Action<bool> report,  object args, float delay)
        {
            _timeLeft = delay;
            _args = args;
            _action = action;
            _abortConditionChecker = abortConditionChecker;

            _finishReport = report;
        }

        public IEnumerator Coroutine => GetCoroutine();

        public void Abort()
        {
            _abort = true;
        }

        private IEnumerator GetCoroutine()
        {
            while (_timeLeft > 0 && !_abort)
            {
                _abort = _abortConditionChecker();
                _timeLeft -= Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }

            if (!_abort)
            {
                _action.Invoke(_args);
            }

            _finishReport.Invoke(!_abort);
        }
    }
}

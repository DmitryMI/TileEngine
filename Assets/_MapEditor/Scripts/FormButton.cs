using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Ui;
using UnityEngine;
using UnityEngine.Events;

namespace Assets._MapEditor.Scripts
{
    class FormButton : UiElement
    {
        [SerializeField] private UnityAction _action;

        private Form _formParent;

        public void SetParentForm(Form form)
        {
            _formParent = form;
        }

        public void SetAction(UnityAction action)
        {
            _action = action;
        }

        public override void Click()
        {
            _action?.Invoke();
        }
    }
}

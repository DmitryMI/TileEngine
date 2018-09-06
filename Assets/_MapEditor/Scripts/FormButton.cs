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
        [SerializeField] protected UnityAction Action;

        protected Form FormParent;

        public void SetParentForm(Form form)
        {
            FormParent = form;
        }

        public void SetAction(UnityAction action)
        {
            Action = action;
        }

        public override void Click()
        {
            Action?.Invoke();
        }
    }
}

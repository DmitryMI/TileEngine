using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Ui
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

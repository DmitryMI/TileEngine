using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Ui;

namespace Assets._MapEditor.Scripts
{
    class BackButton : UiElement
    {
        private EditorController _controller;
        public void SetHandler(EditorController controller)
        {
            _controller = controller;
        }

        public override void Click()
        {
            _controller.PressedBackButton();
        }
    }
}

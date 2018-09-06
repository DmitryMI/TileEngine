using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Objects;
using Assets.Scripts.Ui;

namespace Assets._MapEditor.Scripts
{
    class FileButton : UiElement
    {
        private EditorController _controller;
        private TileObject _prefab;

        public void SetData(EditorController controller, TileObject prefab)
        {
            _controller = controller;
            _prefab = prefab;
        }

        public override void Click()
        {
            _controller.PressedFileButton(_prefab);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Ui;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._MapEditor.Scripts
{
    class FolderButton : UiElement
    {
        [SerializeField] private Text _text;
        [SerializeField] private Image _image;

        private EditorController _controller;
        private GameType _type;
        private GameType _base;

        public void SetData(EditorController controller, GameType type, GameType typeParent)
        {
            _controller = controller;
            _type = type;
            _base = typeParent;
            _text.text = type.Type.Name;
        }

        public override void Click()
        {
            _controller.PressedFolderButton(_type);
        }
    }
}

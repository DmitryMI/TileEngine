using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Ui
{
    class ColoredFormButton : FormButton
    {

        public Color BackGroundColor
        {
            get { return _backgroundColor; }
            set { _backgroundColor = value; }
        }

        private Image _renderer;
        private Color _backgroundColor;

        private  void Start()
        {
            _renderer = GetComponent<Image>();
        }

        protected void Update()
        {
            _renderer.color = _backgroundColor;
        }
    }
}

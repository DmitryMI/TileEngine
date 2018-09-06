using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Ui
{
    [RequireComponent(typeof(RectTransform))]
    class UiVerticalScroller : UiElement, IScroller
    {
        [SerializeField]
        private float _mouseMultiplier;

        [SerializeField]
        private float _value;

        [SerializeField] private UiVerticalScroller _scroller;

        public float ScrollValue { get { return _value; } set { _value = value; StickForce(_value); } }
        public float ContentOveflowPart { get; set; }

        public float Multiplier => _mouseMultiplier;

        public RectTransform RectTransformComponent => GetComponent<RectTransform>();

        public override void Click()
        {
            
        }

        private void StickForce(float value)
        {
            
        }
    }
}

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

        [SerializeField] private UiVerticalScrollerStick _scrollerStick;

        public float ScrollValue { get { return _value; } set { _value = value; StickForce(_value); } }
        public float ContentOveflowPart { get; set; }

        public float Multiplier => _mouseMultiplier;

        public RectTransform RectTransformComponent => GetComponent<RectTransform>();

        private void Start()
        {
            _scrollerStick = GetComponentInChildren<UiVerticalScrollerStick>();

            StickForce(_value);
        }

        public override void Click()
        {
            
        }

        public void SetValueStickAuthority(float value)
        {
            _value = value;
        }

        private void StickForce(float value)
        {
            _scrollerStick.ForceValue(value);
        }
    }
}

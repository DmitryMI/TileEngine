using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.HumanAppearance
{
    public class HairSet : MonoBehaviour
    {
        [SerializeField] private int _hairSetId;

        [SerializeField]
        private Sprite _frontSprite;
        [SerializeField]
        private Sprite _backSprite;
        [SerializeField]
        private Sprite _leftSprite;
        [SerializeField]
        private Sprite _rightSprite;
        [SerializeField] private Vector2 _frontOffset;
        [SerializeField] private Vector2 _backOffset;
        [SerializeField] private Vector2 _leftOffset;
        [SerializeField] private Vector2 _rightOffset;

        public Sprite Front { get { return _frontSprite; } }
        public Sprite Back { get { return _backSprite; } }
        public Sprite Left { get { return _leftSprite; } }
        public Sprite Right { get { return _rightSprite; } }

        public Vector2 FrontOffset
        {
            get { return _frontOffset; }
        }
        public Vector2 BackOffset
        {
            get { return _backOffset; }
        }

        public Vector2 LeftOffset
        {
            get { return _leftOffset; }
        }

        public Vector2 RightOffset
        {
            get { return _rightOffset; }
        }

        public int Id
        {
            get { return _hairSetId; }
        }
    }
}

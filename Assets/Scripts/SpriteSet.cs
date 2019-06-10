using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    [Serializable]
    public struct SpriteSet
    {
        [SerializeField]
        private int _currentSpriteIndex;

        [SerializeField]
        private List<Sprite> _sprites;

        [SerializeField] private bool _animationEnabled;

        [SerializeField]
        private float _spriteDuration;

        private float _time;

        public void NextSprite()
        {
            _currentSpriteIndex++;
            if (_currentSpriteIndex >= _sprites.Count)
                _currentSpriteIndex = 0;
        }

        public Sprite CurrentSprite => _sprites[_currentSpriteIndex];

        public void OnUpdate()
        {
            _time += Time.deltaTime;
            if (_time >= _spriteDuration)
            {
                if (_animationEnabled)
                {
                    NextSprite();
                }

                _time = 0;
            }
        }

        public void ResetAnimation()
        {
            _currentSpriteIndex = 0;
            _time = 0;
        }
    }
}

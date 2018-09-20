using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Controllers;
using Assets.Scripts.Objects.Equipment.Tables;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts
{
    [Serializable]
    class SpatialVariativity
    {
        [SerializeField] private Sprite _idle;
        [SerializeField]
        private Sprite _longTop;
        [SerializeField]
        private Sprite _longBot;
        [SerializeField]
        private Sprite _longLeft;
        [SerializeField]
        private Sprite _longRight;

        [SerializeField] private Sprite _longTopRight;
        [SerializeField] private Sprite _longTopLeft;
        [SerializeField] private Sprite _longTopBot;
        [SerializeField] private Sprite _longLeftRight;

        [SerializeField] private Sprite _longBotLeft;
        [SerializeField] private Sprite _longBotRight;

        [SerializeField] private Sprite _longTopLeftRight;
        [SerializeField] private Sprite _longBotLeftRight;

        [SerializeField] private Sprite _longLeftTopBot;
        [SerializeField] private Sprite _longRightTopBot;

        [SerializeField] private Sprite _longTopBotLeftRight;

        public Sprite GetSprite(bool _isLongLeft, bool _isLongRight, bool _isLongTop, bool _isLongBot)
        {
            if (!_isLongLeft && !_isLongBot && !_isLongTop && !_isLongRight)
            {
                return _idle;
            }

            if (_isLongLeft && _isLongRight && _isLongTop && _isLongBot)
            {
                return _longTopBotLeftRight;
            }

            if (_isLongLeft && _isLongRight && _isLongBot)
            {
                return _longBotLeftRight;
            }

            if (_isLongLeft && _isLongRight && _isLongTop)
            {
                return _longTopLeftRight;
            }

            if (_isLongBot && _isLongRight && _isLongTop)
            {
                return _longRightTopBot;
            }

            if (_isLongLeft && _isLongTop && _isLongBot)
            {
                return _longLeftTopBot;
            }

            if (_isLongLeft && _isLongTop)
            {
                return _longTopLeft;
            }

            if (_isLongRight && _isLongTop)
            {
                return _longTopRight;
            }

            if (_isLongTop && _isLongBot)
            {
                return _longTopBot;
            }

            if (_isLongLeft && _isLongRight)
            {
                return _longLeftRight;
            }

            if (_isLongBot && _isLongLeft)
            {
                return _longBotLeft;
            }

            if (_isLongBot && _isLongRight)
            {
                return _longBotRight;
            }

            if (_isLongLeft)
                return _longLeft;
            if (_isLongRight)
                return _longRight;
            if (_isLongTop)
                return _longTop;
            if (_isLongBot)
                return _longBot;

            return null;
        }

        

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.GameMechanics;
using Assets.Scripts.Objects.Mob.Humanoids;
using UnityEngine;

namespace Assets.Scripts.HumanAppearance
{
    class BodyEffectHandler : MonoBehaviour
    {
        [Header("Effect sprites")]
        [SerializeField] private SpriteSet _forwardSprite;
        [SerializeField] private SpriteSet _backwardSprite;
        [SerializeField] private SpriteSet _leftSideSprite;
        [SerializeField] private SpriteSet _rightSideSprite;

        [Header("Masks for forward and backward directions")] [SerializeField]
        private Sprite _headMaskStraightSprite;

        [SerializeField] private Sprite _chestMaskStraightSprite;
        [SerializeField] private Sprite _leftArmMaskStraightSprite;
        [SerializeField] private Sprite _rightArmMaskStraightSprite;
        [SerializeField] private Sprite _leftLegMaskStraightSprite;
        [SerializeField] private Sprite _rightLegMaskStraightSprite;

        [Header("Masks for left and right directions")] [SerializeField]
        private Sprite _headMaskLeftSprite;

        [SerializeField] private Sprite _headMaskRightSprite;
        [SerializeField] private Sprite _chestMaskLeftSprite;
        [SerializeField] private Sprite _chestMaskRightSprite;
        [SerializeField] private Sprite _armMaskLeftSprite;
        [SerializeField] private Sprite _armMaskRightSprite;
        [SerializeField] private Sprite _legMaskLeftSprite;
        [SerializeField] private Sprite _legMaskRightSprite;


        [Header("SpriteMasks")] [SerializeField]
        private SpriteMask _headMask;

        [SerializeField] private SpriteMask _chestMask;
        [SerializeField] private SpriteMask _leftHandMask;
        [SerializeField] private SpriteMask _rightHandMask;
        [SerializeField] private SpriteMask _leftLegMask;
        [SerializeField] private SpriteMask _rightLegMask;

        [Header("Debug. Should be removed")] [SerializeField]
        private bool _headBloody;

        [SerializeField] private bool _chestBloody;
        [SerializeField] private bool _leftArmBloody;
        [SerializeField] private bool _rightArmBloody;
        [SerializeField] private bool _leftLegBloody;
        [SerializeField] private bool _rightLegBloody;

        private Humanoid _humanoid;
        private SpriteRenderer _spriteRenderer;

        private void Start()
        {
            //_spriteRenderer = GetComponent<SpriteRenderer>();
            _spriteRenderer = GetComponentInParent<SpriteRenderer>();
            _humanoid = GetComponentInParent<Humanoid>();
        }


        private void Update()
        {
            _spriteRenderer.sortingOrder = _humanoid.SortingOrder + 1;

            _headMask.frontSortingOrder = _spriteRenderer.sortingOrder;
            _chestMask.frontSortingOrder = _spriteRenderer.sortingOrder;
            _leftLegMask.frontSortingOrder = _spriteRenderer.sortingOrder;
            _rightLegMask.frontSortingOrder = _spriteRenderer.sortingOrder;
            _leftHandMask.frontSortingOrder = _spriteRenderer.sortingOrder;
            _rightHandMask.frontSortingOrder = _spriteRenderer.sortingOrder;

            Direction direction = _humanoid.SpriteOrientation;

            switch (direction)
            {
                case Direction.Forward:
                    _spriteRenderer.sprite = _forwardSprite.CurrentSprite;
                    break;
                case Direction.Backward:
                    _spriteRenderer.sprite = _backwardSprite.CurrentSprite;
                    break;
                case Direction.Left:
                    _spriteRenderer.sprite = _leftSideSprite.CurrentSprite;
                    break;
                case Direction.Right:
                    _spriteRenderer.sprite = _rightSideSprite.CurrentSprite;
                    break;
            }

            if (_headBloody)
            {
                _headMask.enabled = true;
                switch (direction)
                {
                    case Direction.Forward:
                    case Direction.Backward:
                        _headMask.sprite = _headMaskStraightSprite;
                        break;
                    case Direction.Left:
                        _headMask.sprite = _headMaskLeftSprite;
                        break;
                    case Direction.Right:
                        _headMask.sprite = _headMaskLeftSprite;
                        break;
                }
            }
            else
            {
                _headMask.enabled = false;
            }


            if (_chestBloody)
            {
                _chestMask.enabled = true;
                switch (direction)
                {
                    case Direction.Forward:
                    case Direction.Backward:
                        _chestMask.sprite = _chestMaskStraightSprite;
                        break;
                    case Direction.Left:
                        _chestMask.sprite = _chestMaskLeftSprite;
                        break;
                    case Direction.Right:
                        _chestMask.sprite = _chestMaskRightSprite;
                        break;
                }
            }
            else
            {
                _chestMask.enabled = false;
            }

            if (_leftArmBloody)
            {
                
                switch (direction)
                {
                    case Direction.Forward:
                        _leftHandMask.enabled = true;
                        _leftHandMask.sprite = _leftArmMaskStraightSprite;
                        break;
                    case Direction.Backward:
                        _leftHandMask.enabled = true;
                        _leftHandMask.sprite = _rightArmMaskStraightSprite;
                        break;
                    case Direction.Left:
                        _leftHandMask.enabled = true;
                        _leftHandMask.sprite = _armMaskLeftSprite;
                        break;
                    case Direction.Right:
                        _leftHandMask.enabled = false;
                        break;
                }
            }
            else
            {
                _leftHandMask.enabled = false;
            }

            if (_rightArmBloody)
            {

                switch (direction)
                {
                    case Direction.Forward:
                        _rightHandMask.enabled = true;
                        _rightHandMask.sprite = _rightArmMaskStraightSprite;
                        break;
                    case Direction.Backward:
                        _rightHandMask.enabled = true;
                        _rightHandMask.sprite = _leftArmMaskStraightSprite;
                        break;
                    case Direction.Left:
                        _rightHandMask.enabled = false;
                        break;
                    case Direction.Right:
                        _rightHandMask.enabled = true;
                        _rightHandMask.sprite = _armMaskRightSprite;
                        break;
                }
            }
            else
            {
                _rightHandMask.enabled = false;
            }

            if (_leftLegBloody)
            {

                switch (direction)
                {
                    case Direction.Forward:
                        _leftLegMask.enabled = true;
                        _leftLegMask.sprite = _leftLegMaskStraightSprite;
                        break;
                    case Direction.Backward:
                        _leftLegMask.enabled = true;
                        _leftLegMask.sprite = _leftLegMaskStraightSprite;
                        break;
                    case Direction.Left:
                        _leftLegMask.enabled = true;
                        _leftLegMask.sprite = _legMaskLeftSprite;
                        break;
                    case Direction.Right:
                        _leftLegMask.enabled = false;
                        break;
                }
            }
            else
            {
                _leftLegMask.enabled = false;
            }

            if (_rightLegBloody)
            {

                switch (direction)
                {
                    case Direction.Forward:
                        _rightLegMask.enabled = true;
                        _rightLegMask.sprite = _rightLegMaskStraightSprite;
                        break;
                    case Direction.Backward:
                        _rightLegMask.enabled = true;
                        _rightLegMask.sprite = _rightLegMaskStraightSprite;
                        break;
                    case Direction.Left:
                        _rightLegMask.enabled = false;
                        break;
                    case Direction.Right:
                        _rightLegMask.enabled = true;
                        _rightLegMask.sprite = _armMaskRightSprite;
                        break;
                }
            }
            else
            {
                _rightLegMask.enabled = false;
            }

        }


    }
}

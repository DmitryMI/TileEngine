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
        [SerializeField] protected SpriteSet ForwardSprite;
        [SerializeField] protected SpriteSet BackwardSprite;
        [SerializeField] protected SpriteSet LeftSideSprite;
        [SerializeField] protected SpriteSet RightSideSprite;

        [Header("Masks for forward and backward directions")]
        [SerializeField] protected Sprite HeadMaskStraightSprite;
        [SerializeField] protected Sprite ChestMaskStraightSprite;
        [SerializeField] protected Sprite LeftArmMaskStraightSprite;
        [SerializeField] protected Sprite RightArmMaskStraightSprite;
        [SerializeField] protected Sprite LeftLegMaskStraightSprite;
        [SerializeField] protected Sprite RightLegMaskStraightSprite;

        [Header("Masks for left and right directions")]
        [SerializeField] protected Sprite HeadMaskLeftSprite;
        [SerializeField] protected Sprite HeadMaskRightSprite;
        [SerializeField] protected Sprite ChestMaskLeftSprite;
        [SerializeField] protected Sprite ChestMaskRightSprite;
        [SerializeField] protected Sprite ArmMaskLeftSprite;
        [SerializeField] protected Sprite ArmMaskRightSprite;
        [SerializeField] protected Sprite LegMaskLeftSprite;
        [SerializeField] protected Sprite LegMaskRightSprite;


        [Header("SpriteMasks")]
        [SerializeField] protected SpriteMask HeadMask;
        [SerializeField] protected SpriteMask ChestMask;
        [SerializeField] protected SpriteMask LeftHandMask;
        [SerializeField] protected SpriteMask RightHandMask;
        [SerializeField] protected SpriteMask LeftLegMask;
        [SerializeField] protected SpriteMask RightLegMask;

        [Header("Debug. Should be removed")]
        [SerializeField] protected bool HeadEffect;
        [SerializeField] protected bool ChestEffect;
        [SerializeField] protected bool LeftArmEffect;
        [SerializeField] protected bool RightArmEffect;
        [SerializeField] protected bool LeftLegEffect;
        [SerializeField] protected bool RightLegEffect;

        protected Humanoid Humanoid;
        protected SpriteRenderer SpriteRenderer;

        protected virtual void Start()
        {
            //_spriteRenderer = GetComponent<SpriteRenderer>();
            SpriteRenderer = GetComponentInParent<SpriteRenderer>();
            Humanoid = GetComponentInParent<Humanoid>();
        }


        protected virtual void Update()
        {
            UpdateEffect();
        }

        protected virtual void UpdateEffect()
        {
            SpriteRenderer.sortingOrder = Humanoid.SortingOrder + 1;

            HeadMask.frontSortingOrder = SpriteRenderer.sortingOrder;
            ChestMask.frontSortingOrder = SpriteRenderer.sortingOrder;
            LeftLegMask.frontSortingOrder = SpriteRenderer.sortingOrder;
            RightLegMask.frontSortingOrder = SpriteRenderer.sortingOrder;
            LeftHandMask.frontSortingOrder = SpriteRenderer.sortingOrder;
            RightHandMask.frontSortingOrder = SpriteRenderer.sortingOrder;

            Direction direction = Humanoid.SpriteOrientation;

            switch (direction)
            {
                case Direction.Forward:
                    SpriteRenderer.sprite = ForwardSprite.CurrentSprite;
                    break;
                case Direction.Backward:
                    SpriteRenderer.sprite = BackwardSprite.CurrentSprite;
                    break;
                case Direction.Left:
                    SpriteRenderer.sprite = LeftSideSprite.CurrentSprite;
                    break;
                case Direction.Right:
                    SpriteRenderer.sprite = RightSideSprite.CurrentSprite;
                    break;
            }

            if (HeadEffect)
            {
                HeadMask.enabled = true;
                switch (direction)
                {
                    case Direction.Forward:
                    case Direction.Backward:
                        HeadMask.sprite = HeadMaskStraightSprite;
                        break;
                    case Direction.Left:
                        HeadMask.sprite = HeadMaskLeftSprite;
                        break;
                    case Direction.Right:
                        HeadMask.sprite = HeadMaskRightSprite;
                        break;
                }
            }
            else
            {
                HeadMask.enabled = false;
            }


            if (ChestEffect)
            {
                ChestMask.enabled = true;
                switch (direction)
                {
                    case Direction.Forward:
                    case Direction.Backward:
                        ChestMask.sprite = ChestMaskStraightSprite;
                        break;
                    case Direction.Left:
                        ChestMask.sprite = ChestMaskLeftSprite;
                        break;
                    case Direction.Right:
                        ChestMask.sprite = ChestMaskRightSprite;
                        break;
                }
            }
            else
            {
                ChestMask.enabled = false;
            }

            if (LeftArmEffect)
            {

                switch (direction)
                {
                    case Direction.Forward:
                        LeftHandMask.enabled = true;
                        LeftHandMask.sprite = LeftArmMaskStraightSprite;
                        break;
                    case Direction.Backward:
                        LeftHandMask.enabled = true;
                        LeftHandMask.sprite = RightArmMaskStraightSprite;
                        break;
                    case Direction.Left:
                        LeftHandMask.enabled = true;
                        LeftHandMask.sprite = ArmMaskLeftSprite;
                        break;
                    case Direction.Right:
                        LeftHandMask.enabled = false;
                        break;
                }
            }
            else
            {
                LeftHandMask.enabled = false;
            }

            if (RightArmEffect)
            {

                switch (direction)
                {
                    case Direction.Forward:
                        RightHandMask.enabled = true;
                        RightHandMask.sprite = RightArmMaskStraightSprite;
                        break;
                    case Direction.Backward:
                        RightHandMask.enabled = true;
                        RightHandMask.sprite = LeftArmMaskStraightSprite;
                        break;
                    case Direction.Left:
                        RightHandMask.enabled = false;
                        break;
                    case Direction.Right:
                        RightHandMask.enabled = true;
                        RightHandMask.sprite = ArmMaskRightSprite;
                        break;
                }
            }
            else
            {
                RightHandMask.enabled = false;
            }

            if (LeftLegEffect)
            {

                switch (direction)
                {
                    case Direction.Forward:
                        LeftLegMask.enabled = true;
                        LeftLegMask.sprite = LeftLegMaskStraightSprite;
                        break;
                    case Direction.Backward:
                        LeftLegMask.enabled = true;
                        LeftLegMask.sprite = LeftLegMaskStraightSprite;
                        break;
                    case Direction.Left:
                        LeftLegMask.enabled = true;
                        LeftLegMask.sprite = LegMaskLeftSprite;
                        break;
                    case Direction.Right:
                        LeftLegMask.enabled = false;
                        break;
                }
            }
            else
            {
                LeftLegMask.enabled = false;
            }

            if (RightLegEffect)
            {

                switch (direction)
                {
                    case Direction.Forward:
                        RightLegMask.enabled = true;
                        RightLegMask.sprite = RightLegMaskStraightSprite;
                        break;
                    case Direction.Backward:
                        RightLegMask.enabled = true;
                        RightLegMask.sprite = RightLegMaskStraightSprite;
                        break;
                    case Direction.Left:
                        RightLegMask.enabled = false;
                        break;
                    case Direction.Right:
                        RightLegMask.enabled = true;
                        RightLegMask.sprite = LegMaskRightSprite;
                        break;
                }
            }
            else
            {
                RightLegMask.enabled = false;
            }
        }


    }
}

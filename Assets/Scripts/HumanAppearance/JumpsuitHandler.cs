using System;
using Assets.Scripts.Objects.Item;
using Assets.Scripts.Objects.Mob;
using UnityEngine;

namespace Assets.Scripts.HumanAppearance
{
    public class JumpsuitHandler : MonoBehaviour
    {
        private SpriteRenderer _spriteRenderer;
        private Player _player;
        

        void Start ()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _player = GetComponentInParent<Player>();
        }
	
        void Update ()
        {
            UpdateSprite();
        }

        private IWearable GetClothing()
        {
            // TODO Get clothing from Player object
            return GameObject.FindObjectOfType<GrayCostume>().GetComponent<IWearable>();
        }

        private void UpdateSprite()
        {
            IWearable clothing = GetClothing();

            if (clothing != null)
            {
                switch (_player.SpriteOrientation)
                {
                    case Direction.Forward:
                        _spriteRenderer.sprite = clothing.Back;
                        transform.localPosition = clothing.BackOffset;
                        break;
                    case Direction.Backward:
                        _spriteRenderer.sprite = clothing.Front;
                        transform.localPosition = clothing.FrontOffset;
                        break;
                    case Direction.Left:
                        _spriteRenderer.sprite = clothing.Left;
                        transform.localPosition = clothing.LeftOffset;
                        break;
                    case Direction.Right:
                        _spriteRenderer.sprite = clothing.Right;
                        transform.localPosition = clothing.RightOffset;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else
            {
                _spriteRenderer.sprite = null;
            }
        }
    }
}

using System;
using Assets.Scripts.Controllers;
using Assets.Scripts.GameMechanics;
using Assets.Scripts.Objects.Item;
using Assets.Scripts.Objects.Mob;
using UnityEngine;

namespace Assets.Scripts.HumanAppearance
{
    public class JumpsuitHandler : MonoBehaviour
    {
        private SpriteRenderer _spriteRenderer;
        private Player _player;

        private IWearable _clothing;

        void Start ()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _player = GetComponentInParent<Player>();
        }
	
        void Update ()
        {
            UpdateSprite();
            UpdateCurrentClothing();
        }

        void UpdateCurrentClothing()
        {
            
            Item item = _player.GetItemBySlot(SlotEnum.Costume);
            if (item != null && item is IWearable)
            {
                _clothing = (IWearable) item;
            }
            else
            {
                _clothing = null;
            }
        }


        private void UpdateSprite()
        {
            if (_clothing != null)
            {
                switch (_player.SpriteOrientation)
                {
                    case Direction.Forward:
                        _spriteRenderer.sprite = _clothing.Back;
                        transform.localPosition = _clothing.BackOffset;
                        break;
                    case Direction.Backward:
                        _spriteRenderer.sprite = _clothing.Front;
                        transform.localPosition = _clothing.FrontOffset;
                        break;
                    case Direction.Left:
                        _spriteRenderer.sprite = _clothing.Left;
                        transform.localPosition = _clothing.LeftOffset;
                        break;
                    case Direction.Right:
                        _spriteRenderer.sprite = _clothing.Right;
                        transform.localPosition = _clothing.RightOffset;
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

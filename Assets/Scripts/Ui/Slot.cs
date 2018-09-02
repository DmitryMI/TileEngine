using Assets.Scripts.Controllers;
using Assets.Scripts.Objects.Item;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Ui
{
    public class Slot : UiElement
    {
        [SerializeField]
        protected SlotEnum _slotEnum;

        [SerializeField] protected Image _itemImage;

        [SerializeField] protected Sprite _activeSprite;
        [SerializeField] protected Sprite _inactiveSprite;

        private Image _renderer;
        private RectTransform _rectTransform;
        private RectTransform _itemImageTransform;

        private void Start()
        {
            _renderer = GetComponent<Image>();
            _rectTransform = GetComponent<RectTransform>();
            _itemImageTransform = _itemImage.GetComponent<RectTransform>();
        }

        protected override void Update()
        {
            base.Update();

            if (LocalPlayer != null && _itemImage != null)
            {
                UpdateHeldItem();
            }

            PlayerActionController controller = PlayerActionController.Current;

            if (controller != null)
            {
                if (controller.ActiveHand == _slotEnum)
                    _renderer.sprite = _activeSprite;
                else
                    _renderer.sprite = _inactiveSprite;
            }
        }

        private void UpdateHeldItem()
        {
            IDisplayable displayable = LocalPlayer.GetItemBySlot(_slotEnum);

            if (displayable != null)
            {
                _itemImage.sprite = displayable.GetUiSprite();
                _itemImage.color = new Color(1, 1, 1, 1);
            }
            else
            {
                _itemImage.sprite = null;
                _itemImage.color = new Color(0, 0, 0, 0);
            }

            if (_itemImage.sprite != null)
            {
                var image = _itemImage.sprite.texture;
                _itemImageTransform.sizeDelta = FitSize(image, _rectTransform.sizeDelta);
            }
        }

        /*
            var image = PlayerHuman.Equipment[Equipment.EquipmentSlot.LeftHand].GetComponent<SpriteRenderer>()
                    .sprite.texture;
            LeftItemImage.rectTransform.sizeDelta = image.FitSize(new Vector2(55, 55));

            LeftItemImage.texture = image;
            LeftItemImage.color = new Color(LeftItemImage.color.r, LeftItemImage.color.g, LeftItemImage.color.b, 255f);
         
         */

        public static Vector2 FitSize(Texture2D image, Vector2 size)
        {

            var imageSize = new Vector2(image.width, image.height);

            if (imageSize.x > imageSize.y)
            {
                float coef = imageSize.y / imageSize.x;
                return new Vector2(size.x, size.y * coef);
            }
            else
            {
                float coef = imageSize.x / imageSize.y;
                return new Vector2(size.x * coef, size.y);
            }
        }

        

    }
}

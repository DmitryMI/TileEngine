using Assets.Scripts.Controllers;
using Assets.Scripts.GameMechanics;
using Assets.Scripts.Objects.Item;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Ui
{
    public class Slot : UiElement
    {
        [SerializeField] protected Vector2 _itemImageMaxSize;

        [SerializeField]
        protected SlotEnum _slotEnum;

        [SerializeField] protected Image _itemImage;

        [SerializeField] protected Sprite _activeSprite;
        [SerializeField] protected Sprite _inactiveSprite;
        [SerializeField] protected Color _uiColor = Color.white;
        [SerializeField] protected float _colorDepression = 0.5f;

        private Image _renderer;
        private RectTransform _rectTransform;
        private RectTransform _itemImageTransform;
        private Image _selfRenderer;

        private void Start()
        {
            _renderer = GetComponent<Image>();
            _rectTransform = GetComponent<RectTransform>();
            _itemImageTransform = _itemImage.GetComponent<RectTransform>();
            _selfRenderer = GetComponent<Image>();
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

                Color selfColor = _uiColor;
                selfColor.r *= _colorDepression;
                selfColor.g *= _colorDepression;
                selfColor.b *= _colorDepression;

                _selfRenderer.color = selfColor;

            }
            else
            {
                _itemImage.sprite = null;
                _itemImage.color = new Color(0, 0, 0, 0);
                
                _selfRenderer.color = _uiColor;
            }

            if (_itemImage.sprite != null)
            {
                var image = _itemImage.sprite.texture;

                Vector2 itemImageSize;
                if (_itemImageMaxSize != Vector2.zero)
                    itemImageSize = _itemImageMaxSize;
                else
                {
                    itemImageSize = _rectTransform.sizeDelta;
                }

                _itemImageTransform.sizeDelta = FitSize(image, itemImageSize);
            }
        }

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

        public override void Click()
        {
            base.Click();

            Item activeItem = LocalPlayer.GetItemBySlot(PlayerActionController.Current.ActiveHand);

            Item heldItem = LocalPlayer.GetItemBySlot(_slotEnum);

            if (heldItem == null && activeItem != null)
            {
                LocalPlayer.MoveItem(PlayerActionController.Current.ActiveHand, _slotEnum);
            }

            if (heldItem != null && activeItem == null)
            {
                LocalPlayer.MoveItem(_slotEnum, PlayerActionController.Current.ActiveHand);
                // TODO Moving items via dragging
            }

        }

    }
}

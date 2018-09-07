using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Objects.Item.Chem
{
    class VolumeFiller : MonoBehaviour
    {
        [SerializeField]
        private int _highestLine;
        [SerializeField]
        private int _lowestLine;

        private float _prevFillValue;

        private SpriteRenderer _spriteRenderer;
        private Sprite _originalSprite;

        private Color[] _originalPixels;

        public float FillValue { get; set; }

        public Color ReagentsColor
        {
            get
            {
                if (_spriteRenderer == null)
                    _spriteRenderer = GetComponent<SpriteRenderer>();
                return _spriteRenderer.color;
            }
            set
            {
                if (_spriteRenderer == null)
                    _spriteRenderer = GetComponent<SpriteRenderer>();
                _spriteRenderer.color = value;
            }
        }

        private void Start()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _originalSprite = _spriteRenderer.sprite;
            _originalPixels = _originalSprite.texture.GetPixels();

            OnFillValueChanged();
        }


        private void Update()
        {
            if (Math.Abs(FillValue - _prevFillValue) > 0.001f)
            {
                _prevFillValue = FillValue;
                OnFillValueChanged();
            }
        }

        private void OnFillValueChanged()
        {
            int width = _spriteRenderer.sprite.texture.width;
            int height = _spriteRenderer.sprite.texture.height;

            Color[] pixels = new Color[_originalPixels.Length];
            _originalPixels.CopyTo(pixels, 0);

            int line = (int)((_highestLine - _lowestLine) * FillValue) + _lowestLine;

            if (Math.Abs(FillValue) < 0.001f)
            {
                line = 0;
            }

            if(line < 0)
                return;
            if(line > _highestLine)
                return;

            for (int y = line; y <  height; y++)
            {
                int offset = y * width;
                for (int x = 0; x < width; x++)
                {
                    int index = offset + x;
                    
                    // Изменяем прозрачность только тех пикселей, которые были непрозрачными в изначальной текстуре заполнителя 
                    //if (_originalPixels[index].a > 0)
                    {
                        pixels[index].a = 0;
                    }
                }
            }

            try
            {
                Texture2D nTexture2D = new Texture2D(width, height, TextureFormat.ARGB32, true);
                nTexture2D.filterMode = FilterMode.Point;
                nTexture2D.SetPixels(pixels);
                nTexture2D.Apply();
                Sprite sprite = Sprite.Create(nTexture2D, _originalSprite.rect, new Vector2(0.5f, 0.5f));

                _spriteRenderer.sprite = sprite;
            }
            catch (Exception ex)
            {
                Debug.Log("Exception occured: " + ex.Message);
                throw ex;
            }

            //Destroy(oldSprite);
        }
    }
}

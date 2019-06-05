using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Controllers
{
    class TileMask : MonoBehaviour
    {
        private SpriteRenderer _spriteRenderer;

        [SerializeField]
        private int _cellX;

        [SerializeField]
        private int _cellY;

        private void Start()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void SetAlpha(float alpha)
        {
            Color color = _spriteRenderer.color;
            color.a = alpha;
            _spriteRenderer.color = color;
        }

        public void SetColor(Color nColor)
        {
            if (_spriteRenderer == null)
                _spriteRenderer = GetComponent<SpriteRenderer>();

            Color color = _spriteRenderer.color;
            color.r = nColor.r;
            color.g = nColor.g;
            color.b = nColor.b;
            _spriteRenderer.color = color;
        }

        public void SetActive(bool isActive)
        {
            gameObject.SetActive(isActive);
        }

        public int X
        {
            get => _cellX;
            set => _cellX = value;
        }
        public int Y
        {
            get => _cellY;
            set => _cellY = value;
        }

    }
}

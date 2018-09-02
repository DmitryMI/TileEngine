﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Controllers
{
    public class VisionMask : MonoBehaviour
    {
        private bool _active = true;
        private SpriteRenderer _spriteRenderer;

        [SerializeField]
        private bool _visible;
        private float _brightness;

        private Color _baseColor = Color.black;

        private void Start()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();

            SetInvisible();
        }

        public void SetInvisible()
        {
            _visible = false;
        }

        public void SetVisible()
        {
            _visible = true;
        }

        public void SetLighting(float brightness, Color lightColor)
        {
            _brightness = Mathf.Clamp(brightness, 0, 1);
            _baseColor = lightColor;
            // TODO Process light color
        }

        public float GetBrightness()
        {
            return _brightness;
        }

        public Color GetColor()
        {
            return _spriteRenderer.color;
        }

        public void Disable()
        {
            _active = false;
        }

        public void Enable()
        {
            _active = true;
        }

        public void SetCell(int x, int y)
        {
            Grid grid = FindObjectOfType<Grid>();
            Vector3 position = grid.CellToWorld(new Vector3Int(x, y, 0));
            transform.position = position;
        }

        public bool IsVisible()
        {
            return _visible && _brightness > 0f;
        }

        private void LateUpdate()
        {
            if (_active)
            {
                if (_visible)
                {
                    Color spriteColor = _baseColor;
                    spriteColor.a = 1 - _brightness;
                    _spriteRenderer.color = spriteColor;
                }
                else
                {
                    _spriteRenderer.color = new Color(0, 0, 0, 1);
                }
            }
            else
            {
                _spriteRenderer.color = new Color(0, 0, 0, 0);
            }
        }
    }
}

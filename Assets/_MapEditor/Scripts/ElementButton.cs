using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Objects;
using Assets.Scripts.Ui;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._MapEditor.Scripts
{
    class ElementButton : UiElement
    {
        [SerializeField] private Text _name;
        [SerializeField] private Image _image;

        private TileObject _prefab;
        public void SetData(TileObject prefab)
        {
            _prefab = prefab;
            _name.text = prefab.gameObject.name + " (ID: " + prefab.Id + ")";

            SpriteRenderer toRenderer = prefab.gameObject.GetComponent<SpriteRenderer>();

            if (toRenderer)
            {
                _image.sprite = toRenderer.sprite;
            }
        }

    }
}

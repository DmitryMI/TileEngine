using UnityEngine;

namespace Assets.Scripts.Objects.Equipment.Doors
{
    public class DoorPart : MonoBehaviour
    {
        public Vector2 Position;

        public Color EffectColor;
        public bool EffectVisible;

        //private SpriteRenderer _renderer;

        [SerializeField] private SpriteRenderer _effectsRenderer;

        private void Update()
        {
            transform.localPosition = Position;

            _effectsRenderer.color = EffectColor;
        }
    }
}

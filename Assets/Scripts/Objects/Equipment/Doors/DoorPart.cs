using UnityEngine;

namespace Assets.Scripts.Objects.Equipment.Doors
{
    public class DoorPart : MonoBehaviour
    {
        public Vector2 Position;

        private void Update()
        {
            transform.localPosition = Position;
        }
    }
}

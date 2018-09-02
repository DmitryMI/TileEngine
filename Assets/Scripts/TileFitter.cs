using UnityEngine;

namespace Assets.Scripts
{
    public class TileFitter : MonoBehaviour
    {

        [SerializeField]
        private bool UpdateEachFrame = false;

        Grid grid;
        SpriteRenderer _spriteRenderer;
        // Use this for initialization
        void Start ()
        {
            grid = FindObjectOfType<Grid>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            SetupScale();
        }

        private void Update()
        {
            if(UpdateEachFrame)
                SetupScale();
        }

        void SetupScale()
        {
        
            float cellXSize = grid.cellSize.x;
            float cellYSize = grid.cellSize.y;

            float spriteXSize = _spriteRenderer.sprite.bounds.size.x;
            float spriteYSize = _spriteRenderer.sprite.bounds.size.y;

            float extensionCoeffX = cellXSize / spriteXSize; // Это масштаб объекта по оси X
            float extensionCoeffY = cellYSize / spriteYSize; // Это масштаб объекта по оси Y

            transform.localScale = new Vector3(extensionCoeffX, extensionCoeffY, 0);
        }
	
    }
}

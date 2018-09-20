using Assets.Scripts.Objects.Equipment.Tables;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Objects.Turf
{
    public class MetalWall : Turf
    {
        [SerializeField] private SpatialVariativity _spatialVariativity;

        protected override void Update()
        {
            base.Update();

            if (isServer)
                UpdateConnectedWalls(Cell.x, Cell.y);

            UpdateSprite();
        }

        [SyncVar]
        private bool _isLongTop;
        [SyncVar]
        private bool _isLongBot;
        [SyncVar]
        private bool _isLongLeft;
        [SyncVar]
        private bool _isLongRight;

        protected override void Start()
        {
            base.Start();
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        [Server]
        protected void UpdateConnectedWalls(int x, int y)
        {
            MetalWall wallLeft = TileController.Find<MetalWall>(x - 1, y);
            MetalWall wallRight = TileController.Find<MetalWall>(x + 1, y);
            MetalWall wallTop = TileController.Find<MetalWall>(x, y + 1);
            MetalWall wallBot = TileController.Find<MetalWall>(x, y - 1);

            _isLongLeft = wallLeft;
            _isLongTop = wallTop;
            _isLongBot = wallBot;
            _isLongRight = wallRight;
        }

        protected virtual void UpdateSprite()
        {
            Sprite sprite = _spatialVariativity.GetSprite(_isLongLeft, _isLongRight, _isLongTop, _isLongBot);
            _spriteRenderer.sprite = sprite;
        }

        protected override bool Transperent => false;
        protected override bool CanWalkThrough => false;
        protected override bool PassesGas => false;

        private SpriteRenderer _spriteRenderer;
    }
}

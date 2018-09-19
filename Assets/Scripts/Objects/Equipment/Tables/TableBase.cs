using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Objects.Equipment.Tables
{
    public class TableBase : Table
    {
        [SerializeField] private Sprite _idle;
        [SerializeField]
        private Sprite _longTop;
        [SerializeField]
        private Sprite _longBot;
        [SerializeField]
        private Sprite _longLeft;
        [SerializeField]
        private Sprite _longRight;

        [SerializeField] private Sprite _longTopRight;
        [SerializeField] private Sprite _longTopLeft;
        [SerializeField] private Sprite _longTopBot;
        [SerializeField] private Sprite _longLeftRight;

        [SerializeField] private Sprite _longBotLeft;
        [SerializeField] private Sprite _longBotRight;

        [SerializeField] private Sprite _longTopLeftRight;
        [SerializeField] private Sprite _longBotLeftRight;

        [SerializeField] private Sprite _longLeftTopBot;
        [SerializeField] private Sprite _longRightTopBot;

        [SerializeField] private Sprite _longTopBotLeftRight;

        [SyncVar]
        private bool _isLongTop;
        [SyncVar]
        private bool _isLongBot;
        [SyncVar]
        private bool _isLongLeft;
        [SyncVar]
        private bool _isLongRight;

        protected override bool Transperent => true;

        private SpriteRenderer _spriteRenderer;

        private Sprite GetSprite()
        {
            if (!_isLongLeft && !_isLongBot && !_isLongTop && !_isLongRight)
            {
                return _idle;
            }

            if (_isLongLeft && _isLongRight && _isLongTop && _isLongBot)
            {
                return _longTopBotLeftRight;
            }

            if (_isLongLeft && _isLongRight && _isLongBot)
            {
                return _longBotLeftRight;
            }

            if (_isLongLeft && _isLongRight && _isLongTop)
            {
                return _longTopLeftRight;
            }

            if (_isLongBot && _isLongRight && _isLongTop)
            {
                return _longRightTopBot;
            }

            if (_isLongLeft && _isLongTop && _isLongBot)
            {
                return _longLeftTopBot;
            }

            if (_isLongLeft && _isLongTop)
            {
                return _longTopLeft;
            }

            if (_isLongRight && _isLongTop)
            {
                return _longTopRight;
            }

            if (_isLongTop && _isLongBot)
            {
                return _longTopBot;
            }

            if (_isLongLeft && _isLongRight)
            {
                return _longLeftRight;
            }

            if (_isLongBot && _isLongLeft)
            {
                return _longBotLeft;
            }

            if (_isLongBot && _isLongRight)
            {
                return _longBotRight;
            }

            if (_isLongLeft)
                return _longLeft;
            if (_isLongRight)
                return _longRight;
            if (_isLongTop)
                return _longTop;
            if (_isLongBot)
                return _longBot;

            return null;
        }

        protected override void Update()
        {
            base.Update();

            if(isServer)
                UpdateConnectedTables(Cell.x, Cell.y);

            UpdateSprite();
        }

        protected override void Start()
        {
            base.Start();
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        [Server]
        protected void UpdateConnectedTables(int x, int y)
        {
            TableBase tableLeft = TileController.Find<TableBase>(x - 1, y);
            TableBase tableRight = TileController.Find<TableBase>(x + 1, y);
            TableBase tableTop = TileController.Find<TableBase>(x, y + 1);
            TableBase tableBot = TileController.Find<TableBase>(x, y - 1);

            _isLongLeft = tableLeft;
            _isLongTop = tableTop;
            _isLongBot= tableBot;
            _isLongRight = tableRight;
        }

        protected virtual void UpdateSprite()
        {
            Sprite sprite = GetSprite();
            _spriteRenderer.sprite = sprite;
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Objects.Equipment.Doors;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Objects.Mob
{
    public abstract class Mob : TileObject
    {
        [SerializeField] protected Sprite FrontSprite;
        [SerializeField] protected Sprite BackSprite;
        [SerializeField] protected Sprite LeftSprite;
        [SerializeField] protected Sprite RightSprite;
        [SerializeField] protected Sprite LyingSprite;

        private bool _movementFinished = true;
        protected SpriteRenderer Renderer;

        public abstract bool IsLying { get; }

        private IEnumerator AnimateMovement(Vector2 velocity, float time)
        {
            float timePast = 0;
            while (timePast < time)
            {
                CellOffset += velocity;
                timePast += Time.deltaTime;
                //Debug.Log("TimePast: " + timePast + ", time: " + time);
                //TransformChanged = true;
                yield return new WaitForEndOfFrame();
            }

            CellOffset = Vector2.zero;
            _movementFinished = true;
        }

        protected override void Start()
        {
            base.Start();
            Renderer = GetComponent<SpriteRenderer>();
        }

        protected override void Update()
        {
            base.Update();
            UpdateSprite();
        }

        protected void DoMove(Direction direction, float speed)
        {
            if(IsLying)
                return;

            if (!_movementFinished)
            {
                //Debug.Log("Movement not finished!");
                return;
            }

            Vector2Int nextCell = Cell;
            Vector2 shift;

            Rotation = direction;

            switch (direction)
            {
                case Direction.Forward:
                    
                    nextCell += Vector2Int.up;
                    shift = Vector2.up;
                    break;

                case Direction.Backward:
                    nextCell += Vector2Int.down;
                    shift = Vector2.down;
                    break;

                case Direction.Left:
                    nextCell += Vector2Int.left;
                    shift = Vector2.left;
                    break;

                case Direction.Right:
                    nextCell += Vector2Int.right;
                    shift = Vector2.right;
                    break;

                default:
                    throw new ArgumentOutOfRangeException("direction", direction, null);
            }

            if (WalkController.CanPass(nextCell))
            {
                _movementFinished = false;

                Cell = nextCell;
                Vector3Int cell = new Vector3Int(Cell.x, Cell.y, 0);
                Vector3 offset3d = Grid.CellToWorld(cell) - transform.position;
                Vector2 offset = new Vector2(offset3d.x, offset3d.y);
                CellOffset = -offset;

                StartCoroutine(AnimateMovement(shift * speed, 1 / speed * Time.deltaTime));
            }
            else // Find a door
            {
                Door door = TileController.Find<Door>(nextCell.x, nextCell.y);

                if(door != null)
                    door.TryToPass();
            }
        }

        protected virtual void UpdateSprite()
        {
            if (IsLying)
            {
                Renderer.sprite = FrontSprite;
            }
            else
            {
                switch (Rotation)
                {
                    case Direction.Forward:
                        Renderer.sprite = BackSprite;
                        break;

                    case Direction.Backward:
                        Renderer.sprite = FrontSprite;
                        break;

                    case Direction.Left:
                        Renderer.sprite = LeftSprite;
                        break;

                    case Direction.Right:
                        Renderer.sprite = RightSprite;
                        break;
                }
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Objects;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts
{
    public class Fixer : NetworkBehaviour
    {
        [SerializeField]
        private Vector2Int _cell;

        [SerializeField]
        private Vector2 _cellOffset;

        // Use this for initialization
        void Start ()
        {
            if (isServer)
            {
                Grid grid = FindObjectOfType<Grid>();
                Vector3Int cell = grid.WorldToCell(transform.position);
                _cell = new Vector2Int(cell.x, cell.y);
                Vector3 pos3 = grid.CellToWorld(cell);
                Vector3 offset3 = transform.position - pos3;
                _cellOffset = new Vector2(offset3.x, offset3.y);
            }
            else
            {
                enabled = false;
            }
        }

        [Server]
        public void ApplySavedTransformations()
        {
            //NetworkServer.Spawn(gameObject);

            TileObject to = GetComponent<TileObject>();
            to.Cell = _cell;
            to.CellOffset = _cellOffset;
            to.RpcForceTransformation(_cell.x, _cell.y, _cellOffset);
        }
	
        // Update is called once per frame
        void Update () {
		
        }
    }
}

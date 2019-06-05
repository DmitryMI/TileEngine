using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Controllers
{
    class VisionMatrix
    {
        private int[,] _map;

        private List<Vector2Int> _viewPointList = new List<Vector2Int>();

        
        // < 0 - visible
        // == 0 - unknown
        // > 0 - blocker

        public VisionMatrix(int rows, int cols)
        {
            _map = new int[rows,cols];
        }

        

        public void AddViewPoint(int x, int y)
        {
            _viewPointList.Add(new Vector2Int(x, y));
            _map[x, y] = -1;
        }

        
    }
}

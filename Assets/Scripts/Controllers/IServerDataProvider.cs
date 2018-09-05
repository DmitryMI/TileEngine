using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Controllers
{
    public interface IServerDataProvider
    {
        Vector2Int MapSize { get; }
        void RequestLoadingFinished();
        bool Ready { get; }
        bool IsCellInBounds(Vector2Int cell);
        bool IsCellInBounds(int x, int y);
    }
}

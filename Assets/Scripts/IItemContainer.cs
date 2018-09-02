using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    public interface IItemContainer
    {
        Vector2Int Cell { get; set; }
        Vector2 CellOffset { get; set; }

        GameObject gameObject { get; }

        bool BlocksLightFromInside();
    }
}

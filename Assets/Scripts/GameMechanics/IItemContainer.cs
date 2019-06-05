using UnityEngine;

namespace Assets.Scripts.GameMechanics
{
    public interface IItemContainer
    {
        Vector2Int Cell { get; set; }
        Vector2 CellOffset { get; set; }

        GameObject gameObject { get; }

        bool BlocksLightFromInside { get; }
    }
}

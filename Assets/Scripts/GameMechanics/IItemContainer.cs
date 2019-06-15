using UnityEngine;

namespace Assets.Scripts.GameMechanics
{
    public interface IItemContainer : IPositionProvider
    {
        GameObject gameObject { get; }

        bool BlocksLightFromInside { get; }
    }
}

using UnityEngine;

namespace Assets.Scripts.GameMechanics
{
    interface IChildCollider
    {
        GameObject gameObject { get; }
        GameObject Parent { get; }
    }
}

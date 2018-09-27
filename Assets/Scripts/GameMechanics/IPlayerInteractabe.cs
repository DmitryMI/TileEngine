using Assets.Scripts.Objects.Item;
using UnityEngine;

namespace Assets.Scripts.GameMechanics
{
    public interface IPlayerInteractable
    {
        // ReSharper disable once InconsistentNaming
        GameObject gameObject { get; }

        void ApplyItemClient(Item item);

        void ApplyItemServer(Item item);
    }
}

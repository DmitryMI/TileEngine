using Assets.Scripts.Objects.Item;
using UnityEngine;

namespace Assets.Scripts.GameMechanics
{
    public interface IPlayerApplicable
    {
        // ReSharper disable once InconsistentNaming
        GameObject gameObject { get; }

        void ApplyItemClient(Item item, Intent intent);

        void ApplyItemServer(Item item, Intent intent);
    }
}

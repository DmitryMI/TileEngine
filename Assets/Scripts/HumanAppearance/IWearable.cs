using Assets.Scripts.GameMechanics;
using UnityEngine;

namespace Assets.Scripts.HumanAppearance
{
    public interface IWearable
    {
        GameObject gameObject { get; }

        SlotEnum AppropriateSlot { get; }

        Sprite Front { get; }
        Sprite Back { get; }
        Sprite Left { get; }
        Sprite Right { get; }

        Vector2 FrontOffset { get; }

        Vector2 BackOffset { get; }

        Vector2 LeftOffset { get; }

        Vector2 RightOffset { get; }
    }
}

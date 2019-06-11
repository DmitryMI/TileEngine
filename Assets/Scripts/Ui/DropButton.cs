using Assets.Scripts.Controllers;
using Assets.Scripts.GameMechanics;
using Assets.Scripts.Objects.Item;
using Assets.Scripts.Objects.Mob.Humanoids;
using UnityEngine;

namespace Assets.Scripts.Ui
{
    public class DropButton : UiElement
    {
        public override void Click()
        {
            Humanoid humanoid = PlayerActionController.Current.LocalPlayerMob as Humanoid;

            if (humanoid != null)
            {
                SlotEnum slot = PlayerActionController.Current.ActiveHand;
                Item item = humanoid.GetItemBySlot(slot);
                if (item != null)
                {
                    humanoid.DropItem(slot, LocalPlayer.Cell, Vector2.zero);
                }
            }
        }
    }
}

using Assets.Scripts.Controllers;
using Assets.Scripts.Objects.Item;
using UnityEngine;

namespace Assets.Scripts.Ui
{
    public class DropButton : UiElement
    {
        public override void Click()
        {
            SlotEnum slot = PlayerActionController.Current.ActiveHand;
            Item item = LocalPlayer.GetItemBySlot(slot);
            if (item != null)
            {
                LocalPlayer.DropItem(slot, LocalPlayer.Cell, Vector2.zero);
            }
        }
    }
}

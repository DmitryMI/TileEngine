using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Controllers;
using Assets.Scripts.Objects.Item;

namespace Assets.Scripts.Ui
{
    class HandSlot : Slot
    {
        public override void Click()
        {
            Item itemThisHand = LocalPlayer.GetItemBySlot(_slotEnum);
            Item sourceItem = LocalPlayer.GetItemBySlot(PlayerActionController.Current.ActiveHand);

            if (itemThisHand)
            {
                itemThisHand.ApplyItemClient(sourceItem);
            }

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Controllers;
using Assets.Scripts.Objects.Item;
using Assets.Scripts.Objects.Mob.Humanoids;

namespace Assets.Scripts.Ui
{
    class HandSlot : Slot
    {
        public override void Click()
        {
            base.Click();

            Humanoid humanoid = LocalPlayer as Humanoid;
            if (humanoid != null)
            {
                Item itemThisHand = humanoid.GetItemBySlot(_slotEnum);
                Item sourceItem = humanoid.GetItemBySlot(PlayerActionController.Current.ActiveHand);

                if (itemThisHand)
                {
                    itemThisHand.ApplyItemClient(sourceItem);
                }
            }
        }
    }
}

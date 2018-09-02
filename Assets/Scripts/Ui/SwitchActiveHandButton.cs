using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Controllers;
using UnityEngine;

namespace Assets.Scripts.Ui
{
    class SwitchActiveHandButton : UiElement
    {
        public override void Click()
        {
            PlayerActionController.Current.ActiveHand = PlayerActionController.Current.ActiveHand == SlotEnum.LeftHand ? SlotEnum.RightHand : SlotEnum.LeftHand;
        }
    }
}

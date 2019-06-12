using Assets.Scripts.Controllers;
using Assets.Scripts.GameMechanics;
using Assets.Scripts.Objects.Mob;
using Assets.Scripts.Objects.Mob.Humanoids;
using UnityEngine;

namespace Assets.Scripts.Objects.Equipment
{
    public class Table : Equipment, IPlayerInteractable
    {
        public void ApplyItemClient(Item.Item item, Intent intent)
        {
            //Debug.Log("Table was clicked!");

            PlayerActionController controller = PlayerActionController.Current;
            Humanoid localPlayer = controller.LocalPlayerMob as Humanoid;

            if(localPlayer == null)
                return;
            

            Item.Item heldItem = localPlayer.GetItemBySlot(controller.ActiveHand);

            if (heldItem == item)
            {
                // Calculating item offset
                Vector2 mousePosition = controller.MouseWorldPosition;
                Vector2 tablePosition = transform.position;
                Vector2 offset = mousePosition - tablePosition;
                localPlayer.DropItem(controller.ActiveHand, Cell, offset);
            }
        }

        public void ApplyItemServer(Item.Item item, Intent intent)
        {
            
        }
    }
}

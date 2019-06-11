using Assets.Scripts.Controllers;
using Assets.Scripts.Objects.Mob;
using Assets.Scripts.Objects.Mob.Humanoids;
using UnityEngine;

namespace Assets.Scripts.Ui
{
    public class UiElement : MonoBehaviour
    {
        protected Mob LocalPlayer;
        

        protected bool EnsureMobLoaded()
        {
            if (LocalPlayer == null)
            {
                LocalPlayer = PlayerActionController.Current?.LocalPlayerMob;
            }

            return LocalPlayer != null;
        }
        

        public virtual void Click()
        {
            Debug.Log("Ui element was clicked: " + gameObject.name);
        }

        public virtual void SetActive()
        {
            
        }
    }
}

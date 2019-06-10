using Assets.Scripts.Objects.Mob;
using Assets.Scripts.Objects.Mob.Humanoids;
using UnityEngine;

namespace Assets.Scripts.Ui
{
    public class UiElement : MonoBehaviour
    {
        protected Humanoid LocalPlayer;

        private void FindLocalPlayer()
        {
            Mob[] players = FindObjectsOfType<Mob>();

            foreach (var mob in players)
            {
                var p =  mob as Humanoid;
                if(p == null)
                    continue;
                
                if (p.isLocalPlayer)
                    LocalPlayer = p;
            }
        }

        protected virtual void Update()
        {
            if(LocalPlayer == null)
                FindLocalPlayer();
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

using Assets.Scripts.Objects.Mob;
using UnityEngine;

namespace Assets.Scripts.Ui
{
    public class UiElement : MonoBehaviour
    {
        protected Player LocalPlayer;

        private void FindLocalPlayer()
        {
            Player[] players = FindObjectsOfType<Player>();

            foreach (var p in players)
            {
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
            Debug.Log("Ui element was clicked!");
        }
    }
}

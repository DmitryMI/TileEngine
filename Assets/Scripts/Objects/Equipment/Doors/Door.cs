using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Objects.Equipment.Doors
{
    public abstract class Door : Equipment, IPlayerInteractable
    {
        public abstract void ApplyItemClient(Item.Item item);

        public abstract void ApplyItemServer(Item.Item item);

        public abstract void TryToPass();
        
    }
}

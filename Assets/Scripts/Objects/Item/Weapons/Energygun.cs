using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Objects.Item.Weapons
{
    class EnergyGun : Item
    {
        [SerializeField]
        protected AudioClip[] ShootingSounds;

        public override void ItemTargetPointClient(Vector2Int cell, Vector2 offset)
        {
            base.ItemTargetPointClient(cell, offset);
        }

        public override void ItemTargetPointServer(Vector2Int cell, Vector2 offset)
        {
            PlaySoundOn(Utils.GetRandom(ShootingSounds));
        }
    }
}

using System;
using Assets.Scripts;
using Assets.Scripts.Controllers;
using Assets.Scripts.Controllers.Atmos;
using Assets.Scripts.GameMechanics;
using Assets.Scripts.GameMechanics.Health;
using Assets.Scripts.Objects.Mob;
using Assets.Scripts.Objects.Mob.Humanoids;
using UnityEngine;

namespace Assets
{
    public class TestButtonScript : MonoBehaviour
    {
        [SerializeField]
        private HumanoidHealth target;

        [SerializeField] private DamageType _damageType;

        public void TestButtonClick()
        {
            DamageBuffer damage = new DamageBuffer(_damageType, 10);

            Humanoid player = PlayerActionController.Current.LocalPlayerMob as Humanoid;

            if (player != null)
            {
                HumanoidHealth health = player.Health as HumanoidHealth;

                //health?.ModifyDamage(damage, ImpactLimb.Chest);
                health?.DoBruteDamage(10, ImpactLimb.Chest, BruteAttackType.Blunt);
            }
        }
    }
}

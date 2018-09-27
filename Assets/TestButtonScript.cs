using System;
using Assets.Scripts;
using Assets.Scripts.Controllers;
using Assets.Scripts.Controllers.Atmos;
using Assets.Scripts.GameMechanics;
using Assets.Scripts.Objects.Mob;
using UnityEngine;

namespace Assets
{
    public class TestButtonScript : MonoBehaviour
    {
        [SerializeField]
        private HumanoidImpactTarget target;

        [SerializeField] private DamageType _damageType;

        public void TestButtonClick()
        {
            Damage damage = new Damage(_damageType, 10);

            Player player = PlayerActionController.Current.LocalPlayer;

            HumanHealthData health = player.HealthData;

            health.DoDamage(damage, target);
        }
    }
}

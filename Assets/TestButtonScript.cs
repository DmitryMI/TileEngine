using System;
using Assets.Scripts;
using Assets.Scripts.Controllers;
using Assets.Scripts.Controllers.Atmos;
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
            Damage damage = new Damage(5, 5, 5, 5);

            Player player = PlayerActionController.Current.LocalPlayer;

            HumanHealthData health = player.GetHealthDataCopy();

            health.DoDamage(damage, target);

            player.SetHealthData(health);
        }
    }
}

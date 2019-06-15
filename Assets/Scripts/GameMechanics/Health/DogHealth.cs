using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Controllers;
using Assets.Scripts.GameMechanics.Chemistry;
using Assets.Scripts.Objects.FloorSplashes;
using Assets.Scripts.Objects.Mob;
using Assets.Scripts.Objects.Turf;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.GameMechanics.Health
{
    class DogHealth : AnimalHealth
    {
        public override bool IsCritical => OverallDamage.Summ > 50.0f;
        public override bool CanStandOnLegs => !IsCritical;

        private ISubstanceContainer _bloodStream;
        private ISubstanceContainer _digestiveSystem;

        public override void OnStart()
        {
            base.OnStart();

            _bloodStream = new SimpleSubstanceContainer(2000, 10);
            _digestiveSystem = new SimpleSubstanceContainer(500, 10);
        }

        public override ISubstanceContainer BloodStream => _bloodStream;
        public override ISubstanceContainer DigestiveSystem => _digestiveSystem;

        public DogHealth(Mob owner) : base(owner)
        {
        }

        protected override bool ModifyDamage(DamageBuffer damage, ImpactLimb impactTarget)
        {
            if (OverallDamage.Summ > 30.0f)
            {
                List<Turf> underlying = new List<Turf>();
                TileController.Current.FindAll(Owner.X, Owner.Y, underlying);

                Turf top = Utils.GetTopMost(underlying);

                GameObject go = GameObject.Instantiate(GlobalPreferences.Instance.BloodSplashPrefab, top.gameObject.transform);
                NetworkServer.Spawn(go);
                go.GetComponent<BloodSplash>().CreateFromBody(Owner);
            }

            return base.ModifyDamage(damage, impactTarget);
        }
    }
}

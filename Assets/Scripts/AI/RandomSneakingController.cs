using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.GameMechanics;
using Assets.Scripts.Objects.Mob.Critters;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.AI
{
    class RandomSneakingController : AiController
    {
        [SerializeField]
        protected float DogBehaviourPeriod;

        [SerializeField]
        protected bool DoBehaviour;

        protected override void Start()
        {
            base.Start();

            DoBehaviour = true;

            StartCoroutine(SneakingBehaviour());
        }

        protected void Update()
        {

        }

        protected virtual IEnumerator SneakingBehaviour()
        {
            while (DoBehaviour)
            {
                Direction[] directions = (Direction[])Enum.GetValues(typeof(Direction));
                Direction direction = Utils.GetRandom(directions);
                
                ControlledMob.DoMove(direction);

                yield return new WaitForSeconds(DogBehaviourPeriod);
            }
        }
    }
}

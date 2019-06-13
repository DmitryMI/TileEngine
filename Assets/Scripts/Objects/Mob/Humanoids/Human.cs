using System;
using Assets.Scripts.GameMechanics.Health;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Objects.Mob.Humanoids
{
    public class Human : Humanoid
    {
        


        // Appearance
        [SyncVar]
        protected Color SkinTone;

        [SerializeField]
        protected int SkinToneIndex;

        public override string DescriptiveName => "Soulless human being";

        protected override void CreateHealthData()
        {
            HealthData = new HumanHealth(this);
        }

        protected override void Update()
        {
            base.Update();

            //Renderer.color = SkinTone;
            Renderer.color = HumanSkinTones.AllSkinTones[SkinToneIndex];
        }
    }
}

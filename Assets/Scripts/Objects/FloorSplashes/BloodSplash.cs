using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.GameMechanics.Chemistry;
using Assets.Scripts.Objects.Turf;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Objects.FloorSplashes
{
    class BloodSplash : FloorSplash
    {
        [SerializeField] private float _colorMinimumIntensity = 0.5f;
        [SerializeField] private float _darkeningTime = 30.0f;

        [SerializeField][SyncVar] private float _lifeTime;

        private SubstanceMixture _mixture = new SubstanceMixture();
        private float _transferAmount = 1.0f;

        private SpriteRenderer _spriteRenderer;

        public override string DescriptiveName => "Blood";
        public override float RemainingVolume => 0;
        public override float MaximumVolume => 10;

        private float GetRemainingVolume()
        {
            return MaximumVolume - _mixture.Volume;
        }

        public override void TransferInto(SubstanceMixture incomingMixture)
        {
            float remainingVolume = GetRemainingVolume();

            SubstanceMixture concatinationMixture;

            if (remainingVolume >= incomingMixture.Volume)
            {
                concatinationMixture = incomingMixture.Clone() as SubstanceMixture;
                incomingMixture.Clear();
            }
            else
            {
                concatinationMixture = incomingMixture.SubtractVolume(remainingVolume);
            }

            _mixture.Concatinate(concatinationMixture);

            Debug.Log(gameObject.name + ": New contents: " + _mixture);
        }

        public override void TransferToAnother(ISubstanceContainer container)
        {
            Debug.Log(gameObject.name + ": transfering liquids to " + container.gameObject);

            float amount = Mathf.Min(_transferAmount, _mixture.Volume);

            if (Math.Abs(amount) < 0.0001f)
            {
                Debug.Log("Nothing to transfer!");
            }
            else
            {
                Debug.Log("Amount: " + amount);
                SubstanceMixture subtractedMixture = _mixture.SubtractVolume(amount);

                container.TransferInto(subtractedMixture);

                _mixture.Concatinate(subtractedMixture);
            }
        }

        public override SubstanceMixture Contents => _mixture;

        [Server]
        public void CreateFromBody(Mob.Mob mob)
        {
            // TODO Make mob blood source
            transform.position = mob.transform.position;
            Cell = mob.Cell;
            _lifeTime = 0;
        }
            
        protected override void Start()
        {
            base.Start();

            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        protected override void Update()
        {
            base.Update();

            if (isServer)
            {
                _lifeTime += Time.deltaTime;
            }

            //float speed = (1 - _colorMinimumIntensity) / _darkeningTime * Time.deltaTime;
            float timePercentage = (_darkeningTime - _lifeTime) / _darkeningTime;

            if (timePercentage < _colorMinimumIntensity)
                timePercentage = _colorMinimumIntensity;
           

            if (Utils.GetIntensity(_spriteRenderer.color) > _colorMinimumIntensity)
            {
                _spriteRenderer.color = Utils.SetIntensity(Color.white, timePercentage);
            }
        }
    }
}

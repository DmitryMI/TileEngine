using System;
using Assets.Scripts.Controllers;
using Assets.Scripts.GameMechanics;
using Assets.Scripts.GameMechanics.Health;
using Assets.Scripts.Objects.Mob.Humanoids;
using UnityEngine;

namespace Assets.Scripts.Objects.Item
{
    public class Crowbar : Item, IImpactHandler, IApplicationHandler
    {
        [SerializeField] private float CrowbarDamage = 20.0f;
        [SerializeField] private float CrowbarDamageDispersion = 5.0f;

        private bool _cellChanged;

        public override string DescriptiveName
        {
            get { return "Crowbar"; }
        }

        public void OnImpact(IPlayerImpactable target, Intent intent, ImpactLimb impactTarget)
        {
            Mob.Mob mob = target as Mob.Mob;
            if (mob == null)
            {
                Debug.Log("Crowbar can only impact mobs");
                return;
            }

            switch (intent)
            {
                case Intent.Help:
                    break;
                case Intent.Disarm:
                    break;
                case Intent.Grab:
                    break;
                case Intent.Harm:
                    float mean = CrowbarDamage;
                    float dispersion = GlobalPreferences.Instance.DefaultFistDamageDispersion;
                    float damage = Utils.NextGaussian(mean, dispersion);
                    
                    mob.Health.DoBruteDamage(damage, impactTarget, BruteAttackType.Chopping);

                    AudioClip clip = Utils.GetRandom(GlobalPreferences.Instance.BodyAttackClips);
                    mob.PlaySoundOn(clip);

                    Debug.Log("Crowbar hit: " + damage);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(intent), intent, null);
            }
        }

        public bool OnApplicationClient(IPlayerApplicable target, Intent intent)
        {
            var action = PlayerActionController.Current.StartAction(ApplyCrowbarDelayedAction,
                AbortConditionChecker, AbortHandler, target, null, 3.0f);

            _cellChanged = false;
            ItemHolder.OnCellChanged += CellChangedHandler;

            StartCoroutine(action.Coroutine);

            return true;
        }

        private void ApplyCrowbarDelayedAction(object args)
        {
            (PlayerActionController.Current.LocalPlayerMob as Humanoid)?.ApplyItem(this, (IPlayerApplicable)args, Intent.Help);
        }

        private void AbortHandler(object args)
        {
            Debug.Log("Aborted!");
        }

        private bool AbortConditionChecker()
        {
            Debug.Log("Checking conditions...");

            if (ItemHolder == null)
                return true;

            if (PlayerActionController.Current.ActiveHandItem != this)
                return true;

            if (_cellChanged)
                return true;

            return false;
        }

        private void CellChangedHandler()
        {
            _cellChanged = true;
        }

        public bool OnApplicationServer(IPlayerApplicable target, Intent intent)
        {
            return false;
        }
    }
}

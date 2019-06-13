using System;
using Assets.Scripts.GameMechanics;
using Assets.Scripts.GameMechanics.Health;
using UnityEngine;

namespace Assets.Scripts.Objects.Item
{
    public class Crowbar : Item, IImpactHandler
    {
        [SerializeField] private float CrowbarDamage = 20.0f;
        [SerializeField] private float CrowbarDamageDispersion = 5.0f;

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
    }
}

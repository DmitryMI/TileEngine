using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    class GlobalPreferences : MonoBehaviour
    {
        private static GlobalPreferences _instance;
        public static GlobalPreferences Instance => _instance;
        void Start()
        {
            _instance = this;
        }



        [Header("Nutrition settings")]
        [SerializeField]
        private float _humanNutritionMax = 100.0f;

        [SerializeField]
        private float _humanNutritionDecrement = 0.1f;

        [SerializeField]
        private float _humanNutritionInitial = 100.0f;

        [SerializeField]
        private float _humanWaterMax = 100.0f;

        [SerializeField]
        private float _humanWaterDecrement = 0.5f;

        [SerializeField]
        private float _humanWaterInitial = 100.0f;


        [Header("Health settings")]
        [SerializeField]
        private float _criticalHealthPointsBorder = 0.0f;

        [SerializeField]
        private float _fullHpIndicationBorder = 90.0f;

        [SerializeField]
        private float _humanMaxHp = 100.0f;

        [SerializeField]
        private float _humanDeathHp = -100.0f;

        [SerializeField]
        private float _humanSeverInjuryBorder = 50.0f;

        [SerializeField] private float _humanBloodVolume = 4600;
        [SerializeField] private float _humanStomachVolume = 1000;


        [Header("Fighting and damage settings")] [SerializeField]
        private float _defaultFistDamage = 5.0f;

        [SerializeField] private float _defaultFistDamageDispersion = 0.5f;
        [SerializeField] private AudioClip[] _fistAttackClips;
        [SerializeField] private AudioClip[] _bodyAttackClips;
        [SerializeField] private GameObject _bloodSplashPrefab;


        public float HumanMaxNutrition
        {
            get { return _humanNutritionMax; }
        }

        public float HumanNutritionDecrement
        {
            get { return _humanNutritionDecrement; }
        }

        public float HumanNutritionInitial
        {
            get { return _humanNutritionInitial; }
        }

        public float CriticalHealthPointsBorder
        {
            get { return _criticalHealthPointsBorder; }
        }

        public float FullHpIndicationBorder
        {
            get { return _fullHpIndicationBorder; }
        }

        public float HumanMaxHp
        {
            get { return _humanMaxHp; }
        }

        public float HumanDeathHp
        {
            get { return _humanDeathHp; }
        }

        public float DefaultFistDamage
        {
            get { return _defaultFistDamage; }
        }

        public float DefaultFistDamageDispersion
        {
            get { return _defaultFistDamageDispersion; }
        }

        public AudioClip[] FistAttackClips
        {
            get { return _fistAttackClips; }
        }

        public AudioClip[] BodyAttackClips
        {
            get { return _bodyAttackClips; }
        }

        public GameObject BloodSplashPrefab
        {
            get { return _bloodSplashPrefab; }
        }

        public float HumanSeverInjuryBorder
        {
            get { return _humanSeverInjuryBorder; }
        }

        public float HumanWaterMax
        {
            get { return _humanWaterMax; }
        }

        public float HumanWaterDecrement
        {
            get { return _humanWaterDecrement; }
        }

        public float HumanWaterInitial
        {
            get { return _humanWaterInitial; }
        }

        public float HumanBloodVolume
        {
            get { return _humanBloodVolume; }
        }

        public float HumanStomachVolume
        {
            get { return _humanStomachVolume; }
        }
    }
}

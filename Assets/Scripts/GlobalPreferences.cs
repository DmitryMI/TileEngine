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


        
    }
}

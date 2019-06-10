using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Controllers;
using Assets.Scripts.Objects.Mob;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Ui
{
    class HungerIndicatorUi : MonoBehaviour
    {
        [SerializeField] private Image _foodImage;
        [SerializeField] private Image _messageDisplayer;

        [SerializeField] private SpriteSet _slightlySprite;
        [SerializeField] private SpriteSet _moderateSprite;
        [SerializeField] private SpriteSet _starvingSprite;

        private Mob _localMob;

        void Start()
        {
            
        }


        void Update()
        {
            _slightlySprite.OnUpdate();
            _moderateSprite.OnUpdate();
            _starvingSprite.OnUpdate();

            if (EnsureMobLoaded())
            {
                UpdateHungerIndicator();
            }
        }

        bool EnsureMobLoaded()
        {
            if (_localMob == null)
            {
                _localMob = PlayerActionController.Current?.LocalPlayerMob;
            }

            return _localMob != null;
        }

        void UpdateHungerIndicator()
        {
            float nutritionPercentage = _localMob.Health.CurrentNutrition / _localMob.Health.DefaultMaxNutrition;

            if (nutritionPercentage > 0.85f)
            {
                _messageDisplayer.enabled = false;
            }
            else if (nutritionPercentage > 0.6f)
            {
                _messageDisplayer.enabled = true;
                _messageDisplayer.sprite = _slightlySprite.CurrentSprite;
            }
            else if (nutritionPercentage > 0.4f)
            {
                _messageDisplayer.enabled = true;
                _messageDisplayer.sprite = _moderateSprite.CurrentSprite;
            }
            else
            {
                _messageDisplayer.enabled = true;
                _messageDisplayer.sprite = _starvingSprite.CurrentSprite;
            }
        }
    }
}

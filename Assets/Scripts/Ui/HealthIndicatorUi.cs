using Assets.Scripts.Controllers;
using Assets.Scripts.GameMechanics.Health;
using Assets.Scripts.Objects.Mob;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Ui
{
    public class HealthIndicatorUi : UiElement
    {
        [SerializeField] private Color _critDollColor;

        [SerializeField]
        private Image _humanDoll;
        [SerializeField]
        private Image _stateMessage;

        [SerializeField] private SpriteSet _critMessageImg;

        [SerializeField] private SpriteSet _fullHpMessageImg;


        
        void Update()
        {
            _critMessageImg.OnUpdate();
            _fullHpMessageImg.OnUpdate();
            if (EnsureMobLoaded())
            {
                UpdateHealthIndicator();
            }
            else
            {
                
            }
        }

        private void UpdateHealthIndicator()
        {
            MobHealth health = LocalPlayer.Health;


            //DamageBuffer buffer = humanoidHealth.GetOverallDamage();
            float hp = health.NetHealthData.DamagePercentagePerception;

            float healthPointsValue = hp * GlobalPreferences.Instance.HumanMaxHp;

            if (healthPointsValue >= GlobalPreferences.Instance.FullHpIndicationBorder)
            {
                _stateMessage.enabled = true;
                _stateMessage.sprite = _fullHpMessageImg.CurrentSprite;

                float red = 1 - hp;
                float green = hp;

                Color color = Color.black;
                color.r = red;
                color.g = green;

                color = Utils.ClampRedGreenIntensity(color, 0.7f);
                _humanDoll.color = color;
            }
            else if (healthPointsValue > GlobalPreferences.Instance.CriticalHealthPointsBorder)
            {
                _stateMessage.enabled = false;
                float red = 1 - hp;
                float green = hp;

                Color color = Color.black;
                color.r = red;
                color.g = green;

                color = Utils.ClampRedGreenIntensity(color, 0.7f);
                _humanDoll.color = color;
            }
            else
            {
                // Critical state
                _stateMessage.enabled = true;
                _stateMessage.sprite = _critMessageImg.CurrentSprite;
                _humanDoll.color = _critDollColor;
            }
        }
    }
    
}

using Assets.Scripts.Controllers;
using Assets.Scripts.GameMechanics.Health;
using Assets.Scripts.Objects.Mob;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Ui
{
    public class HealthIndicatorUi : MonoBehaviour
    {
        [SerializeField] private Color _critDollColor;

        [SerializeField]
        private Image _humanDoll;
        [SerializeField]
        private Image _stateMessage;

        [SerializeField] private SpriteSet _critMessageImg;
        //private Sprite _critMessageImg;

        private Mob _localMob;
        private Color _cacheColor;

        void Start()
        {
            _cacheColor = Color.black;
        }

        
        void Update()
        {
            _critMessageImg.OnUpdate();
            if(EnsureMobLoaded())
            {
                UpdateHealthIndicator();
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

        private void UpdateHealthIndicator()
        {
            MobHealth health = _localMob.Health;

            if(health is HumanoidHealth humanoidHealth)
            {
                DamageBuffer buffer = humanoidHealth.GetOverallDamage();
                float hp = (100.0f - buffer.Summ) / 100.0f;

                if (hp > 0)
                {
                    _stateMessage.enabled = false;
                    float red = 1 - hp;
                    float green = hp;

                    Color color = _cacheColor;
                    color.r = red;
                    color.g = green;
                    color.b = 0;

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
}

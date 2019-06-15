using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Controllers;
using Assets.Scripts.Objects.Mob;
using UnityEngine;

namespace Assets.Scripts.Ui
{
    public class VisionEffectHandler : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _blackSquare;
        [SerializeField] private SpriteRenderer _visionEffectRenderer;
        [SerializeField] private Sprite[] _blackoutSprites;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            PlayerActionController controller = PlayerActionController.Current;

            if (controller != null && controller.LocalPlayerMob != null)
            {
                Mob mob = controller.LocalPlayerMob;
                int stage = mob.Health.NetHealthData.BlackoutStage;
                Color color = mob.Health.NetHealthData.BlackoutColor;
                if (stage != 0)
                {
                    _visionEffectRenderer.gameObject.SetActive(true);
                    _blackSquare.gameObject.SetActive(true);
                    _visionEffectRenderer.sprite = _blackoutSprites[stage - 1];
                    _visionEffectRenderer.color = color;
                    _blackSquare.color = color;
                }
                else
                {
                    _visionEffectRenderer.gameObject.SetActive(false);
                    _blackSquare.gameObject.SetActive(false);
                }
            }
        }
    }
}

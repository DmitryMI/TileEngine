using System;
using Assets.Scripts.Controllers;
using Assets.Scripts.GameMechanics;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Ui
{
    public class IntentIndicator : MonoBehaviour
    {
        [SerializeField] private Image _helpImage;
        [SerializeField] private Image _disarmImage;
        [SerializeField] private Image _grabImage;
        [SerializeField] private Image _harmImage;

        [SerializeField] private Color _helpColorEnabled;
        [SerializeField] private Color _helpColorDisabled;
        [SerializeField] private Color _disarmColorEnabled;
        [SerializeField] private Color _disarmColorDisabled;
        [SerializeField] private Color _grabColorEnabled;
        [SerializeField] private Color _grabColorDisabled;
        [SerializeField] private Color _harmColorEnabled;
        [SerializeField] private Color _harmColorDisabled;

        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            if(PlayerActionController.Current == null)
                return;
            
            Intent currentIntent = PlayerActionController.Current.Intent;

            switch (currentIntent)
            {
                case Intent.Help:
                    _helpImage.color = _helpColorEnabled;
                    _disarmImage.color = _disarmColorDisabled;
                    _grabImage.color = _grabColorDisabled;
                    _harmImage.color = _harmColorDisabled;
                    break;
                case Intent.Disarm:
                    _helpImage.color = _helpColorDisabled;
                    _disarmImage.color = _disarmColorEnabled;
                    _grabImage.color = _grabColorDisabled;
                    _harmImage.color = _harmColorDisabled;
                    break;
                case Intent.Grab:
                    _helpImage.color = _helpColorDisabled;
                    _disarmImage.color = _disarmColorDisabled;
                    _grabImage.color = _grabColorEnabled;
                    _harmImage.color = _harmColorDisabled;
                    break;
                case Intent.Harm:
                    _helpImage.color = _helpColorDisabled;
                    _disarmImage.color = _disarmColorDisabled;
                    _grabImage.color = _grabColorDisabled;
                    _harmImage.color = _harmColorEnabled;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Click(int intent)
        {
            if(PlayerActionController.Current != null)
            PlayerActionController.Current.Intent = (Intent)intent;
        }
    }
}

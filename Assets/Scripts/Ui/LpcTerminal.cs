using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Objects.Equipment.Power;
using Assets._MapEditor.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Ui
{
    class LpcTerminal : Form
    {
        [SerializeField]
        private TextMeshProUGUI _stateLabel;
        [SerializeField]
        private TextMeshProUGUI _chargeStateLabel;

        [SerializeField] private FormButton _closeButton;

        [SerializeField] private ColoredFormButton _lightingButton;
        [SerializeField] private ColoredFormButton _equipmentButton;
        [SerializeField] private ColoredFormButton _lifeSupportButton;
        [SerializeField] private ColoredFormButton _containmentButton;

        [SerializeField] private Color _buttonOnColor;
        [SerializeField] private Color _buttonOffColor;

        private LocalPowerController _invoker;

        protected override void Start()
        {
            base.Start();
            _closeButton.SetAction(CloseButtonClick);
            _lightingButton.SetAction(ClickLighting);
            _equipmentButton.SetAction(ClickEquipment);
            _containmentButton.SetAction(ClickContainment);
            _lifeSupportButton.SetAction(ClickLifeSupport);

            StartCoroutine(LabelDelayedUpdater());
        }

        protected  void Update()
        {
            
            UpdateColoredButtons();
        }

        private IEnumerator LabelDelayedUpdater()
        {
            while (true)
            {
                UpdateLabels();
                yield return new WaitForSeconds(1);
                
            }
        }

        private void UpdateLabels()
        {
            _stateLabel.text = (_invoker.StoreState * 100).ToString("0.0") + "%";
            _chargeStateLabel.text = _invoker.ChargeState.ToString();
        }

        private void UpdateColoredButtons()
        {
            _lightingButton.BackGroundColor = _invoker.CheckEnabled(PowerablePriority.Lighting) ? _buttonOnColor : _buttonOffColor;

            _equipmentButton.BackGroundColor = _invoker.CheckEnabled(PowerablePriority.Equipment) ? _buttonOnColor : _buttonOffColor;

            _lifeSupportButton.BackGroundColor = _invoker.CheckEnabled(PowerablePriority.LifeSupport) ? _buttonOnColor : _buttonOffColor;

            _containmentButton.BackGroundColor = _invoker.CheckEnabled(PowerablePriority.Containment) ? _buttonOnColor : _buttonOffColor;
        }

        private void ClickLighting()
        {
            EnsureMobLoaded();
            LocalPlayer?.SendDataToServer(_invoker, _invoker, new byte[1]{(byte)PowerablePriority.Lighting});
        }

        private void ClickEquipment()
        {
            EnsureMobLoaded();
            LocalPlayer?.SendDataToServer(_invoker, _invoker, new byte[1] { (byte)PowerablePriority.Equipment });
        }

        private void ClickLifeSupport()
        {
            EnsureMobLoaded();
            LocalPlayer?.SendDataToServer(_invoker, _invoker, new byte[1] { (byte)PowerablePriority.LifeSupport });
        }

        private void ClickContainment()
        {
            EnsureMobLoaded();
            LocalPlayer?.SendDataToServer(_invoker, _invoker, new byte[1] { (byte)PowerablePriority.Containment });
        }

        private void CloseButtonClick()
        {
            _invoker.OnTerminalDestruction();
            base.Close();
        }

        public void SetInvoker(LocalPowerController invoker)
        {
            _invoker = invoker;
        }
    }
}

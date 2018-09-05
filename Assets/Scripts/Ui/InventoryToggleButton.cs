using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Ui
{
    class InventoryToggleButton : UiElement
    {

        [SerializeField] private HideableInventoryPanel _hideablePanel;
        [SerializeField]
        private bool _panelHidden = true;

        private void Start()
        {
            if(_panelHidden)
                _hideablePanel.HideImmidiately();
        }

        public override void Click()
        {
            _panelHidden = !_panelHidden;
            UpdatePanelState();
        }

        private void UpdatePanelState()
        {
            if (_panelHidden)
            {
                _hideablePanel.Hide();
            }
            else
            {
                _hideablePanel.Show();
            }
        }
    }
}

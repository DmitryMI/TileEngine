using Assets.Scripts;
using Assets.Scripts.Ui;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace Assets._MapEditor.Scripts
{

    [RequireComponent(typeof(Image))]
    public class Form : UiElement
    {
        private CustomInputModule _inputModule;

        private void Start()
        {
            _inputModule = (CustomInputModule)EventSystem.current.currentInputModule;
        }

        private void Update()
        {
            
        }

    }
}

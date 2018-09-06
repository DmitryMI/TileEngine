using UnityEngine;

namespace Assets.Scripts.Ui
{
    public class UiList : UiElement
    {
        [Tooltip("Children of this transfrom will be scrolled")]
        [SerializeField] private Transform _contentGroup;

        private IScroller _scrollerUi;


        private void Start()
        {
            _scrollerUi = GetComponentInChildren<IScroller>();

            if (_scrollerUi == null)
            {
                Debug.LogWarning("No scrollers found");
            }
        }

        protected override void Update()
        {
            base.Update();

            
        }
    }
}

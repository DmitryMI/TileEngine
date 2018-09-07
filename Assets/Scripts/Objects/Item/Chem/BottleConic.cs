using UnityEngine;

namespace Assets.Scripts.Objects.Item.Chem
{
    public class BottleConic : Item
    {
        [SerializeField] private VolumeFiller _filler;

        [SerializeField] private float _fillValue;
        [SerializeField] private Color _color;

        protected override void Update()
        {
            base.Update();

            _filler.FillValue = _fillValue;
            _filler.ReagentsColor = _color;

            if (Holder != null)
            {
                _filler.gameObject.SetActive(false);
            }
            else
            {
                _filler.gameObject.SetActive(true);
            }
        }

        public override void ApplyItemClient(Item item)
        {
            base.ApplyItemClient(item);
            
        }
    }
}

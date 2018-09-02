using UnityEngine;

namespace Assets.Scripts.Objects.Turf
{
    public class Turf : TileObject
    {
        [SerializeField]
        private bool _transperent;
        [SerializeField]
        private bool _canWalkThrough;
        [SerializeField]
        private bool _canContainGas;

        [SerializeField]
        private string _descriptiveName;

        // Use this for initialization
        protected override bool Transperent
        {
            get { return _transperent; }
        }

        protected override bool CanWalkThrough
        {
            get { return _canWalkThrough; }
        }

        protected override bool PassesGas
        {
            get { return _canContainGas; }
        }

        public override string DescriptiveName
        {
            get { return _descriptiveName; }
        }

        protected override void Update()
        {
            base.Update();
        }

    }
}

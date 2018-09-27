using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.GameMechanics.Chemistry
{
    [Serializable]
    public struct SubstanceInfo
    {
        [SerializeField]
        private float _volume;
        [SerializeField]
        private int _substanceId;

        public SubstanceInfo(int id, float volume)
        {
            _substanceId = id;
            _volume = volume;
        }

        public int SubstanceId => _substanceId;

        public float Volume
        {
            get { return _volume; }
            set { _volume = value; }
        }

        public int CompareTo(object obj)
        {
            SubstanceInfo other = (SubstanceInfo)obj;
            return Mathf.Clamp(_substanceId - other.SubstanceId, -1, 1);
        }

        public override string ToString()
        {
            return $"Id: {_substanceId}, Volume: {_volume}";
        }
    }
}

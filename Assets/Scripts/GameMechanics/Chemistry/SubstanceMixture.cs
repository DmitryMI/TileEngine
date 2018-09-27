using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.GameMechanics.Chemistry
{
    [Serializable]
    public class SubstanceMixture : IList<SubstanceInfo>, ICloneable
    {
        [SerializeField]
        private IList<SubstanceInfo> _listImplementation;

        private bool _wasModified;
        private float _volume;

        public SubstanceMixture(int capacity = 0)
        {
            _listImplementation = new List<SubstanceInfo>(capacity);
        }

        public float Volume
        {
            get
            {
                //if(_wasModified)
                    RecalculateValues();
                return _volume;
            }
        }

        public SubstanceMixture SubtractPart(float part)
        {
            if (part > 1 || part <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(part), "Must be inside (0, 1]");
            }

            
            SubstanceMixture mixture = new SubstanceMixture(_listImplementation.Count);

            for (int i = 0; i < _listImplementation.Count; i++)
            {
                SubstanceInfo info = _listImplementation[i];
                SubstanceInfo subtracted = info;
                subtracted.Volume *= part;
                info.Volume -= subtracted.Volume;
                mixture.Add(subtracted);
                _listImplementation[i] = info;
            }

            return mixture;
        }

        public SubstanceMixture SubtractVolume(float volume)
        {
            if(volume > Volume)
                throw new ArgumentOutOfRangeException(nameof(volume), "Must be inside (0, Volume of this instance]. Current volume: " + Volume);

            SubstanceMixture mixture = new SubstanceMixture(_listImplementation.Count);

            for (int i = 0; i < _listImplementation.Count; i++)
            {
                SubstanceInfo info = _listImplementation[i];
                SubstanceInfo subtracted = info;
                subtracted.Volume = volume * GetElementPart(i);
                info.Volume -= subtracted.Volume;
                mixture.Add(subtracted);
                _listImplementation[i] = info;
            }

            return mixture;
        }

        public void Concatinate(IList<SubstanceInfo> otherMixture)
        {
            foreach (var substanceInfo in otherMixture)
            {
                int index = IndexOfSubstance(substanceInfo.SubstanceId);

                if (index == -1)
                {
                    _listImplementation.Add(substanceInfo);
                    //otherMixture.Remove(substanceInfo);
                }
                else
                {
                    SubstanceInfo info = _listImplementation[index];
                    info.Temperature = (substanceInfo.Temperature * substanceInfo.Volume + info.Temperature * info.Volume) / 
                        (info.Volume + substanceInfo.Volume);

                    info.Volume += substanceInfo.Volume;
                    _listImplementation[index] = info;
                }
            }
        }

        public int IndexOfSubstance(int substanceId)
        {
            for(int i = 0; i < _listImplementation.Count; i++)
            {
                if (_listImplementation[i].SubstanceId == substanceId)
                    return i;
            }
            return -1;
        }

        public float GetElementPart(int index)
        {
            return _listImplementation[index].Volume / Volume;
        }

        public float GetElementPart(SubstanceInfo info)
        {
            int index = _listImplementation.IndexOf(info);
            return GetElementPart(index);
        }

        private void RecalculateValues()
        {
            _volume = GetVolume();

            _wasModified = false;
        }

        private float GetVolume()
        {
            float volume = 0;
            foreach (var substance in _listImplementation)
            {
                volume += substance.Volume;
            }
            return volume;
        }

        public IEnumerator<SubstanceInfo> GetEnumerator()
        {
            return _listImplementation.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) _listImplementation).GetEnumerator();
        }

        public void Add(SubstanceInfo item)
        {
            _wasModified = true;
            _listImplementation.Add(item);
        }

        public void Clear()
        {
            _listImplementation.Clear();
            _wasModified = true;
        }

        public bool Contains(SubstanceInfo item)
        {
            return _listImplementation.Contains(item);
        }

        public void CopyTo(SubstanceInfo[] array, int arrayIndex)
        {
            _listImplementation.CopyTo(array, arrayIndex);
        }

        public bool Remove(SubstanceInfo item)
        {
            bool result = _listImplementation.Remove(item);
            if (result)
                _wasModified = true;
            return result;
        }

        public int Count
        {
            get { return _listImplementation.Count; }
        }

        public bool IsReadOnly
        {
            get { return _listImplementation.IsReadOnly; }
        }

        public int IndexOf(SubstanceInfo item)
        {
            return _listImplementation.IndexOf(item);
        }

        public void Insert(int index, SubstanceInfo item)
        {
            _wasModified = true;
            _listImplementation.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            _listImplementation.RemoveAt(index);
            _wasModified = true;
        }

        public SubstanceInfo this[int index]
        {
            get { return _listImplementation[index]; }
            set
            {
                _wasModified = true;
                _listImplementation[index] = value;
            }
        }

        public void AddRange(ICollection<SubstanceInfo> collection)
        {
            foreach (var elem in collection)
            {
                _listImplementation.Add(elem);
            }

            _wasModified = true;
        }

        public override string ToString()
        {
            if (_listImplementation.Count == 0)
                return "<Empty>";

            string message = "";
            for (int i = 0; i < _listImplementation.Count; i++)
            {
                message += $"[{i}]: {_listImplementation[i]}; ";
            }
            return message;
        }

        public object Clone()
        {
            SubstanceMixture mixture = new SubstanceMixture(_listImplementation.Count);
            mixture.AddRange(_listImplementation);
            return mixture;
        }
    }
}

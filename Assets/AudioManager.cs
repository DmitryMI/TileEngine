using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets
{
    public class AudioManager : MonoBehaviour
    {
        [SerializeField] private List<AudioClip> _registeredClips;

        private static AudioManager _instance;
        public static AudioManager Instance => _instance;

        // Start is called before the first frame update
        void Start()
        {
            _instance = this;

            LoadClipsFromResources();
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public AudioClip GetById(int id)
        {
            return _registeredClips[id];
        }

        public int GetId(AudioClip clip)
        {
            return _registeredClips.IndexOf(clip);
        }

        private void LoadClipsFromResources()
        {
            AudioClip[] clips = Resources.FindObjectsOfTypeAll<AudioClip>();
            _registeredClips.AddRange(clips);
            _registeredClips.Sort(new AudioClipsComparer());

            PrintClipsCollection();
        }

        private class AudioClipsComparer : IComparer<AudioClip>
        {
            public int Compare(AudioClip x, AudioClip y)
            {
                /*if (y != null && x != null)
                        return x.GetHashCode() - y.GetHashCode();
                else
                {
                    return 0;
                }*/

                return String.Compare(x.name, y.name, StringComparison.Ordinal);
            }
        }

        private void PrintClipsCollection()
        {
            foreach (var clip in _registeredClips)
            {
                Debug.Log(clip.name);
            }
        }
    }
}

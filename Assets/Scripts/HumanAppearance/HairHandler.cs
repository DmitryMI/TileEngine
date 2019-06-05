using System;
using System.Collections.Generic;
using Assets.Scripts.GameMechanics;
using Assets.Scripts.Objects.Mob;
using UnityEngine;

namespace Assets.Scripts.HumanAppearance
{
    public class HairHandler : MonoBehaviour
    {
        [SerializeField] private HairSet[] _manuallyLoadedHairSets;

        [SerializeField] private Color _defaultHairColor;

        private SpriteRenderer _spriteRenderer;
        private Humanoid _player;
        private List<KeyValuePair<int, HairSet>> _hairSetPrefabs;

        private int _currentSetId = -1;
        private HairSet _currentHairSet;

        private void Start()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            //_player = GetComponent<Player>();
            _player = GetComponentInParent<Humanoid>();

            LoadPrefabs();
        }

        private void LoadPrefabs()
        {
            _hairSetPrefabs = new List<KeyValuePair<int, HairSet>>();

            LoadFromUnity();
            LoadManually();

            Debug.Log("Count of loaded hair sets: " + _hairSetPrefabs.Count);
            bool collisions = CheckForCollisions();

            if (collisions)
            {
                foreach (var prefab in _hairSetPrefabs)
                {
                    Debug.Log(prefab.Value.name + ": " + prefab.Key);
                }
            }
        }

        private void LoadFromUnity()
        {
            HairSet[] found = Resources.FindObjectsOfTypeAll<HairSet>();
            foreach (var hs in found)
            {
                GameObject go = hs.gameObject;
                if (!go.activeInHierarchy && go.activeSelf)
                {
                    _hairSetPrefabs.Add(new KeyValuePair<int, HairSet>(hs.Id, hs));
                }
            }
        }

        private void LoadManually()
        {
            if (_manuallyLoadedHairSets != null)
            {
                foreach (var set in _manuallyLoadedHairSets)
                {
                    if (!AlreadyLoaded(set))
                        _hairSetPrefabs.Add(new KeyValuePair<int, HairSet>(set.Id, set));
                }
            }
        }

        private bool AlreadyLoaded(HairSet set)
        {
            foreach (var prefab in _hairSetPrefabs)
            {
                if (prefab.Value == set)
                    return true;
            }

            return false;
        }

        private bool CheckForCollisions()
        {
            bool flag = false;
            for (int i = 0; i < _hairSetPrefabs.Count; i++)
            {
                for (int j = i + 1; j < _hairSetPrefabs.Count; j++)
                {
                    if (_hairSetPrefabs[i].Key == _hairSetPrefabs[j].Key)
                    {
                        flag = true;
                        string msg = String.Format("Prefab {0} and prefab {1} have same ID: {2}", _hairSetPrefabs[i].Value.name, _hairSetPrefabs[j].Value.name, _hairSetPrefabs[i].Key);
                        Debug.Log(msg);
                    }
                }
            }

            return flag;
        }

        public HairSet FindHairSetById(int id)
        {
            for (int i = 0; i < _hairSetPrefabs.Count; i++)
            {
                if (_hairSetPrefabs[i].Key == id)
                    return _hairSetPrefabs[i].Value;
            }

            Debug.LogError("Hair prefab with ID " + id + " does not exist!");

            return null;
        }

        private void Update()
        {
            UpdateHairSet();
            DrawCurrentHairSet();
        }

        private void UpdateHairSet()
        {
            if (_player.HairSetId != _currentSetId)
            {
                _currentSetId = _player.HairSetId;
                _currentHairSet = FindHairSetById(_currentSetId);
            }
        }

        private void DrawCurrentHairSet()
        {
            HairSet set = _currentHairSet;

            if (set != null)
            {
                switch (_player.SpriteOrientation)
                {
                    case Direction.Forward:
                        _spriteRenderer.sprite = set.Back;
                        transform.localPosition = set.BackOffset;
                        break;
                    case Direction.Backward:
                        _spriteRenderer.sprite = set.Front;
                        transform.localPosition = set.FrontOffset;
                        break;
                    case Direction.Left:
                        _spriteRenderer.sprite = set.Left;
                        transform.localPosition = set.LeftOffset;
                        break;
                    case Direction.Right:
                        _spriteRenderer.sprite = set.Right;
                        transform.localPosition = set.RightOffset;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else
            {
                _spriteRenderer.sprite = null;
            }

            _spriteRenderer.color = _defaultHairColor;

            // TODO Getting hair color from Player object
        }

    }
}

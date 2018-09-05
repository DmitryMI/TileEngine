using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets._MapEditor.Scripts
{
    [Serializable]
    struct PrefabData
    {
        public int Id;
        public GameObject GameObject;

        public bool IsCollision(PrefabData other)
        {
            if (Id == other.Id)
                return true;
            return GameObject == other.GameObject;
        }
    }
}

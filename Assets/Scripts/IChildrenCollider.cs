using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    interface IChildCollider
    {
        GameObject gameObject { get; }
        GameObject Parent { get; }
    }
}

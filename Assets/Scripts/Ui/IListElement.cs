using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Ui
{
    interface IListElement
    {
        void SetAnchor(Vector2 anchor);
        Vector2 AnchoredPosition { get; set; }
        Vector2 Size { get; set; }
    }
}

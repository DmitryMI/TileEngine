using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Ui
{
    interface IUiList
    {
        void Add(RectTransform obj);
        void Remove(RectTransform obj);

        void Clear();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.EventSystems;

namespace Assets.Scripts
{
    class CustomInputModule : StandaloneInputModule
    {
        public PointerEventData GetPointerData()
        {
            if (m_PointerData.ContainsKey(kMouseLeftId))
            {
                return m_PointerData[kMouseLeftId];
            }
            return null;
        }

    }
}

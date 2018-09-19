﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Objects.Mob
{
    interface IHumanoid
    {
        void SetHealthData(HumanHealthData data);
        HumanHealthData GetHealthDataCopy();
    }
}

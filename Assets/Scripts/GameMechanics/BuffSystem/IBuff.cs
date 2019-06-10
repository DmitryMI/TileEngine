﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.GameMechanics.BuffSystem
{
    public interface IDisplayableBuff
    {
        SpriteSet BuffIcon { get; }
        bool IsIconVisible { get; }
    }
}

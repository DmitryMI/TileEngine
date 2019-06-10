using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.GameMechanics.BuffSystem
{
    public interface IBuff
    {
        SpriteSet BuffIcon { get; }
        bool IsIconVisible { get; }

        float SpeedMultiplier { get; }
    }
}

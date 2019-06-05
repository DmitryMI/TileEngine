using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.GameMechanics
{
    public interface IPositionProvider : ICellPositionProvider
    {
        float OffsetX { get; }
        float OffsetY { get; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.GameMechanics
{
    public interface ICellPositionProvider
    {
        int X { get; }
        int Y { get; }
    }
}

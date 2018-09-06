using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Ui
{
    interface IScroller
    {
        float ScrollValue { get; set; }
        float ContentOveflowPart { get; set; }
    }
}

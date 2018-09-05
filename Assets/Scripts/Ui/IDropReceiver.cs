using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Ui
{
    public interface IDropReceiver
    {
        void DropHere(IDragable dragable);
    }
}

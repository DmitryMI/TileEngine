using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Ui
{
    public interface IDragable
    {
        void OnDragStart(IPointerDataProvider pointerDataProvider);
        void OnDragContinue(IPointerDataProvider pointerDataProvider);
        void OnDragEnd(IDropReceiver receiver, IPointerDataProvider pointerDataProvider);
    }
}

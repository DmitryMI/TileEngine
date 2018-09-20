using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Objects.Equipment.Power
{
    public interface IPowerSender
    {
        IPowerConsumer[] FindConsumers();
    }
}

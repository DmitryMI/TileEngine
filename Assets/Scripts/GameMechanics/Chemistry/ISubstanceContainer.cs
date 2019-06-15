using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.GameMechanics.Chemistry
{
    public interface ISubstanceContainer
    {
        GameObject gameObject { get; }
        float RemainingVolume { get; }
        float MaximumVolume { get; }
        void TransferInto(SubstanceMixture mixture);
        void TransferToAnother(ISubstanceContainer container);

        SubstanceMixture Contents { get; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Controllers;
using UnityEngine;

namespace Assets.Scripts.GameMechanics.Chemistry
{
    class ChemistryController : Controller
    {
        public static ChemistryController Current { get; private set; }

        public Color GetSubtanceColor(SubstanceMixture list)
        {
            if (!Current.WasLoaded)
                return Color.clear;

            if (list.Count == 0)
            {
                return Color.clear;
            }

            Color color = GetSubstance(list[0].SubstanceId).Color * list.GetElementPart(0);
            for (int i = 1; i < list.Count; i++)
            {
                Color elementColor = GetSubstance(list[i].SubstanceId).Color * list.GetElementPart(i);
                color += elementColor;
            }

            return color;
        }

        [SerializeField]
        private Substance[] _registeredSubstances;
        

        public override void OnGameLoaded(IServerDataProvider controller)
        {
            Current = this;

            //Debug.Log("ChemistryController: Loaded " + _registeredSubstances.Length + " substances.");

            WasLoaded = true;
        }

        public Substance GetSubstance(int id)
        {
            foreach (var substance in _registeredSubstances)
            {
                if (substance.Id == id)
                    return substance;
            }

            return Substance.IncorrectSubstance;
        }

        public Substance GetSubstance(string substName)
        {
            foreach (var substance in _registeredSubstances)
            {
                if (substance.Name == substName)
                    return substance;
            }

            return Substance.IncorrectSubstance;
        }
    }
}

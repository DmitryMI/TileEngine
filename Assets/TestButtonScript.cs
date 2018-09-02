using Assets.Scripts.Controllers.Atmos;
using UnityEngine;

namespace Assets
{
    public class TestButtonScript : MonoBehaviour {

        // Use this for initialization
        public void AddOxygenGas()
        {
            AtmosController atmos = AtmosController.Current;
            if (atmos != null)
                atmos.AddGas(4, 4, 0, 1, 200, 25);
        }

        public void AddNitrogenGas()
        {
            AtmosController atmos = AtmosController.Current;
            if(atmos != null)
                atmos.AddGas(7, 7, 1, 1, 200, 25);
        }
    }
}

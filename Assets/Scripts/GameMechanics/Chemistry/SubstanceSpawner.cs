using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Controllers;
using Assets.Scripts.Objects.Item;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.GameMechanics.Chemistry
{
    class SubstanceSpawner : NetworkBehaviour
    {
        [SerializeField] private Item _target;

        [SerializeField] private SubstanceInfo[] _spawnMixture;

        void Start()
        {
            if(isServer)
                StartCoroutine(WaitForGameReady());
        }

        IEnumerator WaitForGameReady()
        {
            while(ServerController.Current == null || !ServerController.Current.Ready)
                yield return new WaitForEndOfFrame();

            if (_spawnMixture.Length != 0)
            {
                ISubstanceContainer container;

                if (_target == null)
                {
                    container = GetComponent<ISubstanceContainer>();
                }
                else
                {
                    container = _target as ISubstanceContainer;
                }

                SubstanceMixture mixture = new SubstanceMixture(_spawnMixture.Length);

                mixture.AddRange(_spawnMixture);

                Debug.Log("SubstanceSpawner: " + mixture);


                if (container != null)
                {
                    container.TransferInto(mixture);
                }
                else
                    Debug.LogError("Target not found! Please, attach target to field or make this script a component of an ISubstanceContainer");
            }
        }
    }
}

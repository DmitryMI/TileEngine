using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    class Utils
    {
        public const float PerLayerSpace = 0.1f;
        public const float LayerOrderSpace = 0.001f;

        public float LayerToZ(string layer, int order)
        {
            var sortingLayers = SortingLayer.layers;

            int index;
            for (index = 0; index < sortingLayers.Length; index++)
            {
                if (sortingLayers[index].name == layer)
                {
                    float layerZ = PerLayerSpace * index;
                    float nextLayerZ = layerZ + PerLayerSpace;
                    float orderSpace = layerZ + order * LayerOrderSpace;

                    if (orderSpace >= nextLayerZ)
                    {
                        Debug.LogWarning("Object reached another layer");
                    }

                    return orderSpace;
                }
            }

            return 0;
        }

    }
}

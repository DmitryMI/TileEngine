using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Random = System.Random;
using UnityRandom = UnityEngine.Random;

namespace Assets.Scripts
{
    static class Utils
    {
        public const float PerLayerSpace = 0.1f;
        public const float LayerOrderSpace = 0.001f;

        private static Random _csharpRandom = new Random();

        public static float LayerToZ(string layer, int order)
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

        public static bool IsCellInRectInclusive(int x, int y, int rectCenterX, int rectCenterY, int width, int height)
        {
            int highBound = rectCenterY - height;
            int lowBound = rectCenterY + height;
            int leftBound = rectCenterX + width;
            int rightBound = rectCenterX - width;

            bool fitsVertically = highBound <= y && y <= lowBound;
            bool fitsHorizontally = rightBound <= x && x <= leftBound;

            return fitsVertically  && fitsHorizontally;
        }

        public static T GetRandom<T>(ICollection<T> array)
        {
            int index = UnityRandom.Range(0, array.Count);

            return array.ElementAt(index);
        }

        public static Color ClampRedGreenIntensity(Color c, float maxIntensity)
        {
            Vector2 vector2 = new Vector2(c.r, c.g);
            vector2.Normalize();
            vector2 = vector2 * maxIntensity;

            return new Color(vector2.x, vector2.y, 0);
        }

        public static float NextGaussian(float mu = 0, float sigma = 1)
        {
            if (sigma <= 0)
                throw new ArgumentOutOfRangeException("sigma", "Must be greater than zero.");

            float v1, v2, rSquared;
            do
            {
                // two random values between -1.0 and 1.0
                v1 = 2.0f * (float)_csharpRandom.NextDouble() - 1;
                v2 = 2.0f * (float)_csharpRandom.NextDouble() - 1;
                rSquared = v1 * v1 + v2 * v2;
                // ensure within the unit circle
            } while (rSquared >= 1 || rSquared == 0);

            // calculate polar tranformation for each deviate
            float polar = (float)Math.Sqrt(-2 * Math.Log(rSquared) / rSquared);
            // store first deviate

            // return second deviate
            return v1 * polar * sigma + mu;
        }

        public static double NextGaussian(double mu = 0, double sigma = 1)
        {
            if (sigma <= 0)
                throw new ArgumentOutOfRangeException("sigma", "Must be greater than zero.");

            double v1, v2, rSquared;
            do
            {
                // two random values between -1.0 and 1.0
                v1 = 2 * _csharpRandom.NextDouble() - 1;
                v2 = 2 * _csharpRandom.NextDouble() - 1;
                rSquared = v1 * v1 + v2 * v2;
                // ensure within the unit circle
            } while (rSquared >= 1 || rSquared == 0);

            // calculate polar tranformation for each deviate
            var polar = Math.Sqrt(-2 * Math.Log(rSquared) / rSquared);
            // store first deviate
            
            // return second deviate
            return v1 * polar * sigma + mu;
        }
    }
}

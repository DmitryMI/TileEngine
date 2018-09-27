using System;
using UnityEngine;

namespace Assets.Scripts.GameMechanics
{
    [Serializable]
    public class LightSourceInfo
    {
        private int _x;
        private int _y;
        private Color _color;
        private float _maxRange;
        private float _intensityDecrement = 0.05f;
        private float _initialIntensity;

        public int X
        {
            get { return _x; }
        }

        public int Y
        {
            get { return _y; }
        }

        public Vector2Int SourceCell
        {
            get { return new Vector2Int(_x, _y);}
        }

        public Color LightColor
        {
            get { return _color; }
        }

        public float Range
        {
            get { return _maxRange; }
        }

        public float Decrement
        {
            get {return _intensityDecrement; }
        }

        public float InitialIntensity
        {
            get { return _initialIntensity; }
        }

        public LightSourceInfo(int x, int y, Color color, float range)
        {
            _x = x;
            _y = y;
            _color = color;
            _maxRange = range;
            _initialIntensity = 1f;
        }

        public LightSourceInfo(int x, int y, Color color, float range, float initialIntensity, float decrement)
        {
            _x = x;
            _y = y;
            _color = color;
            _maxRange = range;
            _initialIntensity = initialIntensity;
            _intensityDecrement = decrement;
        }

        public virtual float CalculateIntensityRange(float range)
        {
            float b = _initialIntensity;
            float k = -1f / (_maxRange + 1f);
            return k * range + b;
        }

        public virtual float CalculateIntensity(Vector2Int cell)
        {
            int dx = Mathf.Abs(cell.x - _x);
            int dy = Mathf.Abs(cell.y - _y);

            int range =  Mathf.Max(dx, dy);

            if (range > _maxRange)
                return 0;

            return _initialIntensity - range * _intensityDecrement;
        }

        public override string ToString()
        {
            string msg = "X: {0}, Y: {1}, Color: {2}, Range: {3}, Intensity: {4}";
            return String.Format(msg, _x, _y, _color, _maxRange, _initialIntensity);
        }
    }
}

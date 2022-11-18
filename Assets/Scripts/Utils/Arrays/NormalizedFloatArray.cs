using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Utils.Arrays
{
    public class NormalizedFloatArray : IArray<float>, IFixedDimensional
    {
        public float[] Data { get; }
        private float _minimum;
        private float _maximum;
        private float _range;

        public NormalizedFloatArray(float[] data)
        {
            Data = data;
            _minimum = data.Min();
            _maximum = data.Max();
            _range = _maximum - _minimum;
        }

        public IEnumerator<float> GetEnumerator()
        {
            return (IEnumerator<float>) Data.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int GetLength(int dimension)
        {
            return Data.GetLength(0);
        }

        public float this[int x]
        {
            get => (Data[x] - _minimum) / _range;
            set
            {
                value = value * _range + _minimum;
                if (value > _maximum)
                {
                    _maximum = value;
                    _range = _maximum - _minimum;
                }
                else if (value < _minimum)
                {
                    _minimum = value;
                    _range = _maximum - _minimum;
                }
                
                Data[x] = value;
            }
        }
    }
}
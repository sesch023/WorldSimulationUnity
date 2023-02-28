using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Utils.Arrays
{
    /// <summary>
    /// A one dimensional array which values are visualized in a normalized way. 
    /// </summary>
    public class NormalizedFloatArray : IArray<float>, IFixedDimensional
    {
        public float[] Data { get; }
        private float _minimum;
        private float _maximum;
        private float _range;

        /// <summary>
        /// Create the array with the given data.
        /// </summary>
        /// <param name="data">Data of the array.</param>
        public NormalizedFloatArray(float[] data)
        {
            Data = data;
            _minimum = data.Min();
            _maximum = data.Max();
            _range = _maximum - _minimum;
        }

        /// <summary>
        /// Returns the enumerator for the array.
        /// </summary>
        /// <returns>Enumerator for the array.</returns>
        public IEnumerator<float> GetEnumerator()
        {
            return (IEnumerator<float>) Data.GetEnumerator();
        }

        /// <summary>
        /// Returns the enumerator for the array.
        /// </summary>
        /// <returns>Enumerator for the array.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Returns the length of the given dimension.
        /// </summary>
        /// <param name="dimension">Dimension to return length of.</param>
        /// <returns>Length of given dimension.</returns>
        public int GetLength(int dimension)
        {
            return Data.GetLength(0);
        }

        /// <summary>
        /// Access the array at the given index.
        /// </summary>
        /// <param name="x">Position to access at.</param>
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
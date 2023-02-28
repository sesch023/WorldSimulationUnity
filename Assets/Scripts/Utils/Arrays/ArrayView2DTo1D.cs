using System;
using System.Collections;
using System.Collections.Generic;

namespace Utils.Arrays
{
    /// <summary>
    /// A view of a 2D array as a 1D array.
    /// </summary>
    /// <typeparam name="TReal"></typeparam>
    /// <typeparam name="TView"></typeparam>
    public class ArrayView2DTo1D<TReal, TView> : IArray<TView>
    {
        private I2DArray<TReal> _internal;
        private Func<TReal, TView> _accessor;
        private ArrayMapSetter<TReal, TView> _setter;

        /// <summary>
        /// Constructs a new view of a 2D array as a 1D array.
        /// </summary>
        /// <param name="internalArray">2D Array</param>
        /// <param name="accessor">How to access the array.</param>
        /// <param name="setter">How the set elements in the array.</param>
        public ArrayView2DTo1D(I2DArray<TReal> internalArray, Func<TReal, TView> accessor, ArrayMapSetter<TReal, TView> setter)
        {
            _internal = internalArray;
            _accessor = accessor;
            _setter = setter;
        }
        
        /// <summary>
        /// Returns the enumerator for the array.
        /// </summary>
        /// <returns>Enumerator for the array.</returns>
        public IEnumerator<TView> GetEnumerator()
        {
            return new ArrayMap2DEnumerator<TReal, TView>(_internal, _accessor);
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
        /// Returns the length of the array in one dimension.
        /// </summary>
        public int GetLength(int dimension)
        {
            return _internal.GetLength(0) * _internal.GetLength(1);
        }

        /// <summary>
        /// Accesses the element at the given index through the accessor or setter.
        /// </summary>
        /// <param name="x">Given index.</param>
        public TView this[int x]
        {
            get => _accessor(_internal[x / _internal.GetLength(1), x % _internal.GetLength(1)]);
            set => _setter(_internal, x / _internal.GetLength(1), x % _internal.GetLength(1), value);
        }
    }
}
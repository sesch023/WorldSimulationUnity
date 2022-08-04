using System;

namespace Utils.Arrays
{
    /// <summary>
    /// Creates a view on a 2d array with a type TReal and provides a view of it with type TView using
    /// the provided accessor.
    /// </summary>
    /// <typeparam name="TReal">Real type of the array.</typeparam>
    /// <typeparam name="TView">View type of the array.</typeparam>
    public class ArrayView2D<TReal, TView> : I2DArray<TView>
    {
        /// Internal array.
        private readonly TReal[,] _array;

        /// Accessor to convert a TReal to a TView.
        public Func<TReal, TView> Accessor { get; private set; }

        /// <summary>
        /// Creates a view on a 2d array with a type TReal and provides a view of it with type TView using
        /// the provided accessor.
        /// </summary>
        /// <param name="array">Internal array.</param>
        /// <param name="accessor">Accessor to convert a TReal to a TView.</param>
        public ArrayView2D(TReal[,] array, Func<TReal, TView> accessor)
        {
            _array = array;
            Accessor = accessor;
        }
        
        /// <summary>
        /// Returns the value at the given index in the array.
        /// </summary>
        /// <param name="indexX">Outer dimension.</param>
        /// <param name="indexY">Inner dimension.</param>
        /// <returns>Value at the given index.</returns>
        public TReal GetRealTypeByIndex(int indexX, int indexY)
        {
            return _array[indexX, indexY];
        }
        
        /// <summary>
        /// Returns the value at the given index in the array using the accessor.
        /// </summary>
        /// <param name="indexX">Outer dimension.</param>
        /// <param name="indexY">Inner dimension.</param>
        /// <returns>Converted value at the given index.</returns>
        public TView this[int indexX, int indexY]
        {
            get => Accessor(_array[indexX, indexY]);
        }

        /// <summary>
        /// Creates a 2d enumerator for the array.
        /// </summary>
        /// <returns>2d enumerator for the array.</returns>
        public I2DEnumerator<TView> Get2DEnumerator()
        {
            return new ArrayView2DEnumerator<TReal, TView>(_array, Accessor);
        }

        /// <summary>
        /// Get length of the array in given dimension.
        /// </summary>
        /// <param name="dimension">Either 0 (outer) or 1 (inner).</param>
        /// <returns>Length of the given dimension.</returns>
        public int GetLength(int dimension)
        {
            return _array.GetLength(dimension);
        }
        
        /// <summary>
        /// Returns the internal array.
        /// </summary>
        /// <returns>Internal array.</returns>
        public TReal[,] GetRealArray()
        {
            return _array;
        }
        
        /// <summary>
        /// Creates a string representation of the array.
        /// </summary>
        /// <returns>String representation of the array.</returns>
        public override string ToString()
        {
            return I2DArray<TView>.I2DArrayToString(this);
        }
    }
}
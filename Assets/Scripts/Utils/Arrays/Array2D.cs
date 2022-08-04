namespace Utils.Arrays
{
    /// <summary>
    /// Basic 2d array implementation of the IArray2D interface.
    /// </summary>
    /// <typeparam name="TReal"></typeparam>
    public class Array2D<TReal> : I2DArray<TReal>
    {
        /// Internal array of values.
        private readonly TReal[,] _array;
        
        /// <summary>
        /// Constructor of Array2D.
        /// </summary>
        /// <param name="array">The internal Array.</param>
        public Array2D(TReal[,] array)
        {
            _array = array;
        }
        
        /// <summary>
        /// Returns a value at the given position.
        /// </summary>
        /// <param name="indexX">Position in first dimension.</param>
        /// <param name="indexY">Position in second dimension.</param>
        public TReal this[int indexX, int indexY]
        {
            get => _array[indexX, indexY];
        }

        /// <summary>
        /// Get 2D Enumerator of the array.
        /// </summary>
        /// <returns></returns>
        public I2DEnumerator<TReal> Get2DEnumerator()
        {
            return new Array2DEnumerator<TReal>(_array);
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
            return I2DArray<TReal>.I2DArrayToString(this);
        }
    }
}
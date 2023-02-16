namespace Utils.Arrays
{
    /// <summary>
    /// Basic 2d arrayImmutable implementation of the IArray2D interface.
    /// </summary>
    /// <typeparam name="TReal"></typeparam>
    public class ArrayImmutable2D<TReal> : I2DArrayImmutable<TReal>
    {
        /// Internal arrayImmutable of values.
        private readonly TReal[,] _array;
        
        /// <summary>
        /// Constructor of ArrayImmutable2D.
        /// </summary>
        /// <param name="array">The internal Array.</param>
        public ArrayImmutable2D(TReal[,] array)
        {
            _array = array;
        }
        
        /// <summary>
        /// Returns a value at the given position.
        /// </summary>
        /// <param name="indexX">MapPositionVec in first dimension.</param>
        /// <param name="indexY">MapPositionVec in second dimension.</param>
        public TReal this[int indexX, int indexY] => _array[indexX, indexY];

        /// <summary>
        /// Get 2D Enumerator of the arrayImmutable.
        /// </summary>
        /// <returns></returns>
        public I2DEnumerator<TReal> Get2DEnumerator()
        {
            return new Array2DEnumerator<TReal>(this);
        }

        /// <summary>
        /// Get length of the arrayImmutable in given dimension.
        /// </summary>
        /// <param name="dimension">Either 0 (outer) or 1 (inner).</param>
        /// <returns>Length of the given dimension.</returns>
        public int GetLength(int dimension)
        {
            return _array.GetLength(dimension);
        }
        
        /// <summary>
        /// Returns the internal arrayImmutable.
        /// </summary>
        /// <returns>Internal arrayImmutable.</returns>
        public TReal[,] GetArray()
        {
            return _array;
        }

        /// <summary>
        /// Creates a string representation of the arrayImmutable.
        /// </summary>
        /// <returns>String representation of the arrayImmutable.</returns>
        public override string ToString()
        {
            return I2DArrayImmutable<TReal>.I2DArrayToString(this);
        }
    }
}
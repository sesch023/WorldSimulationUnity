using System;

namespace Utils.Arrays
{
    /// <summary>
    /// Creates a map on a 2d arrayImmutable with a type TReal and provides a map of it with type TMapped using
    /// the provided accessor.
    /// </summary>
    /// <typeparam name="TReal">Real type of the arrayImmutable.</typeparam>
    /// <typeparam name="TMapped">Map type of the arrayImmutable.</typeparam>
    public class ArrayImmutableMap2D<TReal, TMapped> : I2DArrayImmutable<TMapped>
    {
        /// Internal arrayImmutable.
        private readonly I2DArrayImmutable<TReal> _array;

        /// Accessor to convert a TReal to a TMapped.
        public Func<TReal, TMapped> Accessor { get; private set; }

        /// <summary>
        /// Creates a map on a 2d arrayImmutable with a type TReal and provides a map of it with type TMapped using
        /// the provided accessor.
        /// </summary>
        /// <param name="array">Internal arrayImmutable.</param>
        /// <param name="accessor">Accessor to convert a TReal to a TMapped.</param>
        public ArrayImmutableMap2D(I2DArrayImmutable<TReal> array, Func<TReal, TMapped> accessor)
        {
            _array = array;
            Accessor = accessor;
        }
        
        public ArrayImmutableMap2D(TReal[,] array, Func<TReal, TMapped> accessor)
        {
            _array = new ArrayImmutable2D<TReal>(array);
            Accessor = accessor;
        }
        
        /// <summary>
        /// Returns the value at the given index in the arrayImmutable.
        /// </summary>
        /// <param name="indexX">Outer dimension.</param>
        /// <param name="indexY">Inner dimension.</param>
        /// <returns>Value at the given index.</returns>
        public TReal GetRealTypeByIndex(int indexX, int indexY)
        {
            return _array[indexX, indexY];
        }
        
        /// <summary>
        /// Returns the value at the given index in the arrayImmutable using the accessor.
        /// </summary>
        /// <param name="indexX">Outer dimension.</param>
        /// <param name="indexY">Inner dimension.</param>
        /// <returns>Converted value at the given index.</returns>
        public TMapped this[int indexX, int indexY]
        {
            get => Accessor(_array[indexX, indexY]);
        }

        /// <summary>
        /// Creates a 2d enumerator for the arrayImmutable.
        /// </summary>
        /// <returns>2d enumerator for the arrayImmutable.</returns>
        public I2DEnumerator<TMapped> Get2DEnumerator()
        {
            return new ArrayMap2DEnumerator<TReal, TMapped>(_array, Accessor);
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
        public I2DArrayImmutable<TReal> GetRealArray()
        {
            return _array;
        }
        
        /// <summary>
        /// Creates a string representation of the arrayImmutable.
        /// </summary>
        /// <returns>String representation of the arrayImmutable.</returns>
        public override string ToString()
        {
            return I2DArrayImmutable<TMapped>.I2DArrayToString(this);
        }
    }
}
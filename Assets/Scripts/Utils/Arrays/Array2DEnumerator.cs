using System;
using System.Collections;

namespace Utils.Arrays
{
    /// <summary>
    /// Enumerator for a 2D array.
    /// </summary>
    /// <typeparam name="TRealEnum">Internal Type of the 2D array.</typeparam>
    public class Array2DEnumerator<TRealEnum> : I2DEnumerator<TRealEnum>
    {
        /// Reference to the 2D array.
        protected readonly TRealEnum[,] Array;
        /// Current position in first dimension.
        protected int CountX = -1;
        /// Current position in second dimension.
        protected int CountY;

        /// <summary>
        /// Creates a new enumerator for a 2D array.
        /// </summary>
        /// <param name="array">Reference to the 2d array.</param>
        public Array2DEnumerator(TRealEnum[,] array)
        {
            Array = array;
        }

        /// <summary>
        /// Creates a new enumerator for a Array2D.
        /// </summary>
        /// <param name="array">Reference to the Array2D.</param>
        public Array2DEnumerator(Array2D<TRealEnum> array)
        {
            Array = array.GetRealArray();
        }

        /// <summary>
        /// Resets the enumerator.
        /// </summary>
        public void Reset()
        {
            CountX = -1;
            CountY = 0;
        }

        /// Current value of the enumerator.
        object IEnumerator.Current => Current;
        
        /// <summary>
        /// Returns the current value of the enumerator.
        /// </summary>
        /// <exception cref="InvalidOperationException">If the enumerator has ended.</exception>
        public virtual TRealEnum Current
        {
            get
            {
                try
                {
                    return Array[CountX, CountY];
                }
                catch (IndexOutOfRangeException)
                {
                    throw new InvalidOperationException($"InvalidOperationException: {GetType()} - Enumerator has ended!");
                }
            }
        }

        /// <summary>
        /// Disposes the enumerator.
        /// </summary>
        public void Dispose()
        {
        }

        /// <summary>
        /// Moves the enumerator to the next element.
        /// </summary>
        /// <returns>
        /// EnumerationStatus2D.Next: If the enumerator has moved to the next element.
        /// EnumerationStatus2D.NextRow: If the enumerator has moved to the next row.
        /// EnumerationStatus2D.End: If the enumerator has ended.
        /// </returns>
        public EnumerationStatus2D MoveNext2D()
        {
            if (CountX < Array.GetLength(0) - 1)
            {
                CountX++;
                return EnumerationStatus2D.Next;
            }

            if (CountY < Array.GetLength(1) - 1)
            {
                CountX = 0;
                CountY++;
                return EnumerationStatus2D.NextRow;
            }

            return EnumerationStatus2D.End;
        }
        
        /// <summary>
        /// Moves the enumerator to the next element.
        /// </summary>
        /// <returns>False, if the enumerator has ended.</returns>
        public bool MoveNext()
        {
            return MoveNext2D() > 0;
        }
    }
}
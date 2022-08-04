using System.Collections;
using System.Collections.Generic;

namespace Utils.Array2D
{
    /// <summary>
    /// Status of a 2d enumerator.
    /// End: Enumerator has ended.
    /// Next: Next element is available.
    /// NextRow: Next element is available and the row has changed.
    /// </summary>
    public enum EnumerationStatus2D
    {
        End = 0,
        Next = 1,
        NextRow = 2
    }
    
    /// <summary>
    /// Interface for a 2d enumerator.
    /// </summary>
    /// <typeparam name="T">Type of the 2d enumerable.</typeparam>
    public interface I2DEnumerator<out T> : IEnumerator<T>
    {
        /// <summary>
        /// Moves to the next element.
        /// </summary>
        /// <returns>False, if the enumerator has ended.</returns>
        bool IEnumerator.MoveNext()
        {
            return MoveNext();
        }

        /// <summary>
        /// Moves to the next element.
        /// </summary>
        /// <returns>False, if the enumerator has ended.</returns>
        public new bool MoveNext();
        
        /// <summary>
        /// Moves the enumerator to the next element.
        /// </summary>
        /// <returns>
        /// EnumerationStatus2D.Next: If the enumerator has moved to the next element.
        /// EnumerationStatus2D.NextRow: If the enumerator has moved to the next row.
        /// EnumerationStatus2D.End: If the enumerator has ended.
        /// </returns>
        EnumerationStatus2D MoveNext2D();
    }
}
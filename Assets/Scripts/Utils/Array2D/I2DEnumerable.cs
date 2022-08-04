using System.Collections;
using System.Collections.Generic;

namespace Utils.Array2D
{
    /// <summary>
    /// Interface for a 2d enumerable.
    /// </summary>
    /// <typeparam name="T">Type of the enumerable.</typeparam>
    public interface I2DEnumerable<out T> : IEnumerable<T>
    {
        /// <summary>
        /// Returns the 2d enumerator as basic enumerator.
        /// </summary>
        /// <returns>Enumerator of the enumerable.</returns>
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return Get2DEnumerator();
        }

        /// <summary>
        /// Returns the 2d enumerator as basic enumerator.
        /// </summary>
        /// <returns>Enumerator of the enumerable.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return Get2DEnumerator();
        }

        /// <summary>
        /// Returns the 2d enumerator.
        /// </summary>
        /// <returns>2d enumerator.</returns>
        I2DEnumerator<T> Get2DEnumerator();
    }
}
// ReSharper disable once TypeParameterCanBeVariant
using System.Text;

namespace Utils.Array2D
{
    /// <summary>
    /// Interface for a 2D array.
    /// </summary>
    /// <typeparam name="TReal">Type of the 2d array.</typeparam>
    public interface I2DArray<TReal> : I2DEnumerable<TReal>, IFixedDimensional
    {
        /// <summary>
        /// Returns the value at the given position.
        /// </summary>
        /// <param name="x">Outer dimension.</param>
        /// <param name="y">Inner dimension.</param>
        TReal this[int x, int y] { get; }

        /// <summary>
        /// Builds a string from a I2DArray.
        /// </summary>
        /// <param name="array">Array to build a string of.</param>
        /// <returns>String representation of the array.</returns>
        public static string I2DArrayToString(I2DArray<TReal> array)
        {
            I2DEnumerator<TReal> enumerator = array.Get2DEnumerator();
            StringBuilder builder = new StringBuilder("[\n[ ");
            EnumerationStatus2D status = enumerator.MoveNext2D();
            while (status != EnumerationStatus2D.End)
            {
                if (status == EnumerationStatus2D.NextRow)
                {
                    builder.Length -= 2;
                    builder.Append(" ],\n[ ");
                }

                builder.Append(enumerator.Current).Append(", ");
                
                status = enumerator.MoveNext2D();
            }
            builder.Length -= 2;
            builder.Append(" ]\n]");
            return builder.ToString();
        }
    }
}
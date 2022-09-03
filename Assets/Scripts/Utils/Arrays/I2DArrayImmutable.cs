// ReSharper disable once TypeParameterCanBeVariant

using System.Text;

namespace Utils.Arrays
{
    /// <summary>
    /// Interface for a 2D arrayImmutable.
    /// </summary>
    /// <typeparam name="TReal">Type of the 2d arrayImmutable.</typeparam>
    public interface I2DArrayImmutable<TReal> : I2DEnumerable<TReal>, IFixedDimensional
    {
        /// <summary>
        /// Returns the value at the given position.
        /// </summary>
        /// <param name="x">Outer dimension.</param>
        /// <param name="y">Inner dimension.</param>
        TReal this[int x, int y] { get; }

        /// <summary>
        /// Builds a string from a I2DArrayImmutable.
        /// </summary>
        /// <param name="arrayImmutable">Array to build a string of.</param>
        /// <returns>String representation of the arrayImmutable.</returns>
        public static string I2DArrayToString(I2DArrayImmutable<TReal> arrayImmutable)
        {
            I2DEnumerator<TReal> enumerator = arrayImmutable.Get2DEnumerator();
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
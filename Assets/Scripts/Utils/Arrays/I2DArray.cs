// ReSharper disable once TypeParameterCanBeVariant

using System.Text;

namespace Utils.Arrays
{
    /// <summary>
    /// Interface for a 2D arrayImmutable.
    /// </summary>
    /// <typeparam name="TReal">Type of the 2d arrayImmutable.</typeparam>
    public interface I2DArray<TReal> : I2DArrayImmutable<TReal>
    {
        /// <summary>
        /// Returns the value at the given position.
        /// </summary>
        /// <param name="x">Outer dimension.</param>
        /// <param name="y">Inner dimension.</param>
        new TReal this[int x, int y] { get; set; }
    }
}
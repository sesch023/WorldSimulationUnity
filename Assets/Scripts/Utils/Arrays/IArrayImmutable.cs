using System.Collections.Generic;

namespace Utils.Arrays
{
    /// <summary>
    /// A immutable array, which is a enumerable with a fixed size and dimension.
    /// </summary>
    /// <typeparam name="TReal">Type of the array.</typeparam>
    public interface IArrayImmutable<TReal> : IEnumerable<TReal>, IFixedDimensional
    {
        /// <summary>
        /// Access only.
        /// </summary>
        /// <param name="x">Access at.</param>
        TReal this[int x] { get; }
    }
}
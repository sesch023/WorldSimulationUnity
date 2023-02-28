using System.Collections.Generic;

namespace Utils.Arrays
{
    /// <summary>
    /// A array, which is a enumerable with a fixed size and dimension. Also provides a setter.
    /// </summary>
    /// <typeparam name="TReal">Type of the array.</typeparam>
    public interface IArray<TReal> : IEnumerable<TReal>, IFixedDimensional
    {
        /// <summary>
        /// Access and set the value of the array at the given index.
        /// </summary>
        /// <param name="x">Given index.</param>
        TReal this[int x] { get; set; }
    }
}
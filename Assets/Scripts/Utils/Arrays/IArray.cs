using System.Collections.Generic;

namespace Utils.Arrays
{
    public interface IArray<TReal> : IEnumerable<TReal>, IFixedDimensional
    {
        TReal this[int x] { get; set; }
    }
}
using System.Collections.Generic;

namespace Utils.Arrays
{
    public interface IArrayImmutable<TReal> : IEnumerable<TReal>, IFixedDimensional
    {
        TReal this[int x] { get; }
    }
}
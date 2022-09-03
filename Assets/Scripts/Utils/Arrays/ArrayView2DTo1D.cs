using System;
using System.Collections;
using System.Collections.Generic;

namespace Utils.Arrays
{
    public class ArrayView2DTo1D<TReal, TView> : IArray<TView>
    {
        private I2DArrayImmutable<TReal> _internal;
        private Func<TReal, TView> _accessor;

        public ArrayView2DTo1D(I2DArrayImmutable<TReal> internalArrayImmutable, Func<TReal, TView> accessor)
        {
            _internal = internalArrayImmutable;
            _accessor = accessor;
        }
        
        public IEnumerator<TView> GetEnumerator()
        {
            return new ArrayMap2DEnumerator<TReal, TView>(_internal, _accessor);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int GetLength(int dimension)
        {
            return _internal.GetLength(0) * _internal.GetLength(1);
        }

        public TView this[int x] => _accessor(_internal[x / _internal.GetLength(1), x % _internal.GetLength(1)]);
    }
}
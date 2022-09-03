﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace Utils.Arrays
{
    public class ArrayView2DTo1D<TReal, TView> : IArray<TView>
    {
        private I2DArray<TReal> _internal;
        private Func<TReal, TView> _accessor;
        private ArrayMapSetter<TReal, TView> _setter;

        public ArrayView2DTo1D(I2DArray<TReal> internalArray, Func<TReal, TView> accessor, ArrayMapSetter<TReal, TView> setter)
        {
            _internal = internalArray;
            _accessor = accessor;
            _setter = setter;
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

        public TView this[int x]
        {
            get => _accessor(_internal[x / _internal.GetLength(1), x % _internal.GetLength(1)]);
            set => _setter(_internal, x / _internal.GetLength(1), x % _internal.GetLength(1), value);
        }
    }
}
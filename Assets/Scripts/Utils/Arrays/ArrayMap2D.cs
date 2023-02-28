using System;

namespace Utils.Arrays
{
    /// <summary>
    /// Setting a element in a 2d array at position (x,y) to a value. This requires that a conversion
    /// between TReal and TMapped is possible.
    /// </summary>
    /// <typeparam name="TReal">Type of the underlying array.</typeparam>
    /// <typeparam name="TMapped">Type that the array is looked at with.</typeparam>
    public delegate void ArrayMapSetter<TReal, TMapped>(I2DArray<TReal> array, int x, int y, TMapped mapped);
    
    /// <summary>
    /// Creates a Map in a 2d array between two types. This requires that a conversion between TReal and TMapped is possible.
    /// </summary>
    /// <typeparam name="TReal">Type of the underlying array.</typeparam>
    /// <typeparam name="TMapped">Type that the array is looked at with.</typeparam>
    public class ArrayMap2D<TReal, TMapped> : ArrayImmutableMap2D<TReal, TMapped>, I2DArray<TMapped>
    {
        
        /// Internal arrayImmutable.
        private readonly I2DArray<TReal> _array;
        private readonly ArrayMapSetter<TReal, TMapped> _setter;

        public ArrayMap2D(I2DArray<TReal> array, Func<TReal, TMapped> accessor, ArrayMapSetter<TReal, TMapped> setter) : base(array, accessor)
        {
            _array = array;
            _setter = setter;
        }
        
        public ArrayMap2D(TReal[,] array, Func<TReal, TMapped> accessor, ArrayMapSetter<TReal, TMapped> setter) : base(new ArrayImmutable2D<TReal>(array), accessor)
        {
            _array = new Array2D<TReal>(array);
            _setter = setter;
        }
        
        public new TMapped this[int x, int y]
        {
            get => base[x, y];
            set => _setter(_array, x, y, value);
        }
    }
}
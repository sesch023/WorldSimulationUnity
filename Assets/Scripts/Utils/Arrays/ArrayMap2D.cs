using System;

namespace Utils.Arrays
{
    public class ArrayMap2D<TReal, TMapped> : ArrayImmutableMap2D<TReal, TMapped>, I2DArray<TMapped>
    {
        public delegate void Setter(I2DArray<TReal> array, int x, int y, TMapped mapped);
        
        /// Internal arrayImmutable.
        private readonly I2DArray<TReal> _array;
        private readonly Setter _setter;

        public ArrayMap2D(I2DArray<TReal> array, Func<TReal, TMapped> accessor, Setter setter) : base(array, accessor)
        {
            _array = array;
            _setter = setter;
        }
        
        public ArrayMap2D(TReal[,] array, Func<TReal, TMapped> accessor, Setter setter) : base(new ArrayImmutable2D<TReal>(array), accessor)
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
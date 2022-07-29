using System;

namespace Utils.Array2D
{
    public class ArrayView2D<TReal, TView> : I2DArray<TView>
    {
        private readonly TReal[,] _array;
        private readonly Func<TReal, TView> _accessor;

        public ArrayView2D(TReal[,] array, Func<TReal, TView> accessor)
        {
            _array = array;
            _accessor = accessor;
        }
        
        public TReal GetRealTypeByIndex(int indexX, int indexY)
        {
            return _array[indexX, indexY];
        }
        
        public TView this[int indexX, int indexY]
        {
            get => _accessor(_array[indexX, indexY]);
        }

        public I2DEnumerator<TView> Get2DEnumerator()
        {
            return new ArrayView2DEnumerator<TReal, TView>(_array, _accessor);
        }

        public int GetLength(int dimension)
        {
            return _array.GetLength(dimension);
        }
        
        public override string ToString()
        {
            return I2DArray<TView>.I2DArrayToString(this);
        }
    }
}
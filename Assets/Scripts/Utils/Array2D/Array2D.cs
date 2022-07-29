namespace Utils.Array2D
{
    public class Array2D<TReal> : I2DArray<TReal>
    {
        private readonly TReal[,] _array;
        
        public Array2D(TReal[,] array)
        {
            _array = array;
        }
        
        public void Add(int x, int y, TReal value)
        {
            _array[x,y] = value;
        }
        
        public TReal this[int indexX, int indexY]
        {
            get => _array[indexX, indexY];
        }

        public I2DEnumerator<TReal> Get2DEnumerator()
        {
            return new Array2DEnumerator<TReal>(_array);
        }

        public int GetLength(int dimension)
        {
            return _array.GetLength(dimension);
        }

        public override string ToString()
        {
            return I2DArray<TReal>.I2DArrayToString(this);
        }
    }
}
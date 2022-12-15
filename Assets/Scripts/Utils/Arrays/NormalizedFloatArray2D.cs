namespace Utils.Arrays
{
    public class NormalizedFloatArray2D : I2DArray<float>
    {
        public I2DArray<float> Data { get; }
        private float _minimum;
        private float _maximum;
        private float _range;

        public NormalizedFloatArray2D(float[,] data): this(new Array2D<float>(data)){}
        
        public NormalizedFloatArray2D(I2DArray<float> data)
        {
            Data = data;
            _minimum = BaseUtils.Util.MinIn2DArray(data);
            _maximum = BaseUtils.Util.MaxIn2DArray(data);
            _range = _maximum - _minimum;
        }
        
        public I2DEnumerator<float> Get2DEnumerator()
        {
            return new Array2DEnumerator<float>(Data);
        }

        public int GetLength(int dimension)
        {
            return Data.GetLength(dimension);
        }

        public float this[int x, int y]
        {
            get => (Data[x, y] - _minimum) / _range;
            set
            {
                value = value * _range + _minimum;
                if (value > _maximum)
                {
                    _maximum = value;
                    _range = _maximum - _minimum;
                }
                else if (value < _minimum)
                {
                    _minimum = value;
                    _range = _maximum - _minimum;
                }
                
                Data[x, y] = value;
            }
        }
    }
}
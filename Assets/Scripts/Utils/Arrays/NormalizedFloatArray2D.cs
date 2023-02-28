namespace Utils.Arrays
{
    /// <summary>
    /// A one dimensional array which values are visualized in a normalized way. 
    /// </summary>
    public class NormalizedFloatArray2D : I2DArray<float>
    {
        public I2DArray<float> Data { get; }
        private float _minimum;
        private float _maximum;
        private float _range;

        /// <summary>
        /// Constructs the array from a two dimensional array.
        /// </summary>
        /// <param name="data">Two dimensional array.</param>
        public NormalizedFloatArray2D(float[,] data): this(new Array2D<float>(data)){}
        
        /// <summary>
        /// Constructs the array from a I2DArray.
        /// </summary>
        /// <param name="data">I2DArray.</param>
        public NormalizedFloatArray2D(I2DArray<float> data)
        {
            Data = data;
            _minimum = BaseUtils.Util.MinIn2DArray(data);
            _maximum = BaseUtils.Util.MaxIn2DArray(data);
            _range = _maximum - _minimum;
        }
        
        /// <summary>
        /// Returns a two dimensional enumerator.
        /// </summary>
        /// <returns>2d enumerator.</returns>
        public I2DEnumerator<float> Get2DEnumerator()
        {
            return new Array2DEnumerator<float>(Data);
        }

        /// <summary>
        /// Returns the length of the given dimension.
        /// </summary>
        /// <param name="dimension">Dimension to return length of.</param>
        /// <returns>Length of given dimension.</returns>
        public int GetLength(int dimension)
        {
            return Data.GetLength(dimension);
        }

        /// <summary>
        /// Access the array at the given index.
        /// </summary>
        /// <param name="x">Position to access at.</param>
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
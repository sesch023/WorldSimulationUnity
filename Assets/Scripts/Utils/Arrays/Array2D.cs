namespace Utils.Arrays
{
    public class Array2D<T> : ArrayImmutable2D<T>, I2DArray<T>
    {
        public Array2D(T[,] array) : base(array)
        {
        }

        public new T this[int x, int y]
        {
            get => base[x, y];
            set => GetRealArray()[x, y] = value;
        }
    }
}
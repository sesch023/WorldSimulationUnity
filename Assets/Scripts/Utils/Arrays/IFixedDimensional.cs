namespace Utils.Arrays
{
    /// <summary>
    /// Interface for data structures with a fixed dimensional size.
    /// </summary>
    public interface IFixedDimensional
    {
        /// <summary>
        /// Returns the size of the given dimension.
        /// </summary>
        /// <param name="dimension">Dimension to return size to.</param>
        /// <returns>Size of the given dimension.</returns>
        public int GetLength(int dimension);
    }
}
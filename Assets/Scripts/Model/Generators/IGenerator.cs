namespace Model.Generators
{
    /// <summary>
    /// Basic interface for a generator.
    /// </summary>
    public interface IGenerator
    {
        /// <summary>
        /// Method for generating the map as a 2D arrayImmutable of floats.
        /// </summary>
        /// <param name="sizeX">Width of the Map.</param>
        /// <param name="sizeY">Height of the Map.</param>
        /// <returns>2D Array of floats as generated elevation.</returns>
        public (float[,] elevation, float min, float max) GenerateElevation(int sizeX, int sizeY);
        
        /// <summary>
        /// Limits the size of the generated map depending on the generator's algorithm
        /// and the given parameters. The default implementation just returns the given size.
        /// </summary>
        /// <param name="sizeX">Desired width of the map.</param>
        /// <param name="sizeY">Desired height of the map.</param>
        /// <returns>Agreed on size of the map which takes the desired sizes in mind.</returns>
        public (int sizeX, int sizeY) LimitMapSizes(int sizeX, int sizeY);
    }
}
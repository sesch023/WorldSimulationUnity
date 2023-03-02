namespace Model.Processing
{
    /// <summary>
    /// Interface for processing a data array.
    /// </summary>
    public interface IDataArrayProcessing
    {
        /// <summary>
        /// Processes a given array.
        /// </summary>
        /// <param name="map">Array to Process.</param>
        public void ProcessGeneratorData(float[,] map);
    }
}
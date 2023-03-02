namespace Model.Processing
{
    /// <summary>
    /// Interface for processing a map.
    /// </summary>
    public interface IMapProcessing
    {
        /// <summary>
        /// Processes a given map.
        /// </summary>
        /// <param name="map">Map to Process.</param>
        public void ProcessMap(Map.Map map);
    }
}
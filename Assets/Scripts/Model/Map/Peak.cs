using UnityEngine;
using Utils.Array2D;

namespace Model.Map
{
    /// <summary>
    /// Finds and defines a peak in a map or 2D array of floats.
    /// </summary>
    public class Peak : Valley
    {
        /// Start value of the exits elevation. Is updated with each closer exit found.
        protected override float ExitElevationStart => float.NegativeInfinity;

        /// <summary>
        /// Finds a peak in a map. Takes the elevation of the start position as minimum elevation.
        /// </summary>
        /// <param name="start">Start point in the map.</param>
        /// <param name="elevations">Map of MapUnits.</param>
        public Peak(Vector2Int start, MapUnit[,] elevations) : base(start, elevations)
        {
        }

        /// <summary>
        /// Finds a peak in a 2D array of float elevations. Takes the elevation of the start position as minimum
        /// elevation.
        /// </summary>
        /// <param name="start">Start point in the map.</param>
        /// <param name="elevations">2D array of float elevations.</param>
        public Peak(Vector2Int start, I2DArray<float> elevations) : base(start, elevations)
        {
        }
        
        /// <summary>
        /// Finds a peak in a map. Starts at the start point. Takes the given elevation as minimum elevation.
        /// </summary>
        /// <param name="start">Start point in the map.</param>
        /// <param name="elevations">Map of MapUnits.</param>
        /// <param name="minElevation">Minimum elevation of the peak.</param>
        public Peak(Vector2Int start, MapUnit[,] elevations, float minElevation) : base(start, elevations, minElevation)
        {
        }
        
        /// <summary>
        /// Finds a peak in a 2D array of float elevations. Starts at the start point. Takes the given elevation
        /// as minimum elevation.
        /// </summary>
        /// <param name="start">Start point in the map.</param>
        /// <param name="elevations">2D array of float elevations.</param>
        /// <param name="minElevation">Minimum elevation of the peak.</param>
        public Peak(Vector2Int start, I2DArray<float> elevations, float minElevation) : base(start, elevations, minElevation)
        {
        }
        
        /// <summary>
        /// Evaluates if the given position is part of the peak. 
        /// </summary>
        /// <param name="position">Position of the valley to check.</param>
        /// <param name="elevation">Elevation which is checked against.</param>
        /// <returns>If the current position is greater or equal to the given, return true.</returns>
        protected override bool ElevationCondition(Vector2Int position, float elevation)
        {
            return MapElevations[position.x, position.y] >= elevation;
        }
    }
}
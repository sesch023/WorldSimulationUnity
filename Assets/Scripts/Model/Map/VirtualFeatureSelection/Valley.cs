using System.Collections.Generic;
using System.Linq;
using Model.Map.Feature;
using UnityEngine;
using Utils.Arrays;
using Utils.BaseUtils;

namespace Model.Map.VirtualFeatureSelection
{
    /// <summary>
    /// Finds and defines a valley in a map or 2D arrayImmutable of floats.
    /// </summary>
    public class Valley : IFeature
    {
        /// <summary>
        /// Positions of the valley.
        /// </summary>
        public Vector2Int[] CalculatedPositions { get; private set; }
        
        /// <summary>
        /// Exits of the valley. These are not part of the valley, but are the positions of the next highest points
        /// on the border of the valley.
        /// </summary>
        public Vector2Int[] CalculatedExits { get; private set; }
        
        /// <summary>
        /// Border of the valley. These are part of the valley.
        /// </summary>
        public Vector2Int[] CalculatedBorder { get; private set; }
        
        /// Elevation of the start.
        private float _maxElevation;
        /// All elevations in the map.
        protected I2DArrayImmutable<float> MapElevations;
        /// Start position.
        private Vector2Int _start;
        
        /// Current elevation of next highest exit points.
        private float _exitElevation;
        /// Current position of next highest exit points.
        private List<Vector2Int> _currentExitPositions;
        /// Current positions of the valley.
        private List<Vector2Int> _valleyPositions;
        /// Current positions of the border.
        private HashSet<Vector2Int> _borderPositions;
        /// Next positions to be checked.
        private Queue<Vector2Int> _nextPositions;
        /// Positions which were already checked.
        private HashSet<Vector2Int> _visited;

        /// Width of the map.
        private int _sizeX;
        /// Height of the map.
        private int _sizeY;

        /// Start value of the exits elevation. Is updated with each closer exit found.
        protected virtual float ExitElevationStart => float.PositiveInfinity;
        
        /// <summary>
        /// Finds a valley in a map. Takes the elevation of the start position as maximum elevation.
        /// </summary>
        /// <param name="start">Start point in the map.</param>
        /// <param name="elevations">Map of MapUnits.</param>
        public Valley(Vector2Int start, MapUnit[,] elevations)
        {
            ArrayImmutableMap2D<MapUnit, float> immutableMap = new ArrayImmutableMap2D<MapUnit, float>(elevations, unit => unit.Position.Elevation);
            Reset(start, immutableMap);
        }
        
        /// <summary>
        /// Finds a valley in a 2D arrayImmutable of float elevations. Takes the elevation of the start position as maximum
        /// elevation.
        /// </summary>
        /// <param name="start">Start point in the map.</param>
        /// <param name="elevations">2D arrayImmutable of float elevations.</param>
        public Valley(Vector2Int start, I2DArrayImmutable<float> elevations)
        {
            Reset(start, elevations);
        }
        
        /// <summary>
        /// Finds a valley in a map. Starts at the start point. Takes the given elevation as maximum elevation.
        /// </summary>
        /// <param name="start">Start point in the map.</param>
        /// <param name="elevations">Map of MapUnits.</param>
        /// <param name="maxElevation">Maximum elevation of the valley.</param>
        public Valley(Vector2Int start, MapUnit[,] elevations, float maxElevation)
        {
            ArrayImmutableMap2D<MapUnit, float> immutableMap = new ArrayImmutableMap2D<MapUnit, float>(elevations, unit => unit.Position.Elevation);
            Reset(start, immutableMap, maxElevation);
        }

        /// <summary>
        /// Finds a valley in a 2D arrayImmutable of float elevations. Starts at the start point. Takes the given elevation
        /// as maximum elevation.
        /// </summary>
        /// <param name="start">Start point in the map.</param>
        /// <param name="elevations">2D arrayImmutable of float elevations.</param>
        /// <param name="maxElevation">Maximum elevation of the valley.</param>
        public Valley(Vector2Int start, I2DArrayImmutable<float> elevations, float maxElevation)
        {
            Reset(start, elevations, maxElevation);
        }
        
        private void Reset(Vector2Int start, I2DArrayImmutable<float> mapElevations)
        {
            Reset(start, mapElevations, mapElevations[start.x, start.y]);
        }
        
        private void Reset(Vector2Int start, I2DArrayImmutable<float> mapElevations, float maxElevation)
        {
            _start = start;
            MapElevations = mapElevations;
            _maxElevation = maxElevation;
            
            _sizeX = MapElevations.GetLength(0);
            _sizeY = MapElevations.GetLength(1);
            
            FloodFill();
        }

        /// <summary>
        /// Floods the valley from the start position with the given or maximum elevation of the start position.
        /// Initializes all other variables for the a
        /// </summary>
        private void FloodFill()
        {
            _valleyPositions = new List<Vector2Int>();
            _nextPositions = new Queue<Vector2Int>();
            _visited = new HashSet<Vector2Int>();
            _exitElevation = ExitElevationStart;
            _currentExitPositions = new List<Vector2Int>();
            _borderPositions = new HashSet<Vector2Int>();
            _nextPositions.Enqueue(_start);
            _visited.Add(_start);

            // As long as there are positions to check, continue flooding.
            while (_nextPositions.Count > 0)
            {
                FloodFillStep();
            }
            
            CalculatedPositions = _valleyPositions.ToArray();
            CalculatedExits = _currentExitPositions.ToArray();
            CalculatedBorder = _borderPositions.ToArray();
        }

        /// <summary>
        /// A single step of the flooding.
        /// </summary>
        private void FloodFillStep()
        {
            // Try to dequeue the next position.
            bool hasNext = _nextPositions.TryDequeue(out var nextPosition);

            if (hasNext)
            {
                // If the elevation condition is true, execute the step for a next position.
                if (ElevationCondition(nextPosition, _maxElevation))
                {
                    NextElevationStep(nextPosition);
                }
                // If the elevations condition is false, but elevation of the next position is in one unit of the current exits
                // add the next position to the current exits.
                else if (MathUtil.AlmostEquals(_exitElevation, MapElevations[nextPosition.x, nextPosition.y], 1f))
                {
                    _currentExitPositions.Add(nextPosition);
                }
                // If the elevation condition is false and its elevation is smaller than that of the current exits,
                // clear the current exits and add the current position.
                else if(ElevationCondition(nextPosition, _exitElevation))
                {
                    _currentExitPositions.Clear();
                    _currentExitPositions.Add(nextPosition);
                    _exitElevation = MapElevations[nextPosition.x, nextPosition.y];
                }
            }
        }
        
        /// <summary>
        /// The given position is part of the valley. Add it to the valley and add its neighbors to the next positions.
        /// Also checks if the position is part of the border.
        /// </summary>
        /// <param name="nextPosition">Position to add to the valley and check.</param>
        private void NextElevationStep(Vector2Int nextPosition)
        {
            _valleyPositions.Add(nextPosition);
                    
            Vector2Int[] neighbors = MathUtil.GetNeighborPositionsIn2DArray(nextPosition, _sizeX, _sizeY);
            bool isBorder = false;
            foreach (Vector2Int neighbor in neighbors)
            {
                // If the elevation condition is wrong for a single neighbor, the position is part of the border.
                if (!isBorder && !ElevationCondition(neighbor, _maxElevation))
                {
                    _borderPositions.Add(nextPosition);
                    isBorder = true;
                }
                
                // Add the neighbor to the next positions if it is not visited yet.
                if (!_visited.Contains(neighbor))
                {
                    _nextPositions.Enqueue(neighbor);
                    _visited.Add(neighbor);
                }
            }
        }
        
        /// <summary>
        /// Evaluates if the given position is part of the valley. Can be overridden to change the elevation condition.
        /// </summary>
        /// <param name="position">Position of the valley to check.</param>
        /// <param name="elevation">Elevation which is checked against.</param>
        /// <returns>If the current position is smaller or equal to the given, return true.</returns>
        protected virtual bool ElevationCondition(Vector2Int position, float elevation)
        {
            return MapElevations[position.x, position.y] <= elevation;
        }
        
        /// <summary>
        /// Calculates the volume of a valley.
        /// </summary>
        /// <param name="valleyPositions">Positions in the valley.</param>
        /// <param name="mapElevations">Elevations of the map.</param>
        /// <param name="tileArea">Area of a single tile.</param>
        /// <returns>Volume of the given valley.</returns>
        public static float CalculateOpenVolume(Vector2Int[] valleyPositions, I2DArrayImmutable<float> mapElevations, float tileArea=1.0f)
        {
            float high = 0.0f;
            foreach (var vec in valleyPositions)
            {
                if (mapElevations[vec.x, vec.y] > high)
                    high = mapElevations[vec.x, vec.y];
            }
            
            float volume = 0f;
            for (int i = 0; i < valleyPositions.Length; i++)
            {
                Vector2Int position = valleyPositions[i];
                float elevation = mapElevations[position.x, position.y];
                volume += (high - elevation) * tileArea;
            }
            return volume;
        }

        public Vector2Int[] GetFeaturePositions()
        {
            return CalculatedPositions;
        }
    }
}
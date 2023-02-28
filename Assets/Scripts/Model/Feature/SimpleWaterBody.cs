using System;
using System.Linq;
using Model.Map;
using Model.VirtualFeatureSelection;
using UnityEngine;

namespace Model.Feature
{
    /// <summary>
    /// A simple body of water, that fills a valley to a certain height of a selected position. Does not support refilling,
    /// collision, removing water and creating from a certain volume.
    /// </summary>
    public class SimpleWaterBody : IBody
    {
        /// Map on which the body is located.
        private Map.Map _map;
        /// Valley that makes up the body.
        private Valley _bodyValley;
        /// How much water is in the body.
        public float WaterVolume { get; private set; }
        /// Absolute elevation of the water surface.
        private float _maxElevation;

        /// <summary>
        /// Creates a new water body on the selected position.
        /// </summary>
        /// <param name="map">Map on which the body is created.</param>
        /// <param name="initialPosition">Position with the maximum height of the water body.</param>
        public SimpleWaterBody(Map.Map map, Vector2Int initialPosition)
        {
            _map = map;
            WaterVolume = 0;
            map.AddBody(this);
            ResetValley(initialPosition);
        }
        
        /// <summary>
        /// Resets the valley to the selected position.
        /// </summary>
        /// <param name="initialPosition">New position of the valley.</param>
        /// <exception cref="ArgumentException">If there is already a body of water at the position.</exception>
        public void ResetValley(Vector2Int initialPosition)
        {
            if(_map.MapUnits[initialPosition.x, initialPosition.y].WaterLevel > 0)
                throw new ArgumentException($"ArgumentException: {GetType().Name}. Given Position is already filled with Water!");
            
            _bodyValley = new Valley(initialPosition, _map.MapUnits);
            _maxElevation = _bodyValley.CalculatedExits.Min(val => _map.MapUnits[val.x, val.y].Position.Elevation);
            UpdateWaterLevel();
        }

        /// <summary>
        /// Updates the water level of the valley.
        /// </summary>
        private void UpdateWaterLevel()
        {
            foreach (var pos in GetFeaturePositions())
            {
                _map.MapUnits[pos.x, pos.y].WaterLevel = _maxElevation - _map.MapUnits[pos.x, pos.y].Position.Elevation;
                WaterVolume += _map.MapUnits[pos.x, pos.y].WaterLevel;
            }
        }
        
        /// <summary>
        /// Gets the positions of the map units that are part of the body.
        /// </summary>
        /// <returns>Array of positions of the body of water on the map.</returns>
        public Vector2Int[] GetFeaturePositions()
        {
            return _bodyValley.CalculatedPositions;
        }

        /// <summary>
        /// Checks if the body contains the selected position.
        /// </summary>
        /// <param name="position">Position to check.</param>
        /// <returns>True, if the body contains the position.</returns>
        public bool InBody(Vector2Int position)
        {
            return _bodyValley.CalculatedPositions.Contains(position);
        }
    }
}
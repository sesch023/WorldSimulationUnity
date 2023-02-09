using System;
using System.Linq;
using Model.Map;
using Model.VirtualFeatureSelection;
using UnityEngine;

namespace Model.Feature
{
    public class SimpleWaterBody : IBody
    {
        private Map.Map _map;
        private Valley _bodyValley;
        public float WaterVolume { get; private set; }
        private float _maxElevation;

        public SimpleWaterBody(Map.Map map, Vector2Int initialPosition)
        {
            _map = map;
            WaterVolume = 0;
            map.AddBody(this);
            ResetValley(initialPosition);
        }
        
        public void ResetValley(Vector2Int initialPosition)
        {
            if(_map.MapUnits[initialPosition.x, initialPosition.y].WaterLevel > 0)
                throw new ArgumentException($"ArgumentException: {GetType().Name}. Given Position is already filled with Water!");
            
            _bodyValley = new Valley(initialPosition, _map.MapUnits);
            _maxElevation = _bodyValley.CalculatedExits.Min(val => _map.MapUnits[val.x, val.y].Position.Elevation);
            UpdateWaterLevel();
        }

        private void UpdateWaterLevel()
        {
            foreach (var pos in GetFeaturePositions())
            {
                _map.MapUnits[pos.x, pos.y].WaterLevel = _maxElevation - _map.MapUnits[pos.x, pos.y].Position.Elevation;
                WaterVolume += _map.MapUnits[pos.x, pos.y].WaterLevel;
            }
        }
        
        public Vector2Int[] GetFeaturePositions()
        {
            return _bodyValley.CalculatedPositions;
        }

        public bool InBody(Vector2Int position)
        {
            return _bodyValley.CalculatedPositions.Contains(position);
        }
    }
}
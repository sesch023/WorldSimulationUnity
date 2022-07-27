using System;
using System.Collections.Generic;
using System.Linq;
using Manager;
using Unity.VisualScripting;
using UnityEngine;
using Utils.Array2D;
using Utils.BaseUtils;

namespace Model.Map
{
    public class Valley
    {
        public Vector2Int[] CalculatedValleyPositions { get; private set; }
        public Vector2Int[] CalculatedValleyOpenings { get; private set; }
        
        public Vector2Int[] CalculatedValleyBorder { get; private set; }
        
        private float _elevation;
        private I2DArray<float> _mapElevations;
        private Vector2Int _start;

        private float _openingElevation;
        private List<Vector2Int> _currentOpeningPositions;
        private List<Vector2Int> _valleyPositions;
        private HashSet<Vector2Int> _borderPositions;
        private Queue<Vector2Int> _nextPositions;
        private HashSet<Vector2Int> _visited;

        public Valley(Vector2Int start, MapUnit[,] mapUnits)
        {
            ArrayView2D<MapUnit, float> view = new ArrayView2D<MapUnit, float>(mapUnits, unit => unit.Position.Elevation);
            Reset(start, view);
        }

        public Valley(Vector2Int start, I2DArray<float> mapUnits)
        {
            Reset(start, mapUnits);
        }
        
        public Valley(Vector2Int start, MapUnit[,] mapUnits, float elevation)
        {
            ArrayView2D<MapUnit, float> view = new ArrayView2D<MapUnit, float>(mapUnits, unit => unit.Position.Elevation);
            Reset(start, view, elevation);
        }

        public Valley(Vector2Int start, I2DArray<float> mapUnits, float elevation)
        {
            Reset(start, mapUnits, elevation);
        }
        
        public void Reset(Vector2Int start, I2DArray<float> mapElevations)
        {
            Reset(start, mapElevations, mapElevations[start.x, start.y]);
        }
        
        public void Reset(Vector2Int start, I2DArray<float> mapElevations, float elevation)
        {
            _start = start;
            _mapElevations = mapElevations;
            _elevation = elevation;
            
            FloodFillValley();
        }

        private void FloodFillValley()
        {
            _valleyPositions = new List<Vector2Int>();
            _nextPositions = new Queue<Vector2Int>();
            _visited = new HashSet<Vector2Int>();
            _openingElevation = float.PositiveInfinity;
            _currentOpeningPositions = new List<Vector2Int>();
            _borderPositions = new HashSet<Vector2Int>();
            _nextPositions.Enqueue(_start);
            _visited.Add(_start);

            while (_nextPositions.Count > 0)
            {
                FloodFillStep();
            }
            
            CalculatedValleyPositions = _valleyPositions.ToArray();
            CalculatedValleyOpenings = _currentOpeningPositions.ToArray();
            CalculatedValleyBorder = _borderPositions.ToArray();
        }

        private void FloodFillStep()
        {
            int sizeX = _mapElevations.GetLength(0);
            int sizeY = _mapElevations.GetLength(1);
            bool hasNext = _nextPositions.TryDequeue(out var nextPosition);

            if (hasNext)
            {
                if (_mapElevations[nextPosition.x, nextPosition.y] <= _elevation)
                {
                    NextElevationStep(nextPosition, sizeX, sizeY);
                }
                else if (MathUtil.AlmostEquals(_openingElevation, _mapElevations[nextPosition.x, nextPosition.y], 1f))
                {
                    _currentOpeningPositions.Add(nextPosition);
                }
                else if(_openingElevation > _mapElevations[nextPosition.x, nextPosition.y])
                {
                    _currentOpeningPositions.Clear();
                    _currentOpeningPositions.Add(nextPosition);
                    _openingElevation = _mapElevations[nextPosition.x, nextPosition.y];
                }
            }
        }

        private void NextElevationStep(Vector2Int nextPosition, int sizeX, int sizeY)
        {
            _valleyPositions.Add(nextPosition);
                    
            Vector2Int[] neighbors = MathUtil.GetNeighborPositionsIn2DArray(nextPosition, sizeX, sizeY);
            bool isBorder = false;
            foreach (Vector2Int neighbor in neighbors)
            {
                if (!isBorder && _mapElevations[neighbor.x, neighbor.y] > _elevation)
                {
                    _borderPositions.Add(nextPosition);
                    isBorder = true;
                }
                
                if (!_visited.Contains(neighbor))
                {
                    _nextPositions.Enqueue(neighbor);
                    _visited.Add(neighbor);
                }
            }
        }
    }
}
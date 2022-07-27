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
        public Vector2Int[] CalculatedPositions { get; private set; }
        public Vector2Int[] CalculatedExits { get; private set; }
        
        public Vector2Int[] CalculatedBorder { get; private set; }
        
        private float _elevation;
        protected I2DArray<float> MapElevations;
        private Vector2Int _start;
        
        private float _exitElevation;
        private List<Vector2Int> _currentExitPositions;
        private List<Vector2Int> _valleyPositions;
        private HashSet<Vector2Int> _borderPositions;
        private Queue<Vector2Int> _nextPositions;
        private HashSet<Vector2Int> _visited;
        
        protected virtual float ExitElevationStart => float.PositiveInfinity;
        
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
            MapElevations = mapElevations;
            _elevation = elevation;
            
            FloodFill();
        }

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

            while (_nextPositions.Count > 0)
            {
                FloodFillStep();
            }
            
            CalculatedPositions = _valleyPositions.ToArray();
            CalculatedExits = _currentExitPositions.ToArray();
            CalculatedBorder = _borderPositions.ToArray();
        }

        private void FloodFillStep()
        {
            int sizeX = MapElevations.GetLength(0);
            int sizeY = MapElevations.GetLength(1);
            bool hasNext = _nextPositions.TryDequeue(out var nextPosition);

            if (hasNext)
            {
                if (ElevationCondition(nextPosition, _elevation))
                {
                    NextElevationStep(nextPosition, sizeX, sizeY);
                }
                else if (MathUtil.AlmostEquals(_exitElevation, MapElevations[nextPosition.x, nextPosition.y], 1f))
                {
                    _currentExitPositions.Add(nextPosition);
                }
                else if(ElevationCondition(nextPosition, _exitElevation))
                {
                    _currentExitPositions.Clear();
                    _currentExitPositions.Add(nextPosition);
                    _exitElevation = MapElevations[nextPosition.x, nextPosition.y];
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
                if (!isBorder && !ElevationCondition(neighbor, _elevation))
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
        
        protected virtual bool ElevationCondition(Vector2Int position, float elevation)
        {
            return MapElevations[position.x, position.y] <= elevation;
        }
        
        public static float CalculateValleyVolume(Vector2Int[] valleyPositions, I2DArray<float> mapElevations, float tileArea=1.0f)
        {
            float volume = 0f;
            for (int i = 0; i < valleyPositions.Length; i++)
            {
                Vector2Int position = valleyPositions[i];
                float elevation = mapElevations[position.x, position.y];
                volume += elevation * tileArea;
            }
            return volume;
        }
    }
}
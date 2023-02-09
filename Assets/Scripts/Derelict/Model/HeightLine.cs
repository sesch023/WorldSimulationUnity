using System.Collections.Generic;
using Model;
using UnityEngine;
using Utils.Arrays;
using Utils.BaseUtils;

namespace Derelict.Model
{
    public class HeightLine
    {
        public Vector2Int[] CalculatedHeightLine { get; private set; }
        
        private float _elevation;
        private I2DArrayImmutable<float> _mapElevations;
        private Vector2Int _start;

        public HeightLine(Vector2Int start, MapUnit[,] mapUnits, float elevation)
        {
            ArrayImmutableMap2D<MapUnit, float> immutableMap = new ArrayImmutableMap2D<MapUnit, float>(mapUnits, unit => unit.Position.Elevation);
            Reset(start, immutableMap, elevation);
        }

        public HeightLine(Vector2Int start, I2DArrayImmutable<float> mapUnits, float elevation)
        {
            Reset(start, mapUnits, elevation);
        }

        public void Reset(Vector2Int start, I2DArrayImmutable<float> mapElevations, float elevation)
        {
            _start = start;
            _mapElevations = mapElevations;
            _elevation = elevation;

            CalculatedHeightLine = FindHeightLine();
        }
        
        //TODO: Algorithmus ist nicht zielführend in den meisten Fällen.
        public Vector2Int[] FindHeightLine()
        {
            List<Vector2Int> heightLine = new List<Vector2Int>();
            heightLine.Add(_start);

            int sizeX = _mapElevations.GetLength(0);
            int sizeY = _mapElevations.GetLength(1);

            Vector2Int currentPos = _start;
            int borderCounter = 0;
            int iterations = 0;
            bool nextFound;
            
            do
            {
                Vector2Int[] neighbors = MathUtil.GetNeighborPositionsIn2DArray(currentPos, sizeX, sizeY);
                Vector2Int next = neighbors[0];
                float error = float.PositiveInfinity;
                nextFound = false;

                foreach (var neighbor in neighbors)
                {
                    float nextError = Mathf.Abs(_mapElevations[neighbor.x, neighbor.y] - _elevation);
                    if (nextError < error && !heightLine.Contains(neighbor) && !MathUtil.NextPointCrossesLineDiagonally(neighbor, heightLine))
                    {
                        error = nextError;
                        next = neighbor;
                        nextFound = true;
                    }
                }

                if (!nextFound)
                {
                    break;
                }
                
                currentPos = next;
                heightLine.Add(currentPos);
                borderCounter = MathUtil.At2DArrayBorder(currentPos, sizeX, sizeY) ? borderCounter + 1 : 0;
                iterations++;
            } while (iterations < 10000 && currentPos != _start);

            return heightLine.ToArray();
        }
    }
}
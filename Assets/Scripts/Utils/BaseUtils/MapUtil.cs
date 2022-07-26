using System.Collections.Generic;
using System.IO;
using Model;
using Unity.VisualScripting;
using UnityEngine;
using Utils.Array2D;

namespace Utils
{
    public static class MapUtil
    {
        public static Vector2Int[] GetSlopeLine(Vector2Int start, MapUnit[,] mapUnits, float momentumMultiplier = 1.0f,
            float maxMomentumFraction = 1.0f)
        {
            ArrayView2D<MapUnit, float> view = new ArrayView2D<MapUnit, float>(mapUnits, unit => unit.Position.Elevation);
            return GetSlopeLine(start, view, momentumMultiplier, maxMomentumFraction);
        }
        
        public static Vector2Int[] GetSlopeLine(Vector2Int start, I2DArray<float> elevations, float momentumMultiplier=1.0f, float maxMomentumFraction=1.0f)
        {
            List<Vector2Int> slopeLine = new List<Vector2Int>();
            slopeLine.Add(start);
            bool found;
            Vector2Int currentPos = start;
            float previousElevation = float.PositiveInfinity;
            float momentumLeft = 0;
            int momentumPopCounter = 0;
            
            do
            {
                found = false;
                bool foundNeighbor = false;
                Vector2Int[] neighbors = MathUtil.GetNeighborPositionsIn2DArray(currentPos, elevations.GetLength(0), elevations.GetLength(1));
                Vector2Int next = neighbors[0];
                float nextElevation = float.PositiveInfinity;
                
                foreach (var neighbor in neighbors)
                {
                    float neightborElevation = elevations[neighbor.x, neighbor.y];
                    if (neightborElevation < nextElevation && !slopeLine.Contains(neighbor) && !MathUtil.NextPointCrossesLineDiagonally(neighbor, slopeLine))
                    {
                        nextElevation = neightborElevation;
                        next = neighbor;
                        foundNeighbor = true;
                    }
                }

                if (!foundNeighbor)
                    break;

                currentPos = next;
                float momentumTerm = momentumLeft * maxMomentumFraction;
                
                if ((nextElevation - momentumLeft) <= previousElevation)
                {
                    if (nextElevation <= previousElevation)
                    {
                        momentumLeft += Mathf.Abs((float.IsPositiveInfinity(previousElevation)) ? maxMomentumFraction * nextElevation : previousElevation - nextElevation);
                        momentumLeft *= momentumMultiplier;
                        momentumPopCounter = 0;
                    }
                    else
                    {
                        momentumLeft -= momentumTerm;
                        momentumPopCounter++;
                    }
                    found = true;
                    slopeLine.Add(currentPos);
                    previousElevation = nextElevation;
                }
            } while (found);
            
            if(momentumPopCounter > 0)
                slopeLine.RemoveRange(slopeLine.Count - momentumPopCounter, momentumPopCounter);
            
            return slopeLine.ToArray();
        }

        public static Vector2Int[] FindHeightLine(Vector2Int start, MapUnit[,] mapUnits, float elevation)
        {
            ArrayView2D<MapUnit, float> view = new ArrayView2D<MapUnit, float>(mapUnits, unit => unit.Position.Elevation);
            return FindHeightLine(start, view, elevation);
        }
        
        // Algorithmus ist nicht zielführend in den meisten Fällen.
        public static Vector2Int[] FindHeightLine(Vector2Int start, I2DArray<float> mapUnits, float elevation)
        {
            List<Vector2Int> heightLine = new List<Vector2Int>();
            heightLine.Add(start);

            int sizeX = mapUnits.GetLength(0);
            int sizeY = mapUnits.GetLength(1);

            Vector2Int currentPos = start;
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
                    float nextError = Mathf.Abs(mapUnits[neighbor.x, neighbor.y] - elevation);
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
            } while (iterations < 10000 && currentPos != start);

            return heightLine.ToArray();
        }
    }
}
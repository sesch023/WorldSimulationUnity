using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Base;
using Model.Generators;
using UnityEngine;
using UnityEngine.Serialization;
using Utils;
using Vector2 = UnityEngine.Vector2;

namespace Model
{
    [CreateAssetMenu(fileName = "Map", menuName = "ScriptableObjects/Map", order = 1)]
    public class Map : ScriptableObject, IUpdatable
    {
        [field: SerializeField] public int SizeX { get; private set; } = 1025;
        [field: SerializeField] public int SizeY { get; private set; } = 1025;

        [SerializeField] 
        private BaseGenerator generator;

        public MapUnit[,] MapUnits { get; private set; }

        private void OnEnable()
        {
            if (generator == null)
            {
                throw new MissingReferenceException("MissingReferenceException: Illegal Map. Generator missing!");
            }

            (int sizeX, int sizeY) clippedSizes = generator.LimitMapSizes(SizeX, SizeY);
            SizeX = clippedSizes.sizeX;
            SizeY = clippedSizes.sizeY;
            float[,] mapElevation = generator.GenerateElevation(SizeX, SizeY);
            MapUnits = new MapUnit[SizeX, SizeY];
            for (var x = 0; x < MapUnits.GetLength(0); x++)
            {
                for (var y = 0; y < MapUnits.GetLength(1); y++)
                {
                    (float lat, float lon) latLong = CalculateLatLong(x, y);
                    MapUnits[x, y] = new MapUnit(0.0f, 0.0f, new MapUnit.MapPosition(latLong.lat, latLong.lon, mapElevation[x, y]));
                }
            }
        }

        private (float lat, float lon) CalculateLatLong(int x, int y)
        {
            return (((float)x / SizeX) * 360.0f, ((float)y / SizeY) * 180.0f) ;
        }

        public void Update()
        {
            foreach (MapUnit unit in MapUnits)
            {
                unit.Update();
            }
        }

        public Vector2Int[][] GetHeightLine(Vector2Int position)
        {
            MapUnit startTile = MapUnits[position.x, position.y];
            Vector2Int[] firstLine = FindHeightLine(position, startTile.Position.Elevation);

            return new[] { firstLine };
        }

        // Algorithmus ist nicht zielführend in den meisten Fällen.
        private Vector2Int[] FindHeightLine(Vector2Int start, float elevation)
        {
            List<Vector2Int> heightLine = new List<Vector2Int>();
            heightLine.Add(start);

            Vector2Int currentPos = start;
            int borderCounter = 0;
            int iterations = 0;
            bool nextFound;
            
            do
            {
                Vector2Int[] neighbors = MathUtil.GetNeighborPositionsIn2DArray(currentPos, SizeX, SizeY);
                Vector2Int next = neighbors[0];
                float error = float.PositiveInfinity;
                nextFound = false;

                foreach (var neighbor in neighbors)
                {
                    float nextError = Mathf.Abs(MapUnits[neighbor.x, neighbor.y].Position.Elevation - elevation);
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
                borderCounter = MathUtil.At2DArrayBorder(currentPos, SizeX, SizeY) ? borderCounter + 1 : 0;
                iterations++;
            } while (iterations < 10000 && currentPos != start);

            return heightLine.ToArray();
        }

        public Vector2Int[] GetSlopeLine(Vector2Int start, float momentumMultiplier=1.0f, float maxMomentumFraction=1.0f)
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
                Vector2Int[] neighbors = MathUtil.GetNeighborPositionsIn2DArray(currentPos, SizeX, SizeY);
                Vector2Int next = neighbors[0];
                float nextElevation = float.PositiveInfinity;
                
                foreach (var neighbor in neighbors)
                {
                    float neightborElevation = MapUnits[neighbor.x, neighbor.y].Position.Elevation;
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
    }
}
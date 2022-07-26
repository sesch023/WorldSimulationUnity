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
            Vector2Int[] firstLine = MapUtil.FindHeightLine(position, MapUnits, startTile.Position.Elevation);

            return new[] { firstLine };
        }

        public Vector2Int[] GetSlopeLine(Vector2Int start, float momentumMultiplier = 1.0f,
            float maxMomentumFraction = 1.0f)
        {
            return MapUtil.GetSlopeLine(start, MapUnits, momentumMultiplier, maxMomentumFraction);
        }
    }
}
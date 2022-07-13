using System;
using Base;
using UnityEngine;
using UnityEngine.Serialization;

namespace Model
{
    [Serializable]
    public class Map : IUpdatable
    {
        [field: SerializeField] public uint SizeX { get; private set; } = 2000;
        [field: SerializeField] public uint SizeY { get; private set; } = 1000;

        public MapUnit[,] MapUnits { get; private set; }

        public Map()
        {
            Debug.Log("Hello There");
            MapUnits = new MapUnit[SizeX, SizeY];
            for (var x = 0; x < MapUnits.GetLength(0); x++)
            {
                for (var y = 0; y < MapUnits.GetLength(1); y++)
                {
                    (float lat, float lon) latLong = CalculateLatLong(x, y);
                    MapUnits[x, y] = new MapUnit(0.0f, 0.0f, new MapUnit.MapPosition(latLong.lat, latLong.lon, -8000.0f));
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
    }
}
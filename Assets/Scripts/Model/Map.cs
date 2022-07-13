using System;
using System.Linq;
using Base;
using UnityEngine;
using UnityEngine.Serialization;
using Util;

namespace Model
{
    [Serializable]
    public class Map : IUpdatable
    {
        [field: SerializeField] public int SizeX { get; private set; } = 1025;
        [field: SerializeField] public int SizeY { get; private set; } = 1025;

        public MapUnit[,] MapUnits { get; private set; }

        public Map()
        {
            float[,] diamondSquare = DiamondSquare.CalculateValues(SizeX, 1024, DateTime.Now.Millisecond);
            float min = diamondSquare.Cast<float>().Min();
            float max = diamondSquare.Cast<float>().Max();

            for (int x = 0; x < diamondSquare.GetLength(0); x++)
            {
                for (int y = 0; y < diamondSquare.GetLength(1); y++)
                {
                    diamondSquare[x, y] = (((diamondSquare[x, y] - min) / max) * 18000) - 8000;
                }
            }
            
            Debug.Log(diamondSquare.Cast<float>().Min());
            Debug.Log(diamondSquare.Cast<float>().Max());
            MapUnits = new MapUnit[SizeX, SizeY];
            for (var x = 0; x < MapUnits.GetLength(0); x++)
            {
                for (var y = 0; y < MapUnits.GetLength(1); y++)
                {
                    (float lat, float lon) latLong = CalculateLatLong(x, y);
                    MapUnits[x, y] = new MapUnit(0.0f, 0.0f, new MapUnit.MapPosition(latLong.lat, latLong.lon, diamondSquare[x, y]));
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
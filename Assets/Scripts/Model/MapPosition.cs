using System;
using UnityEngine;

namespace Model
{
    /// <summary>
    /// A mapPositionVec in the map. Has longitude, latitude and elevation.
    /// </summary>
    public class MapPosition
    {
        public MapPosition(float latitude, float longitude, float elevation, Vector2Int mapPositionVec)
        {
            Longitude = longitude;
            Latitude = latitude;
            Elevation = elevation;
            MapPositionVec = mapPositionVec;
        }
        
        public MapPosition(MapUnit parent, float latitude, float longitude, float elevation, Vector2Int position)
            : this(latitude, longitude, elevation, position)
        {
            Parent = parent;
        }

        internal MapUnit Parent { get; set; }
        
        public float Longitude { get; private set; }
        public float Latitude { get; private set; }

        private float _elevation;
        
        public Vector2Int MapPositionVec { get; private set; }
        
        public float Elevation
        {
            get => _elevation;
            set
            {
                _elevation = value;
                Parent?.Changed();
            }
        }
    }
}
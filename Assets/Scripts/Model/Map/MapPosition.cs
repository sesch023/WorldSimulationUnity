using System;

namespace Model.Map
{
    /// <summary>
    /// A position in the map. Has longitude, latitude and elevation.
    /// </summary>
    public class MapPosition
    {
        public MapPosition(float latitude, float longitude, float elevation)
        {
            Longitude = longitude;
            Latitude = latitude;
            Elevation = elevation;
        }
        
        public MapPosition(MapUnit parent, float latitude, float longitude, float elevation)
            : this(latitude, longitude, elevation)
        {
            Parent = parent;
        }

        internal MapUnit Parent { get; set; }
        
        public float Longitude { get; private set; }
        public float Latitude { get; private set; }

        private float _elevation;
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
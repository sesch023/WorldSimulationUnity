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

        public float Longitude { get; private set; }
        public float Latitude { get; private set; }
        public float Elevation { get; set; }
    }
}
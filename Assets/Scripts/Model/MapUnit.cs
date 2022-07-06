using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Model.UnitBehaviors;
using Unity.VisualScripting;

namespace Model
{
    public class MapUnit
    {
        public struct MapPosition
        {
            public MapPosition(float longitude, float latitude, float elevation)
            {
                Longitude = longitude;
                Latitude = latitude;
                Elevation = elevation;
            }

            public float Longitude { get; private set; }
            public float Latitude { get; private set; }
            public float Elevation { get; private set; }
        }
        
        private float _temperature;
        public float Temperature
        {
            get => _temperature; 
            private set => Math.Clamp(value, 0, float.MaxValue);
        }
        
        private float _humidity;
        public float Humidity
        {
            get => _humidity;
            private set => _humidity = Math.Clamp(value, 0, 100);
        }

        public MapPosition Position { get; private set; }

        public IList<BaseUnitBehavior> Behaviors { get; private set; }

        public MapUnit()
        {
            Behaviors = new List<BaseUnitBehavior>();
        }

        public MapUnit(float temperature, float humidity, MapPosition position) : this()
        {
            Temperature = temperature;
            Humidity = humidity;
            Position = position;
        }

        public MapUnit(float temperature, float humidity, MapPosition position,
            [NotNull] IList<BaseUnitBehavior> behaviors) : this(temperature, humidity, position)
        {
            Behaviors.AddRange(behaviors);
        }

        public void AddUnitBehavior(BaseUnitBehavior behavior)
        {
            Behaviors.Add(behavior);
        }

        public void RemoveUnitBehavior(BaseUnitBehavior behavior)
        {
            Behaviors.Remove(behavior);
        }

        public void ClearBehaviors()
        {
            Behaviors.Clear();
        }
    }
}
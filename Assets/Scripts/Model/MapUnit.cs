using System;
using System.Collections.Generic;
using Base;
using JetBrains.Annotations;
using Model.UnitBehaviors;
using Unity.VisualScripting;
using UnityEngine.Tilemaps;

namespace Model
{
    public class MapUnit : IUpdatable
    {
        public struct MapPosition
        {
            public MapPosition(float latitude, float longitude, float elevation)
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
            Init();
        }

        public MapUnit(float temperature, float humidity, MapPosition position) : this()
        {
            Init(temperature, humidity, position);
        }

        public MapUnit(float temperature, float humidity, MapPosition position,
            [NotNull] IList<BaseUnitBehavior> behaviors) : this(temperature, humidity, position)
        {
            Init(temperature, humidity, position, behaviors);
        }

        public void Init()
        {
            Behaviors = new List<BaseUnitBehavior>();
        }
        
        public void Init(float temperature, float humidity, MapPosition position)
        {
            Temperature = temperature;
            Humidity = humidity;
            Position = position;
        }

        public void Init(float temperature, float humidity, MapPosition position,
            [NotNull] IList<BaseUnitBehavior> behaviors)
        {
            Init(temperature, humidity, position);
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

        public void Update()
        {
            
        }
    }
}
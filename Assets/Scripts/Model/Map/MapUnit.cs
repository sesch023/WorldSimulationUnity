using System;
using System.Collections.Generic;
using Base;
using JetBrains.Annotations;
using Model.UnitBehaviors;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Model.Map
{
    public delegate void MapUnitChanged(MapUnit unit);
    
    /// <summary>
    /// A single unit at a position in a map. Has a position with longitude, latitude and elevation. Also has a
    /// temperature, humidity and behaviors which are to be added depending on unit conditions.
    /// </summary>
    public class MapUnit : IUpdatable
    {
        /// Temperature of the unit in Kelvin.
        private float _temperature;
        
        /// <summary>
        /// Temperature of the unit in Kelvin.
        /// </summary>
        public float Temperature
        {
            get => _temperature;
            set
            {
                Math.Clamp(value, 0, float.MaxValue);
                Changed();
            }
        }

        /// Humidity of the unit in percent.
        private float _humidity;
        
        /// <summary>
        /// Humidity of the unit in percent.
        /// </summary>
        public float Humidity
        {
            get => _humidity;
            set
            {
                _humidity = Math.Clamp(value, 0, 100);
                Changed();
            }
        }

        private float _waterLevel;
        public float WaterLevel
        {
            get => _waterLevel;
            set
            {
                _waterLevel = Math.Clamp(value, 0, float.MaxValue);
                Changed();
            }
        }

        /// <summary>
        /// Atmospheric pressure of the unit in pascal.
        /// </summary>
        public float AtmosphericPressure
        {
            get;
        }

        public MapUnitGroundMaterial GroundMaterial { get; private set; } = new();
        /// <summary>
        /// Position of the unit in the map with longitude, latitude and elevation.
        /// </summary>
        public MapPosition Position { get; private set; }
        
        /// This Method is a bit hacky and should be changed in the future.
        public void ErodeElevation(float newElevation, float gravelMultiplier=25f, float sandMultiplier=5f)
        {
            float currentElevation = Position.Elevation;
            float elevationDifference = currentElevation - newElevation;
            float percentage = Math.Abs(elevationDifference / currentElevation);

            if (elevationDifference < 0)
            {
                GroundMaterial.Gravel = Math.Max(0, GroundMaterial.Gravel - percentage * gravelMultiplier);
                GroundMaterial.Sand = Math.Max(0, GroundMaterial.Sand - percentage * GroundMaterial.Gravel * sandMultiplier);
            }
            else
            {
                GroundMaterial.Gravel += percentage * gravelMultiplier;
                GroundMaterial.Sand += percentage * GroundMaterial.Gravel * sandMultiplier;
            }
            
            Position.Elevation = newElevation;
        }

        /// <summary>
        /// Behaviors of the unit. These are to be added depending on unit conditions.
        /// </summary>
        public IList<BaseUnitBehavior> Behaviors { get; private set; }
        
        public IList<MapUnitChanged> ChangeSubscribers { get; private set; }

        /// <summary>
        /// Initializes the unit empty.
        /// </summary>
        private MapUnit()
        {
            Init();
        }
        
        /// <summary>
        /// Initializes the unit with a temperature, humidity and position.
        /// </summary>
        /// <param name="temperature">Temperature of the unit.</param>
        /// <param name="humidity">Humidity of the unit.</param>
        /// <param name="position">Position of the unit.</param>
        public MapUnit(float temperature, float humidity, MapPosition position) : this()
        {
            Init(temperature, humidity, position);
        }
        
        /// <summary>
        /// Initializes the unit with a temperature, humidity, position and a list of behaviors.
        /// </summary>
        /// <param name="temperature">Temperature of the unit.</param>
        /// <param name="humidity">Humidity of the unit.</param>
        /// <param name="position">Position of the unit.</param>
        /// <param name="behaviors">Behaviors of the unit.</param>
        public MapUnit(float temperature, float humidity, MapPosition position,
            [NotNull] IList<BaseUnitBehavior> behaviors) : this(temperature, humidity, position)
        {
            Init(temperature, humidity, position, behaviors);
        }

        private void Init()
        {
            Behaviors = new List<BaseUnitBehavior>();
            ChangeSubscribers = new List<MapUnitChanged>();
        }
        
        private void Init(float temperature, float humidity, MapPosition position)
        {
            Temperature = temperature;
            Humidity = humidity;
            Position = position;
            position.Parent = this;
        }

        private void Init(float temperature, float humidity, MapPosition position,
            [NotNull] IList<BaseUnitBehavior> behaviors)
        {
            Init(temperature, humidity, position);
            Behaviors.AddRange(behaviors);
        }

        /// <summary>
        /// Adds a behavior to the unit.
        /// </summary>
        /// <param name="behavior">Behavior to be added.</param>
        public void AddUnitBehavior(BaseUnitBehavior behavior)
        {
            Behaviors.Add(behavior);
            Changed();
        }

        /// <summary>
        /// Removes a behavior from the unit.
        /// </summary>
        /// <param name="behavior">Behavior to be removed.</param>
        public void RemoveUnitBehavior(BaseUnitBehavior behavior)
        {
            Behaviors.Remove(behavior);
            Changed();
        }
        
        public void AddChangeSubscriber(MapUnitChanged subscriber)
        {
            ChangeSubscribers.Add(subscriber);
        }
        
        public  void RemoveChangeSubscriber(MapUnitChanged subscriber)
        {
            ChangeSubscribers.Remove(subscriber);
        }
        
        /// <summary>
        /// Clears all behaviors from the unit.
        /// </summary>
        public void ClearBehaviors()
        {
            Behaviors.Clear();
            Changed();
        }
        
        public void ClearChangeSubscribers()
        {
            ChangeSubscribers.Clear();
        }
        
        /// <summary>
        /// Updates the unit.
        /// </summary>
        public void Update()
        {
            foreach (var behavior in Behaviors)
            {
                behavior.Update();
            }
        }

        internal void Changed()
        {
            foreach (var subscriber in ChangeSubscribers)
            {
                subscriber.Invoke(this);
            }
        }
    }
}
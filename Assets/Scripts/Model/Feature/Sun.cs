using System;
using Base;
using UnityEngine;
using Utils.BaseUtils;

namespace Model.Feature
{
    /// <summary>
    /// This class represents a sun in the game, which is a feature of a map, and heats up the map depending on
    /// different parameters.
    /// </summary>
    [CreateAssetMenu(fileName = "Sun", menuName = "ScriptableObjects/Sun", order = 4)]
    public class Sun : ScriptableObject, IUpdatable
    {
        private const float Au = 149597870700f;
        private const float StefanBoltzmannConst = 5.670367e-8f;

        /// Map which the sun is on.
        [SerializeField]
        private Map.Map map;
        /// Temperature of the sun.
        [SerializeField]
        private float surfaceTemperature = 6000.0f;
        /// Distance of the sun from the map in astronomical units.
        [SerializeField]
        private float distanceToPlanetAu = 1.0f;
        /// Accuracy of the temperature calculation.
        [SerializeField]
        private float simulationAccuracy = 1f;
        /// Radius of the sun in meters.
        [SerializeField]
        private float starRadius = 696340000f;
        /// Span between the day and night temperature in Kelvin.
        [SerializeField]
        private float absoluteDayNightTemperatureDifference = 20f;
        /// The span in ticks between each temperature update.
        [SerializeField]
        private int updateInterval = 10;
        
        /// The steps a sun takes in a update.
        private int _unitStepsPerTick;
        /// The distance of the sun from the map in meters.
        private float _metersToPlanet;
        /// The equilibrium temperature of the map.
        private float _equilibriumSurfaceTemperature;

        // Number of temperature zones.
        private int _numberOfPlanetaryTemperatureZones;
        
        /// Temperatures of the different daycycle zones of the planet.
        public float[] PlanetaryTemperatureZones { get; private set; }
        /// Map indices of the different daycycle zones of the planet.
        public (int start, int end)[] PlanetaryTemperatureUpdateZoneIndices { get; private set; }

        /// <summary>
        /// Gets the wattage of the star.
        /// </summary>
        /// <param name="starSurfaceTemperature">Surface temperature of the star.</param>
        /// <param name="starRadius">Radius of the star.</param>
        /// <returns>Wattage of the star.</returns>
        public static float GetStarWattage(float starSurfaceTemperature, float starRadius)
        {
            return MathUtil.SphereSurfaceArea(starRadius) * StefanBoltzmannConst * Mathf.Pow(starSurfaceTemperature, 4);
        }
        
        /// <summary>
        /// The equilibrium temperature of the given body.
        /// </summary>
        /// <param name="starWattage">Wattage of the star.</param>
        /// <param name="bodyDistanceInMeters">Distance from the planet to the start in meters.</param>
        /// <param name="bodyAlbedo">Albedo of the body.</param>
        /// <returns>Effective Temperature of the map.</returns>
        public static float GetEffectiveTemperature(float starWattage, float bodyDistanceInMeters, float bodyAlbedo)
        {
            return Mathf.Pow((starWattage * (1 - bodyAlbedo)) / (16*Mathf.PI*bodyDistanceInMeters*bodyDistanceInMeters*StefanBoltzmannConst), 1f/4);
        }
        
        /// <summary>
        /// The equilibrium temperature of the given body.
        /// </summary>
        /// <param name="starSurfaceTemperature">Surface Temperature of the star.</param>
        /// <param name="starRadius">Radius of the star in meters.</param>
        /// <param name="bodyDistanceInMeters">Distance from the planet to the start in meters.</param>
        /// <param name="bodyAlbedo">Albedo of the body.</param>
        /// <returns>Effective Temperature of the map.</returns>
        public static float GetEffectiveTemperature(float starSurfaceTemperature, float starRadius, float bodyDistanceInMeters, float bodyAlbedo)
        {
            return GetEffectiveTemperature(GetStarWattage(starSurfaceTemperature, starRadius), bodyDistanceInMeters, bodyAlbedo);
        }
        
        public void OnValidate()
        {
            if (map == null)
                throw new MissingComponentException($"MissingComponentException: {GetType().Name}. No map given for sun instance!");

            
            // Calculate the equilibrium temperature of the map.
            _metersToPlanet = distanceToPlanetAu * Au;
            _equilibriumSurfaceTemperature =
                GetEffectiveTemperature(surfaceTemperature, starRadius, _metersToPlanet, map.Albedo);
            _unitStepsPerTick = (map.SizeX / map.TicksPerRotation);
            _numberOfPlanetaryTemperatureZones = Math.Max(2, (int)(((float)map.SizeX / _unitStepsPerTick)*simulationAccuracy));
            int planetSteps = map.SizeX / _numberOfPlanetaryTemperatureZones;
            PlanetaryTemperatureZones = new float[_numberOfPlanetaryTemperatureZones];
            
            float tempStep = absoluteDayNightTemperatureDifference / _numberOfPlanetaryTemperatureZones;
            float noonTemp = _equilibriumSurfaceTemperature + absoluteDayNightTemperatureDifference / 2;
            float midnightTemp = _equilibriumSurfaceTemperature - absoluteDayNightTemperatureDifference / 2;
            
            // Find the temperature of each zone.
            for (int i = 0; i < (int)Mathf.Floor(_numberOfPlanetaryTemperatureZones/2f); i++)
            {
                PlanetaryTemperatureZones[i] = noonTemp - tempStep * i;
            }
            for (int i = (int)Mathf.Ceil(_numberOfPlanetaryTemperatureZones/2f); i < _numberOfPlanetaryTemperatureZones; i++)
            {
                PlanetaryTemperatureZones[i] = midnightTemp + tempStep * i;
            }
            
            PlanetaryTemperatureUpdateZoneIndices = new (int, int)[_numberOfPlanetaryTemperatureZones];
            for (int i = 0; i < _numberOfPlanetaryTemperatureZones; i++)
            {
                PlanetaryTemperatureUpdateZoneIndices[i] = (i * planetSteps , (i + 1) * planetSteps + _unitStepsPerTick);
            }
        }

        private int k = 0;

        /// <summary>
        /// Updates the temperature of the map, every updateInterval ticks.
        /// </summary>
        public void Update()
        {
            if (k >= updateInterval)
            {
                k = 0;
                Rotate();
            }
            else
            {
                k++;
            }
        }

        /// <summary>
        /// Rotates the planet.
        /// </summary>
        private void Rotate()
        {
            // Rotates the temperature zones.
            for(int i = 0; i < _numberOfPlanetaryTemperatureZones; i++)
            {
                (int start, int end) el = PlanetaryTemperatureUpdateZoneIndices[i];
                el.start -= updateInterval*_unitStepsPerTick;
                el.end -= updateInterval*_unitStepsPerTick;
                if (el.start < 0)
                {
                    el.start += map.SizeX;
                }
                if (el.end < 0)
                {
                    el.end += map.SizeX;
                }
                PlanetaryTemperatureUpdateZoneIndices[i] = el;
            }

            // Updates the temperature of the map.
            for(int i = 0; i < _numberOfPlanetaryTemperatureZones; i++)
            {
                (int start, int end) current = PlanetaryTemperatureUpdateZoneIndices[i];
                (int start, int end) next = (i + 1 < PlanetaryTemperatureZones.Length) ? PlanetaryTemperatureUpdateZoneIndices[i+1] : PlanetaryTemperatureUpdateZoneIndices[0];
                int currentX = current.start;
                
                while (currentX != next.start)
                {
                    for (int y = 0; y < map.SizeY; y++)
                    {
                        map.MapUnits[currentX, y].Temperature = PlanetaryTemperatureZones[i];
                    }
                    currentX++;
                    if (currentX >= map.SizeX)
                    {
                        currentX = 0;
                    }
                }
            }
        }
    }
}
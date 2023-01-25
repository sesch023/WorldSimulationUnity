using System;
using Base;
using Unity.VisualScripting;
using UnityEngine;
using Utils.BaseUtils;

namespace Model.Map.Feature
{
    [CreateAssetMenu(fileName = "Sun", menuName = "ScriptableObjects/Sun", order = 4)]
    public class Sun : ScriptableObject, IUpdatable
    {
        private const float Au = 149597870700f;
        private const float StefanBoltzmannConst = 5.670367e-8f;

        [SerializeField]
        private Map map;
        [SerializeField]
        private float surfaceTemperature = 6000.0f;
        [SerializeField]
        private float distanceToPlanetAu = 1.0f;
        [SerializeField]
        private float simulationAccuracy = 1f;
        [SerializeField]
        private float starRadius = 696340f;
        [SerializeField]
        private float absoluteDayNightTemperatureDifference = 20f;

        private int _currentPlanetNoonPosition;
        private int _unitStepsPerTick;
        private float _kmToPlanet;
        private float _equilibriumSurfaceTemperature;

        private int _numberOfPlanetaryTemperatureZones;
        private float[] _planetaryTemperatureZones;
        private (int, int)[] _planetaryTemperatureUpdateZoneIndices;
        
        public static float GetStarWattage(float starSurfaceTemperature, float starRadius)
        {
            return MathUtil.SphereSurfaceArea(starRadius) * StefanBoltzmannConst * Mathf.Pow(starSurfaceTemperature, 4);
        }
        
        public static float GetEffectiveTemperature(float starWattage, float bodyDistanceInKm, float bodyAlbedo)
        {
            return Mathf.Pow((starWattage * (1 - bodyAlbedo)) / (16*Mathf.PI*bodyDistanceInKm*StefanBoltzmannConst), -4);
        }

        public static float GetEffectiveTemperature(float starSurfaceTemperature, float starRadius, float bodyDistanceInKm, float bodyAlbedo)
        {
            return GetEffectiveTemperature(GetStarWattage(starSurfaceTemperature, starRadius), bodyDistanceInKm, bodyAlbedo);
        }
        
        public void OnValidate()
        {
            if (map == null)
                throw new MissingComponentException($"MissingComponentException: {GetType().Name}. No map given for sun instance!");

            _kmToPlanet = distanceToPlanetAu * Au;
            _equilibriumSurfaceTemperature =
                GetEffectiveTemperature(surfaceTemperature, starRadius, _kmToPlanet, map.Albedo);
            _unitStepsPerTick = (map.SizeX / map.TicksPerRotation);
            _numberOfPlanetaryTemperatureZones = Math.Max(2, (int)(((float)map.SizeX / _unitStepsPerTick)*simulationAccuracy));
            _planetaryTemperatureZones = new float[_numberOfPlanetaryTemperatureZones];
            
            float tempStep = absoluteDayNightTemperatureDifference / _numberOfPlanetaryTemperatureZones;
            float noonTemp = _equilibriumSurfaceTemperature + absoluteDayNightTemperatureDifference / 2;
            float midnightTemp = _equilibriumSurfaceTemperature - absoluteDayNightTemperatureDifference / 2;
            for (int i = 0; i < (int)Mathf.Floor(_numberOfPlanetaryTemperatureZones/2f); i++)
            {
                _planetaryTemperatureZones[i] = noonTemp - tempStep * i;
            }
            for (int i = (int)Mathf.Ceil(_numberOfPlanetaryTemperatureZones/2f); i < _numberOfPlanetaryTemperatureZones; i++)
            {
                _planetaryTemperatureZones[i] = midnightTemp + tempStep * i;
            }
            
            _planetaryTemperatureUpdateZoneIndices = new (int, int)[_numberOfPlanetaryTemperatureZones];
            for (int i = 0; i < _numberOfPlanetaryTemperatureZones; i++)
            {
                _planetaryTemperatureUpdateZoneIndices[i] = (i * _unitStepsPerTick, (i + 1) * _unitStepsPerTick);
            }
        }

        private int k = 0;

        public void Update()
        {
            Debug.Log("Sun: Updating sun position.");
            
            if(k % 10 == 0)
                Rotate();
            k++;
        }

        private void Rotate()
        {
            for(int i = 0; i < _numberOfPlanetaryTemperatureZones; i++)
            {
                (int start, int end) el = _planetaryTemperatureUpdateZoneIndices[i];
                el.start -= 10*_unitStepsPerTick;
                el.end -= 10*_unitStepsPerTick;
                if (el.start < 0)
                {
                    el.start += map.SizeX;
                }
                if (el.end < 0)
                {
                    el.end += map.SizeX;
                }
                _planetaryTemperatureUpdateZoneIndices[i] = el;
            }

            for(int i = 0; i < _planetaryTemperatureZones.Length; i++)
            {
                (int start, int end) current = _planetaryTemperatureUpdateZoneIndices[i];
                (int start, int end) next = (i + 1 < _planetaryTemperatureZones.Length) ? _planetaryTemperatureUpdateZoneIndices[i+1] : _planetaryTemperatureUpdateZoneIndices[0];
                int currentX = current.start;

                while (currentX != next.start)
                {
                    for (int y = 0; y < map.SizeY; y++)
                    {
                        map.MapUnits[currentX, y].Temperature = _planetaryTemperatureZones[i];
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
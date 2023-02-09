using System;
using Manager;
using Model.Feature;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;

namespace Views.GameViews
{
    public class SunView : MonoBehaviour
    {
        [SerializeField]
        private Sun sun;
        [SerializeField] 
        private GameObject borderView;

        private GameObject[] _views;
        private Vector3 _tileOffset;
        
        private Light2D _light;
        private Light2D _secondaryLight;
        private Vector3 _tileScale;

        public void Awake()
        {
            _light = GetComponent<Light2D>();
            
            if (_light == null)
            {
                throw new MissingComponentException(
                    $"MissingComponentException: {GetType()} - Missing Sun Light in Sun View!");
            }
            
            if (borderView == null)
            {
                throw new MissingReferenceException(
                    $"MissingReferenceException: {GetType()} - Missing Border Highlight in Sun View!");
            }
            
            if (sun == null)
            {
                throw new MissingReferenceException(
                    $"MissingReferenceException: {GetType()} - Missing Sun in Sun View!");
            }
        }

        public void Start()
        {
            _tileScale = MapManager.Instance.MapController.TileMap.transform.localScale;

            _tileOffset = new Vector3(_tileScale.x / 2, _tileScale.y / 2, 0);
            borderView.transform.position = new Vector3(0, (_tileScale.y*MapManager.Instance.MapController.UnitMap.SizeY)/2,0);
            borderView.transform.localScale = new Vector3(_tileScale.x, _tileScale.y * MapManager.Instance.MapController.UnitMap.SizeY, _tileScale.z);

            _light.falloffIntensity = 0.05f;
            _light.volumeIntensityEnabled = true;
            _light.intensity = 0.3f;
            _light.lightType = Light2D.LightType.Freeform;

            transform.localScale = new Vector3((_tileScale.x * MapManager.Instance.MapController.UnitMap.SizeX) / 2,
                _tileScale.y * MapManager.Instance.MapController.UnitMap.SizeY, 0);
            transform.position = new Vector3(0, (float)MapManager.Instance.MapController.UnitMap.SizeY, 0) ;
            
            _secondaryLight = Instantiate(_light, transform.parent, true);
            Destroy(_secondaryLight.GetComponent<SunView>());

            _views = new GameObject[sun.PlanetaryTemperatureZones.Length];
            for (int i = 0; i < sun.PlanetaryTemperatureZones.Length; i++)
            {
                _views[i] = Instantiate(borderView);
            }
            borderView.SetActive(false);
            SetPositions();
        }

        private void SetPositions()
        {
            float offset = Mathf.Abs(sun.PlanetaryTemperatureUpdateZoneIndices[0].start -
                                     sun.PlanetaryTemperatureUpdateZoneIndices[1].start) / 2f;
            float sunPos = sun.PlanetaryTemperatureUpdateZoneIndices[0].start * _tileScale.x + offset;
            float scaledPos = MapManager.Instance.MapController.UnitMap.SizeX * _tileScale.x;
            float secondaryPos;
            if (sunPos > scaledPos / 2)
                secondaryPos = sunPos - scaledPos;
            else
                secondaryPos = sunPos + scaledPos;
                
            transform.position = new Vector3(sunPos, transform.position.y, 0);
            _secondaryLight.transform.position = new Vector3(secondaryPos, _secondaryLight.transform.position.y, 0);
            for (int i = 0; i < sun.PlanetaryTemperatureZones.Length; i++)
            {
                Vector2Int tile = new Vector2Int(sun.PlanetaryTemperatureUpdateZoneIndices[i].start, 0);
                _views[i].transform.position = new Vector3(MapManager.Instance.MapController.TileMapPositionToGlobalPosition(tile).x + _tileOffset.x, _views[i].transform.position.y, 0);
            }
        }

        public void Update()
        {
            SetPositions();
        }
    }
}
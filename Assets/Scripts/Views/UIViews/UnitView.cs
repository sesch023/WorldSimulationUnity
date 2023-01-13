using System;
using System.Text;
using Manager;
using Model.Map;
using TMPro;
using UnityEngine;

namespace Views.UIViews
{
    /// <summary>
    /// UIView for displaying information of a tile.
    /// </summary>
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class UnitView : MonoBehaviour
    {
        private TextMeshProUGUI _baseView;
        
        /// View for displaying position information.
        [SerializeField]
        private TextMeshProUGUI positionView;
        
        /// View for displaying temperature information.
        [SerializeField]
        private TextMeshProUGUI temperatureView;
        
        /// View for displaying humidity information.
        [SerializeField]
        private TextMeshProUGUI humidityView;
        
        /// View for displaying water Level.
        [SerializeField]
        private TextMeshProUGUI waterLevelView;
        
        [SerializeField]
        private TextMeshProUGUI atmosphericPressureView;

        /// View for displaying behavior information.
        [SerializeField] 
        private TextMeshProUGUI behaviorView;

        /// <summary>
        /// View for displaying ground material information.
        /// </summary>
        [SerializeField] 
        private TextMeshProUGUI materialView;

        /// Highlight to display on the tilemap if a tile is clicked.
        [SerializeField]
        private GameObject tileHightlight;

        /// Unit that was currently selected.
        private MapUnit _shownUnit;

        /// <summary>
        /// Initializes the view.
        /// </summary>
        /// <exception cref="MissingComponentException">If the text mesh is missing.</exception>
        /// <exception cref="MissingReferenceException">If a reference is missing.</exception>
        private void Awake()
        {
            _baseView = GetComponent<TextMeshProUGUI>();
            
            if(_baseView == null)
                throw new MissingComponentException("MissingComponentException: Illegal Unit View. TextMesh missing!");
            
            if(positionView == null || temperatureView == null || humidityView == null || waterLevelView == null || atmosphericPressureView == null)
                throw new MissingReferenceException("MissingReferenceException: Illegal Unit View. TextMesh Reference missing!");
            
            if(tileHightlight == null)
                throw new MissingReferenceException("MissingReferenceException: Illegal Unit View. TileHighlight Reference missing!");
            
            if(behaviorView == null)
                throw new MissingReferenceException("MissingReferenceException: Illegal Unit View. BehaviorView Reference missing!");
            
            if(materialView == null)
                throw new MissingReferenceException("MissingReferenceException: Illegal Material View. MaterialView Reference missing!");
            
            DisableUnit();
        }

        /// <summary>
        /// Disables the unit view and hides all its information.
        /// </summary>
        public void DisableUnit()
        {
            _shownUnit = null;
            positionView.enabled = false;
            temperatureView.enabled = false;
            humidityView.enabled = false;
            waterLevelView.enabled = false;
            atmosphericPressureView.enabled = false;
            behaviorView.gameObject.SetActive(false);
            materialView.gameObject.SetActive(false);
            _baseView.enabled = false;
            tileHightlight.SetActive(false);
        }
        
        /// <summary>
        /// Enables the view with a given map unit and position of it.
        /// </summary>
        /// <param name="shownUnit">MapUnit and Position of MapUnit to display the information of.</param>
        public void EnableUnit((MapUnit unit, Vector2Int vec) shownUnit)
        {
            _shownUnit = shownUnit.unit;
            SetTextData();
            Vector3 tileScale = MapManager.Instance.MapController.TileMap.transform.localScale;
            tileHightlight.transform.localScale = tileScale;
            tileHightlight.transform.position = new Vector3(tileScale.x / 2, tileScale.y / 2, 0) + MapManager.Instance.MapController.TileMapPositionToGlobalPosition(shownUnit.vec);
            positionView.enabled = true;
            temperatureView.enabled = true;
            humidityView.enabled = true;
            waterLevelView.enabled = true;
            atmosphericPressureView.enabled = true;
            behaviorView.gameObject.SetActive(true);
            materialView.gameObject.SetActive(true);
            _baseView.enabled = true;
            tileHightlight.SetActive(true);
        }

        /// <summary>
        /// Sets all the information into the belonging views.
        /// </summary>
        private void SetTextData()
        {
            SetPositionData();
            SetTemperatureData();
            SetHumidityData();
            SetWaterLevelData();
            SetBehaviorData();
            SetAtmosphericPressureData();
            SetMaterialData();
        }

        private void SetPositionData()
        {
            MapPosition position = _shownUnit.Position;
            String data = $"Position: ({position.Latitude:0.00}, {position.Longitude:0.00}, {position.Elevation:0.00})";
            positionView.SetText(data);
        }

        private void SetTemperatureData()
        {
            temperatureView.SetText($"Temperature: {_shownUnit.Temperature:0.00}");
        }

        private void SetHumidityData()
        {
            humidityView.SetText($"Humidity: {_shownUnit.Humidity:0.00}");
        }

        private void SetAtmosphericPressureData()
        {
            atmosphericPressureView.SetText($"Atmospheric Pressure: {_shownUnit.AtmosphericPressure:0.00}");
        }
        
        private void SetWaterLevelData()
        {
            waterLevelView.SetText($"Water Level: {_shownUnit.WaterLevel:0.00}");
        }
        
        private void SetMaterialData()
        {
            materialView.SetText(_shownUnit.GroundMaterial.ToString());
            Debug.Log(_shownUnit.GroundMaterial);
        }

        private void SetBehaviorData()
        {
            StringBuilder behaviorString = new StringBuilder();
            if (_shownUnit.Behaviors.Count > 0)
            {
                foreach (var behavior in _shownUnit.Behaviors)
                {
                    behaviorString.Append("\t" + behavior.GetBehaviorDescription() + ",\n");
                }
                behaviorString.Remove(behaviorString.Length - 1, 2);
            }
            else
            {
                behaviorString.Append("\tNone");
            }
            behaviorView.SetText(behaviorString);
        }
        
        /// <summary>
        /// Continuously updates the view with the current information of the unit.
        /// </summary>
        private void Update()
        {
            if (_shownUnit != null)
            {
                SetTextData();
            }
        }
    }
}
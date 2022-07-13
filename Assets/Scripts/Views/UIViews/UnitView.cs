using System;
using System.Text;
using Model;
using TMPro;
using UnityEngine;

namespace Views.UIViews
{
    public class UnitView : MonoBehaviour
    {
        private TextMeshProUGUI _baseView;
        
        [SerializeField]
        private TextMeshProUGUI positionView;
        
        [SerializeField]
        private TextMeshProUGUI temperatureView;
        
        [SerializeField]
        private TextMeshProUGUI humidityView;

        [SerializeField] 
        private TextMeshProUGUI behaviorView;

        [SerializeField]
        private GameObject tileHightlight;

        private MapUnit _shownUnit;

        private void Awake()
        {
            _baseView = GetComponent<TextMeshProUGUI>();
            
            if(_baseView == null)
                throw new MissingComponentException("MissingComponentException: Illegal Unit View. TextMesh missing!");
            
            if(positionView == null || temperatureView == null || humidityView == null)
                throw new MissingReferenceException("MissingReferenceException: Illegal Unit View. TextMesh Reference missing!");
            
            if(tileHightlight == null)
                throw new MissingReferenceException("MissingReferenceException: Illegal Unit View. TileHighlight Reference missing!");
            
            if(behaviorView == null)
                throw new MissingReferenceException("MissingReferenceException: Illegal Unit View. BehaviorView Reference missing!");
            
            DisableUnit();
        }

        public void DisableUnit()
        {
            _shownUnit = null;
            positionView.enabled = false;
            temperatureView.enabled = false;
            humidityView.enabled = false;
            behaviorView.gameObject.SetActive(false);
            _baseView.enabled = false;
            tileHightlight.SetActive(false);
        }
        
        public void EnableUnit((MapUnit unit, Vector3Int vec) shownUnit)
        {
            _shownUnit = shownUnit.unit;
            SetTextData();
            tileHightlight.transform.position = new Vector3(shownUnit.vec.x + 0.5f, shownUnit.vec.y + 0.5f, 0);
            positionView.enabled = true;
            temperatureView.enabled = true;
            humidityView.enabled = true;
            behaviorView.gameObject.SetActive(true);
            _baseView.enabled = true;
            tileHightlight.SetActive(true);
        }

        private void SetTextData()
        {
            SetPositionData();
            SetTemperatureData();
            SetHumidityData();
            SetBehaviorData();
        }

        private void SetPositionData()
        {
            MapUnit.MapPosition position = _shownUnit.Position;
            String data = $"Position: ({position.Latitude:0.00}, {position.Longitude:0.00}, {position.Elevation:0.00})";
            positionView.SetText(data);
        }

        private void SetTemperatureData()
        {
            temperatureView.SetText($"Temperature: {_shownUnit.Temperature:0.00}");
        }

        private void SetHumidityData()
        {
            humidityView.SetText($"Temperature: {_shownUnit.Humidity:0.00}");
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
        
        private void Update()
        {
            if (_shownUnit != null)
            {
                SetTextData();
            }
        }
    }
}
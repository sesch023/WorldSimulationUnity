using System;
using Manager;
using Model;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using Views;
using Views.GameViews;
using Views.UIViews;

namespace Controllers {
    public class TilesClickController : MonoBehaviour
    {
        [FormerlySerializedAs("_unitView")] [SerializeField]
        private UnitView unitView;

        [SerializeField]
        private HeightLineView heightLineView;

        [FormerlySerializedAs("_clickCamera")] [SerializeField]
        private Camera clickCamera;

        private void Awake()
        {
            if (unitView == null)
            {
                throw new MissingReferenceException($"MissingReferenceException: {GetType()} - UnitView missing!");
            }
            
            if (heightLineView == null)
            {
                throw new MissingReferenceException($"MissingReferenceException: {GetType()} - UnitView missing!");
            }
            
            if (clickCamera == null)
            {
                throw new MissingReferenceException($"MissingReferenceException: {GetType()} Camera missing!");
            }
        }

        private void OnLeftMouse()
        {
            DeselectAll();
            
            if (SimulationManager.Instance.CurrentInteractionMode == SimulationManager.InteractionMode.HeightLineSelection)
            {
                SelectHeightLineMode();
            } 
            else if (SimulationManager.Instance.CurrentInteractionMode ==
                     SimulationManager.InteractionMode.SlopeSelection)
            {
                SelectSlopeLineMode();
            }
            else
            {
                SelectTileMode();
            }
        }

        private void OnRightMouse()
        {
            DeselectAll();
        }

        private void DeselectAll()
        {
            DeselectHeightLineMode();
            DeselectSlopeLineMode();
            DeselectTileMode();
        }

        private void SelectTileMode()
        {
            (MapUnit, Vector2Int) unit = GetMapUnitAndPosition();
            unitView.DisableUnit();
            unitView.EnableUnit(unit);
        }

        private void DeselectTileMode()
        {
            unitView.DisableUnit();
        }

        private void SelectHeightLineMode()
        {
            heightLineView.DisableLine();
            (MapUnit unit, Vector2Int vec) unit = GetMapUnitAndPosition();
            Debug.Log("hello");
            Vector2Int[][] heightLine = MapManager.Instance.MapController.UnitMap.GetHeightLine(unit.vec);
            heightLineView.SetHeightLinePoints(heightLine[0]);
        }
        
        private void DeselectHeightLineMode()
        {
            heightLineView.DisableLine();
        }

        private void SelectSlopeLineMode()
        {
            
        }
        
        private void DeselectSlopeLineMode()
        {
            
        }

        private (MapUnit, Vector2Int) GetMapUnitAndPosition()
        {
            Vector3 worldPosition = clickCamera.ScreenToWorldPoint((Mouse.current.position.ReadValue()));
            return MapManager.Instance.MapController.GetMapUnitByGlobalPosition(worldPosition);
        }
    }
}
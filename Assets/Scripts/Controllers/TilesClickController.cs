using System;
using Manager;
using Model;
using UnityEngine;
using UnityEngine.EventSystems;
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

        [FormerlySerializedAs("heightLineView")] [SerializeField]
        private TileLineView tileLineView;

        [FormerlySerializedAs("_clickCamera")] [SerializeField]
        private Camera clickCamera;

        private void Awake()
        {
            if (unitView == null)
            {
                throw new MissingReferenceException($"MissingReferenceException: {GetType()} - UnitView missing!");
            }
            
            if (tileLineView == null)
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
            if (SimulationManager.Instance.PointerOverUI)
                return;
            
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
            if (SimulationManager.Instance.PointerOverUI)
                return;
            
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
            (MapUnit unit, Vector2Int vec) unit = GetMapUnitAndPosition();
            Vector2Int[][] heightLine = MapManager.Instance.MapController.UnitMap.GetHeightLine(unit.vec);
            tileLineView.SetTileLinePoints(heightLine[0]);
        }
        
        private void DeselectHeightLineMode()
        {
            tileLineView.DisableLine();
        }

        private void SelectSlopeLineMode()
        {
            (MapUnit unit, Vector2Int vec) unit = GetMapUnitAndPosition();
            Vector2Int[] slopeLine = MapManager.Instance.MapController.UnitMap.GetSlopeLine(unit.vec);
            Debug.Log(slopeLine.Length);
            foreach(Vector2Int vec in slopeLine)
                Debug.Log(vec);
            
            tileLineView.SetTileLinePoints(slopeLine);
        }
        
        private void DeselectSlopeLineMode()
        {
            tileLineView.DisableLine();
        }

        private (MapUnit, Vector2Int) GetMapUnitAndPosition()
        {
            Vector3 worldPosition = clickCamera.ScreenToWorldPoint((Mouse.current.position.ReadValue()));
            return MapManager.Instance.MapController.GetMapUnitByGlobalPosition(worldPosition);
        }
    }
}
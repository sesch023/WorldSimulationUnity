﻿using System;
using Manager;
using Model.Map;
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

        [SerializeField]
        private TileGroupView tileGroupView;
        
        [SerializeField]
        private TileGroupView tileGroupViewExits;
        
        [SerializeField]
        private TileGroupView tileGroupViewBorder;

        [SerializeField] 
        private TileLineView tileLineView;

        [FormerlySerializedAs("_clickCamera")] [SerializeField]
        private Camera clickCamera;

        private void Awake()
        {
            if (unitView == null)
            {
                throw new MissingReferenceException($"MissingReferenceException: {GetType()} - UnitView missing!");
            }
            
            if (tileGroupView == null)
            {
                throw new MissingReferenceException($"MissingReferenceException: {GetType()} - Tile Group View missing!");
            }
            
            if (tileGroupViewExits == null)
            {
                throw new MissingReferenceException($"MissingReferenceException: {GetType()} - Tile Group View missing!");
            }
            
            if (tileGroupViewBorder == null)
            {
                throw new MissingReferenceException($"MissingReferenceException: {GetType()} - Tile Group View missing!");
            }
            
            if (tileLineView == null)
            {
                throw new MissingReferenceException($"MissingReferenceException: {GetType()} - Tile Line View missing!");
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
            
            if (SimulationManager.Instance.CurrentInteractionMode == SimulationManager.InteractionMode.ValleySelection)
            {
                SelectValleyMode();
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
            DeselectValley();
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

        private void SelectValleyMode()
        {
            (MapUnit unit, Vector2Int vec) unit = GetMapUnitAndPosition();
            var valleyDef = MapManager.Instance.MapController.UnitMap.GetTerrainGroup(unit.vec);
            tileGroupView.tiles = valleyDef.group;
            tileGroupViewExits.tiles = valleyDef.groupExits;
            tileGroupViewBorder.tiles = valleyDef.groupBorder;
            tileGroupViewBorder.Enable();
            tileGroupView.Enable();
            tileGroupViewExits.Enable();
        }
        
        private void DeselectValley()
        {
            tileGroupView.Disable();
            tileGroupViewExits.Disable();
            tileGroupViewBorder.Disable();
        }

        private void SelectSlopeLineMode()
        {
            (MapUnit unit, Vector2Int vec) unit = GetMapUnitAndPosition();
            Vector2Int[] slopeLine = MapManager.Instance.MapController.UnitMap.GetSlopeLine(unit.vec);
            
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
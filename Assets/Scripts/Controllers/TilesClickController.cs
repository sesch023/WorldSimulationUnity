using Manager;
using Model.Map;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using Views.GameViews;
using Views.UIViews;

namespace Controllers {
    /// <summary>
    /// Controller class for dealing with click infos on a component and delegating
    /// between views and units in the MapController of the MapManager.
    /// Requires a PlayerInput to get Mouse Clicks via messages.
    /// </summary>
    [RequireComponent(typeof(PlayerInput))]
    public class TilesClickController : MonoBehaviour
    {
        /// View for information of a single tile / unit. 
        [FormerlySerializedAs("_unitView")] [SerializeField]
        private UnitView unitView;
        
        /// View for displaying a group of tiles like a selection of a valley or peak.
        [SerializeField]
        private TileGroupView tileGroupView;
        
        /// View for displaying exits to a peak or valley.
        [SerializeField]
        private TileGroupView tileGroupViewExits;
        
        /// View for displaying a border around a valley or peak.
        [SerializeField]
        private TileGroupView tileGroupViewBorder;
        
        /// View for displaying a line on the map like a slope.
        [SerializeField] 
        private TileLineView tileLineView;
        
        /// Camera to get the world position from the mouse position.
        [FormerlySerializedAs("_clickCamera")] [SerializeField]
        private Camera clickCamera;
        
        /// <summary>
        /// Checks if every required reference is set.
        /// </summary>
        /// <exception cref="MissingReferenceException">If a view or the camera is missing.</exception>
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
        
        /// <summary>
        /// If the mouse is clicked on the component, execute a mode depending on the current interaction mode.
        /// </summary>
        private void OnLeftMouse()
        {
            // If the mouse is not over a UI element, execute a mode.
            if (SimulationManager.Instance.PointerOverUI)
                return;
            
            DeselectAll();
            
            if (SimulationManager.Instance.CurrentInteractionMode == SimulationManager.InteractionMode.GroupSelection)
            {
                SelectGroupMode();
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
        
        /// <summary>
        /// On a right mouse click close all views.
        /// </summary>
        private void OnRightMouse()
        {
            if (SimulationManager.Instance.PointerOverUI)
                return;
            
            DeselectAll();
        }
        
        /// <summary>
        /// Methods for closing all views.
        /// </summary>
        private void DeselectAll()
        {
            DeselectGroup();
            DeselectSlopeLineMode();
            DeselectTileMode();
        }
        
        /// <summary>
        /// Select a tile and show the information of the tile.
        /// </summary>
        private void SelectTileMode()
        {
            (MapUnit, Vector2Int) unit = GetMapUnitAndPosition();
            unitView.DisableUnit();
            unitView.EnableUnit(unit);
        }

        /// <summary>
        /// Deselect a tile and hide the information of the tile.
        /// </summary>
        private void DeselectTileMode()
        {
            unitView.DisableUnit();
        }
        
        /// <summary>
        /// Select a terrain group (valley or peak) and show the information of the valley (area, exits, border).
        /// </summary>
        private void SelectGroupMode()
        {
            (MapUnit unit, Vector2Int vec) unit = GetMapUnitAndPosition();
            var valleyDef = MapManager.Instance.MapController.UnitMap.GetTerrainGroup(unit.vec);
            tileGroupView.Tiles = valleyDef.group;
            tileGroupViewExits.Tiles = valleyDef.groupExits;
            tileGroupViewBorder.Tiles = valleyDef.groupBorder;
            tileGroupViewBorder.Enable();
            tileGroupView.Enable();
            tileGroupViewExits.Enable();
        }
        
        /// <summary>
        /// Deselect a terrain group.
        /// </summary>
        private void DeselectGroup()
        {
            tileGroupView.Disable();
            tileGroupViewExits.Disable();
            tileGroupViewBorder.Disable();
        }

        /// <summary>
        /// Select a slope line and show the information of the slope line.
        /// </summary>
        private void SelectSlopeLineMode()
        {
            (MapUnit unit, Vector2Int vec) unit = GetMapUnitAndPosition();
            Vector2Int[] slopeLine = MapManager.Instance.MapController.UnitMap.GetSlopeLine(unit.vec);
            
            tileLineView.SetTileLinePoints(slopeLine);
        }
        
        /// <summary>
        /// Deselect a slope line.
        /// </summary>
        private void DeselectSlopeLineMode()
        {
            tileLineView.DisableLine();
        }
        
        /// <summary>
        /// Gets the MapUnit and the position of the mouse click.
        /// </summary>
        /// <returns>MapUnit and position of the mouse click.</returns>
        private (MapUnit, Vector2Int) GetMapUnitAndPosition()
        {
            Vector3 worldPosition = clickCamera.ScreenToWorldPoint((Mouse.current.position.ReadValue()));
            return MapManager.Instance.MapController.GetMapUnitByGlobalPosition(worldPosition);
        }
    }
}
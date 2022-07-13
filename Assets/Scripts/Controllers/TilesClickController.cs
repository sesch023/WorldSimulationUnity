using System;
using Manager;
using Model;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using Views;
using Views.UIViews;

namespace Controllers {
    public class TilesClickController : MonoBehaviour
    {
        [SerializeField]
        private UnitView _unitView;

        [SerializeField]
        private Camera _clickCamera;

        private void Awake()
        {
            if (_unitView == null)
            {
                throw new MissingReferenceException("MissingReferenceException: TilesClickController. UnitView missing!");
            }
            
            if (_clickCamera == null)
            {
                throw new MissingReferenceException("MissingReferenceException: TilesClickController. Camera missing!");
            }
        }

        private void OnLeftMouse()
        {
            Vector3 worldPosition = _clickCamera.ScreenToWorldPoint((Mouse.current.position.ReadValue()));
            (MapUnit, Vector3Int) unit = MapManager.Instance.MapController.GetMapUnitByGlobalPosition(worldPosition);
            _unitView.DisableUnit();
            _unitView.EnableUnit(unit);
        }
    }
}
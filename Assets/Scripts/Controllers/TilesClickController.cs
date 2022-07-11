using System;
using Manager;
using Model;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using Views;
using Views.UIViews;

namespace Controllers {
    public class TilesClickController : MonoBehaviour
    {
        [SerializeField]
        private UnitView _unitView;

        private void Awake()
        {
            if (_unitView == null)
            {
                throw new MissingComponentException("MissingComponentException: TilesClickController. UnitView missing!");
            }
        }

        private void OnMouseDown()
        {
            (MapUnit, Vector3Int) unit = MapManager.Instance.GetMapUnitByGlobalPosition(Input.mousePosition);
            _unitView.DisableUnit();
            _unitView.EnableUnit(unit);
        }
    }
}
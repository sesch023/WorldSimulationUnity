using System;
using Manager;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Controllers {
    public class TilesController : MonoBehaviour
    {
        private void OnMouseDown()
        {
            Vector3Int tilemapPos = MapManager.Instance.TileMap.WorldToCell(SimulationManager.Instance.MainCamera.ScreenToWorldPoint(Input.mousePosition));
            Tile tile = MapManager.Instance.TileMap.GetTile<Tile>(tilemapPos);
            Debug.Log(tile);
        }
    }
}
using System;
using Base;
using Manager;
using Model;
using Model.Map;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;
using Views.GameViews;

namespace Controllers
{
    [Serializable]
    public class TileMapController : IUpdatable
    {
        [field: SerializeField] public Tilemap TileMap { get; private set; }

        [field: SerializeField]
        public MapTileViews TileViews { get; private set; }

        [field: SerializeField] public Map UnitMap { get; private set; }

        public void Init()
        {
            if (TileViews == null)
            {
                throw new MissingReferenceException($"MissingReferenceException: {GetType().Name}. MapTiles missing!");
            }
            
            if (TileMap == null)
            {
                throw new MissingReferenceException($"MissingReferenceException: {GetType().Name}. TileMap missing!");
            }
            
            if (UnitMap == null)
            {
                throw new MissingReferenceException($"MissingReferenceException: {GetType().Name}. UnitMap missing!");
            }
            
            for (var x = 0; x < UnitMap.SizeX; x++)
            {
                for (var y = 0; y < UnitMap.SizeY; y++)
                {
                    MapUnit unit = UnitMap.MapUnits[x, y];
                    Tile tile = TileViews.GetTileByMapUnit(unit);
                    TileMap.SetTile(new(x, y, 0), tile);
                }
            }
        }
        
        public void Update()
        {
            UnitMap.Update();
        }
        
        public (MapUnit, Vector2Int) GetMapUnitByGlobalPosition(Vector3 pos)
        {
            // This is in Tile Units
            Vector3Int tilemapPos = TileMap.WorldToCell(pos);
            return (UnitMap.MapUnits[tilemapPos.x, tilemapPos.y], (Vector2Int)tilemapPos);
        }

        public Vector3 TileMapPositionToGlobalPosition(Vector2Int position)
        {
            return TileMapPositionToGlobalPosition((Vector2) position);
        }
        
        public Vector3 TileMapPositionToGlobalPosition(Vector2 position)
        {
            var transform = TileMap.transform;
            Vector3 basePosition = transform.position;
            var localScale = transform.localScale;
            return (new Vector3(position.x * localScale.x, position.y *  localScale.y, 0)) + basePosition;
        }
    }
}
using System;
using Base;
using Manager;
using Model;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Controllers
{
    [Serializable]
    public class TileMapController : IUpdatable
    {
        [field: SerializeField] public Tilemap TileMap { get; private set; }
        
        [SerializeField]
        private MapTiles tiles;

        [field: SerializeField] public Map UnitMap { get; private set; }

        public void Init()
        {
            if (tiles == null)
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
                    Tile tile = tiles.GetTileByMapUnit(unit);
                    TileMap.SetTile(new(x, y, 0), tile);
                }
            }
        }
        
        public void Update()
        {
            UnitMap.Update();
        }
        
        public (MapUnit, Vector3Int) GetMapUnitByGlobalPosition(Vector3 pos)
        {
            // This is in Tile Units
            Vector3Int tilemapPos = TileMap.WorldToCell(pos);
            return (UnitMap.MapUnits[tilemapPos.x, tilemapPos.y], tilemapPos);
        }
    }
}
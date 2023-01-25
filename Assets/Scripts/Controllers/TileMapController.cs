using System;
using Base;
using Manager;
using Model.Map;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using Views.GameViews;

namespace Controllers
{
    /// <summary>
    /// Controller of a Tilemap. Delegates between a Map, Tilemap and a TileView Container class for tile textures.
    /// Is an updateable controlled by the MapManager.
    /// </summary>
    [Serializable]
    public class TileMapController : IUpdatable
    {
        /// Tilemap to which a map is displayed.
        [field: SerializeField] 
        public Tilemap TileMap { get; private set; }
        
        /// Container class for tile textures. Decides on which texture to display on a tile depending on map unit conditions.
        [field: SerializeField]
        public MapTileViews TileViews { get; private set; }
        
        /// Map of the world. Contains all map units and their conditions.
        [field: SerializeField] public Map UnitMap { get; private set; }
        
        /// <summary>
        /// Initializes the TileMapController. Delegates the texture exchange between the Tilemap and the TileViews
        /// under conditions set by the unit map. This code cannot be called from the constructor, because at the point
        /// of construction the Monobehaviours and ScriptableData are not yet initialized.
        /// </summary>
        /// <exception cref="MissingReferenceException">Thrown if the TileMap, TileViews or UnitMap are not set.</exception>
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
                    UpdateTile(x, y);
                }
            }
        }

        /// <summary>
        /// Calls the Update method of the Map.
        /// </summary>
        public void Update()
        {
            UnitMap.Update();
        }

        public void UpdateTile(int x, int y)
        {
            MapUnit unit = UnitMap.MapUnits[x, y];
            Tile tile = TileViews.GetTileForMapUnit(unit);
            var pos = new Vector3Int(x, y, 0);
            TileMap.SetTile(pos, tile);
            TileMap.RefreshTile(pos);
            unit.AddChangeSubscriber((mapUnit) =>
            {
                Tile changed = TileViews.GetTileForMapUnit(mapUnit);
                TileMap.SetTile(pos, changed);
                TileMap.RefreshTile(pos);
            });
        }

        /// <summary>
        /// Given a global position, returns the map unit at that position if possible and the position itself.
        /// If not, throws an IndexOutOfRangeException.
        /// </summary>
        /// <param name="pos">Global MapPositionVec that is converted.</param>
        /// <returns>MapUnit and position on the tilemap as 0 based integer vector.</returns>
        /// <exception cref="IndexOutOfRangeException">Thrown if the position is out of bounds.</exception>
        public (MapUnit, Vector2Int) GetMapUnitByGlobalPosition(Vector3 pos)
        {
            // This is in Tile Units
            Vector3Int tilemapPos = TileMap.WorldToCell(pos);
            if(UnitMap.MapUnits.GetLength(0) > tilemapPos.x && UnitMap.MapUnits.GetLength(1) > tilemapPos.y)
            {
                return (UnitMap.MapUnits[tilemapPos.x, tilemapPos.y], (Vector2Int)tilemapPos);
            }

            throw new IndexOutOfRangeException($"IndexOutOfRangeException: {GetType().Name}. Tilemap position {tilemapPos} is out of range!");
        }

        /// <summary>
        /// Converts a int position on the tilemap to a global position in the world space.
        /// </summary>
        /// <param name="position">Vector2 position on the tilemap.</param>
        /// <returns>Vector3 position in the world space.</returns>
        public Vector3 TileMapPositionToGlobalPosition(Vector2Int position)
        {
            return TileMapPositionToGlobalPosition((Vector2) position);
        }
        
        /// <summary>
        /// Converts a float position on the tilemap to a global position in the world space.
        /// </summary>
        /// <param name="position">Vector2 position on the tilemap.</param>
        /// <returns>Vector3 position in the world space.</returns>
        public Vector3 TileMapPositionToGlobalPosition(Vector2 position)
        {
            var transform = TileMap.transform;
            Vector3 basePosition = transform.position;
            var localScale = transform.localScale;
            return (new Vector3(position.x * localScale.x, position.y *  localScale.y, 0)) + basePosition;
        }
    }
}
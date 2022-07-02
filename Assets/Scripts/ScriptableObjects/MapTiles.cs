using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "MapTiles", menuName = "ScriptableObjects/SpawnManagerScriptableObject", order = 1)]
    public class MapTiles : ScriptableObject
    {
        [Serializable]
        private class TileData
        {
            public Sprite sprite;
            public Color color = new(255,255,255,255);
            
            public static void SetTileData(Tile tile, TileData data)
            {
                tile.color = data.color;
                tile.sprite = data.sprite;
            }
        }
        
        public Tile TestTile { get; private set; }
        public Tile TestTile2 { get; private set; }

        [SerializeField]
        private TileData TestTile1Data;
        [SerializeField]
        private TileData TestTile2Data;

        protected void OnEnable()
        {
            TestTile = CreateInstance<Tile>();
            TestTile2 = CreateInstance<Tile>();
            
            TileData.SetTileData(TestTile, TestTile1Data);
            TileData.SetTileData(TestTile2, TestTile1Data);
            Debug.Log(TestTile);
        }

        
    }
}

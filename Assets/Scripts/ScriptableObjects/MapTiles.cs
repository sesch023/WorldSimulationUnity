using System;
using JetBrains.Annotations;
using Model;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "MapTiles", menuName = "ScriptableObjects/SpawnManagerScriptableObject", order = 1)]
    public class MapTiles : ScriptableObject
    {
        [Serializable]
        private struct TileData
        {
            public Sprite sprite;
            public Color color;

            public TileData(Sprite sprite, Color color)
            {
                this.sprite = sprite;
                this.color = color;
            }
            
            public static void SetTileData(Tile tile, TileData data)
            {
                tile.color = data.color;
                tile.sprite = data.sprite;
            }
        }

        [SerializeField] 
        private float[] heightSteps = 
        {
            -8000, -6000, -4000, -2000, -1000, -500, -200, -100, -50,
            0, 50, 100, 200, 500, 1000, 2000, 4000, 6000, 8000
        };

        [SerializeField]
        private float minHeightColor = 100f;

        private Tile[] TestTile { get; set; }
        private Tile[] TestTile2 { get; set; }

        [FormerlySerializedAs("TestTile1Data")] [SerializeField]
        private TileData testTile1Data;
        [FormerlySerializedAs("TestTile2Data")] [SerializeField]
        private TileData testTile2Data;

        protected void OnEnable()
        {
            TestTile = new Tile[heightSteps.Length];
            TestTile2 = new Tile[heightSteps.Length];

            float colorStep = (255f - minHeightColor) / (heightSteps.Length - 1);
            
            for (int i = 0; i < heightSteps.Length; i++)
            {
                float colorValue = colorStep * i + minHeightColor;
                Color newColor = new Color(colorValue, colorValue, colorValue, 255);
                Tile newTile = CreateInstance<Tile>();
                TileData.SetTileData(newTile, new TileData(testTile1Data.sprite, newColor));
                TestTile[i] = newTile;
                
                newTile = CreateInstance<Tile>();
                TileData.SetTileData(newTile, new TileData(testTile2Data.sprite, newColor));
                TestTile2[i] = newTile;
            }
        }

        public Tile GetTileByMapUnit([NotNull] MapUnit mapUnit)
        {
            for (int i = 0; i < heightSteps.Length; i++)
            {
                if (heightSteps[i] <= mapUnit.Position.Elevation)
                {
                    return TestTile2[i];
                }
            }

            return TestTile2[0];
        }
    }
}

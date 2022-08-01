using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Model.Map;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

namespace Views.GameViews
{
    /// <summary>
    /// The MapTileView is responsible for managing the appearance of tiles on the map. It decides which tile is
    /// used for a map unit depending on the conditions it.
    /// </summary>
    [CreateAssetMenu(fileName = "MapTiles", menuName = "ScriptableObjects/MapTiles", order = 1)]
    public class MapTileViews : ScriptableObject
    {
        /// <summary>
        /// Data to initializes a tile. Contains a sprite and color.
        /// </summary>
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

        /// Steps of heights to be used for the map.
        private List<float> _heightSteps = new();

        /// Minimum height of the map.
        [field: SerializeField] 
        public float LowestHeight { get; } = -8000;

        /// Maximum height of the map.
        [field: SerializeField] 
        public float HeighestHeight { get; } = 10000;
        
        /// Steps between heights.
        [SerializeField] 
        private float heightStep = 1000;

        /// Minimum darkness heighest elevation.
        [SerializeField]
        private float minHeightColor = 0.3f;
        
        /// Steps between darkness.
        private float _colorStep;

        private Tile[] TestTile { get; set; }
        private Tile[] TestTile2 { get; set; }

        [FormerlySerializedAs("TestTile1Data")] [SerializeField]
        private TileData testTile1Data;
        [FormerlySerializedAs("TestTile2Data")] [SerializeField]
        private TileData testTile2Data;

        protected void OnEnable()
        {
            CalculateHeightSteps();
            TestTile = new Tile[_heightSteps.Count];
            TestTile2 = new Tile[_heightSteps.Count];

            Debug.Log("Height Steps: " + TestTile.Length);

            _colorStep = (1.0f - minHeightColor) / _heightSteps.Count;
            
            for (int i = 0; i < _heightSteps.Count; i++)
            {
                float colorValue = 1.0f - _colorStep * i;
                Color newColor = new Color(colorValue, colorValue, colorValue, 1.0f);
                Tile newTile = CreateInstance<Tile>();
                newTile.flags = TileFlags.None;
                TileData.SetTileData(newTile, new TileData(testTile1Data.sprite, newColor));
                TestTile[i] = newTile;
                
                newTile = CreateInstance<Tile>();
                newTile.flags = TileFlags.None;
                TileData.SetTileData(newTile, new TileData(testTile2Data.sprite, newColor));
                TestTile2[i] = newTile;
            }
        }

        private void CalculateHeightSteps()
        {
            _heightSteps = new List<float>();
            float currentHeight = LowestHeight;
            do
            {
                _heightSteps.Add(currentHeight);
                currentHeight += heightStep;
            } while (currentHeight < HeighestHeight);
            _heightSteps.Add(currentHeight);
        }

        public Color GetTileColorOffsetByElevation(float elevation)
        {
            for (int i = 0; i < _heightSteps.Count; i++)
            {
                if (_heightSteps[i] >= elevation)
                {
                    float colorValue = 1.0f - _colorStep * Math.Max(i - 1, 0);
                    return new Color(colorValue, colorValue, colorValue, 1.0f);
                }
            }

            return new Color(1.0f, 1.0f, 1.0f);
        }
        
        public Tile GetTileByMapUnit([NotNull] MapUnit mapUnit)
        {
            for (int i = 0; i < _heightSteps.Count; i++)
            {
                if (_heightSteps[i] >= mapUnit.Position.Elevation)
                {
                    int take = Math.Max(i - 1, 0);
                    return TestTile[take];
                }
            }
            return TestTile[0];
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Model;
using Unity.VisualScripting.FullSerializer;
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
            public TileCondition condition;

            public TileData(Sprite sprite, Color color, TileCondition condition)
            {
                this.sprite = sprite;
                this.color = color;
                this.condition = condition;
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
        public float LowestHeight { get; private set; } = -8000;

        /// Maximum height of the map.
        [field: SerializeField] 
        public float HeighestHeight { get; private set; } = 10000;
        
        /// Steps between heights.
        [SerializeField] 
        private float heightStep = 1000;

        /// Minimum darkness heighest elevation.
        [SerializeField]
        private float minHeightColor = 0.3f;

        [SerializeField] 
        private float maxColor = 0.6f;
        
        [SerializeField] 
        private TileData defaultTile;
        
        /// Steps between darkness.
        private float _colorStep;

        private readonly Dictionary<TileCondition, Tile[]> _tileData = new();
        private TileCondition[] _relevantConditions;

        [SerializeField]
        private TileData[] tileDefinitions;

        private float GetColorValue(int step)
        {
            return maxColor - _colorStep * step;
        }
        
        protected void OnEnable()
        {
            CalculateHeightSteps();
            
            _colorStep = (maxColor - minHeightColor) / _heightSteps.Count;

            for(int k = 0; k < tileDefinitions.Length; k++)
            {
                _tileData.Add(tileDefinitions[k].condition, new Tile[_heightSteps.Count]);
                
                for (int i = 0; i < _heightSteps.Count; i++)
                {
                    float colorValue = GetColorValue(i);
                    Color newColor = new Color(colorValue, colorValue, colorValue, 1.0f);
                    Tile newTile = CreateInstance<Tile>();
                    TileData.SetTileData(newTile, new TileData(tileDefinitions[k].sprite, newColor, tileDefinitions[k].condition));
                    _tileData[tileDefinitions[k].condition][i] = newTile;
                }
            }
            
            _relevantConditions = _tileData.Keys.ToArray();
            
            _tileData.Add(defaultTile.condition, new Tile[_heightSteps.Count]);
            for (int i = 0; i < _heightSteps.Count; i++)
            {
                float colorValue = GetColorValue(i);
                Color newColor = new Color(colorValue, colorValue, colorValue, 1.0f);
                Tile newTile = CreateInstance<Tile>();
                TileData.SetTileData(newTile, new TileData(defaultTile.sprite, newColor, defaultTile.condition));
                _tileData[defaultTile.condition][i] = newTile;
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
        
        public Tile GetTileForMapUnit([NotNull] MapUnit mapUnit)
        {
            Tile tile = null;

            TileCondition relevant = defaultTile.condition;
            foreach (var con in _relevantConditions)
            {
                if (con.CheckCondition(mapUnit))
                {
                    relevant = con;
                    break;  
                }
            }
            
            for (int i = 0; i < _heightSteps.Count; i++)
            {
                if (_heightSteps[i] >= mapUnit.Position.Elevation)
                {
                    int take = Math.Max(i - 1, 0);
                    tile = _tileData[relevant][take];
                    break;
                }
            }
            if(tile == null) 
                tile =_tileData[relevant][_tileData[relevant].GetLength(0) - 1];

            return tile;
        }
    }
}

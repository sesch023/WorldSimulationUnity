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
            public GroundMaterialType materialType;

            public TileData(Sprite sprite, Color color, GroundMaterialType materialType)
            {
                this.sprite = sprite;
                this.color = color;
                this.materialType = materialType;
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
        
        /// Steps between darkness.
        private float _colorStep;

        private readonly Dictionary<GroundMaterialType, Tile[]> _tileData = new();

        [SerializeField]
        private TileData[] tileDefinitions;

        protected void OnEnable()
        {
            CalculateHeightSteps();
            
            _colorStep = (1.0f - minHeightColor) / _heightSteps.Count;
            for(int k = 0; k < tileDefinitions.Length; k++)
            {
                _tileData.Add(tileDefinitions[k].materialType, new Tile[_heightSteps.Count]);
                
                for (int i = 0; i < _heightSteps.Count; i++)
                {
                    float colorValue = 1.0f - _colorStep * i;
                    Color newColor = new Color(colorValue, colorValue, colorValue, 1.0f);
                    Tile newTile = CreateInstance<Tile>();
                    newTile.flags = TileFlags.None;
                    TileData.SetTileData(newTile, new TileData(tileDefinitions[k].sprite, newColor, tileDefinitions[k].materialType));
                    _tileData[tileDefinitions[k].materialType][i] = newTile;
                }
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
        
        public Tile GetTileForMapUnit([NotNull] MapUnit mapUnit)
        {
            Tile tile = null;
                
            GroundMaterialType materialType = mapUnit.GroundMaterial.FindMostSignificantMaterial();
            for (int i = 0; i < _heightSteps.Count; i++)
            {
                if (_heightSteps[i] >= mapUnit.Position.Elevation)
                {
                    int take = Math.Max(i - 1, 0);
                    tile = _tileData[materialType][take];
                    break;
                }
            }
            if(tile == null) 
                tile =_tileData[materialType][_tileData[materialType].GetLength(0) - 1];

            tile = Instantiate(tile);
            
            return tile;
        }
    }
}

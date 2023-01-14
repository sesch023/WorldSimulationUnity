using System.Collections.Generic;
using Base;
using Manager;
using Model.Generators;
using Model.Map.Feature;
using Model.Map.Processing;
using Model.Map.VirtualFeatureSelection;
using UnityEngine;
using Views.GameViews;

namespace Model.Map
{
    /// <summary>
    /// Map in the Game. It represents the entire world.
    /// </summary>
    [CreateAssetMenu(fileName = "Map", menuName = "ScriptableObjects/Map", order = 1)]
    public class Map : ScriptableObject, IUpdatable
    {
        /// Area of a tile in the map.
        public const float UnitArea = 1.0f;
        
        /// Width of the Map.
        [field: SerializeField] public int SizeX { get; private set; } = 1025;
        /// Height of the Map.
        [field: SerializeField] public int SizeY { get; private set; } = 1025;
        
        /// BaseGenerator used to generate the map.
        [SerializeField] 
        private BaseGenerator generator;

        [SerializeReference] 
        private BaseGeneralProcessing[] preprocessMapSteps;
        
        /// Units of the map.
        public MapUnit[,] MapUnits { get; private set; }
        
        private readonly List<WaterBody> _waterBodies = new ();
        
        public void AddWaterBody(WaterBody waterBody)
        {
            _waterBodies.Add(waterBody);
        }
        
        public void AddWaterBody(Vector2Int initialPosition, float volume)
        {
            // ReSharper disable once ObjectCreationAsStatement
            new WaterBody(this, initialPosition, volume);
        }

        public WaterBody GetBodyOfWaterByPosition(Vector2Int pos)
        {
            foreach(var body in _waterBodies)
            {
                if (body.InBody(pos))
                    return body;
            }
            return null;
        }

        public void MergeBodyOfWater(Vector2Int pos1, Vector2Int pos2)
        {
            var body1 = GetBodyOfWaterByPosition(pos1);
            var body2 = GetBodyOfWaterByPosition(pos2);
            if (body1 == null || body2 == null){
                Debug.LogWarning("Trying to merge bodies of water that don't exist");
                return;
            }

            if (body1 == body2)
            {
                Debug.LogWarning("Trying to merge bodies of water that are the same");
                return;
            }
            
            _waterBodies.Remove(body2);
            _waterBodies.Remove(body1);
            _waterBodies.Add(WaterBody.MergeBodiesOfWaterIntoFirst(body1, body2));
        }
        
        /// <summary>
        /// Enables and initializes the map and initializes the units.
        /// </summary>
        /// <exception cref="MissingReferenceException">If the generator is not set.</exception>
        private void OnEnable()
        {
            if (generator == null)
            {
                throw new MissingReferenceException($"MissingReferenceException: {GetType()} - Illegal Map. Generator missing!");
            }
            
            // Limit the sizes of the map.
            (int sizeX, int sizeY) clippedSizes = generator.LimitMapSizes(SizeX, SizeY);
            SizeX = clippedSizes.sizeX;
            SizeY = clippedSizes.sizeY;
            // Generate the map.
            (float[,] mapElevation, float minHeight, float maxHeight) = generator.GenerateElevation(SizeX, SizeY);
            MapUnits = new MapUnit[SizeX, SizeY];
            // Create the units.
            for (var x = 0; x < MapUnits.GetLength(0); x++)
            {
                for (var y = 0; y < MapUnits.GetLength(1); y++)
                {
                    (float lat, float lon) latLong = CalculateLatLong(x, y);
                    MapUnits[x, y] = new MapUnit(0.0f, 0.0f, new MapPosition(latLong.lat, latLong.lon, mapElevation[x, y]));
                }
            }

            foreach (var step in preprocessMapSteps)
            {
                step.ProcessMap(this);
            }
        }
        
        /// <summary>
        /// Calculates the latitude and longitude of a tile by its position in the map.
        /// </summary>
        /// <param name="x">X-Position of the tile.</param>
        /// <param name="y">Y-Position of the tile.</param>
        /// <returns>Latitude and longitude of the tile.</returns>
        private (float lat, float lon) CalculateLatLong(int x, int y)
        {
            return (((float)x / SizeX) * 360.0f, ((float)y / SizeY) * 180.0f) ;
        }

        /// <summary>
        /// Executes the update method of the map by triggering the update of the units.
        /// </summary>
        public void Update()
        {
            foreach (MapUnit unit in MapUnits)
            {
                unit.Update();
            }
        }
        
        /// <summary>
        /// Returns a valley at the given position by taking all tiles with lower elevation next to it.
        /// Its returns all the tiles in the valley, its borders and exit tiles (lowest positions next to the border).
        /// </summary>
        /// <param name="position">Position to find a valley at.</param>
        /// <returns>Tiles of the valley, Border of the valley, Exits of the valley.</returns>
        public (Vector2Int[] valley, Vector2Int[] valleyBorder, Vector2Int[] valleyExits) GetValley(Vector2Int position)
        {
            MapUnit startTile = MapUnits[position.x, position.y];
            Valley valley = new Valley(position, MapUnits, startTile.Position.Elevation);
            
            return (valley.CalculatedPositions, valley.CalculatedBorder, valley.CalculatedExits);
        }
        
        /// <summary>
        /// Returns a peak at the given position by taking all tiles with higher elevation next to it.
        /// Its returns all the tiles in the peak, its borders and exit tiles (lowest positions next to the border).
        /// </summary>
        /// <param name="position">Position to find a peak at.</param>
        /// <returns>Tiles of the peak, Border of the peak, Exits of the peak.</returns>
        public (Vector2Int[] peak, Vector2Int[] peakBorder, Vector2Int[] peakExits) GetPeak(Vector2Int position)
        {
            MapUnit startTile = MapUnits[position.x, position.y];
            Valley valley = new Peak(position, MapUnits, startTile.Position.Elevation);
            
            return (valley.CalculatedPositions, valley.CalculatedBorder, valley.CalculatedExits);
        }
        
        /// <summary>
        /// Returns a terrain group at the given position by taking all tiles with certain features next to it.
        /// Its returns all the tiles in the group, its borders and exit tiles.
        ///
        /// Currently only works for peaks and valleys.
        /// </summary>
        /// <param name="position">Position to find a group at.</param>
        /// <returns>Tiles of the group, Border of the group, Exits of the group.</returns>
        public (Vector2Int[] group, Vector2Int[] groupBorder, Vector2Int[] groupExits) GetTerrainGroup(Vector2Int position)
        {
            float startTileElevation = MapUnits[position.x, position.y].Position.Elevation;
            MapTileViews view = MapManager.Instance.MapController.TileViews;
            float middleElevation = (view.HeighestHeight - view.LowestHeight) / 2 + view.LowestHeight;

            if (startTileElevation <= middleElevation)
            {
                return GetValley(position);
            }

            return GetPeak(position);
        } 

        /// <summary>
        /// Finds a slope at the given position by trying to find a way to the bottom of a valley. It is not always
        /// possible to find a well defined slope at the moment.
        /// </summary>
        /// <param name="start">Position to find a slope at.</param>
        /// <param name="momentumMultiplier">Multiplier of momentum to make the algorithm slip over local minimals.</param>
        /// <param name="maxMomentumFraction">Max fraction of momentum to take from the slope in a single step.</param>
        /// <returns>Positions of the slope in order from highest to lowest.</returns>
        public Vector2Int[] GetSlopeLine(Vector2Int start, float momentumMultiplier = 1.0f,
            float maxMomentumFraction = 1.0f)
        {
            return new Slope(start, MapUnits, momentumMultiplier, maxMomentumFraction).CalculatedSlope;
        }
        
        public MapUnit Vector2IntToMapUnit(Vector2Int position)
        {
            return MapUnits[position.x, position.y];
        }
        
        public MapUnit[] Vector2IntArrayToMapUnits(Vector2Int[] positions)
        {
            MapUnit[] units = new MapUnit[positions.Length];
            for (int i = 0; i < positions.Length; i++)
            {
                units[i] = MapUnits[positions[i].x, positions[i].y];
            }

            return units;
        }
    }
}
using Base;
using Manager;
using Model.Generators;
using UnityEngine;
using Views.GameViews;

namespace Model.Map
{
    [CreateAssetMenu(fileName = "Map", menuName = "ScriptableObjects/Map", order = 1)]
    public class Map : ScriptableObject, IUpdatable
    {
        public readonly float TileArea = 1.0f;
        
        [field: SerializeField] public int SizeX { get; private set; } = 1025;
        [field: SerializeField] public int SizeY { get; private set; } = 1025;

        [SerializeField] 
        private BaseGenerator generator;

        public MapUnit[,] MapUnits { get; private set; }
        
        private void OnEnable()
        {
            if (generator == null)
            {
                throw new MissingReferenceException("MissingReferenceException: Illegal Map. Generator missing!");
            }

            (int sizeX, int sizeY) clippedSizes = generator.LimitMapSizes(SizeX, SizeY);
            SizeX = clippedSizes.sizeX;
            SizeY = clippedSizes.sizeY;
            float[,] mapElevation = generator.GenerateElevation(SizeX, SizeY);
            MapUnits = new MapUnit[SizeX, SizeY];
            for (var x = 0; x < MapUnits.GetLength(0); x++)
            {
                for (var y = 0; y < MapUnits.GetLength(1); y++)
                {
                    (float lat, float lon) latLong = CalculateLatLong(x, y);
                    MapUnits[x, y] = new MapUnit(0.0f, 0.0f, new MapUnit.MapPosition(latLong.lat, latLong.lon, mapElevation[x, y]));
                }
            }
        }

        private (float lat, float lon) CalculateLatLong(int x, int y)
        {
            return (((float)x / SizeX) * 360.0f, ((float)y / SizeY) * 180.0f) ;
        }

        public void Update()
        {
            foreach (MapUnit unit in MapUnits)
            {
                unit.Update();
            }
        }

        public (Vector2Int[] valley, Vector2Int[] valleyBorder, Vector2Int[] valleyExits) GetValley(Vector2Int position)
        {
            MapUnit startTile = MapUnits[position.x, position.y];
            Valley valley = new Valley(position, MapUnits, startTile.Position.Elevation);
            
            return (valley.CalculatedPositions, valley.CalculatedBorder, valley.CalculatedExits);
        }
        
        public (Vector2Int[] peak, Vector2Int[] peakBorder, Vector2Int[] peakExits) GetPeak(Vector2Int position)
        {
            MapUnit startTile = MapUnits[position.x, position.y];
            Valley valley = new Peak(position, MapUnits, startTile.Position.Elevation);
            
            return (valley.CalculatedPositions, valley.CalculatedBorder, valley.CalculatedExits);
        }
        
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

        public Vector2Int[] GetSlopeLine(Vector2Int start, float momentumMultiplier = 1.0f,
            float maxMomentumFraction = 1.0f)
        {
            return new Slope(start, MapUnits, momentumMultiplier, maxMomentumFraction).CalculatedSlope;
        }
    }
}
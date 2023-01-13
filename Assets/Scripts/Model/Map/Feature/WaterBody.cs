using System.Collections.Generic;
using Model.Map.VirtualFeatureSelection;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;

namespace Model.Map.Feature
{
    public class WaterBody : IFeature
    {
        private Map _map;
        public Vector2Int[] ShallowPoints { get; private set; }
        public float ShallowPointElevation { get; private set; }
        public Vector2Int DeepestPoint { get; private set; }

        private HashSet<Vector2Int> _points;
        public float WaterVolume { get; set; }
        
        public static WaterBody MergeBodiesOfWater(WaterBody body1, WaterBody body2)
        {
            var points = new HashSet<Vector2Int>(body1._points.Union(body2._points));

            var shallowPointElevation = body1.ShallowPointElevation;
            var shallowPoints = body1.ShallowPoints;
            if (body1.ShallowPointElevation < body2.ShallowPointElevation)
            {
                shallowPointElevation = body2.ShallowPointElevation;
                shallowPoints = body2.ShallowPoints;
            }
            
            var deepestPoint = body1.DeepestPoint;
            var deepestElevation1 = body1._map.MapUnits[body1.DeepestPoint.x, body1.DeepestPoint.y].Position.Elevation;
            var deepestElevation2 = body2._map.MapUnits[body2.DeepestPoint.x, body2.DeepestPoint.y].Position.Elevation;
            
            if(deepestElevation2 < deepestElevation1)
                deepestPoint = body2.DeepestPoint;
            
            var waterVolume = body1.WaterVolume + body2.WaterVolume;

            WaterBody newBody = new()
            {
                _points = points,
                ShallowPointElevation = shallowPointElevation,
                ShallowPoints = shallowPoints,
                DeepestPoint = deepestPoint,
                WaterVolume = waterVolume
            };

            return newBody;
        }
        
        private WaterBody(){}
        
        public WaterBody(Map map, Vector2Int initialPosition, float waterVolume)
        {
            this._map = map;

            ShallowPoints = new []{ DeepestPointFromInitialPosition(initialPosition) };
            ShallowPointElevation = map.MapUnits[ShallowPoints[0].x, ShallowPoints[0].y].Position.Elevation;
            DeepestPoint = ShallowPoints[0];
            _points = new HashSet<Vector2Int>(){ DeepestPoint };
            WaterVolume = waterVolume;
            CreateBodyOfWater();
        }

        private Vector2Int DeepestPointFromInitialPosition(Vector2Int initialPosition)
        {
            Slope slope = new Slope(initialPosition, _map.MapUnits);
            return slope.CalculatedSlope[^1];
        }
        
        private void CreateBodyOfWater()
        {
            float unassignedWaterVolume = WaterVolume;
            while (unassignedWaterVolume > 0)
            {
                Valley valley = new Valley(ShallowPoints[0], _map.MapUnits);
                Vector2Int[] newShallowPoints = valley.CalculatedExits;
                float newElevation = _map.MapUnits[newShallowPoints[0].x, newShallowPoints[0].y].Position.Elevation;
                float volDiff = (newElevation - ShallowPointElevation) * (_points.Count + newShallowPoints.Length);
                ShallowPoints = newShallowPoints;
                ShallowPointElevation = newElevation;
                _points.AddRange(valley.CalculatedPositions);
                
                foreach(var vec in _points)
                {
                    float pointElevation = _map.MapUnits[vec.x, vec.y].Position.Elevation;
                    _map.MapUnits[vec.x, vec.y].WaterLevel = ShallowPointElevation - pointElevation;
                }
                
                unassignedWaterVolume -= volDiff;
            }
        }

        public Vector2Int[] GetFeaturePositions()
        {
            return _points.ToArray();
        }
        
        public bool InBody(Vector2Int position)
        {
            return _points.Contains(position);
        }
    }
}
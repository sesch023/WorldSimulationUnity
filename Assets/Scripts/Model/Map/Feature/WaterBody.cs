using System;
using System.Collections.Generic;
using Model.Map.VirtualFeatureSelection;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;
using Utils.BaseUtils;

namespace Model.Map.Feature
{
    public class WaterBody : IFeature
    {
        private Map _map;
        private Valley _bodyValley;
        
        public Vector2Int[] ShallowPoints => _bodyValley.CalculatedExits;
        public float ShallowPointElevation { get; private set; }
        public Vector2Int DeepestPoint { get; private set; }

        private Dictionary<Vector2Int, WaterBody> _overflows;
        public float WaterVolume { get; set; }
        
        public float CurrentAbsoluteWaterLevel { get; set; }
        
        public static WaterBody MergeBodiesOfWaterIntoFirst(WaterBody body1, WaterBody body2)
        {
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

            body1._bodyValley = new Valley(shallowPoints[0], body1._map.MapUnits);
            body1.ShallowPointElevation = shallowPointElevation;
            body1.DeepestPoint = deepestPoint;
            body1.WaterVolume = waterVolume;
            body1._overflows.AddRange(body2._overflows);
            foreach (var overflow in body1._overflows)
            {
                if(overflow.Value == body1 || overflow.Value == body2)
                    body1._overflows.Remove(overflow.Key);
            }
            
            return body1;
        }
        
        private WaterBody(){}
        
        public WaterBody(Map map, Vector2Int initialPosition, float waterVolume)
        {
            _map = map;
            DeepestPoint = DeepestPointFromInitialPosition(initialPosition);
            _bodyValley = new Valley(DeepestPoint, _map.MapUnits);
            ShallowPointElevation = map.MapUnits[ShallowPoints[0].x, ShallowPoints[0].y].Position.Elevation;
            _overflows = new Dictionary<Vector2Int, WaterBody>();
            WaterVolume = 0;
            CurrentAbsoluteWaterLevel = map.MapUnits[DeepestPoint.x, DeepestPoint.y].Position.Elevation;
            AddVolume(waterVolume);
            map.AddWaterBody(this);
        }

        private Vector2Int DeepestPointFromInitialPosition(Vector2Int initialPosition)
        {
            Slope slope = new Slope(initialPosition, _map.MapUnits);
            return slope.CalculatedSlope[^1];
        }

        private void CreateOverflow(WaterBody secondary, Vector2Int overflowFrom)
        {
            _overflows.Add(overflowFrom, secondary);
        }

        private void FindOverflowValley()
        {
            foreach (var shallows in ShallowPoints)
            {
                if (_overflows.ContainsKey(shallows))
                {
                    continue;
                }

                Vector2Int[] neighbors = MathUtil.GetNeighborPositionsIn2DArray(shallows, _map.SizeX, _map.SizeY);
                foreach (var neighbor in neighbors)
                {
                    if (_map.MapUnits[neighbor.x, neighbor.y].Position.Elevation < ShallowPointElevation)
                    {
                        Slope slope = new Slope(neighbor, _map.MapUnits);
                        WaterBody body = _map.GetBodyOfWaterByPosition(slope.CalculatedSlope[^1]);
                        
                        if(body == null)
                            body = new WaterBody(_map, neighbor, 0);
                        CreateOverflow(body, shallows);
                    }
                }
            }
        }
        
        public float GetCapacityBeforeResize()
        {
            Vector2Int firstExit = _bodyValley.CalculatedExits[0];
            float capHeightDiff = (_map.MapUnits[firstExit.x, firstExit.y].Position.Elevation -
                                   CurrentAbsoluteWaterLevel);
            return capHeightDiff * _bodyValley.CalculatedPositions.Length;
        }

        private void SteppedOverflow(ref float unassignedWaterVolume)
        {
            float fillPerStepAndOverflow = unassignedWaterVolume / (_overflows.Count * 100);
            while (unassignedWaterVolume > 0 && _overflows.Count > 0)
            {
                foreach (var overflow in _overflows)
                {
                    overflow.Value.AddVolume(fillPerStepAndOverflow);
                    unassignedWaterVolume -= fillPerStepAndOverflow;
                    if(overflow.Value.ShallowPoints.Intersect(ShallowPoints).Any() || 
                       overflow.Value.GetFeaturePositions().Intersect(GetFeaturePositions()).Any()){
                        _overflows.Remove(overflow.Key);
                        MergeBodiesOfWaterIntoFirst(this, overflow.Value);
                    }
                }
            }
        }

        private void Overflow(ref float unassignedWaterVolume)
        {
            float overflowVolume = 0;
            float[] caps = new float[_overflows.Count];
            int i = 0;
            foreach(var overflow in _overflows)
            {
                caps[i] = overflow.Value.GetCapacityBeforeResize();
                overflowVolume += caps[i];
                i++;
            }
                    
            if(overflowVolume >= unassignedWaterVolume)
            {
                i = 0;
                foreach (var overflow in _overflows)
                {
                    float fillWith = Math.Min(caps[i], unassignedWaterVolume / (i - _overflows.Count));
                    i++;
                    overflow.Value.AddVolume(fillWith);
                    unassignedWaterVolume -= fillWith;
                }
            }
            else
            {
                SteppedOverflow(ref unassignedWaterVolume);
            }
        }
        
        public void AddVolume(float volume)
        {
            float unassignedWaterVolume = volume;
            WaterVolume += volume;
            while (unassignedWaterVolume > 0)
            {
                float valleyCapLeft = GetCapacityBeforeResize();

                if (valleyCapLeft >= unassignedWaterVolume)
                {
                    float addedHeight = unassignedWaterVolume / _bodyValley.CalculatedPositions.Length;
                    CurrentAbsoluteWaterLevel += addedHeight;
                    
                    foreach(var vec in _bodyValley.CalculatedPositions)
                    {
                        float pointElevation = _map.MapUnits[vec.x, vec.y].Position.Elevation;
                        _map.MapUnits[vec.x, vec.y].WaterLevel = CurrentAbsoluteWaterLevel - pointElevation;
                    }
                    
                    break;
                } 
                
                FindOverflowValley();
                
                if (_overflows.Count > 0)
                {
                    Overflow(ref unassignedWaterVolume);
                }
                else
                {
                    _bodyValley = new Valley(ShallowPoints[0], _map.MapUnits); 
                    CurrentAbsoluteWaterLevel = _map.MapUnits[ShallowPoints[0].x, ShallowPoints[0].y].Position.Elevation;
                }
            }
        }

        public Vector2Int[] GetFeaturePositions()
        {
            return _bodyValley.CalculatedPositions;
        }
        
        public bool InBody(Vector2Int position)
        {
            return _bodyValley.CalculatedPositions.Contains(position);
        }
    }
}
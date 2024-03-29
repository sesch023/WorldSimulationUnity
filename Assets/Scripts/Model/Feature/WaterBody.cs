﻿using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Manager;
using Model.Map;
using Model.VirtualFeatureSelection;
using Unity.VisualScripting;
using Utils.BaseUtils;

namespace Model.Feature
{
    /// <summary>
    /// A complex water body that can be filled by a specific amount of water. It will create overflows if
    /// the water would in nature flow over the edge of the water body. It also supports merging with
    /// other water bodies. Does not support splitting or removing water.
    ///
    /// Currently there is still a bug where the water bodies multiply water volume when merging and overflowing.
    /// </summary>
    public class WaterBody : IBody
    {
        private Map.Map _map;
        private Valley _bodyValley;
        
        /// Shallow points of the water body.
        public Vector2Int[] ShallowPoints => _bodyValley.CalculatedExits;
        
        /// Elevations of the shallow points.
        public float ShallowPointElevation { get; private set; }
        
        /// Deep points of the water body.
        public Vector2Int DeepestPoint { get; private set; }

        /// Overflows of the water body.
        private Dictionary<Vector2Int, WaterBody> _overflows;
        
        /// Total volume of water in the water body.
        public float WaterVolume { get; set; }
        
        /// Absolut water level over the ground level.
        public float CurrentAbsoluteWaterLevel { get; set; }
        
        /// <summary>
        /// Merge the water body with another water body.
        /// </summary>
        /// <param name="body1">Body to merge into.</param>
        /// <param name="body2">Body that merges.</param>
        /// <returns>Merged body of water.</returns>
        public static WaterBody MergeBodiesOfWaterIntoFirst(WaterBody body1, WaterBody body2)
        {
            var shallowPointElevation = body1.ShallowPointElevation;
            var shallowPoints = body1.ShallowPoints;
            if (body1.ShallowPointElevation < body2.ShallowPointElevation)
            {
                shallowPointElevation = body2.ShallowPointElevation;
                shallowPoints = body2.ShallowPoints;
            }

            var shallowest = body1.ShallowPoints[0];
            foreach (var pt in body1.ShallowPoints)
            {
                if (body1._map.MapUnits[pt.x, pt.y].Position.Elevation > shallowPointElevation)
                {
                    
                    shallowPointElevation = body1._map.MapUnits[pt.x, pt.y].Position.Elevation;
                    shallowest = pt;
                }
            }
            
            foreach (var pt in body2.ShallowPoints)
            {
                if (body2._map.MapUnits[pt.x, pt.y].Position.Elevation > shallowPointElevation)
                {
                    shallowPointElevation = body2._map.MapUnits[pt.x, pt.y].Position.Elevation;
                    shallowest = pt;
                }
            }
            
            var deepestPoint = body1.DeepestPoint;
            var deepestElevation1 = body1._map.MapUnits[body1.DeepestPoint.x, body1.DeepestPoint.y].Position.Elevation;
            var deepestElevation2 = body2._map.MapUnits[body2.DeepestPoint.x, body2.DeepestPoint.y].Position.Elevation;
            
            if(deepestElevation2 < deepestElevation1)
                deepestPoint = body2.DeepestPoint;
            
            var waterVolume = body1.WaterVolume + body2.WaterVolume;

            body1._bodyValley = new Valley(shallowest, body1._map.MapUnits);
            body1.ShallowPointElevation = shallowPointElevation;
            body1.DeepestPoint = deepestPoint;
            body1.WaterVolume = waterVolume;

            body1._overflows.AddRange(body2._overflows.Where(ov => !body1._overflows.ContainsKey(ov.Key)));
            
            List<Vector2Int> pointsToRemove = new List<Vector2Int>();
            foreach (var overflow in body1._overflows)
            {
                if(overflow.Value == body1 || overflow.Value == body2)
                    pointsToRemove.Add(overflow.Key);
            }
            DictUtil.RemoveMultiFromDict(body1._overflows, pointsToRemove);

            if (body1._map == body2._map)
            {
                body1._map.RemoveBody(body2);
            }

            return body1;
        }
        
        private WaterBody(){}
        
        /// <summary>
        /// Create a new water body at the given position with the given volume on the given map.
        /// </summary>
        /// <param name="map">Map the body is created on.</param>
        /// <param name="initialPosition"></param>
        /// <param name="waterVolume"></param>
        public WaterBody(Map.Map map, Vector2Int initialPosition, float waterVolume)
        {
            _map = map;
            DeepestPoint = DeepestPointFromInitialPosition(initialPosition);
            _bodyValley = new Valley(DeepestPoint, _map.MapUnits);
            ShallowPointElevation = map.MapUnits[ShallowPoints[0].x, ShallowPoints[0].y].Position.Elevation;
            _overflows = new Dictionary<Vector2Int, WaterBody>();
            WaterVolume = 0;
            CurrentAbsoluteWaterLevel = map.MapUnits[DeepestPoint.x, DeepestPoint.y].Position.Elevation;
            map.AddBody(this);
            AddVolume(waterVolume);
        }

        /// <summary>
        /// Find the deepest point from the initial position without momentum.
        /// </summary>
        /// <param name="initialPosition">Position to find the deepest point from.</param>
        /// <returns>Deepest point</returns>
        private Vector2Int DeepestPointFromInitialPosition(Vector2Int initialPosition)
        {
            Slope slope = new Slope(initialPosition, _map.MapUnits, 0, 0);
            return slope.CalculatedSlope[^1];
        }

        /// <summary>
        /// Creates an overflow from the given point to the given water body.
        /// </summary>
        /// <param name="secondary">Overflow body.</param>
        /// <param name="overflowFrom">Position to overflow from.</param>
        private void CreateOverflow(WaterBody secondary, Vector2Int overflowFrom)
        {
            _overflows.Add(overflowFrom, secondary);
        }

        /// <summary>
        ///  Finds an overflow from the shallow points.
        /// </summary>
        private void FindOverflowValley()
        {
            foreach (var shallows in ShallowPoints)
            {
                // Already overflowing from this point.
                if (_overflows.ContainsKey(shallows))
                {
                    continue;
                }
                
                
                // Do any neighbors of the shallow point lead into a valley or different water body?
                Vector2Int[] neighbors = MathUtil.GetNeighborPositionsIn2DArray(shallows, _map.SizeX, _map.SizeY);
                foreach (var neighbor in neighbors)
                {
                    if (_map.MapUnits[neighbor.x, neighbor.y].Position.Elevation < ShallowPointElevation)
                    {
                        Slope slope = new Slope(neighbor, _map.MapUnits, 0, 0);
                        
                        // Does not lead into a different valley.
                        if(slope.CalculatedSlope[^1] == DeepestPoint)
                            continue;
                        
                        
                        
                        LoggingManager.GetInstance().LogDebug("Found overflow from " + shallows + " to " + slope.CalculatedSlope[^1]);
                        LoggingManager.GetInstance().LogDebug(this.GetHashCode());
                        
                        WaterBody body = _map.GetBodyOfWaterByPosition(slope.CalculatedSlope[^1]);
                        
                        if (body != this && !_bodyValley.CalculatedPositions.Contains(slope.CalculatedSlope[^1]))
                        {
                            if(body == null)
                                body = new WaterBody(_map, neighbor, 0);
                            LoggingManager.GetInstance().LogDebug(body.GetHashCode());
                            // For now we only allow one overflow per shallow point
                            CreateOverflow(body, shallows);
                            break;
                        }
                    }
                }
            }
        }
        
        /// <summary>
        /// Gets the capacity of the next resizing step.
        /// </summary>
        /// <returns></returns>
        public float GetCapacityBeforeResize()
        {
            Vector2Int firstExit = _bodyValley.CalculatedExits[0];
            float capHeightDiff = (_map.MapUnits[firstExit.x, firstExit.y].Position.Elevation -
                                   CurrentAbsoluteWaterLevel);
            return capHeightDiff * _bodyValley.CalculatedPositions.Length;
        }

        /// <summary>
        /// Step by step overflow from this body to the overflow bodies.
        /// </summary>
        /// <param name="unassignedWaterVolume"></param>
        private void SteppedOverflow(ref float unassignedWaterVolume)
        {
            LoggingManager.GetInstance().LogDebug("Stepped overflow");
            float fillPerStepAndOverflow = unassignedWaterVolume / (_overflows.Count * 10);
            bool invalidated = false;
            while ((!MathUtil.AlmostEquals(unassignedWaterVolume,0.0f, 0.5f)) && unassignedWaterVolume > 0
                   && _overflows.Count > 0 && !invalidated)
            {
                foreach (var overflow in new Dictionary<Vector2Int, WaterBody>(_overflows))
                {
                    overflow.Value.AddVolume(fillPerStepAndOverflow);
                    unassignedWaterVolume -= fillPerStepAndOverflow;
                    if(overflow.Value.ShallowPoints.Intersect(ShallowPoints).Any() || 
                       overflow.Value.GetFeaturePositions().Intersect(GetFeaturePositions()).Any()){
                        _overflows.Remove(overflow.Key);
                        MergeBodiesOfWaterIntoFirst(this, overflow.Value);
                        invalidated = true;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Overflow all unassigned water volume to the overflow bodies.
        /// </summary>
        /// <param name="unassignedWaterVolume"></param>
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
                    
            LoggingManager.GetInstance().LogDebug("Overflow volume: " + overflowVolume);
            
            if(overflowVolume >= unassignedWaterVolume)
            {
                LoggingManager.GetInstance().LogDebug("Normal overflow");
                i = 0;
                foreach (var overflow in _overflows)
                {
                    float fillWith = Math.Min(caps[i], unassignedWaterVolume / (i - _overflows.Count));
                    i++;
                    overflow.Value.AddVolume(fillWith);
                    unassignedWaterVolume -= fillWith;
                }

                unassignedWaterVolume = 0;
            }
            else
            {
                SteppedOverflow(ref unassignedWaterVolume);
            }
        }
        
        /// <summary>
        /// Adds water volume to the water body.
        /// </summary>
        /// <param name="volume">Amount of volume to add.</param>
        public void AddVolume(float volume)
        {
            float unassignedWaterVolume = volume;
            WaterVolume += volume;
            while ((!MathUtil.AlmostEquals(unassignedWaterVolume,0.0f, 0.5f)) && unassignedWaterVolume > 0)
            {
                float valleyCapLeft = GetCapacityBeforeResize();
                
                LoggingManager.GetInstance().LogDebug($"Body of Water {GetHashCode() % 1000} at: {DeepestPoint}");
                LoggingManager.GetInstance().LogDebug($"{valleyCapLeft} capacity left in valley");
                LoggingManager.GetInstance().LogDebug($"{unassignedWaterVolume} water volume left to assign");
                LoggingManager.GetInstance().LogDebug($"{_overflows.Count} overflows");
                
                foreach (var keyValuePair in _overflows)
                {
                    LoggingManager.GetInstance().LogDebug($"{keyValuePair}, {keyValuePair.Value.GetHashCode() % 1000}");
                }
                
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
                
                CurrentAbsoluteWaterLevel = ShallowPointElevation;
                foreach(var vec in _bodyValley.CalculatedPositions)
                {
                    float pointElevation = _map.MapUnits[vec.x, vec.y].Position.Elevation;
                    _map.MapUnits[vec.x, vec.y].WaterLevel = ShallowPointElevation - pointElevation;
                }
                unassignedWaterVolume -= valleyCapLeft;

                FindOverflowValley();

                if (_overflows.Count > 0)
                {
                    Overflow(ref unassignedWaterVolume);
                }
                else
                {
                    _bodyValley = new Valley(ShallowPoints[0], _map.MapUnits); 
                    ShallowPointElevation = _map.MapUnits[ShallowPoints[0].x, ShallowPoints[0].y].Position.Elevation;
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
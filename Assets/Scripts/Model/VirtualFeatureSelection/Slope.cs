using System.Collections.Generic;
using Model.Feature;
using UnityEngine;
using Utils.Arrays;
using Utils.BaseUtils;

namespace Model.VirtualFeatureSelection
{
    /// <summary>
    /// Finds a slope in a map or 2D arrayImmutable of floats.
    /// </summary>
    public class Slope : IFeature
    {
        /// Start point of the slope.
        private Vector2Int _start;
        /// Elevations of the map.
        private I2DArrayImmutable<float> _elevations;
        /// Momentum multiplier which changes the level of momentum the slope uses for passing local minimals.
        private float _momentumMultiplier;
        /// Maximum momentum to use in a single step. 
        private float _maxMomentumFraction;
        
        /// <summary>
        /// Positions of the slope. Sorted from highest to lowest elevation.
        /// </summary>
        public Vector2Int[] CalculatedSlope { get; private set; }

        /// <summary>
        /// Constructor for the slope.
        /// </summary>
        /// <param name="start">Start position of the slope.</param>
        /// <param name="mapUnits">Map of MapUnits.</param>
        /// <param name="momentumMultiplier">Momentum multiplier which changes the level of momentum the slope uses for passing local minimals.</param>
        /// <param name="maxMomentumFraction">Maximum momentum to use in a single step.</param>
        public Slope(Vector2Int start, MapUnit[,] mapUnits, float momentumMultiplier = 1.0f,
            float maxMomentumFraction = 1.0f)
        {
            ArrayImmutableMap2D<MapUnit, float> immutableMap = new ArrayImmutableMap2D<MapUnit, float>(mapUnits, unit => unit.Position.Elevation);
            Reset(start, immutableMap, momentumMultiplier, maxMomentumFraction);
        }
        
        /// <summary>
        /// Constructor for the slope.
        /// </summary>
        /// <param name="start">Start position of the slope.</param>
        /// <param name="elevations">2d arrayImmutable of floats a elevations.</param>
        /// <param name="momentumMultiplier">Momentum multiplier which changes the level of momentum the slope uses for passing local minimals.</param>
        /// <param name="maxMomentumFraction">Maximum momentum to use in a single step.</param>
        public Slope(Vector2Int start, I2DArrayImmutable<float> elevations, 
            float momentumMultiplier=1.0f, float maxMomentumFraction=1.0f)
        {
            Reset(start, elevations, momentumMultiplier, maxMomentumFraction);
        }
        
        private void Reset(Vector2Int start, I2DArrayImmutable<float> elevations, 
            float momentumMultiplier=1.0f, float maxMomentumFraction=1.0f)
        {
            _start = start;
            _elevations = elevations;
            _momentumMultiplier = momentumMultiplier;
            _maxMomentumFraction = maxMomentumFraction;

            CalculatedSlope = GetSlopeLine();
        }

        /// <summary>
        /// Context for the calculation of the slope.
        /// </summary>
        private class SlopeCalculationContext
        {
            public readonly List<Vector2Int> SlopeLine = new();
            public Vector2Int CurrentPos;
            public Vector2Int NextPos;
            public float NextElevation = float.PositiveInfinity;
            public float PreviousElevation = float.PositiveInfinity;
            public float MomentumLeft;
            public int MomentumPopCounter;
        }
        
        /// <summary>
        /// Calculates the slope line.
        /// </summary>
        /// <returns>Calculated slope line.</returns>
        private Vector2Int[] GetSlopeLine()
        {
            SlopeCalculationContext context = new SlopeCalculationContext();
            context.PreviousElevation = _elevations[_start.x, _start.y];
            context.SlopeLine.Add(_start);
            context.CurrentPos = _start;
            bool found;
            
            do
            {
                // Search for a new neighbor with the lowest elevation.
                if (!FindNextNeighbor(context))
                    break;

                context.CurrentPos = context.NextPos;
                // Search for a new neighbor with lower elevation or pass it with momentum if enough left.
                found = NextPointWithMomentum(context);
            } while (found);
            
            // Remove all tiles that were reached by momentum.
            if(context.MomentumPopCounter > 0)
                context.SlopeLine.RemoveRange(context.SlopeLine.Count - context.MomentumPopCounter, context.MomentumPopCounter);
            
            return context.SlopeLine.ToArray();
        }

        /// <summary>
        /// Checks if the next elevation is lower than the current one or if the slope has enough momentum to pass it.
        /// </summary>
        /// <param name="context">Context for the calculation.</param>
        /// <returns>True, if it found a next possible point.</returns>
        private bool NextPointWithMomentum(SlopeCalculationContext context)
        {
            bool found = false;
            float momentumTerm = context.MomentumLeft * _maxMomentumFraction;

            if ((context.NextElevation - momentumTerm) <= context.PreviousElevation)
            {
                CalculateNewMomentum(context, momentumTerm);

                found = true;
                context.SlopeLine.Add(context.CurrentPos);
                context.PreviousElevation = context.NextElevation;
            }

            return found;
        }

        /// <summary>
        /// Calculates the new momentum left.
        /// </summary>
        /// <param name="context">Context to calculate with.</param>
        /// <param name="momentumTerm">Term of momentum used, that was calculated before.</param>
        private void CalculateNewMomentum(SlopeCalculationContext context, float momentumTerm)
        {
            // Reset the momentum, if the next elevation is lower than the current one.
            if (context.NextElevation <= context.PreviousElevation)
            {
                context.MomentumLeft += Mathf.Abs((float.IsPositiveInfinity(context.PreviousElevation))
                    ? _maxMomentumFraction * context.NextElevation
                    : context.PreviousElevation - context.NextElevation);
                context.MomentumLeft *= _momentumMultiplier;
                context.MomentumPopCounter = 0;
            }
            else
            {
                context.MomentumLeft -= momentumTerm;
                context.MomentumPopCounter++;
            }
        }

        /// <summary>
        /// Finds a nightbor with the lowest elevation, that was not visited yet and does not cross the slope.
        /// </summary>
        /// <param name="context">Context to calculate with.</param>
        /// <returns>If a neighbor was found.</returns>
        private bool FindNextNeighbor(SlopeCalculationContext context)
        {
            bool foundNeighbor = false;
            Vector2Int[] neighbors = MathUtil.GetNeighborPositionsIn2DArray(context.CurrentPos,
                _elevations.GetLength(0), _elevations.GetLength(1));
            context.NextPos = neighbors[0];
            context.NextElevation = float.PositiveInfinity;

            foreach (var neighbor in neighbors)
            {
                float neightborElevation = _elevations[neighbor.x, neighbor.y];
                if (neightborElevation < context.NextElevation &&
                    !context.SlopeLine.Contains(neighbor) && !MathUtil.NextPointCrossesLineDiagonally(neighbor, context.SlopeLine))
                {
                    context.NextElevation = neightborElevation;
                    context.NextPos = neighbor;
                    foundNeighbor = true;
                }
            }

            return foundNeighbor;
        }

        public Vector2Int[] GetFeaturePositions()
        {
            return CalculatedSlope;
        }
    }
}
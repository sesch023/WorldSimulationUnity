using System.Collections.Generic;
using UnityEngine;
using Utils.Array2D;
using Utils.BaseUtils;

namespace Model.Map
{
    public class Slope
    {
        private Vector2Int _start;
        private I2DArray<float> _elevations;
        private float _momentumMultiplier;
        private float _maxMomentumFraction;
        
        public Vector2Int[] CalculatedSlope { get; private set; }

        public Slope(Vector2Int start, MapUnit[,] mapUnits, float momentumMultiplier = 1.0f,
            float maxMomentumFraction = 1.0f)
        {
            ArrayView2D<MapUnit, float> view = new ArrayView2D<MapUnit, float>(mapUnits, unit => unit.Position.Elevation);
            Reset(start, view, momentumMultiplier, maxMomentumFraction);
        }
        
        public Slope(Vector2Int start, I2DArray<float> elevations, 
            float momentumMultiplier=1.0f, float maxMomentumFraction=1.0f)
        {
            Reset(start, elevations, momentumMultiplier, maxMomentumFraction);
        }
        
        public void Reset(Vector2Int start, I2DArray<float> elevations, 
            float momentumMultiplier=1.0f, float maxMomentumFraction=1.0f)
        {
            _start = start;
            _elevations = elevations;
            _momentumMultiplier = momentumMultiplier;
            _maxMomentumFraction = maxMomentumFraction;

            CalculatedSlope = GetSlopeLine();
        }

        private class SlopeCalculationContext
        {
            public readonly List<Vector2Int> SlopeLine = new List<Vector2Int>();
            public Vector2Int CurrentPos;
            public Vector2Int NextPos;
            public float NextElevation = float.PositiveInfinity;
            public float PreviousElevation = float.PositiveInfinity;
            public float MomentumLeft = 0;
            public int MomentumPopCounter = 0;
        }
        
        private Vector2Int[] GetSlopeLine()
        {
            SlopeCalculationContext context = new SlopeCalculationContext();
            context.SlopeLine.Add(_start);
            context.CurrentPos = _start;
            bool found;
            
            do
            {
                if (!FindNextNeighbor(context))
                    break;

                context.CurrentPos = context.NextPos;
                found = NextPointWithMomentum(context);
            } while (found);
            
            if(context.MomentumPopCounter > 0)
                context.SlopeLine.RemoveRange(context.SlopeLine.Count - context.MomentumPopCounter, context.MomentumPopCounter);
            
            return context.SlopeLine.ToArray();
        }

        private bool NextPointWithMomentum(SlopeCalculationContext context)
        {
            bool found = false;
            float momentumTerm = context.MomentumLeft * _maxMomentumFraction;

            if ((context.NextElevation - context.MomentumLeft) <= context.PreviousElevation)
            {
                CalculateNewMomentum(context, momentumTerm);

                found = true;
                context.SlopeLine.Add(context.CurrentPos);
                context.PreviousElevation = context.NextElevation;
            }

            return found;
        }

        private void CalculateNewMomentum(SlopeCalculationContext context, float momentumTerm)
        {
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
    }
}
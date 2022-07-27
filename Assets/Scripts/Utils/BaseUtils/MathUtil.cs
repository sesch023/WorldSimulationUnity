using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Utils.BaseUtils
{
    public static class MathUtil
    {
        public class Range<T> where T : IComparable, IComparable<T>
        {
            private readonly T _start;
            private readonly T _end;
            
            public Range(T start, T end)
            {
                _start = start;
                _end = end;
            }

            public bool InRange(T check)
            {
                return check.CompareTo(_start) > 0 && check.CompareTo(_end) >= 0;
            }
        }
        
        public static bool AlmostEquals(double double1, double double2, double precision)
        {
            return (Math.Abs(double1 - double2) <= precision);
        }
        
        public static bool AlmostEquals(float float1, float float2, float precision)
        {
            return (Math.Abs(float1 - float2) <= precision);
        }
        
        public static bool IsPowerOfTwo(int x)
        {
            return (x != 0) && ((x & (x - 1)) == 0);
        }

        //https://answers.unity.com/questions/392606/line-drawing-how-can-i-interpolate-between-points.html
        public static Vector2[] ToBezierCurve(Vector2[] arrayToCurve, float smoothness=3.0f)
        {
            List<Vector2> points;
            List<Vector2> curvedPoints;
            int pointsLength = 0;
            int curvedLength = 0;
         
            if(smoothness < 1.0f) smoothness = 1.0f;
         
            pointsLength = arrayToCurve.Length;
         
            curvedLength = (pointsLength*Mathf.RoundToInt(smoothness))-1;
            curvedPoints = new List<Vector2>(curvedLength);
         
            float t = 0.0f;
            for(int pointInTimeOnCurve = 0;pointInTimeOnCurve < curvedLength+1;pointInTimeOnCurve++){
                t = Mathf.InverseLerp(0,curvedLength,pointInTimeOnCurve);
             
                points = new List<Vector2>(arrayToCurve);
             
                for(int j = pointsLength-1; j > 0; j--){
                    for (int i = 0; i < j; i++){
                        points[i] = (1-t)*points[i] + t*points[i+1];
                    }
                }
             
                curvedPoints.Add(points[0]);
            }
         
            return(curvedPoints.ToArray());
        }

        public static Vector2Int[] GetNeighborPositionsIn2DArray(Vector2Int position, int sizeX, int sizeY)
        {
            List<Vector2Int> vector2S = new List<Vector2Int>();
            
            foreach (int x in Enumerable.Range(Math.Max(position.x - 1, 0), Math.Min(3, sizeX - position.x)))
            {
                foreach (int y in Enumerable.Range(Math.Max(position.y - 1, 0), Math.Min(3, sizeY - position.y)))
                {
                    Vector2Int current = new Vector2Int(x, y);
                    if (current != position)
                    {
                        vector2S.Add(current);
                    }
                }
            }
            return vector2S.ToArray();
        }

        public static bool NextPointCrossesLineDiagonally(Vector2Int position, ICollection<Vector2Int> line)
        {
            Vector2Int lastPosition = line.Last();
            int xJump = position.x - lastPosition.x;
            int yJump = position.y - lastPosition.y;
            int jump = Math.Abs(xJump) + Math.Abs(yJump);

            if (jump < 2)
            {
                return false;
            }

            Vector2Int posX = new Vector2Int(lastPosition.x + xJump, lastPosition.y);
            Vector2Int posY = new Vector2Int(lastPosition.x, lastPosition.y + yJump);

            return line.Contains(posX) && line.Contains(posY);
        }

        public static bool At2DArrayBorder(Vector2Int position, int sizeX, int sizeY)
        {
            return position.x == 0 || position.y == 0 || position.x == sizeX || position.y == sizeY;
        }
    }
}
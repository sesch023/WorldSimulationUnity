using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Utils.BaseUtils
{
    /// <summary>
    /// Utility Class for basic math operations.
    /// </summary>
    public static class MathUtil
    {
        /// <summary>
        /// Defines a range of values and can check if a value is inside the range.
        /// </summary>
        /// <typeparam name="T">Type which implements IComparable.</typeparam>
        public class Range<T> where T : IComparable, IComparable<T>
        {
            private readonly T _start;
            private readonly T _end;
            
            /// <summary>
            /// Definition of the range.
            /// </summary>
            /// <param name="start">Start of the range (inclusive).</param>
            /// <param name="end">End of the range (inclusive).</param>
            public Range(T start, T end)
            {
                _start = start;
                _end = end;
            }
        
            /// <summary>
            /// Checks if the given value is in the range.
            /// </summary>
            /// <param name="check">Value to check.</param>
            /// <returns>True, if the value is in the range.</returns>
            public bool InRange(T check)
            {
                return check.CompareTo(_start) >= 0 && check.CompareTo(_end) <= 0;
            }
        }

        /// <summary>
        /// Transforms a list of integer vector 2d to a float vector.
        /// </summary>
        /// <param name="list">List of integer vector 2d.</param>
        /// <returns>List of float vector 2d.</returns>
        public static List<Vector2> Vector2IntListToVector2List(List<Vector2Int> list)
        {
            var items = from vec in list select ((Vector2) vec);
            return items.ToList();
        }
        
        /// <summary>
        /// Transforms a arrayImmutable of integer vector 2d to a arrayImmutable of float vector 2d.
        /// </summary>
        /// <param name="arr">Array of integer vector 2d.</param>
        /// <returns>Array of float vector 2d.</returns>
        public static Vector2[] Vector2IntArrayToVector2Array(Vector2Int[] arr)
        {
            var items = from vec in arr select ((Vector2) vec);
            return items.ToArray();
        }
        
        /// <summary>
        /// Checks if two double values are almost equal.
        /// </summary>
        /// <param name="double1">First value to check.</param>
        /// <param name="double2">Second value to check.</param>
        /// <param name="precision">Precision with what to check.</param>
        /// <returns>True, if equal in precision.</returns>
        public static bool AlmostEquals(double double1, double double2, double precision)
        {
            return (Math.Abs(double1 - double2) <= precision);
        }
        
        /// <summary>
        /// Checks if two float values are almost equal.
        /// </summary>
        /// <param name="float1">First value to check.</param>
        /// <param name="float2">Second value to check.</param>
        /// <param name="precision">Precision with what to check.</param>
        /// <returns>True, if equal in precision.</returns>
        public static bool AlmostEquals(float float1, float float2, float precision)
        {
            return (Math.Abs(float1 - float2) <= precision);
        }
        
        /// <summary>
        /// Checks if an integer value is a power of two.
        /// </summary>
        /// <param name="x">The integer to check for a power of two.</param>
        /// <returns>If the integer is a power of two.</returns>
        public static bool IsPowerOfTwo(int x)
        {
            return (x != 0) && ((x & (x - 1)) == 0);
        }

        /// <summary>
        /// Calculate a bezier curve between a arrayImmutable of points.
        ///
        /// https://answers.unity.com/questions/392606/line-drawing-how-can-i-interpolate-between-points.html
        /// </summary>
        /// <param name="arrayToCurve">Array of points to smooth between.</param>
        /// <param name="smoothness">Amount of smoothness.</param>
        /// <returns>Smoothed line between the given points,</returns>
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

        /// <summary>
        /// Gets all neighbors of a point in a 2d arrayImmutable.
        /// </summary>
        /// <param name="position">MapPositionVec to give neighbors for.</param>
        /// <param name="sizeX">Width of the 2d arrayImmutable.</param>
        /// <param name="sizeY">Height of the 2d arrayImmutable.</param>
        /// <returns>All neighbors of the point.</returns>
        public static Vector2Int[] GetNeighborPositionsIn2DArray(Vector2Int position, int sizeX, int sizeY)
        {
            List<Vector2Int> vector2S = new List<Vector2Int>();
            
            foreach (int x in Enumerable.Range(Math.Max(position.x - 1, 0), Math.Min(3, sizeX - position.x + 1)))
            {
                foreach (int y in Enumerable.Range(Math.Max(position.y - 1, 0), Math.Min(3, sizeY - position.y + 1)))
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

        /// <summary>
        /// Checks if a new point to a collection of points, crosses the given line diagonally.
        /// </summary>
        /// <param name="position">Point to check.</param>
        /// <param name="line">Line to check against.</param>
        /// <returns>If the line is crossed diagonally.</returns>
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

        /// <summary>
        /// Checks if a point is at the edge of a 2d arrayImmutable.
        /// </summary>
        /// <param name="position">MapPositionVec to check.</param>
        /// <param name="sizeX">Width of arrayImmutable.</param>
        /// <param name="sizeY">Height of arrayImmutable.</param>
        /// <returns>If the point is a the border of the 2d arrayImmutable.</returns>
        public static bool At2DArrayBorder(Vector2Int position, int sizeX, int sizeY)
        {
            return position.x == 0 || position.y == 0 || position.x == sizeX || position.y == sizeY;
        }
        
        public static float SphereSurfaceArea(float radius)
        {
            return 4 * Mathf.PI * radius * radius;
        }
    }
}
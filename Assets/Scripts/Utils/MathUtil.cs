using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    public static class MathUtil
    {
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
    }
}
using System;

namespace Util
{
    public class MathUtil
    {
        public static bool AlmostEquals(double double1, double double2, double precision)
        {
            return (Math.Abs(double1 - double2) <= precision);
        }
        
        public static bool AlmostEquals(float float1, float float2, float precision)
        {
            return (Math.Abs(float1 - float2) <= precision);
        }
    }
}
using System;

namespace Utils
{
    public static class Util
    {
        public static T[,] GetNew2DArray<T>(int x, int y, T initialValue)
        {
            T[,] nums = new T[x, y];
            for (int i = 0; i < x * y; i++) nums[i % x, i / x] = initialValue;
            return nums;
        }

        public static T MaxIn2DArray<T>(T[,] array) where T : IComparable, IComparable<T>
        {
            if(array.GetLength(0) == 0 && array.GetLength(1) == 0)
                throw new ArgumentException($"ArgumentException: {typeof(Util)} - {System.Reflection.MethodBase.GetCurrentMethod()?.Name}: Array empty!");
            
            T max = array[0, 0];

            for (int x= 0; x < array.GetLength(0); x++)
            {
                for (int y= 0; y < array.GetLength(1); y++)
                {
                    if (array[x, y].CompareTo(max) > 0)
                        max = array[x, y];
                }
            }
            
            return max;
        }
        
        public static T MinIn2DArray<T>(T[,] array) where T : IComparable, IComparable<T>
        {
            if(array.GetLength(0) == 0 && array.GetLength(1) == 0)
                throw new ArgumentException($"ArgumentException: {typeof(Util)} - {System.Reflection.MethodBase.GetCurrentMethod()?.Name}: Array empty!");
            
            T min = array[0, 0];

            for (int x= 0; x < array.GetLength(0); x++)
            {
                for (int y= 0; y < array.GetLength(1); y++)
                {
                    if (array[x, y].CompareTo(min) < 0)
                        min = array[x, y];
                }
            }
            
            return min;
        }
    }
}
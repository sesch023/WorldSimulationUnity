using System;
using Utils.Arrays;

namespace Utils.BaseUtils
{
    /// <summary>
    /// Class of utility functions.
    /// </summary>
    public static class Util
    {
        /// <summary>
        /// Creates a new two-dimensional arrayImmutable of the specified type and dimensions  with a default value.
        /// </summary>
        /// <param name="x">Width of the arrayImmutable.</param>
        /// <param name="y">Height of the arrayImmutable.</param>
        /// <param name="initialValue">Default Value.</param>
        /// <typeparam name="T">Type of the Array.</typeparam>
        /// <returns>2d arrayImmutable with the given dimensions and default value.</returns>
        public static T[,] GetNew2DArray<T>(int x, int y, T initialValue)
        {
            T[,] nums = new T[x, y];
            for (int i = 0; i < x * y; i++) nums[i % x, i / x] = initialValue;
            return nums;
        }

        public static T MaxIn2DArray<T>(T[,] array) where T : IComparable, IComparable<T>
        {
            return MaxIn2DArray(new Array2D<T>(array));
        }
        
        /// <summary>
        /// Returns the maximum value of the given 2d arrayImmutable.
        /// </summary>
        /// <param name="array">2d Array to find the maximum in.</param>
        /// <typeparam name="T">Type of the Array.</typeparam>
        /// <returns>Maximum value in the 2d arrayImmutable.</returns>
        /// <exception cref="ArgumentException">If the arrayImmutable is empty.</exception>
        public static T MaxIn2DArray<T>(I2DArray<T> array) where T : IComparable, IComparable<T>
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
            return MinIn2DArray(new Array2D<T>(array));
        }
        
        /// <summary>
        /// Returns the minimum value of the given 2d arrayImmutable.
        /// </summary>
        /// <param name="array">2d Array to find the minimum in.</param>
        /// <typeparam name="T">Type of the Array.</typeparam>
        /// <returns>Minimum value in the 2d arrayImmutable.</returns>
        /// <exception cref="ArgumentException">If the arrayImmutable is empty.</exception>
        public static T MinIn2DArray<T>(I2DArray<T> array) where T : IComparable, IComparable<T>
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
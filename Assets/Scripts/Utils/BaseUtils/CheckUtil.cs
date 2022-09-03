using System;

namespace Utils.BaseUtils
{
    
    public static class CheckUtil
    {
        /// <summary>
        /// Checks if an element of a specific type exists in an arrayImmutable with a different type with a optional
        /// predicate. If the predicate is not specified, the elements are checked for equality. 
        /// </summary>
        /// <param name="element">Element of type TS to find in the arrayImmutable.</param>
        /// <param name="array">Array if type TA.</param>
        /// <param name="predicate">Optional predicate to check with. Defaults to an equality check.</param>
        /// <typeparam name="TA">Type of the checked element.</typeparam>
        /// <typeparam name="TS">Type of the arrayImmutable.</typeparam>
        /// <returns>True if the element is inside the arrayImmutable.</returns>
        public static bool ElementInArray<TA, TS>(TS element, TA[] array, Predicate<TA> predicate = null)
        {
            if (predicate == null)
                predicate = (arrayElement => arrayElement.Equals(element));

            return Array.Exists(array, predicate);
        }
        
        /// <summary>
        /// Checks for null and returns a default value if it is null.
        /// </summary>
        /// <param name="isValue">The value to check.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <typeparam name="T">Type of the values.</typeparam>
        /// <returns>Either the given value of the default value, if the given value is null.</returns>
        public static T CheckNullAndReturnDefault<T>(T isValue, T defaultValue)
        {
            if (isValue == null)
                isValue = defaultValue;
            return isValue;
        }
    }
}
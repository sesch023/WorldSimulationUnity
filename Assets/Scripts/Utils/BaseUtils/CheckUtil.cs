using System;

namespace Utils.BaseUtils
{
    public static class CheckUtil
    {
        public static bool ElementInArray<TA, TS>(TS element, TA[] array, Predicate<TA> predicate = null)
        {
            if (predicate == null)
                predicate = (arrayElement => arrayElement.Equals(element));

            return Array.Exists(array, predicate);
        }
        
        public static T CheckNullAndReturnDefault<T>(T isValue, T defaultValue)
        {
            if (isValue == null)
                isValue = defaultValue;
            return isValue;
        }

        public static void CheckNullAndThrowException<T>(T isValue, Exception baseException)
        {
            if (isValue == null)
                throw baseException;
        }
    }
}
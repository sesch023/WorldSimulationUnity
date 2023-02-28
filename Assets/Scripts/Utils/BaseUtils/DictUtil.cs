using System.Collections.Generic;

namespace Utils.BaseUtils
{
    /// <summary>
    /// Utility class for working with dictionaries.
    /// </summary>
    public static class DictUtil
    {
        /// <summary>
        /// Removes multiple keys from a dictionary
        /// </summary>
        /// <param name="dict">Dictionary to remove from.</param>
        /// <param name="keys">Keys to remove</param>
        /// <typeparam name="TKey">Dictionary Key.</typeparam>
        /// <typeparam name="TValue">Dictionary Value.</typeparam>
        /// <returns>True, if all removed sucessfully.</returns>
        public static bool RemoveMultiFromDict<TKey, TValue>(IDictionary<TKey, TValue> dict, IEnumerable<TKey> keys)
        {
            bool allSuccess = true;
            foreach (var key in keys)
            {
                allSuccess &= dict.Remove(key);
            }
            return allSuccess;
        }
    }
}
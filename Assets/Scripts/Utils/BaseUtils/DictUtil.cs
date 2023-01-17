using System.Collections.Generic;

namespace Utils.BaseUtils
{
    public static class DictUtil
    {
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
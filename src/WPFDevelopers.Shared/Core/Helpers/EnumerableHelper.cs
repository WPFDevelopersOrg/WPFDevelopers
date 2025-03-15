using System.Collections;
using System.Collections.Generic;

namespace WPFDevelopers.Helpers
{
    public static class EnumerableHelper
    {
        public static int Count(this IEnumerable eunmerable)
        {
            if (eunmerable == null)
                return 0;

            var nSize = 0;
            foreach (var item in eunmerable)
                nSize++;

            return nSize;
        }

        public static TValue GetValueOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key,
            TValue defaultValue = default)
        {
            if (dictionary == null)
                return defaultValue;

            if (dictionary.TryGetValue(key, out var value))
                return value;
            return defaultValue;
        }
    }
}
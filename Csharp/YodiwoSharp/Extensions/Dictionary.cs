using System;
using System.Collections.Generic;
using System.Linq;


namespace Yodiwo
{
    public static partial class Extensions
    {
        //-----------------------------------------------------------------------------------------------------------------------------------------------------
        public static void AddFromSource<TKey, TValue>(this IDictionary<TKey, TValue> set, IEnumerable<KeyValuePair<TKey, TValue>> source)
        {
            if (source != null)
                foreach (var entry in source)
                    set.ForceAdd(entry.Key, entry.Value);
        }
        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public static TValue TryGetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey Key, TValue Default = default(TValue))
        {
            TValue val;
            DebugEx.Assert(Key != null, "Null key used in dictionary access");
            if (Key != null && dictionary.TryGetValue(Key, out val))
                return val;
            else
                return Default;
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public static TValue TryGetOrDefaultReadOnly<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, TKey Key, TValue Default = default(TValue))
        {
            TValue val;
            DebugEx.Assert(Key != null, "Null key used in dictionary access");
            if (Key != null && dictionary.TryGetValue(Key, out val))
                return val;
            else
                return Default;
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public static TValueOut TryGetOrDefault<TKey, TValue, TValueOut>(this IDictionary<TKey, TValue> dictionary, TKey Key, Func<TValue, TValueOut> Transform, TValueOut Default = default(TValueOut))
        {
            TValue val;
            DebugEx.Assert(Key != null, "Null key used in dictionary access");
            if (Key != null && dictionary.TryGetValue(Key, out val))
                return Transform(val);
            else
                return Default;
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public static Dictionary<TKey, TValue> Clone<TKey, TValue>(this IDictionary<TKey, TValue> dictionary)
        {
            var ret = new Dictionary<TKey, TValue>(capacity: dictionary.Count);
            try
            {
                foreach (var entry in dictionary)
                    ret.Add(entry.Key, entry.Value);
            }
            catch { }
            //return clone
            return ret;
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public static void ForceAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            DebugEx.Assert(key != null, "Null key used in dictionary access");
            if (key == null)
                return;
            try
            {
                if (dictionary.ContainsKey(key) == false)
                    dictionary.Add(key, value);
                else
                    dictionary[key] = value;
            }
            catch { }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public static bool TryAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value, bool overwrite = false)
        {
            DebugEx.Assert(key != null, "Null key used in dictionary access");
            if (key == null)
                return false;
            try
            {
                if (dictionary.ContainsKey(key) == false)
                {
                    dictionary.Add(key, value);
                    return true;
                }
                else
                {
                    if (!overwrite)
                        return false;
                    else
                    {
                        dictionary[key] = value;
                        return true;
                    }
                }
            }
            catch { return false; }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Removes all keys for which predicate returns true
        /// </summary>
        public static bool RemoveWhere<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, Func<KeyValuePair<TKey, TValue>, bool> Predicate)
        {
            bool ret = false;
            if (dictionary != null)
                foreach (var item in dictionary.Where(Predicate).ToArray())
                    ret |= dictionary.Remove(item.Key);
            return ret;
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Removes all keys that match input value
        /// </summary>
        /// <typeparam name="TKey">Type of dictionary's Key</typeparam>
        /// <typeparam name="TValue">Type of dictionary's Value</typeparam>
        /// <param name="dictionary">dictionary this extension applies to</param>
        /// <param name="value">Value to match against key to remove</param>
        /// <returns>True if any keys were removed; false otherwise</returns>
        public static bool RemoveAllKeysOfValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TValue value)
        {
            bool ret = false;
            try
            {
                foreach (var item in dictionary.Where(kvp => kvp.Value != null && kvp.Value.Equals(value)).ToArray())
                    ret |= dictionary.Remove(item.Key);
            }
            catch { }
            return ret;
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Removes only the first key that matchs input value
        /// </summary>
        /// <typeparam name="TKey">Type of dictionary's Key</typeparam>
        /// <typeparam name="TValue">Type of dictionary's Value</typeparam>
        /// <param name="dictionary">dictionary this extension applies to</param>
        /// <param name="value">Value to match against key to remove</param>
        /// <returns>True if a key was removed; false otherwise</returns>
        public static bool RemoveFirstKeyOfValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TValue value)
        {
            bool ret = false;
            try
            {
                var item = dictionary.First(kvp => kvp.Value != null && kvp.Value.Equals(value));
                ret = dictionary.Remove(item.Key);
            }
            catch { }
            return ret;
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Removes single key that matches input value. Throws exception if more than one is found
        /// </summary>
        /// <typeparam name="TKey">Type of dictionary's Key</typeparam>
        /// <typeparam name="TValue">Type of dictionary's Value</typeparam>
        /// <param name="dictionary">dictionary this extension applies to</param>
        /// <param name="value">Value to match against key to remove</param>
        /// <returns>True if any keys were removed; false otherwise</returns>
        /// <exception cref="IndexOutOfRangeException">If more than one key is found</exception>
        public static bool RemoveOnlyKeyOfValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TValue value)
        {
            bool ret = false;
            try
            {
                var list = dictionary.Where(kvp => kvp.Value != null && kvp.Value.Equals(value)).ToArray();
                if (list.Length > 1)
                    throw new IndexOutOfRangeException("More than one key found matching input value");
                dictionary.Remove(list.ElementAt(0).Key);
                ret = true;
            }
            catch { }
            return ret;
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public static IReadOnlyDictionary<TKey, TValue> AsReadOnly<TKey, TValue>(this IDictionary<TKey, TValue> dictionary)
        {
            return new ReadOnlyDictionary<TKey, TValue>(dictionary);
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------
    }
}


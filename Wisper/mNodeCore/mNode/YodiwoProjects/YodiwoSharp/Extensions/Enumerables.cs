using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yodiwo
{
    public static partial class Extensions
    {
        //----------------------------------------------------------------------------------------------------------------------------------------------
        public static ListTS<T> ToListTS<T>(this IEnumerable<T> source, ListTS<T> inPlace = null)
        {
            var ret = inPlace ?? new ListTS<T>();
            if (source != null)
                foreach (var entry in source)
                    ret.Add(entry);
            return ret;
        }
        //--------------------------------------------------------------------------------------------------------------------
        public static DictionaryTS<TKey, TValue> ToDictionaryTS<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> source, DictionaryTS<TKey, TValue> inPlace = null)
        {
            var ret = inPlace ?? new DictionaryTS<TKey, TValue>();
            if (source != null)
                ret.AddFromSource(source.WhereNotNull());
            return ret;
        }
        //--------------------------------------------------------------------------------------------------------------------
        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> source, Dictionary<TKey, TValue> inPlace = null)
        {
            var ret = inPlace ?? new Dictionary<TKey, TValue>();
            if (source != null)
                ret.AddFromSource(source.WhereNotNull());
            return ret;
        }
        //--------------------------------------------------------------------------------------------------------------------
        public static HashSetTS<T> ToHashSetTS<T>(this IEnumerable<T> source, HashSetTS<T> inPlace = null)
        {
            var ret = inPlace ?? new HashSetTS<T>();
            if (source != null)
                ret.AddFromSource(source.WhereNotNull());
            return ret;
        }
        //--------------------------------------------------------------------------------------------------------------------
        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source, HashSet<T> inPlace = null)
        {
            var ret = inPlace ?? new HashSet<T>();
            if (source != null)
                ret.AddFromSource(source.WhereNotNull());
            return ret;
        }
        //----------------------------------------------------------------------------------------------------------------------------------------------
        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source, IEqualityComparer<T> cmp, HashSet<T> inPlace = null)
        {
            var ret = inPlace ?? new HashSet<T>(cmp);
            if (source != null)
                ret.AddFromSource(source.WhereNotNull());
            return ret;
        }
        //----------------------------------------------------------------------------------------------------------------------------------------------
        public static Stack<T> ToStack<T>(this IEnumerable<T> source, Stack<T> inPlace = null)
        {
            var ret = inPlace ?? new Stack<T>();
            if (source != null)
                foreach (var entry in source)
                    ret.Push(entry);
            return ret;
        }
        //----------------------------------------------------------------------------------------------------------------------------------------------
        public static StackTS<T> ToStackTS<T>(this IEnumerable<T> source, StackTS<T> inPlace = null)
        {
            var ret = inPlace ?? new StackTS<T>();
            if (source != null)
                foreach (var entry in source)
                    ret.Push(entry);
            return ret;
        }
        //----------------------------------------------------------------------------------------------------------------------------------------------
        public static QueueTS<T> ToQueueTS<T>(this IEnumerable<T> source, QueueTS<T> inPlace = null)
        {
            var ret = inPlace ?? new QueueTS<T>();
            if (source != null)
                foreach (var entry in source)
                    ret.Enqueue(entry);
            return ret;
        }
        //----------------------------------------------------------------------------------------------------------------------------------------------
        public static Queue<T> ToQueue<T>(this IEnumerable<T> source, Queue<T> inPlace = null)
        {
            var ret = inPlace ?? new Queue<T>();
            if (source != null)
                foreach (var entry in source)
                    ret.Enqueue(entry);
            return ret;
        }
        //----------------------------------------------------------------------------------------------------------------------------------------------
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            if (source != null)
                foreach (var entry in source)
                    action(entry);
        }
        //----------------------------------------------------------------------------------------------------------------------------------------------
        public static void ForEachSafe<T>(this IEnumerable<T> source, Action<T> action, Func<T, Exception, bool> ExceptionHandler = null)
        {
            if (source != null)
                foreach (var entry in source)
                    try { action(entry); }
                    catch (Exception ex)
                    {
                        if (!(ExceptionHandler?.Invoke(entry, ex) ?? true))
                            break;
                    }
        }
        //----------------------------------------------------------------------------------------------------------------------------------------------
        public static int GetSequenceHashCode<T>(this IEnumerable<T> source)
        {
            int res = 0;
            if (source != null)
                unchecked
                {
                    foreach (var entry in source)
                        res = (res ^ entry.GetHashCode()) * 397;
                }
            return res;
        }
        //----------------------------------------------------------------------------------------------------------------------------------------------
        public static IEnumerable<T> As<T>(this IEnumerable source) where T : class
        {
            if (source != null)
                foreach (var entry in source)
                    if (entry is T) //filter out value that cannot be cast to
                        yield return entry as T;
        }
        //----------------------------------------------------------------------------------------------------------------------------------------------
        public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T> source)
        {
            if (source != null)
                foreach (var entry in source)
                    if (entry != null)
                        yield return entry;
        }
        //----------------------------------------------------------------------------------------------------------------------------------------------
        public static IEnumerable<T> SelectRange<T>(this IEnumerable<T> source, int StartIndex, int Length)
        {
            var i = 0;
            //yield range
            if (source != null)
                foreach (var entry in source)
                {
                    //check end
                    if (i >= StartIndex + Length)
                        break;
                    //pass start?
                    if (i >= StartIndex)
                        yield return entry;
                    //next
                    i++;
                }
        }
        //----------------------------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// will enumerate up to "limit" number of items and stop
        /// </summary>
        public static IEnumerable<T> EnumerateWithLimit<T>(this IEnumerable<T> source, int limit)
        {
            if (limit >= 0)
            {
                int i = 0;
                if (source != null)
                    foreach (var entry in source)
                    {
                        if (i < limit)
                            yield return entry;
                        else
                            break;
                        //inc counter
                        i++;
                    }
            }
        }
        //----------------------------------------------------------------------------------------------------------------------------------------------
        public static T MinItem<T>(this IEnumerable<T> source, Func<T, float> selector)
        {
            if (source != null)
            {
                T result = default(T);
                float min = float.MaxValue;
                foreach (var entry in source)
                {
                    var v = selector(entry);
                    if (v < min)
                    {
                        min = v;
                        result = entry;
                    }
                }
                return result;
            }
            else
                return default(T);
        }
        //----------------------------------------------------------------------------------------------------------------------------------------------
        public static T MaxItem<T>(this IEnumerable<T> source, Func<T, float> selector)
        {
            if (source != null)
            {
                T result = default(T);
                float max = float.MinValue;
                foreach (var entry in source)
                {
                    var v = selector(entry);
                    if (v > max)
                    {
                        max = v;
                        result = entry;
                    }
                }
                return result;
            }
            else
                return default(T);
        }
        //----------------------------------------------------------------------------------------------------------------------------------------------
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> source)
        {
            return source == null || !source.Any();
        }
        //----------------------------------------------------------------------------------------------------------------------------------------------
    }
}

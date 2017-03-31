using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo
{
    [Serializable]
    public class DictionaryOfSets<TKey, TValue> : IEnumerable<KeyValuePair<TKey, HashSetTS<TValue>>>, IReadOnlyDictionary<TKey, IReadOnlySet<TValue>>, IDictionary, IDictionary<TKey, HashSetTS<TValue>>
    {
        //------------------------------------------------------------------------------------------------------------------------
        [NonSerialized]
        protected object locker = new object();
        //------------------------------------------------------------------------------------------------------------------------
        [DB_UseDefaultDictionarySerializer]
        DictionaryTS<TKey, HashSetTS<TValue>> InternalObject = new DictionaryTS<TKey, HashSetTS<TValue>>();
        //------------------------------------------------------------------------------------------------------------------------
        [DoNotStoreInDB]
        public int Count { get { return InternalObject.Count; } }
        //------------------------------------------------------------------------------------------------------------------------
        [DoNotStoreInDB]
        public IEnumerable<TKey> Keys { get { return InternalObject.Keys; } }
        //------------------------------------------------------------------------------------------------------------------------
        [DoNotStoreInDB]
        public IEnumerable<IEnumerable<TValue>> Values { get { return InternalObject.Values; } }
        //------------------------------------------------------------------------------------------------------------------------
        public int CountForKey(TKey key)
        {
            var values = InternalObject.TryGetOrDefault(key);
            return values == null ? 0 : values.Count;
        }
        //------------------------------------------------------------------------------------------------------------------------
        public bool ContainsKey(TKey key)
        {
            return InternalObject.ContainsKey(key);
        }
        public HashSetTS<TValue> TryGet(TKey key)
        {
            lock (locker)
            {
                return InternalObject.TryGetOrDefault(key);
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        public bool Add(TKey key, TValue value)
        {
            lock (locker)
            {
                if (!InternalObject.ContainsKey(key))
                    InternalObject.Add(key, new HashSetTS<TValue>());

                return InternalObject[key].Add(value);
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        public bool RemoveKey(TKey key)
        {
            lock (locker)
                return InternalObject.Remove(key);
        }
        //------------------------------------------------------------------------------------------------------------------------
        public bool RemoveValueFromKey(TKey key, TValue value)
        {
            lock (locker)
            {
                if (!InternalObject.ContainsKey(key))
                    return false;

                var rc = InternalObject[key].Remove(value);

                if (InternalObject[key].Count == 0)
                    InternalObject.Remove(key);

                return rc;
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        public bool RemoveValueFromAll(TValue value)
        {
            bool rc = false;
            lock (locker)
            {
                foreach (var kv in InternalObject)
                {
                    rc |= InternalObject[kv.Key].Remove(value);

                    if (InternalObject[kv.Key].Count == 0)
                        InternalObject.Remove(kv.Key);
                }
            }
            return rc;
        }
        //------------------------------------------------------------------------------------------------------------------------
        public void Clear()
        {
            InternalObject.Clear();
        }
        //------------------------------------------------------------------------------------------------------------------------
        //implement IEnumerable
        //------------------------------------------------------------------------------------------------------------------------
        public IEnumerator<KeyValuePair<TKey, HashSetTS<TValue>>> GetEnumerator()
        {
            foreach (var kv in InternalObject)
                yield return kv;
        }
        //------------------------------------------------------------------------------------------------------------------------
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        //------------------------------------------------------------------------------------------------------------------------
        //implement IReadonlyDictionary
        //------------------------------------------------------------------------------------------------------------------------
        IEnumerable<IReadOnlySet<TValue>> IReadOnlyDictionary<TKey, IReadOnlySet<TValue>>.Values { get { return InternalObject.Values; } }

        ICollection IDictionary.Keys { get { return InternalObject.Keys.ToArray(); } }

        ICollection IDictionary.Values { get { return InternalObject.Values.ToArray(); } }

        public bool IsReadOnly { get { return false; } }

        public bool IsFixedSize { get { return false; } }

        public object SyncRoot { get { return null; } }

        public bool IsSynchronized { get { return true; } }

        public object this[object key]
        {
            get
            {
                return this[(TKey)key];
            }
            set
            {
                InternalObject[(TKey)key] = (HashSetTS<TValue>)value;
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        public IReadOnlySet<TValue> this[TKey key] { get { return InternalObject.TryGetOrDefaultReadOnly(key); } }
        //------------------------------------------------------------------------------------------------------------------------
        public bool TryGetValue(TKey key, out IReadOnlySet<TValue> value)
        {
            HashSetTS<TValue> val_ts;
            var rc = InternalObject.TryGetValue(key, out val_ts);
            value = val_ts;
            return rc;
        }
        //------------------------------------------------------------------------------------------------------------------------
        IEnumerator<KeyValuePair<TKey, IReadOnlySet<TValue>>> IEnumerable<KeyValuePair<TKey, IReadOnlySet<TValue>>>.GetEnumerator()
        {
            foreach (var kv in InternalObject)
                yield return new KeyValuePair<TKey, IReadOnlySet<TValue>>(kv.Key, kv.Value);
        }
        //------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------
        // IDictionary
        //------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------
        public bool Contains(object key)
        {
            return ContainsKey((TKey)key);
        }
        //------------------------------------------------------------------------------------------------------------------------
        public void Add(object key, object value)
        {
            Add((TKey)key, (HashSetTS<TValue>)value);
        }
        //------------------------------------------------------------------------------------------------------------------------
        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            lock (locker)
                if (InternalObject == null)
                    return (new Dictionary<TKey, TValue>() as IDictionary).GetEnumerator();
                else
                    return (InternalObject.Clone() as IDictionary).GetEnumerator();
        }
        //------------------------------------------------------------------------------------------------------------------------
        public void Remove(object key)
        {
            RemoveKey((TKey)key);
        }
        //------------------------------------------------------------------------------------------------------------------------
        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }
        //------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------
        // IDictionary<TKey, HashSetTS<TValue>>
        //------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------
        ICollection<TKey> IDictionary<TKey, HashSetTS<TValue>>.Keys
        {
            get
            {
                return Keys.ToArray();
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        ICollection<HashSetTS<TValue>> IDictionary<TKey, HashSetTS<TValue>>.Values
        {
            get
            {
                return InternalObject.Values;
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        HashSetTS<TValue> IDictionary<TKey, HashSetTS<TValue>>.this[TKey key]
        {
            get
            {
                lock (locker)
                    return InternalObject[key];
            }

            set
            {
                lock (locker)
                    InternalObject[key] = value;
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        public void Add(TKey key, HashSetTS<TValue> value)
        {
            lock (locker)
                InternalObject.Add(key, value);
        }
        //------------------------------------------------------------------------------------------------------------------------
        public bool Remove(TKey key)
        {
            lock (locker)
                return InternalObject.Remove(key);
        }
        //------------------------------------------------------------------------------------------------------------------------
        public bool TryGetValue(TKey key, out HashSetTS<TValue> value)
        {
            lock (locker)
                return InternalObject.TryGetValue(key, out value);
        }
        //------------------------------------------------------------------------------------------------------------------------
        public void Add(KeyValuePair<TKey, HashSetTS<TValue>> item)
        {
            lock (locker)
                InternalObject.Add(item.Key, item.Value);
        }
        //------------------------------------------------------------------------------------------------------------------------
        public bool Remove(KeyValuePair<TKey, HashSetTS<TValue>> item)
        {
            lock (locker)
                return InternalObject.Remove(item);
        }
        //------------------------------------------------------------------------------------------------------------------------
        public bool Contains(KeyValuePair<TKey, HashSetTS<TValue>> item)
        {
            lock (locker)
                return InternalObject.Contains(item);
        }
        //------------------------------------------------------------------------------------------------------------------------
        public void CopyTo(KeyValuePair<TKey, HashSetTS<TValue>>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }
        //------------------------------------------------------------------------------------------------------------------------
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo
{
    [Serializable]
    public class DictionaryOfLists<TKey, TValue> : IEnumerable<KeyValuePair<TKey, ListTS<TValue>>>, IReadOnlyDictionary<TKey, IReadOnlyCollection<TValue>>, IDictionary, IDictionary<TKey, ListTS<TValue>>
    {
        //------------------------------------------------------------------------------------------------------------------------
        [NonSerialized]
        protected object locker = new object();
        //------------------------------------------------------------------------------------------------------------------------
        [DB_UseDefaultDictionarySerializer]
        DictionaryTS<TKey, ListTS<TValue>> InternalObject = new DictionaryTS<TKey, ListTS<TValue>>();
        //------------------------------------------------------------------------------------------------------------------------
        public int Count { get { return InternalObject.Count; } }
        //------------------------------------------------------------------------------------------------------------------------
        public IEnumerable<TKey> Keys { get { return InternalObject.Keys; } }
        //------------------------------------------------------------------------------------------------------------------------
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
        public ListTS<TValue> TryGet(TKey key)
        {
            lock (locker)
            {
                return InternalObject.TryGetOrDefault(key);
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        public void Add(TKey key, TValue value)
        {
            lock (locker)
            {
                if (!InternalObject.ContainsKey(key))
                    InternalObject.Add(key, new ListTS<TValue>());

                InternalObject[key].Add(value);
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
        public IEnumerator<KeyValuePair<TKey, ListTS<TValue>>> GetEnumerator()
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
        //------------------------------------------------------------------------------------------------------------------------
        //implement IReadonlyDictionary
        //------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------
        IEnumerable<IReadOnlyCollection<TValue>> IReadOnlyDictionary<TKey, IReadOnlyCollection<TValue>>.Values
        {
            get
            {
                return InternalObject.Values;
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        public IReadOnlyCollection<TValue> this[TKey key]
        {
            get
            {
                return InternalObject.TryGetOrDefault(key);
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        public bool TryGetValue(TKey key, out IReadOnlyCollection<TValue> value)
        {
            ListTS<TValue> val_ts;
            var rc = InternalObject.TryGetValue(key, out val_ts);
            value = val_ts;
            return rc;
        }
        //------------------------------------------------------------------------------------------------------------------------
        IEnumerator<KeyValuePair<TKey, IReadOnlyCollection<TValue>>> IEnumerable<KeyValuePair<TKey, IReadOnlyCollection<TValue>>>.GetEnumerator()
        {
            foreach (var kv in InternalObject)
                yield return new KeyValuePair<TKey, IReadOnlyCollection<TValue>>(kv.Key, kv.Value);
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
            Add((TKey)key, (TValue)value);
        }
        //------------------------------------------------------------------------------------------------------------------------
        public object this[object key]
        {
            get
            {
                return InternalObject[(TKey)key];
            }

            set
            {
                InternalObject[(TKey)key] = (ListTS<TValue>)value;
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            lock (locker)
                if (InternalObject == null)
                    return (new Dictionary<TKey, TValue>() as System.Collections.IDictionary).GetEnumerator();
                else
                    return (InternalObject.Clone() as System.Collections.IDictionary).GetEnumerator();
        }
        //------------------------------------------------------------------------------------------------------------------------
        public void Remove(object key)
        {
            Remove((TKey)key);
        }
        //------------------------------------------------------------------------------------------------------------------------
        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }
        //------------------------------------------------------------------------------------------------------------------------
        ICollection IDictionary.Keys { get { return Keys.ToArray(); } }
        //------------------------------------------------------------------------------------------------------------------------
        ICollection IDictionary.Values { get { return Values.ToArray(); } }
        //------------------------------------------------------------------------------------------------------------------------
        public bool IsReadOnly { get { return false; } }
        //------------------------------------------------------------------------------------------------------------------------
        public bool IsFixedSize { get { return false; } }
        //------------------------------------------------------------------------------------------------------------------------
        public object SyncRoot { get { return null; } }
        //------------------------------------------------------------------------------------------------------------------------
        public bool IsSynchronized { get { return true; } }
        //------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------
        // IDictionary<TKey, ListTS<TValue>>
        //------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------
        ICollection<TKey> IDictionary<TKey, ListTS<TValue>>.Keys
        {
            get
            {
                lock (locker)
                    return InternalObject.Keys;
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        ICollection<ListTS<TValue>> IDictionary<TKey, ListTS<TValue>>.Values
        {
            get
            {
                lock (locker)
                    return InternalObject.Values;
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        ListTS<TValue> IDictionary<TKey, ListTS<TValue>>.this[TKey key]
        {
            get
            {
                return InternalObject[key];
            }
            set
            {
                InternalObject[key] = value;
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        public void Add(TKey key, ListTS<TValue> value)
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
        public bool TryGetValue(TKey key, out ListTS<TValue> value)
        {
            lock (locker)
                return InternalObject.TryGetValue(key, out value);
        }
        //------------------------------------------------------------------------------------------------------------------------
        public void Add(KeyValuePair<TKey, ListTS<TValue>> item)
        {
            lock (locker)
                InternalObject.Add(item);
        }
        //------------------------------------------------------------------------------------------------------------------------
        public bool Contains(KeyValuePair<TKey, ListTS<TValue>> item)
        {
            lock (locker)
                return InternalObject.Contains(item);
        }
        //------------------------------------------------------------------------------------------------------------------------
        public void CopyTo(KeyValuePair<TKey, ListTS<TValue>>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }
        //------------------------------------------------------------------------------------------------------------------------
        public bool Remove(KeyValuePair<TKey, ListTS<TValue>> item)
        {
            lock (locker)
                return InternalObject.Remove(item);
        }
    }
}
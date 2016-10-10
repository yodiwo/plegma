using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo
{
    [Serializable]
    public class DictionaryTS<TKey, TValue> : IDictionary<TKey, TValue>, System.Collections.IDictionary, IReadOnlyDictionary<TKey, TValue>
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        protected object locker = new object();
        //------------------------------------------------------------------------------------------------------------------------
        protected Dictionary<TKey, TValue> InternalObject = null;
        //------------------------------------------------------------------------------------------------------------------------
        protected uint revision = 0;
        [NonSerialized]
        protected uint cached_revision = 0;
        //------------------------------------------------------------------------------------------------------------------------
        public uint Revision { get { return revision; } }
        //------------------------------------------------------------------------------------------------------------------------
        protected static ICollection<TKey> empty_Keys = new TKey[0];
        protected static ICollection<TValue> empty_Values = new TValue[0];
        protected static ICollection<KeyValuePair<TKey, TValue>> empty_KeyValue = new KeyValuePair<TKey, TValue>[0];
        [NonSerialized]
        protected ICollection<TKey> cached_Keys = empty_Keys;
        [NonSerialized]
        protected ICollection<TValue> cached_Values = empty_Values;
        [NonSerialized]
        protected ICollection<KeyValuePair<TKey, TValue>> cached_KeyValue = empty_KeyValue;
        //------------------------------------------------------------------------------------------------------------------------
        #endregion


        #region Constructors
        //------------------------------------------------------------------------------------------------------------------------
        public DictionaryTS()
        {
        }
        //------------------------------------------------------------------------------------------------------------------------
        public DictionaryTS(IEnumerable<KeyValuePair<TKey, TValue>> source)
        {
            InternalObject = new Dictionary<TKey, TValue>();
            if (source != null)
                foreach (var entry in source)
                    InternalObject.Add(entry.Key, entry.Value);
            //increase revision if we already have items
            if (InternalObject.Count > 0)
                revision++;
            else
                InternalObject = null;
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion


        #region Functions
        //------------------------------------------------------------------------------------------------------------------------
        protected void RebuildCachedCollections()
        {
            lock (locker)
            {
                if (cached_revision != revision)
                {
                    //update revision
                    cached_revision = revision;
                    //rebuild cached sets
                    cached_Keys = InternalObject == null || InternalObject.Count == 0 ? empty_Keys : InternalObject.Keys.ToArray();
                    cached_Values = InternalObject == null || InternalObject.Count == 0 ? empty_Values : InternalObject.Values.ToArray();
                    cached_KeyValue = InternalObject == null || InternalObject.Count == 0 ? empty_KeyValue : InternalObject.ToArray();
                }
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        protected virtual void IncreaseRevision()
        {
            //increase collection revision number
            revision++;
            //never Zero for revision helps with deserialization, since cached_revision will default to 0
            if (revision == 0)
                revision = 1;
            //let go of cached references
            cached_Keys = empty_Keys;
            cached_Values = empty_Values;
            cached_KeyValue = empty_KeyValue;
        }
        //------------------------------------------------------------------------------------------------------------------------
        public virtual void Add(TKey key, TValue value)
        {
            DebugEx.Assert(key != null, "Null key used in dictionary access");
            if (key == null)
                return;

            //perform operation
            lock (locker)
            {
                IncreaseRevision();
                if (InternalObject == null)
                    InternalObject = new Dictionary<TKey, TValue>();
                //add
                if (InternalObject.ContainsKey(key))
                    InternalObject[key] = value;
                else
                    InternalObject.Add(key, value);
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        public virtual void AddFromSource(IEnumerable<KeyValuePair<TKey, TValue>> source)
        {
            if (source == null)
                return;

            //perform operation
            lock (locker)
            {
                IncreaseRevision();
                if (InternalObject == null)
                    InternalObject = new Dictionary<TKey, TValue>();
                foreach (var entry in source)
                {
                    if (InternalObject.ContainsKey(entry.Key))
                        InternalObject[entry.Key] = entry.Value;
                    else
                        InternalObject.ForceAdd(entry.Key, entry.Value);
                }
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        public virtual bool ContainsKey(TKey key)
        {
            DebugEx.Assert(key != null, "Null key used in dictionary access");
            if (key == null)
                return false;

            //perform operation
            lock (locker)
                if (InternalObject == null)
                    return false;
                else
                    return InternalObject.ContainsKey(key);
        }
        //------------------------------------------------------------------------------------------------------------------------
        public virtual bool ContainsValue(TValue value)
        {
            lock (locker)
                if (InternalObject == null)
                    return false;
                else
                    return InternalObject.Values.Contains(value);
        }
        //------------------------------------------------------------------------------------------------------------------------
        public virtual ICollection<TKey> Keys
        {
            get
            {
                lock (locker)
                {
                    RebuildCachedCollections();
                    return cached_Keys == null ? empty_Keys : cached_Keys;
                }
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        public virtual bool Remove(TKey key)
        {
            DebugEx.Assert(key != null, "Null key used in dictionary access");
            if (key == null)
                return false;

            //perform operation
            lock (locker)
            {
                if (InternalObject != null && InternalObject.Remove(key))
                {
                    IncreaseRevision();
                    if (InternalObject.Count == 0)
                        InternalObject = null;
                    return true;
                }
                else
                    return false;
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        public virtual bool TryGetValue(TKey key, out TValue value)
        {
            lock (locker)
                if (InternalObject == null)
                {
                    value = default(TValue);
                    return false;
                }
                else
                    return InternalObject.TryGetValue(key, out value);
        }
        //------------------------------------------------------------------------------------------------------------------------
        public virtual ICollection<TValue> Values
        {
            get
            {
                lock (locker)
                {
                    RebuildCachedCollections();
                    return cached_Values == null ? empty_Values : cached_Values;
                }
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        public virtual TValue this[TKey key]
        {
            get
            {
                lock (locker)
                    if (InternalObject == null)
                        throw new Exception("Collection is Empty");
                    else
                        return InternalObject[key];
            }
            set
            {
                lock (locker)
                {
                    IncreaseRevision();
                    if (InternalObject == null)
                    {
                        InternalObject = new Dictionary<TKey, TValue>();
                        InternalObject.Add(key, value);
                    }
                    else
                        InternalObject[key] = value;
                }
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        public virtual void Add(KeyValuePair<TKey, TValue> item)
        {
            lock (locker)
            {
                IncreaseRevision();
                if (InternalObject == null)
                    InternalObject = new Dictionary<TKey, TValue>();
                //add
                if (InternalObject.ContainsKey(item.Key))
                    InternalObject[item.Key] = item.Value;
                else
                    InternalObject.Add(item.Key, item.Value);
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        public virtual void Clear()
        {
            lock (locker)
                if (InternalObject != null)
                {
                    if (InternalObject.Count > 0)
                    {
                        IncreaseRevision();
                        InternalObject.Clear();
                        RebuildCachedCollections();
                    }
                    InternalObject = null;
                }
        }
        //------------------------------------------------------------------------------------------------------------------------
        public virtual bool Contains(KeyValuePair<TKey, TValue> item)
        {
            lock (locker)
                if (InternalObject == null)
                    return false;
                else
                    return InternalObject.Contains(item);
        }
        //------------------------------------------------------------------------------------------------------------------------
        public virtual void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            lock (locker)
            {
                int i = 0;
                if (InternalObject != null)
                    foreach (var entry in InternalObject)
                    {
                        array[i + arrayIndex] = entry;
                        i++;
                    }
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        public virtual void CopyTo(IDictionary<TKey, TValue> set)
        {
            foreach (var entry in this)
                set.Add(entry);
        }
        //------------------------------------------------------------------------------------------------------------------------
        public virtual int Count
        {
            get { lock (locker) return InternalObject == null ? 0 : InternalObject.Count; }
        }
        //------------------------------------------------------------------------------------------------------------------------
        public virtual bool IsReadOnly
        {
            get { return false; }
        }
        //------------------------------------------------------------------------------------------------------------------------
        public virtual bool Remove(KeyValuePair<TKey, TValue> item)
        {
            lock (locker)
            {
                if (InternalObject != null && InternalObject.Remove(item.Key))
                {
                    IncreaseRevision();
                    if (InternalObject.Count == 0)
                        InternalObject = null;
                    return true;
                }
                else
                    return false;
            }
        }
        //-----------------------------------------------------------------------------------------------------------------------------------------------------
        public virtual void ForceAdd(TKey key, TValue value)
        {
            DebugEx.Assert(key != null, "Null key used in dictionary access");
            if (key == null)
                return;

            //perform operation
            lock (locker)
            {
                IncreaseRevision();
                if (InternalObject == null)
                {
                    InternalObject = new Dictionary<TKey, TValue>();
                    InternalObject.Add(key, value);
                }
                else if (InternalObject.ContainsKey(key) == false)
                    InternalObject.Add(key, value);
                else
                    InternalObject[key] = value;
            }
        }
        //-----------------------------------------------------------------------------------------------------------------------------------------------------
        public virtual bool TryAdd(TKey key, TValue value, bool overwrite = false)
        {
            DebugEx.Assert(key != null, "Null key used in dictionary access");
            if (key == null)
                return false;

            //perform operation
            lock (locker)
            {
                IncreaseRevision();

                if (InternalObject == null)
                {
                    InternalObject = new Dictionary<TKey, TValue>();
                    InternalObject.Add(key, value);
                    return true;
                }
                else if (InternalObject.ContainsKey(key) == false)
                {
                    InternalObject.Add(key, value);
                    return true;
                }
                else
                {
                    if (!overwrite)
                        return false;
                    else
                    {
                        InternalObject[key] = value;
                        return true;
                    }
                }
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys { get { return Keys; } }
        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values { get { return Values; } }
        //------------------------------------------------------------------------------------------------------------------------
        public virtual IEnumerator<KeyValuePair<TKey, TValue>> EnumerateLocked()
        {
            lock (locker)
                if (InternalObject != null)
                    foreach (var entry in InternalObject)
                        yield return entry;
        }
        //------------------------------------------------------------------------------------------------------------------------
        public TValue TryGetAndRemove(TKey key)
        {
            lock (locker)
            {
                if (InternalObject == null)
                    return default(TValue);
                else
                {
                    var ret = InternalObject.TryGetOrDefault(key);
                    Remove(key);
                    return ret;
                }
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        public IEnumerable<KeyValuePair<TKey, TValue>> GetAndClear()
        {
            lock (locker)
            {
                RebuildCachedCollections();
                var cached = cached_KeyValue == null ? empty_KeyValue : cached_KeyValue;
                Clear();
                return cached ?? Enumerable.Empty<KeyValuePair<TKey, TValue>>();
            }
        }
        //-----------------------------------------------------------------------------------------------------------------------------------------------------
        public DictionaryTS<TKey, TValue> Clone()
        {
            lock (locker)
                if (InternalObject == null)
                    return new DictionaryTS<TKey, TValue>();
                else
                    return new DictionaryTS<TKey, TValue>(InternalObject);
        }
        //------------------------------------------------------------------------------------------------------------------------
        public virtual IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            lock (locker)
            {
                RebuildCachedCollections();
                return (cached_KeyValue == null ? empty_KeyValue : cached_KeyValue).GetEnumerator();
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            lock (locker)
            {
                RebuildCachedCollections();
                return (cached_KeyValue == null ? empty_KeyValue : cached_KeyValue).GetEnumerator();
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion


        #region System.Collections.IDictionary

        void System.Collections.IDictionary.Add(object key, object value)
        {
            ForceAdd((TKey)key, (TValue)value);
        }

        void System.Collections.IDictionary.Clear()
        {
            Clear();
        }

        bool System.Collections.IDictionary.Contains(object key)
        {
            return ContainsKey((TKey)key);
        }

        System.Collections.IDictionaryEnumerator System.Collections.IDictionary.GetEnumerator()
        {
            lock (locker)
                if (InternalObject == null)
                    return (new Dictionary<TKey, TValue>() as System.Collections.IDictionary).GetEnumerator();
                else
                    return (InternalObject.Clone() as System.Collections.IDictionary).GetEnumerator();
        }

        bool System.Collections.IDictionary.IsFixedSize
        {
            get { return false; }
        }

        bool System.Collections.IDictionary.IsReadOnly
        {
            get { return false; }
        }

        System.Collections.ICollection System.Collections.IDictionary.Keys
        {
            get { return Keys.ToArray(); }
        }

        void System.Collections.IDictionary.Remove(object key)
        {
            Remove((TKey)key);
        }

        System.Collections.ICollection System.Collections.IDictionary.Values
        {
            get { return Values.ToArray(); }
        }

        object System.Collections.IDictionary.this[object key]
        {
            get
            {
                return this[(TKey)key];
            }
            set
            {
                this[(TKey)key] = (TValue)value;
            }
        }

        void System.Collections.ICollection.CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        int System.Collections.ICollection.Count
        {
            get { return Count; }
        }

        bool System.Collections.ICollection.IsSynchronized
        {
            get { return true; }
        }

        object System.Collections.ICollection.SyncRoot
        {
            get { return null; }
        }
        #endregion
    }
}

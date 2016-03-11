using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo
{
    [Serializable]
    public class ListTS<T> : IList<T>, IReadOnlyList<T>
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        protected List<T> InternalObject = new List<T>();
        //------------------------------------------------------------------------------------------------------------------------
        protected uint revision = 0;
        [NonSerialized]
        protected uint cached_revision = 0;
        //------------------------------------------------------------------------------------------------------------------------
        public uint Revision { get { return revision; } }
        //------------------------------------------------------------------------------------------------------------------------
        protected static ICollection<T> empty = new T[0];
        [NonSerialized]
        protected ICollection<T> cached_Values = empty;
        //------------------------------------------------------------------------------------------------------------------------
        private ReadOnlyList<T> cached_ReadOnlyWrapper = null;
        //------------------------------------------------------------------------------------------------------------------------
        #endregion


        #region Constructors
        //------------------------------------------------------------------------------------------------------------------------
        public ListTS()
        {
        }
        //------------------------------------------------------------------------------------------------------------------------
        public ListTS(IEnumerable<T> source)
        {
            InternalObject = source != null ? new List<T>(source) : new List<T>();
            //increase revision if we already have items
            if (InternalObject.Count > 0)
                revision++;
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion


        #region Functions
        //------------------------------------------------------------------------------------------------------------------------
        void RebuildCachedCollections()
        {
            lock (InternalObject)
            {
                if (cached_revision != revision)
                {
                    //update revision
                    cached_revision = revision;
                    //rebuild cached sets
                    cached_Values = InternalObject.Count == 0 ? empty : InternalObject.ToArray();
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
            cached_Values = empty;
        }
        //------------------------------------------------------------------------------------------------------------------------        
        public virtual int IndexOf(T item)
        {
            lock (InternalObject)
                return InternalObject.IndexOf(item);
        }
        //------------------------------------------------------------------------------------------------------------------------
        public virtual void AddRange(IEnumerable<T> source)
        {
            if (source != null)
                foreach (var entry in source)
                    Add(entry);
        }
        //------------------------------------------------------------------------------------------------------------------------
        public virtual void AddRangeLocked(IEnumerable<T> source)
        {
            if (source != null)
                lock (InternalObject)
                {
                    IncreaseRevision();
                    InternalObject.AddRange(source);
                }
        }
        //------------------------------------------------------------------------------------------------------------------------
        public virtual void Insert(int index, T item)
        {
            lock (InternalObject)
            {
                IncreaseRevision();
                InternalObject.Insert(index, item);
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        public virtual void RemoveAt(int index)
        {
            lock (InternalObject)
            {
                IncreaseRevision();
                InternalObject.RemoveAt(index);
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        public virtual T this[int index]
        {
            get
            {
                lock (InternalObject)
                    return InternalObject[index];
            }
            set
            {
                lock (InternalObject)
                {
                    IncreaseRevision();
                    InternalObject[index] = value;
                }
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        public virtual void Add(T item)
        {
            lock (InternalObject)
            {
                IncreaseRevision();
                InternalObject.Add(item);
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        public virtual void Clear()
        {
            lock (InternalObject)
                if (InternalObject.Count > 0)
                {
                    IncreaseRevision();
                    InternalObject.Clear();
                    RebuildCachedCollections();
                }
        }
        //------------------------------------------------------------------------------------------------------------------------
        public virtual bool Contains(T item)
        {
            lock (InternalObject)
                return InternalObject.Contains(item);
        }
        //------------------------------------------------------------------------------------------------------------------------
        public virtual void CopyTo(T[] array, int arrayIndex)
        {
            lock (InternalObject)
                InternalObject.CopyTo(array, arrayIndex);
        }
        //------------------------------------------------------------------------------------------------------------------------
        public virtual void CopyTo(IList<T> list)
        {
            foreach (var entry in this)
                list.Add(entry);
        }
        //------------------------------------------------------------------------------------------------------------------------
        public virtual int Count
        {
            get { lock (InternalObject) return InternalObject.Count; }
        }
        //------------------------------------------------------------------------------------------------------------------------
        public virtual bool IsReadOnly
        {
            get { return false; }
        }
        //------------------------------------------------------------------------------------------------------------------------
        public virtual bool Remove(T item)
        {
            lock (InternalObject)
            {
                IncreaseRevision();
                return InternalObject.Remove(item);
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        public virtual int RemoveAll(Predicate<T> match)
        {
            lock (InternalObject)
            {
                IncreaseRevision();
                return InternalObject.RemoveAll(match);
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        public bool Exists(Predicate<T> match) { lock (InternalObject) return InternalObject.Exists(match); }
        public T Find(Predicate<T> match) { lock (InternalObject) return InternalObject.Find(match); }
        public List<T> FindAll(Predicate<T> match) { lock (InternalObject) return InternalObject.FindAll(match); }
        public int FindIndex(Predicate<T> match) { lock (InternalObject) return InternalObject.FindIndex(match); }
        public int FindIndex(int startIndex, Predicate<T> match) { lock (InternalObject) return InternalObject.FindIndex(match); }
        public int FindIndex(int startIndex, int count, Predicate<T> match) { lock (InternalObject) return InternalObject.FindIndex(match); }
        public T FindLast(Predicate<T> match) { lock (InternalObject) return InternalObject.FindLast(match); }
        public int FindLastIndex(Predicate<T> match) { lock (InternalObject) return InternalObject.FindLastIndex(match); }
        public int FindLastIndex(int startIndex, Predicate<T> match) { lock (InternalObject) return InternalObject.FindLastIndex(match); }
        public int FindLastIndex(int startIndex, int count, Predicate<T> match) { lock (InternalObject) return InternalObject.FindLastIndex(match); }
        //------------------------------------------------------------------------------------------------------------------------
        public IReadOnlyList<T> AsReadOnly()
        {
            if (cached_ReadOnlyWrapper == null)
                cached_ReadOnlyWrapper = new ReadOnlyList<T>(this);
            return cached_ReadOnlyWrapper;
        }
        //------------------------------------------------------------------------------------------------------------------------
        public virtual IEnumerator<T> EnumerateLocked()
        {
            lock (InternalObject)
                foreach (var entry in InternalObject)
                    yield return entry;
        }
        //------------------------------------------------------------------------------------------------------------------------
        public virtual IEnumerator<T> GetEnumerator()
        {
            lock (InternalObject)
            {
                RebuildCachedCollections();
                return (cached_Values == null ? empty : cached_Values).GetEnumerator();
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            lock (InternalObject)
            {
                RebuildCachedCollections();
                return (cached_Values == null ? empty : cached_Values).GetEnumerator();
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion
    }
}

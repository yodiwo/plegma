using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo
{
    [Serializable]
    public class TSBaseISet<T> : ISet<T>, IReadOnlySet<T>
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        protected ISet<T> InternalObject;
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
        #endregion


        #region Constructors
        //------------------------------------------------------------------------------------------------------------------------
        public TSBaseISet(ISet<T> Set)
        {
            InternalObject = Set;
            //increase revision if we already have items
            if (InternalObject.Count > 0)
                revision++;
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion


        #region Functions
        //------------------------------------------------------------------------------------------------------------------------
        protected void RebuildCachedCollections()
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
            //let go of cached references
            cached_Values = empty;
        }
        //------------------------------------------------------------------------------------------------------------------------
        public virtual bool Add(T item)
        {
            lock (InternalObject)
            {
                IncreaseRevision();
                return InternalObject.Add(item);
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        public virtual void AddFromSource(IEnumerable<T> source)
        {
            foreach (var entry in source)
                Add(entry);
        }
        //------------------------------------------------------------------------------------------------------------------------
        public virtual void ExceptWith(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }
        //------------------------------------------------------------------------------------------------------------------------
        public virtual void IntersectWith(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }
        //------------------------------------------------------------------------------------------------------------------------
        public virtual bool IsProperSubsetOf(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }
        //------------------------------------------------------------------------------------------------------------------------
        public virtual bool IsProperSupersetOf(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }
        //------------------------------------------------------------------------------------------------------------------------
        public virtual bool IsSubsetOf(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }
        //------------------------------------------------------------------------------------------------------------------------
        public virtual bool IsSupersetOf(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }
        //------------------------------------------------------------------------------------------------------------------------
        public virtual bool Overlaps(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }
        //------------------------------------------------------------------------------------------------------------------------
        public virtual bool SetEquals(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }
        //------------------------------------------------------------------------------------------------------------------------
        public virtual void SymmetricExceptWith(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }
        //------------------------------------------------------------------------------------------------------------------------
        public virtual void UnionWith(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }
        //------------------------------------------------------------------------------------------------------------------------
        void ICollection<T>.Add(T item)
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
        public virtual void CopyTo(ISet<T> set)
        {
            foreach (var entry in this)
                set.Add(entry);
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
                if (InternalObject.Remove(item))
                {
                    IncreaseRevision();
                    return true;
                }
                else
                    return false;
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        public virtual bool Remove(IEnumerable<T> source)
        {
            bool ret = false;
            foreach (var entry in source)
                ret |= Remove(entry);
            return ret;
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

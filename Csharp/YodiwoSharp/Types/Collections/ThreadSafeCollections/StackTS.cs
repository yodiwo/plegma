using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo
{
    [Serializable]
    public class StackTS<T> : IEnumerable<T>, System.Collections.ICollection
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        protected Stack<T> InternalObject = new Stack<T>();
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
        public StackTS()
        {
        }
        //------------------------------------------------------------------------------------------------------------------------
        public StackTS(IEnumerable<T> source)
        {
            InternalObject = new Stack<T>(source);
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
        public T Pop()
        {
            lock (InternalObject)
            {
                IncreaseRevision();
                return InternalObject.Pop();
            }
        }
        //------------------------------------------------------------------------------------------------------------------------        
        public T TryPopOrDefault(T Default = default(T))
        {
            lock (InternalObject)
            {
                IncreaseRevision();
                return InternalObject.Count == 0 ? Default : InternalObject.Pop();
            }
        }
        //------------------------------------------------------------------------------------------------------------------------        
        public void Push(T item)
        {
            lock (InternalObject)
            {
                IncreaseRevision();
                InternalObject.Push(item);
            }
        }
        //------------------------------------------------------------------------------------------------------------------------        
        public void TrimExcess()
        {
            lock (InternalObject)
            {
                IncreaseRevision();
                InternalObject.TrimExcess();
            }
        }
        //------------------------------------------------------------------------------------------------------------------------        
        public void Clear()
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
        public bool Contains(T item)
        {
            lock (InternalObject)
                return InternalObject.Contains(item);
        }
        //------------------------------------------------------------------------------------------------------------------------        
        public T Peek()
        {
            lock (InternalObject)
                return InternalObject.Peek();
        }
        //------------------------------------------------------------------------------------------------------------------------        
        public T[] ToArray()
        {
            lock (InternalObject)
                return InternalObject.ToArray();
        }
        //------------------------------------------------------------------------------------------------------------------------        
        public void CopyTo(Array array, int index)
        {
            lock (InternalObject)
                (InternalObject as System.Collections.ICollection).CopyTo(array, index);
        }
        //------------------------------------------------------------------------------------------------------------------------
        public virtual void CopyTo(T[] array, int arrayIndex)
        {
            lock (InternalObject)
                InternalObject.CopyTo(array, arrayIndex);
        }
        //------------------------------------------------------------------------------------------------------------------------
        public virtual void CopyTo(Stack<T> stack)
        {
            lock (InternalObject)
                foreach (var entry in InternalObject)
                    stack.Push(entry);
        }
        //------------------------------------------------------------------------------------------------------------------------        
        public int Count
        {
            get { lock (InternalObject) return InternalObject.Count; }
        }
        //------------------------------------------------------------------------------------------------------------------------        
        public bool IsSynchronized { get { return false; } }
        //------------------------------------------------------------------------------------------------------------------------        
        public object SyncRoot { get { throw new NotImplementedException(); } }
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

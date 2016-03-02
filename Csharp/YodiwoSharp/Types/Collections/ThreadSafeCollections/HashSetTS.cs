using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo
{
    [Serializable]
    public class HashSetTS<T> : TSBaseISet<T>
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------
        #endregion


        #region Constructors
        //------------------------------------------------------------------------------------------------------------------------
        public HashSetTS()
            : base(new HashSet<T>())
        {
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion


        #region Functions
        //------------------------------------------------------------------------------------------------------------------------
#if NETFX
        [System.Runtime.Serialization.OnDeserialized()]
        protected void OnDeserializedMethod(System.Runtime.Serialization.StreamingContext context)
        {
            (InternalObject as HashSet<T>).OnDeserialization(this);
        }
#endif
        //------------------------------------------------------------------------------------------------------------------------
        public virtual int RemoveWhere(Predicate<T> match)
        {
            lock (InternalObject)
            {
                var cnt = (InternalObject as HashSet<T>).RemoveWhere(match);
                if (cnt > 0)
                    IncreaseRevision();
                return cnt;
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo
{
    [Serializable]
    public class SortedSetTS<T> : TSBaseISet<T>
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------
        #endregion


        #region Constructors
        //------------------------------------------------------------------------------------------------------------------------
        public SortedSetTS()
            : base(new SortedSet<T>())
        {
        }
        //------------------------------------------------------------------------------------------------------------------------
        public SortedSetTS(IComparer<T> comparer)
            : base(new SortedSet<T>(comparer))
        {
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion


        #region Functions
        //------------------------------------------------------------------------------------------------------------------------
        public virtual int RemoveWhere(Predicate<T> match)
        {
            lock (InternalObject)
            {
                var cnt = (InternalObject as SortedSet<T>).RemoveWhere(match);
                if (cnt > 0)
                    IncreaseRevision();
                return cnt;
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yodiwo
{
    public static partial class Extensions
    {
        //--------------------------------------------------------------------------------------------------------------------
        public static void AddFromSource<T>(this ISet<T> set, IEnumerable<T> source)
        {
            if (source != null)
                foreach (var entry in source)
                    set.Add(entry);
        }
        //--------------------------------------------------------------------------------------------------------------------
    }
}

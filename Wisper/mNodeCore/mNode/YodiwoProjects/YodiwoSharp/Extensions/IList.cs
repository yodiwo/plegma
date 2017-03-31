using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo
{
    public static partial class Extensions
    {
        public static void AddFromSource(this System.Collections.IList list, System.Collections.IEnumerable source)
        {
            if (source != null)
                foreach (var item in source)
                    list.Add(item);
        }

        public static void AddFromSource<T>(this IList<T> list, IEnumerable<T> source)
        {
            if (source != null)
            {
                if (list is ListTS<T>)
                    (list as ListTS<T>).AddRange(source);
                else if (list is List<T>)
                    (list as List<T>).AddRange(source);
                else
                    foreach (var item in source)
                        list.Add(item);
            }
        }
    }
}

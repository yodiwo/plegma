using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo
{
    [Serializable]
    public class ReadOnlyList<T> : IReadOnlyList<T>
    {
        IList<T> source;

        public ReadOnlyList(IList<T> source)
        {
            this.source = source;
        }

        public T this[int i]
        {
            get
            {
                return source[i];
            }
        }

        public int Count { get { return source.Count; } }

        public IEnumerator<T> GetEnumerator()
        {
            return source.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return source.GetEnumerator();
        }
    }
}

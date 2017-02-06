using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo
{
    [Serializable]
    public class ReadOnlyArray<T> : IReadOnlyList<T>
    {
        T[] source;

        public ReadOnlyArray(T[] source)
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

        public int Count { get { return source.Length; } }

        public IEnumerator<T> GetEnumerator()
        {
            return source.AsEnumerable().GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return source.GetEnumerator();
        }
    }
}

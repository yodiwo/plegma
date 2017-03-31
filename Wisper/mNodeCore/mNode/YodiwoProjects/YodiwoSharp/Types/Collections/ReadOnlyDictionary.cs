using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo
{
    [Serializable]
    public class ReadOnlyDictionary<TKey, TValue> : IReadOnlyDictionary<TKey, TValue>
    {
        IDictionary<TKey, TValue> source;

        public ReadOnlyDictionary(IDictionary<TKey, TValue> source)
        {
            this.source = source;
        }

        public TValue this[TKey key]
        {
            get { return source[key]; }
        }

        public int Count { get { return source.Count; } }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return source.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return source.GetEnumerator();
        }

        public bool ContainsKey(TKey key)
        {
            return source.ContainsKey(key);
        }

        public IEnumerable<TKey> Keys
        {
            get { return source.Keys; }
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return source.TryGetValue(key, out value);
        }

        public IEnumerable<TValue> Values
        {
            get { return source.Values; }
        }
    }
}

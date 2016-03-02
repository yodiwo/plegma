using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo
{
    [Serializable]
    public struct TupleStructUnorderer<T1, T2> : IEquatable<TupleStructUnorderer<T1, T2>>
    {
        public T1 Item1;
        public T2 Item2;

        public TupleStructUnorderer(T1 item1, T2 item2)
        {
            this.Item1 = item1;
            this.Item2 = item2;
        }

        #region Equality
        public bool Equals(ref TupleStructUnorderer<T1, T2> other)
        {
            return (Object.Equals(Item1, other.Item1) && Object.Equals(Item2, other.Item2)) ||
                   (Object.Equals(Item1, other.Item2) && Object.Equals(Item2, other.Item1))
                   ;
        }
        public bool Equals(TupleStructUnorderer<T1, T2> other)
        {
            return Equals(ref other);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is TupleStructUnorderer<T1, T2>)) return false;
            return Equals((TupleStructUnorderer<T1, T2>)obj);
        }

        public override int GetHashCode()
        {
            int hash = 0;
            hash ^= (Item1 == null ? 0 : Item1.GetHashCode());
            hash ^= (Item2 == null ? 0 : Item2.GetHashCode());
            return hash;
        }

        public static bool operator ==(TupleStructUnorderer<T1, T2> left, TupleStructUnorderer<T1, T2> right) { return left.Equals(right); }
        public static bool operator !=(TupleStructUnorderer<T1, T2> left, TupleStructUnorderer<T1, T2> right) { return !left.Equals(right); }
        #endregion
    }
}
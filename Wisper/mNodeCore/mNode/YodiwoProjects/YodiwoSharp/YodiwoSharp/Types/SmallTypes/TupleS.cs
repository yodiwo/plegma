using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo
{
    //===================================================================================================================

    public static class TupleS
    {
        public static TupleS<T1, T2> Create<T1, T2>(T1 item1, T2 item2) { return new TupleS<T1, T2>(item1, item2); }
        public static TupleS<T1, T2, T3> Create<T1, T2, T3>(T1 item1, T2 item2, T3 item3) { return new TupleS<T1, T2, T3>(item1, item2, item3); }
        public static TupleS<T1, T2, T3, T4> Create<T1, T2, T3, T4>(T1 item1, T2 item2, T3 item3, T4 item4) { return new TupleS<T1, T2, T3, T4>(item1, item2, item3, item4); }
        public static TupleS<T1, T2, T3, T4, T5> Create<T1, T2, T3, T4, T5>(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5) { return new TupleS<T1, T2, T3, T4, T5>(item1, item2, item3, item4, item5); }
    }

    //===================================================================================================================

    [Serializable]
    public struct TupleS<T1, T2> : IEquatable<TupleS<T1, T2>>
    {
        public T1 Item1;
        public T2 Item2;

        public TupleS(T1 item1, T2 item2)
        {
            this.Item1 = item1;
            this.Item2 = item2;
        }

        #region Equality
        public bool Equals(TupleS<T1, T2> other)
        {
            return Equals(ref other);
        }

        public bool Equals(ref TupleS<T1, T2> other)
        {
            return
                (Item1 == null ? other.Item1 == null : Item1.Equals(other.Item1)) &&
                (Item2 == null ? other.Item2 == null : Item2.Equals(other.Item2))
                ;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is TupleS<T1, T2>)) return false;
            return Equals((TupleS<T1, T2>)obj);
        }

        public override int GetHashCode()
        {
            int hash = 0;
            hash = (hash * 397) ^ (Item1 == null ? 0 : Item1.GetHashCode());
            hash = (hash * 397) ^ (Item2 == null ? 0 : Item2.GetHashCode());
            return hash;
        }

        public static bool operator ==(TupleS<T1, T2> left, TupleS<T1, T2> right) { return left.Equals(ref right); }
        public static bool operator !=(TupleS<T1, T2> left, TupleS<T1, T2> right) { return !left.Equals(ref right); }
        #endregion
    }

    //===================================================================================================================

    [Serializable]
    public struct TupleS<T1, T2, T3> : IEquatable<TupleS<T1, T2, T3>>
    {
        public T1 Item1;
        public T2 Item2;
        public T3 Item3;

        public TupleS(T1 item1, T2 item2, T3 item3)
        {
            this.Item1 = item1;
            this.Item2 = item2;
            this.Item3 = item3;
        }

        #region Equality
        public bool Equals(TupleS<T1, T2, T3> other)
        {
            return Equals(ref other);
        }

        public bool Equals(ref TupleS<T1, T2, T3> other)
        {
            return
                (Item1 == null ? other.Item1 == null : Item1.Equals(other.Item1)) &&
                (Item2 == null ? other.Item2 == null : Item2.Equals(other.Item2)) &&
                (Item3 == null ? other.Item3 == null : Item3.Equals(other.Item3))
                ;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is TupleS<T1, T2, T3>)) return false;
            return Equals((TupleS<T1, T2, T3>)obj);
        }

        public override int GetHashCode()
        {
            int hash = 0;
            hash = (hash * 397) ^ (Item1 == null ? 0 : Item1.GetHashCode());
            hash = (hash * 397) ^ (Item2 == null ? 0 : Item2.GetHashCode());
            hash = (hash * 397) ^ (Item3 == null ? 0 : Item3.GetHashCode());
            return hash;
        }

        public static bool operator ==(TupleS<T1, T2, T3> left, TupleS<T1, T2, T3> right) { return left.Equals(ref right); }
        public static bool operator !=(TupleS<T1, T2, T3> left, TupleS<T1, T2, T3> right) { return !left.Equals(ref right); }
        #endregion
    }

    //===================================================================================================================

    [Serializable]
    public struct TupleS<T1, T2, T3, T4> : IEquatable<TupleS<T1, T2, T3, T4>>
    {
        public T1 Item1;
        public T2 Item2;
        public T3 Item3;
        public T4 Item4;

        public TupleS(T1 item1, T2 item2, T3 item3, T4 item4)
        {
            this.Item1 = item1;
            this.Item2 = item2;
            this.Item3 = item3;
            this.Item4 = item4;
        }

        #region Equality
        public bool Equals(TupleS<T1, T2, T3, T4> other)
        {
            return Equals(ref other);
        }

        public bool Equals(ref TupleS<T1, T2, T3, T4> other)
        {
            return
                (Item1 == null ? other.Item1 == null : Item1.Equals(other.Item1)) &&
                (Item2 == null ? other.Item2 == null : Item2.Equals(other.Item2)) &&
                (Item3 == null ? other.Item3 == null : Item3.Equals(other.Item3)) &&
                (Item4 == null ? other.Item4 == null : Item4.Equals(other.Item4))
                ;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is TupleS<T1, T2, T3, T4>)) return false;
            return Equals((TupleS<T1, T2, T3, T4>)obj);
        }

        public override int GetHashCode()
        {
            int hash = 0;
            hash = (hash * 397) ^ (Item1 == null ? 0 : Item1.GetHashCode());
            hash = (hash * 397) ^ (Item2 == null ? 0 : Item2.GetHashCode());
            hash = (hash * 397) ^ (Item3 == null ? 0 : Item3.GetHashCode());
            hash = (hash * 397) ^ (Item4 == null ? 0 : Item4.GetHashCode());
            return hash;
        }

        public static bool operator ==(TupleS<T1, T2, T3, T4> left, TupleS<T1, T2, T3, T4> right) { return left.Equals(ref right); }
        public static bool operator !=(TupleS<T1, T2, T3, T4> left, TupleS<T1, T2, T3, T4> right) { return !left.Equals(ref right); }
        #endregion
    }

    //===================================================================================================================

    [Serializable]
    public struct TupleS<T1, T2, T3, T4, T5> : IEquatable<TupleS<T1, T2, T3, T4, T5>>
    {
        public T1 Item1;
        public T2 Item2;
        public T3 Item3;
        public T4 Item4;
        public T5 Item5;

        public TupleS(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5)
        {
            this.Item1 = item1;
            this.Item2 = item2;
            this.Item3 = item3;
            this.Item4 = item4;
            this.Item5 = item5;
        }

        #region Equality
        public bool Equals(TupleS<T1, T2, T3, T4, T5> other)
        {
            return Equals(ref other);
        }

        public bool Equals(ref TupleS<T1, T2, T3, T4, T5> other)
        {
            return
                (Item1 == null ? other.Item1 == null : Item1.Equals(other.Item1)) &&
                (Item2 == null ? other.Item2 == null : Item2.Equals(other.Item2)) &&
                (Item3 == null ? other.Item3 == null : Item3.Equals(other.Item3)) &&
                (Item4 == null ? other.Item4 == null : Item4.Equals(other.Item4)) &&
                (Item5 == null ? other.Item5 == null : Item5.Equals(other.Item5))
                ;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is TupleS<T1, T2, T3, T4, T5>)) return false;
            return Equals((TupleS<T1, T2, T3, T4, T5>)obj);
        }

        public override int GetHashCode()
        {
            int hash = 0;
            hash = (hash * 397) ^ (Item1 == null ? 0 : Item1.GetHashCode());
            hash = (hash * 397) ^ (Item2 == null ? 0 : Item2.GetHashCode());
            hash = (hash * 397) ^ (Item3 == null ? 0 : Item3.GetHashCode());
            hash = (hash * 397) ^ (Item4 == null ? 0 : Item4.GetHashCode());
            hash = (hash * 397) ^ (Item5 == null ? 0 : Item5.GetHashCode());
            return hash;
        }

        public static bool operator ==(TupleS<T1, T2, T3, T4, T5> left, TupleS<T1, T2, T3, T4, T5> right) { return left.Equals(ref right); }
        public static bool operator !=(TupleS<T1, T2, T3, T4, T5> left, TupleS<T1, T2, T3, T4, T5> right) { return !left.Equals(ref right); }
        #endregion
    }

    //===================================================================================================================

    //If you need more than 5, you are doing it wrong 
}

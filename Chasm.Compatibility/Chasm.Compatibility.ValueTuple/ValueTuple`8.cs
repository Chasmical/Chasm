#if NETCOREAPP1_0_OR_GREATER || NETSTANDARD1_0_OR_GREATER || NET45_OR_GREATER
using System.Runtime.CompilerServices;
[assembly: TypeForwardedTo(typeof(System.ValueTuple<,,,,,,,>))]
#else
// Adapted from .NET's source code
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

// ReSharper disable SuspiciousTypeConversion.Global
// ReSharper disable once CheckNamespace
namespace System
{
    [Serializable, StructLayout(LayoutKind.Auto)]
    public struct ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest>
        : IEquatable<ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest>>, IComparable<ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest>>
        , IComparable, ITupleInternal
#if NET40_OR_GREATER
        , IStructuralEquatable, IStructuralComparable
#endif
        where TRest : struct
    {
        public T1 Item1;
        public T2 Item2;
        public T3 Item3;
        public T4 Item4;
        public T5 Item5;
        public T6 Item6;
        public T7 Item7;
        public TRest Rest;

        readonly int ITupleInternal.Size => Rest is ITupleInternal tuple ? 7 + tuple.Size : 8;

        public ValueTuple(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7, TRest rest)
        {
            if (rest is not ITupleInternal)
                throw new ArgumentException("The TRest type argument of ValueTuple`8 must be a ValueTuple.");

            Item1 = item1;
            Item2 = item2;
            Item3 = item3;
            Item4 = item4;
            Item5 = item5;
            Item6 = item6;
            Item7 = item7;
            Rest = rest;
        }

        private static readonly EqualityComparer<T1> t1Comparer = EqualityComparer<T1>.Default;
        private static readonly EqualityComparer<T2> t2Comparer = EqualityComparer<T2>.Default;
        private static readonly EqualityComparer<T3> t3Comparer = EqualityComparer<T3>.Default;
        private static readonly EqualityComparer<T4> t4Comparer = EqualityComparer<T4>.Default;
        private static readonly EqualityComparer<T5> t5Comparer = EqualityComparer<T5>.Default;
        private static readonly EqualityComparer<T6> t6Comparer = EqualityComparer<T6>.Default;
        private static readonly EqualityComparer<T7> t7Comparer = EqualityComparer<T7>.Default;
        private static readonly EqualityComparer<TRest> tRestComparer = EqualityComparer<TRest>.Default;

        public readonly bool Equals(ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest> other)
            => t1Comparer.Equals(Item1, other.Item1) && t2Comparer.Equals(Item2, other.Item2) && t3Comparer.Equals(Item3, other.Item3)
            && t4Comparer.Equals(Item4, other.Item4) && t5Comparer.Equals(Item5, other.Item5) && t6Comparer.Equals(Item6, other.Item6)
            && t7Comparer.Equals(Item7, other.Item7) && tRestComparer.Equals(Rest, other.Rest);
        public readonly override bool Equals(object? obj)
            => obj is ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest> tuple && Equals(tuple);
#if NET40_OR_GREATER
        readonly bool IStructuralEquatable.Equals(object? other, IEqualityComparer comparer)
            => other is ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest> tuple
            && comparer.Equals(Item1, tuple.Item1) && comparer.Equals(Item2, tuple.Item2) && comparer.Equals(Item3, tuple.Item3)
            && comparer.Equals(Item4, tuple.Item4) && comparer.Equals(Item5, tuple.Item5) && comparer.Equals(Item6, tuple.Item6)
            && comparer.Equals(Item7, tuple.Item7) && comparer.Equals(Rest, tuple.Rest);
#endif

        public readonly override int GetHashCode()
        {
            if (Rest is not ITupleInternal tuple)
            {
                return HashCodeShim.Combine(
                    t1Comparer.GetHashCode(Item1), t2Comparer.GetHashCode(Item2), t3Comparer.GetHashCode(Item3),
                    t4Comparer.GetHashCode(Item4), t5Comparer.GetHashCode(Item5), t6Comparer.GetHashCode(Item6),
                    t7Comparer.GetHashCode(Item7)
                );
            }
            int size = tuple.Size;
            if (size >= 8) return tuple.GetHashCode();

            return (8 - size) switch
            {
                1 => HashCodeShim.Combine(t7Comparer.GetHashCode(Item7), tuple.GetHashCode()),
                2 => HashCodeShim.Combine(t6Comparer.GetHashCode(Item6), t7Comparer.GetHashCode(Item7), tuple.GetHashCode()),
                3 => HashCodeShim.Combine(
                    t5Comparer.GetHashCode(Item5), t6Comparer.GetHashCode(Item6), t7Comparer.GetHashCode(Item7),
                    tuple.GetHashCode()
                ),
                4 => HashCodeShim.Combine(
                    t4Comparer.GetHashCode(Item4), t5Comparer.GetHashCode(Item5), t6Comparer.GetHashCode(Item6),
                    t7Comparer.GetHashCode(Item7), tuple.GetHashCode()
                ),
                5 => HashCodeShim.Combine(
                    t3Comparer.GetHashCode(Item3), t4Comparer.GetHashCode(Item4), t5Comparer.GetHashCode(Item5),
                    t6Comparer.GetHashCode(Item6), t7Comparer.GetHashCode(Item7), tuple.GetHashCode()
                ),
                6 => HashCodeShim.Combine(
                    t2Comparer.GetHashCode(Item2), t3Comparer.GetHashCode(Item3), t4Comparer.GetHashCode(Item4),
                    t5Comparer.GetHashCode(Item5), t6Comparer.GetHashCode(Item6), t7Comparer.GetHashCode(Item7),
                    tuple.GetHashCode()
                ),
                7 or 8 => HashCodeShim.Combine(
                    t1Comparer.GetHashCode(Item1), t2Comparer.GetHashCode(Item2), t3Comparer.GetHashCode(Item3),
                    t4Comparer.GetHashCode(Item4), t5Comparer.GetHashCode(Item5), t6Comparer.GetHashCode(Item6),
                    t7Comparer.GetHashCode(Item7), tuple.GetHashCode()
                ),
                _ => -1,
            };
        }
        private readonly int GetHashCodeCore(IEqualityComparer comparer)
        {
            if (Rest is not ITupleInternal tuple)
            {
                return HashCodeShim.Combine(
                    comparer.GetHashCode(Item1), comparer.GetHashCode(Item2), comparer.GetHashCode(Item3),
                    comparer.GetHashCode(Item4), comparer.GetHashCode(Item5), comparer.GetHashCode(Item6),
                    comparer.GetHashCode(Item7)
                );
            }
            int size = tuple.Size;
            if (size >= 8) return tuple.GetHashCode(comparer);

            return (8 - size) switch
            {
                1 => HashCodeShim.Combine(comparer.GetHashCode(Item7), tuple.GetHashCode(comparer)),
                2 => HashCodeShim.Combine(comparer.GetHashCode(Item6), comparer.GetHashCode(Item7), tuple.GetHashCode(comparer)),
                3 => HashCodeShim.Combine(
                    comparer.GetHashCode(Item5), comparer.GetHashCode(Item6), comparer.GetHashCode(Item7),
                    tuple.GetHashCode(comparer)
                ),
                4 => HashCodeShim.Combine(
                    comparer.GetHashCode(Item4), comparer.GetHashCode(Item5), comparer.GetHashCode(Item6),
                    comparer.GetHashCode(Item7), tuple.GetHashCode(comparer)
                ),
                5 => HashCodeShim.Combine(
                    comparer.GetHashCode(Item3), comparer.GetHashCode(Item4), comparer.GetHashCode(Item5),
                    comparer.GetHashCode(Item6), comparer.GetHashCode(Item7), tuple.GetHashCode(comparer)
                ),
                6 => HashCodeShim.Combine(
                    comparer.GetHashCode(Item2), comparer.GetHashCode(Item3), comparer.GetHashCode(Item4),
                    comparer.GetHashCode(Item5), comparer.GetHashCode(Item6), comparer.GetHashCode(Item7),
                    tuple.GetHashCode(comparer)
                ),
                7 or 8 => HashCodeShim.Combine(
                    comparer.GetHashCode(Item1), comparer.GetHashCode(Item2), comparer.GetHashCode(Item3),
                    comparer.GetHashCode(Item4), comparer.GetHashCode(Item5), comparer.GetHashCode(Item6),
                    comparer.GetHashCode(Item7), tuple.GetHashCode(comparer)
                ),
                _ => -1,
            };
        }

#if NET40_OR_GREATER
        readonly int IStructuralEquatable.GetHashCode(IEqualityComparer comparer) => GetHashCodeCore(comparer);
#endif
        readonly int ITupleInternal.GetHashCode(IEqualityComparer comparer) => GetHashCodeCore(comparer);

        public readonly int CompareTo(ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest> other)
        {
            int res = Comparer<T1>.Default.Compare(Item1, other.Item1);
            if (res != 0) return res;
            res = Comparer<T2>.Default.Compare(Item2, other.Item2);
            if (res != 0) return res;
            res = Comparer<T3>.Default.Compare(Item3, other.Item3);
            if (res != 0) return res;
            res = Comparer<T4>.Default.Compare(Item4, other.Item4);
            if (res != 0) return res;
            res = Comparer<T5>.Default.Compare(Item5, other.Item5);
            if (res != 0) return res;
            res = Comparer<T6>.Default.Compare(Item6, other.Item6);
            if (res != 0) return res;
            res = Comparer<T7>.Default.Compare(Item7, other.Item7);
            if (res != 0) return res;
            return Comparer<TRest>.Default.Compare(Rest, other.Rest);
        }
        readonly int IComparable.CompareTo(object? other)
        {
            if (other is null) return 1;
            if (other is ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest> tuple) return CompareTo(tuple);
            throw new ArgumentException(SR.TupleInvalidType, nameof(other));
        }
#if NET40_OR_GREATER
        readonly int IStructuralComparable.CompareTo(object? other, IComparer comparer)
        {
            if (other is null) return 1;
            if (other is ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest> tuple)
            {
                int res = comparer.Compare(Item1, tuple.Item1);
                if (res != 0) return res;
                res = comparer.Compare(Item2, tuple.Item2);
                if (res != 0) return res;
                res = comparer.Compare(Item3, tuple.Item3);
                if (res != 0) return res;
                res = comparer.Compare(Item4, tuple.Item4);
                if (res != 0) return res;
                res = comparer.Compare(Item5, tuple.Item5);
                if (res != 0) return res;
                res = comparer.Compare(Item6, tuple.Item6);
                if (res != 0) return res;
                res = comparer.Compare(Item7, tuple.Item7);
                if (res != 0) return res;
                return comparer.Compare(Rest, tuple.Rest);
            }
            throw new ArgumentException(SR.TupleInvalidType, nameof(other));
        }
#endif

        public readonly override string ToString()
        {
            if (Rest is ITupleInternal tuple)
                return $"({Item1}, {Item2}, {Item3}, {Item4}, {Item5}, {Item6}, {Item7}, {tuple.ToStringEnd()}";
            return $"({Item1}, {Item2}, {Item3}, {Item4}, {Item5}, {Item6}, {Item7}, {Rest})";
        }
        readonly string ITupleInternal.ToStringEnd()
        {
            if (Rest is ITupleInternal tuple)
                return $"{Item1}, {Item2}, {Item3}, {Item4}, {Item5}, {Item6}, {Item7}, {tuple.ToStringEnd()}";
            return $"{Item1}, {Item2}, {Item3}, {Item4}, {Item5}, {Item6}, {Item7}, {Rest})";
        }

    }
}
#endif

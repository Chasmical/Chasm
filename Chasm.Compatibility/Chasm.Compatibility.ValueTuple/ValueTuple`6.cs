#if NETCOREAPP1_0_OR_GREATER || NETSTANDARD1_0_OR_GREATER || NET45_OR_GREATER
using System.Runtime.CompilerServices;
[assembly: TypeForwardedTo(typeof(System.ValueTuple<,,,,,>))]
#else
// Adapted from .NET's source code
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

// ReSharper disable once CheckNamespace
namespace System
{
    [Serializable, StructLayout(LayoutKind.Auto)]
    public struct ValueTuple<T1, T2, T3, T4, T5, T6>(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6)
        : IEquatable<ValueTuple<T1, T2, T3, T4, T5, T6>>, IComparable<ValueTuple<T1, T2, T3, T4, T5, T6>>
        , IComparable, ITupleInternal
#if NET40_OR_GREATER
        , IStructuralEquatable, IStructuralComparable
#endif
    {
        public T1 Item1 = item1;
        public T2 Item2 = item2;
        public T3 Item3 = item3;
        public T4 Item4 = item4;
        public T5 Item5 = item5;
        public T6 Item6 = item6;

        readonly int ITupleInternal.Size => 6;

        private static readonly EqualityComparer<T1> t1Comparer = EqualityComparer<T1>.Default;
        private static readonly EqualityComparer<T2> t2Comparer = EqualityComparer<T2>.Default;
        private static readonly EqualityComparer<T3> t3Comparer = EqualityComparer<T3>.Default;
        private static readonly EqualityComparer<T4> t4Comparer = EqualityComparer<T4>.Default;
        private static readonly EqualityComparer<T5> t5Comparer = EqualityComparer<T5>.Default;
        private static readonly EqualityComparer<T6> t6Comparer = EqualityComparer<T6>.Default;

        public readonly bool Equals(ValueTuple<T1, T2, T3, T4, T5, T6> other)
            => t1Comparer.Equals(Item1, other.Item1) && t2Comparer.Equals(Item2, other.Item2) && t3Comparer.Equals(Item3, other.Item3)
            && t4Comparer.Equals(Item4, other.Item4) && t5Comparer.Equals(Item5, other.Item5) && t6Comparer.Equals(Item6, other.Item6);
        public readonly override bool Equals(object? obj)
            => obj is ValueTuple<T1, T2, T3, T4, T5, T6> tuple && Equals(tuple);
#if NET40_OR_GREATER
        readonly bool IStructuralEquatable.Equals(object? other, IEqualityComparer comparer)
            => other is ValueTuple<T1, T2, T3, T4, T5, T6> tuple
            && comparer.Equals(Item1, tuple.Item1) && comparer.Equals(Item2, tuple.Item2) && comparer.Equals(Item3, tuple.Item3)
            && comparer.Equals(Item4, tuple.Item4) && comparer.Equals(Item5, tuple.Item5) && comparer.Equals(Item6, tuple.Item6);
#endif

        public readonly override int GetHashCode() => HashCodeShim.Combine(
            t1Comparer.GetHashCode(Item1), t2Comparer.GetHashCode(Item2), t3Comparer.GetHashCode(Item3), t4Comparer.GetHashCode(Item4),
            t5Comparer.GetHashCode(Item5), t6Comparer.GetHashCode(Item6)
        );
        private readonly int GetHashCodeCore(IEqualityComparer comparer) => HashCodeShim.Combine(
            comparer.GetHashCode(Item1), comparer.GetHashCode(Item2), comparer.GetHashCode(Item3), comparer.GetHashCode(Item4),
            comparer.GetHashCode(Item5), comparer.GetHashCode(Item6)
        );

#if NET40_OR_GREATER
        readonly int IStructuralEquatable.GetHashCode(IEqualityComparer comparer) => GetHashCodeCore(comparer);
#endif
        readonly int ITupleInternal.GetHashCode(IEqualityComparer comparer) => GetHashCodeCore(comparer);

        public readonly int CompareTo(ValueTuple<T1, T2, T3, T4, T5, T6> other)
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
            return Comparer<T6>.Default.Compare(Item6, other.Item6);
        }
        readonly int IComparable.CompareTo(object? other)
        {
            if (other is null) return 1;
            if (other is ValueTuple<T1, T2, T3, T4, T5, T6> tuple) return CompareTo(tuple);
            throw new ArgumentException(SR.TupleInvalidType, nameof(other));
        }
#if NET40_OR_GREATER
        readonly int IStructuralComparable.CompareTo(object? other, IComparer comparer)
        {
            if (other is null) return 1;
            if (other is ValueTuple<T1, T2, T3, T4, T5, T6> tuple)
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
                return comparer.Compare(Item6, tuple.Item6);
            }
            throw new ArgumentException(SR.TupleInvalidType, nameof(other));
        }
#endif

        public readonly override string ToString()
            => $"({Item1}, {Item2}, {Item3}, {Item4}, {Item5}, {Item6})";
        readonly string ITupleInternal.ToStringEnd()
            => $"{Item1}, {Item2}, {Item3}, {Item4}, {Item5}, {Item6})";

    }
}
#endif

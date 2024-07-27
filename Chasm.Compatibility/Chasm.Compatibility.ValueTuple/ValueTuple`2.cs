#if NETCOREAPP1_0_OR_GREATER || NETSTANDARD1_0_OR_GREATER || NET45_OR_GREATER
using System.Runtime.CompilerServices;
[assembly: TypeForwardedTo(typeof(System.ValueTuple<,>))]
#else
// Adapted from .NET's source code
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

// ReSharper disable once CheckNamespace
namespace System
{
    [Serializable, StructLayout(LayoutKind.Auto)]
    public struct ValueTuple<T1, T2>(T1 item1, T2 item2)
        : IEquatable<ValueTuple<T1, T2>>, IComparable<ValueTuple<T1, T2>>
        , IComparable, ITupleInternal
#if NET40_OR_GREATER
        , IStructuralEquatable, IStructuralComparable
#endif
    {
        public T1 Item1 = item1;
        public T2 Item2 = item2;

        readonly int ITupleInternal.Size => 2;

        private static readonly EqualityComparer<T1> t1Comparer = EqualityComparer<T1>.Default;
        private static readonly EqualityComparer<T2> t2Comparer = EqualityComparer<T2>.Default;

        public readonly bool Equals(ValueTuple<T1, T2> other)
            => t1Comparer.Equals(Item1, other.Item1) && t2Comparer.Equals(Item2, other.Item2);
        public readonly override bool Equals(object? obj)
            => obj is ValueTuple<T1, T2> tuple && Equals(tuple);
#if NET40_OR_GREATER
        readonly bool IStructuralEquatable.Equals(object? other, IEqualityComparer comparer)
            => other is ValueTuple<T1, T2> tuple && comparer.Equals(Item1, tuple.Item1) && comparer.Equals(Item2, tuple.Item2);
#endif

        public readonly override int GetHashCode()
            => HashCodeShim.Combine(t1Comparer.GetHashCode(Item1), t2Comparer.GetHashCode(Item2));
        private readonly int GetHashCodeCore(IEqualityComparer comparer)
            => HashCodeShim.Combine(comparer.GetHashCode(Item1), comparer.GetHashCode(Item2));

#if NET40_OR_GREATER
        readonly int IStructuralEquatable.GetHashCode(IEqualityComparer comparer) => GetHashCodeCore(comparer);
#endif
        readonly int ITupleInternal.GetHashCode(IEqualityComparer comparer) => GetHashCodeCore(comparer);

        public readonly int CompareTo(ValueTuple<T1, T2> other)
        {
            int res = Comparer<T1>.Default.Compare(Item1, other.Item1);
            if (res != 0) return res;
            return Comparer<T2>.Default.Compare(Item2, other.Item2);
        }
        readonly int IComparable.CompareTo(object? other)
        {
            if (other is null) return 1;
            if (other is ValueTuple<T1, T2> tuple) return CompareTo(tuple);
            throw new ArgumentException(SR.TupleInvalidType, nameof(other));
        }
#if NET40_OR_GREATER
        readonly int IStructuralComparable.CompareTo(object? other, IComparer comparer)
        {
            if (other is null) return 1;
            if (other is ValueTuple<T1, T2> tuple)
            {
                int res = comparer.Compare(Item1, tuple.Item1);
                if (res != 0) return res;
                return comparer.Compare(Item2, tuple.Item2);
            }
            throw new ArgumentException(SR.TupleInvalidType, nameof(other));
        }
#endif

        public readonly override string ToString()
            => $"({Item1}, {Item2})";
        readonly string ITupleInternal.ToStringEnd()
            => $"{Item1}, {Item2})";

    }
}
#endif

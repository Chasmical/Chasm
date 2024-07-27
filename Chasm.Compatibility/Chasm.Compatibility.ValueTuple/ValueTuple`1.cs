#if NETCOREAPP1_0_OR_GREATER || NETSTANDARD1_0_OR_GREATER || NET45_OR_GREATER
using System.Runtime.CompilerServices;
[assembly: TypeForwardedTo(typeof(System.ValueTuple<>))]
#else
// Adapted from .NET's source code
using System.Collections;
using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace System
{
    [Serializable]
    public struct ValueTuple<T1>(T1 item1)
        : IEquatable<ValueTuple<T1>>, IComparable<ValueTuple<T1>>
        , IComparable, ITupleInternal
#if NET40_OR_GREATER
        , IStructuralEquatable, IStructuralComparable
#endif
    {
        public T1 Item1 = item1;

        readonly int ITupleInternal.Size => 1;

        private static readonly EqualityComparer<T1> t1Comparer = EqualityComparer<T1>.Default;

        public readonly bool Equals(ValueTuple<T1> other)
            => t1Comparer.Equals(Item1, other.Item1);
        public readonly override bool Equals(object? obj)
            => obj is ValueTuple<T1> tuple && Equals(tuple);
#if NET40_OR_GREATER
        readonly bool IStructuralEquatable.Equals(object? other, IEqualityComparer comparer)
            => other is ValueTuple<T1> tuple && comparer.Equals(Item1, tuple.Item1);
#endif

        public readonly override int GetHashCode()
            => t1Comparer.GetHashCode(Item1);
#if NET40_OR_GREATER
        readonly int IStructuralEquatable.GetHashCode(IEqualityComparer comparer)
            => comparer.GetHashCode(Item1);
#endif
        readonly int ITupleInternal.GetHashCode(IEqualityComparer comparer)
            => comparer.GetHashCode(Item1);

        public readonly int CompareTo(ValueTuple<T1> other)
            => Comparer<T1>.Default.Compare(Item1, other.Item1);
        readonly int IComparable.CompareTo(object? other)
        {
            if (other is null) return 1;
            if (other is ValueTuple<T1> tuple) return Comparer<T1>.Default.Compare(Item1, tuple.Item1);
            throw new ArgumentException(SR.TupleInvalidType, nameof(other));
        }
#if NET40_OR_GREATER
        readonly int IStructuralComparable.CompareTo(object? other, IComparer comparer)
        {
            if (other is null) return 1;
            if (other is ValueTuple<T1> tuple) return comparer.Compare(Item1, tuple.Item1);
            throw new ArgumentException(SR.TupleInvalidType, nameof(other));
        }
#endif

        public readonly override string ToString()
            => $"({Item1})";
        readonly string ITupleInternal.ToStringEnd()
            => $"{Item1})";

    }
}
#endif

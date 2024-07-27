#if NETCOREAPP1_0_OR_GREATER || NETSTANDARD1_0_OR_GREATER || NET45_OR_GREATER
using System.Runtime.CompilerServices;
[assembly: TypeForwardedTo(typeof(System.ValueTuple))]
#else
// Adapted from .NET's source code
using System.Collections;

// ReSharper disable once CheckNamespace
namespace System
{
    [Serializable]
    public struct ValueTuple
        : IEquatable<ValueTuple>, IComparable<ValueTuple>
        , IComparable, ITupleInternal
#if NET40_OR_GREATER
        , IStructuralEquatable, IStructuralComparable
#endif
    {
        readonly int ITupleInternal.Size => 0;

        public readonly bool Equals(ValueTuple other) => true;
        public readonly override bool Equals(object? obj) => obj is ValueTuple;
#if NET40_OR_GREATER
        readonly bool IStructuralEquatable.Equals(object? other, IEqualityComparer comparer) => other is ValueTuple;
#endif

        public readonly override int GetHashCode() => 0;
#if NET40_OR_GREATER
        readonly int IStructuralEquatable.GetHashCode(IEqualityComparer comparer) => 0;
#endif
        readonly int ITupleInternal.GetHashCode(IEqualityComparer comparer) => 0;

        public readonly int CompareTo(ValueTuple other) => 0;
        readonly int IComparable.CompareTo(object? other)
        {
            if (other is null) return 1;
            if (other is ValueTuple) return 0;
            throw new ArgumentException(SR.TupleInvalidType, nameof(other));
        }
#if NET40_OR_GREATER
        readonly int IStructuralComparable.CompareTo(object? other, IComparer comparer)
        {
            if (other is null) return 1;
            if (other is ValueTuple) return 0;
            throw new ArgumentException(SR.TupleInvalidType, nameof(other));
        }
#endif

        public readonly override string ToString() => "()";
        readonly string ITupleInternal.ToStringEnd() => ")";

        public static ValueTuple Create()
            => default;
        public static ValueTuple<T1> Create<T1>(T1 item1)
            => new ValueTuple<T1>(item1);
        public static ValueTuple<T1, T2> Create<T1, T2>(T1 item1, T2 item2)
            => new ValueTuple<T1, T2>(item1, item2);
        public static ValueTuple<T1, T2, T3> Create<T1, T2, T3>(T1 item1, T2 item2, T3 item3)
            => new ValueTuple<T1, T2, T3>(item1, item2, item3);
        public static ValueTuple<T1, T2, T3, T4> Create<T1, T2, T3, T4>(T1 item1, T2 item2, T3 item3, T4 item4)
            => new ValueTuple<T1, T2, T3, T4>(item1, item2, item3, item4);
        public static ValueTuple<T1, T2, T3, T4, T5> Create<T1, T2, T3, T4, T5>(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5)
            => new ValueTuple<T1, T2, T3, T4, T5>(item1, item2, item3, item4, item5);
        public static ValueTuple<T1, T2, T3, T4, T5, T6> Create<T1, T2, T3, T4, T5, T6>(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6)
            => new ValueTuple<T1, T2, T3, T4, T5, T6>(item1, item2, item3, item4, item5, item6);
        public static ValueTuple<T1, T2, T3, T4, T5, T6, T7> Create<T1, T2, T3, T4, T5, T6, T7>(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7)
            => new ValueTuple<T1, T2, T3, T4, T5, T6, T7>(item1, item2, item3, item4, item5, item6, item7);
        public static ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8>> Create<T1, T2, T3, T4, T5, T6, T7, T8>(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7, T8 item8)
            => new ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8>>(item1, item2, item3, item4, item5, item6, item7, Create(item8));

    }
    internal interface ITupleInternal
    {
        int Size { get; }
        int GetHashCode(IEqualityComparer comparer);
        string ToStringEnd();
    }
}
#endif

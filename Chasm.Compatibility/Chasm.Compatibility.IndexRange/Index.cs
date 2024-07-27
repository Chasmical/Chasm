#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
using System.Runtime.CompilerServices;
[assembly: TypeForwardedTo(typeof(System.Index))]
#else
// Adapted from .NET's source code
#if NETCOREAPP1_0_OR_GREATER || NETSTANDARD1_0_OR_GREATER || NET45_OR_GREATER
using System.Runtime.CompilerServices;
#endif

// ReSharper disable once CheckNamespace
namespace System
{
    public readonly struct Index : IEquatable<Index>
    {
        private readonly int _value;

#if NETCOREAPP1_0_OR_GREATER || NETSTANDARD1_0_OR_GREATER || NET45_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public Index(int value, bool fromEnd = false)
        {
            if (value < 0) ThrowValueArgumentOutOfRange_NeedNonNegNumException();
            _value = fromEnd ? ~value : value;
        }

        private Index(int value)
            => _value = value;

        public static Index Start => new Index(0);
        public static Index End => new Index(~0);

#if NETCOREAPP1_0_OR_GREATER || NETSTANDARD1_0_OR_GREATER || NET45_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Index FromStart(int value)
        {
            if (value < 0) ThrowValueArgumentOutOfRange_NeedNonNegNumException();
            return new Index(value);
        }
#if NETCOREAPP1_0_OR_GREATER || NETSTANDARD1_0_OR_GREATER || NET45_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Index FromEnd(int value)
        {
            if (value < 0) ThrowValueArgumentOutOfRange_NeedNonNegNumException();
            return new Index(~value);
        }

        public int Value => _value < 0 ? ~_value : _value;
        public bool IsFromEnd => _value < 0;

#if NETCOREAPP1_0_OR_GREATER || NETSTANDARD1_0_OR_GREATER || NET45_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public int GetOffset(int length)
        {
            int offset = _value;
            if (IsFromEnd) offset += length + 1;
            return offset;
        }

        public override bool Equals(object? value) => value is Index index && _value == index._value;
        public bool Equals(Index other) => _value == other._value;
        public override int GetHashCode() => _value;

        public static implicit operator Index(int value) => FromStart(value);

        public override string ToString()
        {
            string value = ((uint)Value).ToString();
            return IsFromEnd ? "^" + value : value;
        }

        private static void ThrowValueArgumentOutOfRange_NeedNonNegNumException()
            => throw new ArgumentOutOfRangeException("value", "value must be non-negative");

    }
}
#endif

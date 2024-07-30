#if !(NETCOREAPP1_0_OR_GREATER || NETSTANDARD1_0_OR_GREATER || NET45_OR_GREATER)
// ReSharper disable once CheckNamespace
namespace System
{
    internal static class ThrowHelper
    {
        public static void ThrowArgumentException_TupleIncorrectType(IValueTupleInternal other)
            => throw new ArgumentException("The parameter should be a ValueTuple type of appropriate arity.", nameof(other));
    }
}
#endif

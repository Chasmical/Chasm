#if !(NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER)
using System.Buffers;

namespace Chasm.Formatting
{
    // Shim for the String.Create(,,) method
    internal static class String
    {
        public static string Create<TState>(int length, TState state, SpanAction<char, TState> action)
        {
            char[] array = new char[length];
            action(array, state);
            return new string(array);
        }
    }
}

namespace System.Buffers
{
    internal delegate void SpanAction<T, in TArg>(Span<T> span, TArg arg);
}
#endif

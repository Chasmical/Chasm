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

#if !(NETSTANDARD1_1_OR_GREATER || NETCOREAPP1_0_OR_GREATER || NET45_OR_GREATER)
namespace System
{
    internal static class GetPinnableReferenceExtensions
    {
        public static ref readonly char GetPinnableReference(this ReadOnlySpan<char> span)
            => ref span.DangerousGetPinnableReference();
    }
}
#endif

#if !(NETSTANDARD1_1_OR_GREATER || NET11_OR_GREATER || NETCOREAPP1_0_OR_GREATER)
namespace System.Runtime.InteropServices
{
    [AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
    internal sealed class InAttribute : Attribute;
}
#endif

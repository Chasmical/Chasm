#if NET6_0_OR_GREATER
global using ANE = System.ArgumentNullException;
#else
using System.Runtime.CompilerServices;

// ReSharper disable once CheckNamespace
namespace System
{
    // ReSharper disable once InconsistentNaming
    internal static class ANE
    {
        public static void ThrowIfNull(object? argument, [CallerArgumentExpression(nameof(argument))] string? paramName = default)
        {
            if (argument is null) Throw(paramName);
        }
        private static void Throw(string? paramName)
            => throw new ArgumentNullException(paramName);
    }
}
#endif

#if !NETCOREAPP3_0_OR_GREATER
// ReSharper disable once CheckNamespace
namespace System.Runtime.CompilerServices
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    internal sealed class CallerArgumentExpressionAttribute(string parameterName) : Attribute
    {
        public string ParameterName { get; } = parameterName;
    }
}
#endif

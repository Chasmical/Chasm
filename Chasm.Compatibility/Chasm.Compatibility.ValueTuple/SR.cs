#if !(NETCOREAPP1_0_OR_GREATER || NETSTANDARD1_0_OR_GREATER || NET45_OR_GREATER)
// ReSharper disable once CheckNamespace
namespace System
{
    // ReSharper disable once InconsistentNaming
    internal static class SR
    {
        public const string TupleInvalidType = "The parameter should be a ValueTuple type of appropriate arity.";
    }
}
#endif

namespace Chasm.SemanticVersioning.Ranges
{
    /// <summary>
    ///   <para>Specifies primitive operators, used by <see cref="PrimitiveComparator"/> and <see cref="XRangeComparator"/> classes.</para>
    /// </summary>
    public enum PrimitiveOperator : byte
    {
        /// <summary>
        ///   <para>An 'equal to' operator, <c>=</c>.</para>
        /// </summary>
        Equal,
        /// <summary>
        ///   <para>A 'greater than' operator, <c>&gt;</c>.</para>
        /// </summary>
        GreaterThan,
        /// <summary>
        ///   <para>A 'less than' operator, <c>&lt;</c>.</para>
        /// </summary>
        LessThan,
        /// <summary>
        ///   <para>A 'greater than or equal to' operator, <c>&gt;=</c>.</para>
        /// </summary>
        GreaterThanOrEqual,
        /// <summary>
        ///   <para>A 'less than or equal to' operator, <c>&lt;=</c>.</para>
        /// </summary>
        LessThanOrEqual,
    }
}

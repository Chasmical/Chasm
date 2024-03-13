using System;
using Chasm.Formatting;
using JetBrains.Annotations;

namespace Chasm.SemanticVersioning.Ranges
{
    using static PrimitiveComparator;

    /// <summary>
    ///   <para>Represents a valid <c>node-semver</c> caret version comparator.</para>
    /// </summary>
    public sealed class CaretComparator : AdvancedComparator
    {
        /// <summary>
        ///   <para>Initializes a new instance of the <see cref="CaretComparator"/> class with the specified <paramref name="operand"/>.</para>
        /// </summary>
        /// <param name="operand">The caret version comparator's operand.</param>
        /// <exception cref="ArgumentNullException"><paramref name="operand"/> is <see langword="null"/>.</exception>
        public CaretComparator(PartialVersion operand) : base(operand) { }

        /// <inheritdoc/>
        [Pure] protected override (PrimitiveComparator?, PrimitiveComparator?) ConvertToPrimitives()
        {
            throw new NotImplementedException("Need to rewrite this one completely, just to avoid any minor inconsistencies.");
        }

        /// <inheritdoc/>
        [Pure] protected internal override int CalculateLength()
            => Operand.CalculateLength() + 1; // '^'
        /// <inheritdoc/>
        protected internal override void BuildString(ref SpanBuilder sb)
        {
            sb.Append('^');
            Operand.BuildString(ref sb);
        }

    }
}

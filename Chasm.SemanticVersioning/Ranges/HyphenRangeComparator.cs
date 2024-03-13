using System;
using System.Diagnostics;
using Chasm.Formatting;
using JetBrains.Annotations;

namespace Chasm.SemanticVersioning.Ranges
{
    using static PrimitiveComparator;

    /// <summary>
    ///   <para>Represents a valid <c>node-semver</c> hyphen range version comparator.</para>
    /// </summary>
    public sealed class HyphenRangeComparator : AdvancedComparator
    {
        /// <summary>
        ///   <para>Gets the hyphen range version comparator's lower bound.</para>
        /// </summary>
        public PartialVersion From => base.Operand;
        /// <summary>
        ///   <para>Gets the hyphen range version comparator's upper bound.</para>
        /// </summary>
        public PartialVersion To { get; }

        /// <summary>
        ///   <para>Gets the hyphen range version comparator's lower bound.<br/>You should use <see cref="From"/> property instead.</para>
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [Obsolete($"You should use {nameof(From)} property instead, when you know it's an instance of the {nameof(HyphenRangeComparator)} class.")]
        public new PartialVersion Operand => base.Operand;

        /// <summary>
        ///   <para>Initializes a new instance of the <see cref="HyphenRangeComparator"/> class with the specified <paramref name="from"/> and <paramref name="to"/> operands.</para>
        /// </summary>
        /// <param name="from">The hyphen range version comparator's lower bound.</param>
        /// <param name="to">The hyphen range version comparator's upper bound.</param>
        /// <exception cref="ArgumentNullException"><paramref name="from"/> or <paramref name="to"/> is <see langword="null"/>.</exception>
        public HyphenRangeComparator(PartialVersion from, PartialVersion to) : base(from)
            => To = to ?? throw new ArgumentNullException(nameof(to));

        /// <inheritdoc/>
        [Pure] protected override (PrimitiveComparator?, PrimitiveComparator?) ConvertToPrimitives()
        {
            throw new NotImplementedException("Need to rewrite this one completely, just to avoid any minor inconsistencies.");
        }

        /// <inheritdoc/>
        [Pure] protected internal override int CalculateLength()
            => From.CalculateLength() + To.CalculateLength() + 3; // ' - '
        /// <inheritdoc/>
        protected internal override void BuildString(ref SpanBuilder sb)
        {
            From.BuildString(ref sb);
            sb.Append(" - ");
            To.BuildString(ref sb);
        }

    }
}

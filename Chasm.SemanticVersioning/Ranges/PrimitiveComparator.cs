using System;
using System.ComponentModel;
using System.Diagnostics;
using Chasm.Formatting;
using JetBrains.Annotations;

namespace Chasm.SemanticVersioning.Ranges
{
    /// <summary>
    ///   <para>Represents a valid <c>node-semver</c> primitive version comparator.</para>
    /// </summary>
    public sealed class PrimitiveComparator : Comparator
    {
        /// <summary>
        ///   <para>Returns <see langword="true"/>, since this version comparator is primitive.</para>
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [Obsolete("You already know that it's a primitive version comparator.")]
        public new bool IsPrimitive => true;
        /// <summary>
        ///   <para>Returns <see langword="false"/>, since this version comparator is not advanced.</para>
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [Obsolete("You already know that it's not an advanced version comparator.")]
        public new bool IsAdvanced => false;

        /// <summary>
        ///   <para>Gets the primitive version comparator's operand.</para>
        /// </summary>
        public SemanticVersion Operand { get; }
        /// <summary>
        ///   <para>Gets the primitive version comparator's operator.</para>
        /// </summary>
        public PrimitiveOperator Operator { get; }

        /// <summary>
        ///   <para>Initializes a new instance of the <see cref="PrimitiveComparator"/> class with the specified <paramref name="operand"/> and implicit equality operator.</para>
        /// </summary>
        /// <param name="operand">The primitive version comparator's operand.</param>
        /// <exception cref="ArgumentNullException"><paramref name="operand"/> is <see langword="null"/>.</exception>
        public PrimitiveComparator(SemanticVersion operand) : this(operand, PrimitiveOperator.ImplicitEqual) { }
        /// <summary>
        ///   <para>Initializes a new instance of the <see cref="PrimitiveComparator"/> class with the specified <paramref name="operand"/> and comparison <paramref name="operator"/>.</para>
        /// </summary>
        /// <param name="operand">The primitive version comparator's operand.</param>
        /// <param name="operator">The primitive version comparator's operator.</param>
        /// <exception cref="ArgumentNullException"><paramref name="operand"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvalidEnumArgumentException"><paramref name="operator"/> is not a valid primitive operator.</exception>
        public PrimitiveComparator(SemanticVersion operand, PrimitiveOperator @operator)
        {
            if (operand is null) throw new ArgumentNullException(nameof(operand));
            if (@operator > PrimitiveOperator.LessThanOrEqual)
                throw new InvalidEnumArgumentException(nameof(@operator), (int)@operator, typeof(PrimitiveOperator));
            Operand = operand;
            Operator = @operator;
        }

        /// <inheritdoc/>
        [Pure] public override bool CanMatchPreRelease(int major, int minor, int patch)
            => CanMatchPreRelease(Operand, major, minor, patch);
        /// <inheritdoc/>
        [Pure] protected internal override bool IsSatisfiedByCore(SemanticVersion version)
        {
            int res = version.CompareTo(Operand);
            return Operator switch
            {
                PrimitiveOperator.GreaterThan => res > 0,
                PrimitiveOperator.GreaterThanOrEqual => res >= 0,
                PrimitiveOperator.LessThan => res < 0,
                PrimitiveOperator.LessThanOrEqual => res <= 0,
                // PrimitiveOperator.Equal or PrimitiveOperator.ImplicitEqual
                _ => res == 0,
            };
        }

        /// <summary>
        ///   <para>Creates an implicit 'equal to' primitive version comparator with the specified <paramref name="operand"/>.</para>
        /// </summary>
        /// <param name="operand">The primitive version comparator's operand.</param>
        /// <returns>The new implicit 'equal to' primitive version comparator with the specified <paramref name="operand"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="operand"/> is <see langword="null"/>.</exception>
        [Pure] public static PrimitiveComparator ImplicitEqual(SemanticVersion operand)
            => new PrimitiveComparator(operand, PrimitiveOperator.ImplicitEqual);
        /// <summary>
        ///   <para>Creates an 'equal to' primitive version comparator with the specified <paramref name="operand"/>.</para>
        /// </summary>
        /// <param name="operand">The primitive version comparator's operand.</param>
        /// <returns>The new 'equal to' primitive version comparator with the specified <paramref name="operand"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="operand"/> is <see langword="null"/>.</exception>
        [Pure] public static PrimitiveComparator Equal(SemanticVersion operand)
            => new PrimitiveComparator(operand, PrimitiveOperator.Equal);
        /// <summary>
        ///   <para>Creates a 'greater than' primitive version comparator with the specified <paramref name="operand"/>.</para>
        /// </summary>
        /// <param name="operand">The primitive version comparator's operand.</param>
        /// <returns>The new 'greater than' primitive version comparator with the specified <paramref name="operand"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="operand"/> is <see langword="null"/>.</exception>
        [Pure] public static PrimitiveComparator GreaterThan(SemanticVersion operand)
            => new PrimitiveComparator(operand, PrimitiveOperator.GreaterThan);
        /// <summary>
        ///   <para>Creates a 'less than' primitive version comparator with the specified <paramref name="operand"/>.</para>
        /// </summary>
        /// <param name="operand">The primitive version comparator's operand.</param>
        /// <returns>The new 'less than' primitive version comparator with the specified <paramref name="operand"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="operand"/> is <see langword="null"/>.</exception>
        [Pure] public static PrimitiveComparator LessThan(SemanticVersion operand)
            => new PrimitiveComparator(operand, PrimitiveOperator.LessThan);
        /// <summary>
        ///   <para>Creates a 'greater than or equal to' primitive version comparator with the specified <paramref name="operand"/>.</para>
        /// </summary>
        /// <param name="operand">The primitive version comparator's operand.</param>
        /// <returns>The new 'greater than or equal to' primitive version comparator with the specified <paramref name="operand"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="operand"/> is <see langword="null"/>.</exception>
        [Pure] public static PrimitiveComparator GreaterThanOrEqual(SemanticVersion operand)
            => new PrimitiveComparator(operand, PrimitiveOperator.GreaterThanOrEqual);
        /// <summary>
        ///   <para>Creates a 'less than or equal to' primitive version comparator with the specified <paramref name="operand"/>.</para>
        /// </summary>
        /// <param name="operand">The primitive version comparator's operand.</param>
        /// <returns>The new 'less than or equal to' primitive version comparator with the specified <paramref name="operand"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="operand"/> is <see langword="null"/>.</exception>
        [Pure] public static PrimitiveComparator LessThanOrEqual(SemanticVersion operand)
            => new PrimitiveComparator(operand, PrimitiveOperator.LessThanOrEqual);

        /// <summary>
        ///   <para>Gets a primitive comparator (<c>&lt;0.0.0-0</c>) that doesn't match any versions.</para>
        /// </summary>
        public static PrimitiveComparator None { get; } = LessThan(SemanticVersion.MinValue);
        /// <summary>
        ///   <para>Gets a primitive comparator (<c>&gt;=0.0.0</c>) that matches all non-pre-release versions (or all versions, with <c>includePreReleases</c> option).</para>
        /// </summary>
        public static PrimitiveComparator All { get; } = GreaterThanOrEqual(new SemanticVersion(0, 0, 0));

        /// <inheritdoc/>
        [Pure] protected internal override int CalculateLength()
        {
            // ImplicitEqual      = (0 + 2) / 3 = 0
            // Equal              = (1 + 2) / 3 = 1, '-'
            // GreaterThan        = (2 + 2) / 3 = 1, '>'
            // LessThan           = (3 + 2) / 3 = 1, '<'
            // GreaterThanOrEqual = (4 + 2) / 3 = 2, '>='
            // LessThanOrEqual    = (5 + 2) / 3 = 2, '<='
            return Operand.CalculateLength() + ((int)Operator + 2) / 3;
        }
        /// <inheritdoc/>
        protected internal override void BuildString(ref SpanBuilder sb)
        {
            // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
            switch (Operator)
            {
                // case PrimitiveOperator.ImplicitEqual:
                    // break;
                case PrimitiveOperator.Equal:
                    sb.Append('=');
                    break;
                case PrimitiveOperator.GreaterThan:
                    sb.Append('>');
                    break;
                case PrimitiveOperator.LessThan:
                    sb.Append('<');
                    break;
                case PrimitiveOperator.GreaterThanOrEqual:
                    sb.Append('>');
                    goto case PrimitiveOperator.Equal;
                case PrimitiveOperator.LessThanOrEqual:
                    sb.Append('<');
                    goto case PrimitiveOperator.Equal;
            }
            Operand.BuildString(ref sb);
        }

    }
}

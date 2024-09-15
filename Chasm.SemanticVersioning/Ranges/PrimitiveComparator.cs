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
    public sealed class PrimitiveComparator : Comparator, IEquatable<PrimitiveComparator>
    {
        /// <summary>
        ///   <para>Returns <see langword="true"/>, since this comparator is primitive.</para>
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [Obsolete("You already know that it's a primitive comparator.")]
        public new bool IsPrimitive => true;
        /// <summary>
        ///   <para>Returns <see langword="false"/>, since this comparator is not advanced.</para>
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [Obsolete("You already know that it's not an advanced comparator.")]
        public new bool IsAdvanced => false;

        /// <summary>
        ///   <para>Gets the primitive comparator's operand.</para>
        /// </summary>
        public SemanticVersion Operand { get; }
        /// <summary>
        ///   <para>Gets the primitive comparator's operator.</para>
        /// </summary>
        public PrimitiveOperator Operator { get; }

        /// <summary>
        ///   <para>Initializes a new instance of the <see cref="PrimitiveComparator"/> class with the specified <paramref name="operand"/> and implicit equality operator.</para>
        /// </summary>
        /// <param name="operand">The primitive comparator's operand.</param>
        /// <exception cref="ArgumentNullException"><paramref name="operand"/> is <see langword="null"/>.</exception>
        public PrimitiveComparator(SemanticVersion operand) : this(operand, PrimitiveOperator.ImplicitEqual) { }
        /// <summary>
        ///   <para>Initializes a new instance of the <see cref="PrimitiveComparator"/> class with the specified <paramref name="operand"/> and comparison <paramref name="operator"/>.</para>
        /// </summary>
        /// <param name="operand">The primitive comparator's operand.</param>
        /// <param name="operator">The primitive comparator's operator.</param>
        /// <exception cref="ArgumentNullException"><paramref name="operand"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvalidEnumArgumentException"><paramref name="operator"/> is not a valid primitive operator.</exception>
        public PrimitiveComparator(SemanticVersion operand, PrimitiveOperator @operator)
        {
            ANE.ThrowIfNull(operand);
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
        ///   <para>Creates an implicit 'equal to' primitive comparator with the specified <paramref name="operand"/>.</para>
        /// </summary>
        /// <param name="operand">The primitive comparator's operand.</param>
        /// <returns>The new implicit 'equal to' primitive comparator with the specified <paramref name="operand"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="operand"/> is <see langword="null"/>.</exception>
        [Pure] public static PrimitiveComparator ImplicitEqual(SemanticVersion operand)
            => new PrimitiveComparator(operand, PrimitiveOperator.ImplicitEqual);
        /// <summary>
        ///   <para>Creates an 'equal to' primitive comparator with the specified <paramref name="operand"/>.</para>
        /// </summary>
        /// <param name="operand">The primitive comparator's operand.</param>
        /// <returns>The new 'equal to' primitive comparator with the specified <paramref name="operand"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="operand"/> is <see langword="null"/>.</exception>
        [Pure] public static PrimitiveComparator Equal(SemanticVersion operand)
            => new PrimitiveComparator(operand, PrimitiveOperator.Equal);
        /// <summary>
        ///   <para>Creates a 'greater than' primitive comparator with the specified <paramref name="operand"/>.</para>
        /// </summary>
        /// <param name="operand">The primitive comparator's operand.</param>
        /// <returns>The new 'greater than' primitive comparator with the specified <paramref name="operand"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="operand"/> is <see langword="null"/>.</exception>
        [Pure] public static PrimitiveComparator GreaterThan(SemanticVersion operand)
            => new PrimitiveComparator(operand, PrimitiveOperator.GreaterThan);
        /// <summary>
        ///   <para>Creates a 'less than' primitive comparator with the specified <paramref name="operand"/>.</para>
        /// </summary>
        /// <param name="operand">The primitive comparator's operand.</param>
        /// <returns>The new 'less than' primitive comparator with the specified <paramref name="operand"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="operand"/> is <see langword="null"/>.</exception>
        [Pure] public static PrimitiveComparator LessThan(SemanticVersion operand)
            => new PrimitiveComparator(operand, PrimitiveOperator.LessThan);
        /// <summary>
        ///   <para>Creates a 'greater than or equal to' primitive comparator with the specified <paramref name="operand"/>.</para>
        /// </summary>
        /// <param name="operand">The primitive comparator's operand.</param>
        /// <returns>The new 'greater than or equal to' primitive comparator with the specified <paramref name="operand"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="operand"/> is <see langword="null"/>.</exception>
        [Pure] public static PrimitiveComparator GreaterThanOrEqual(SemanticVersion operand)
            => new PrimitiveComparator(operand, PrimitiveOperator.GreaterThanOrEqual);
        /// <summary>
        ///   <para>Creates a 'less than or equal to' primitive comparator with the specified <paramref name="operand"/>.</para>
        /// </summary>
        /// <param name="operand">The primitive comparator's operand.</param>
        /// <returns>The new 'less than or equal to' primitive comparator with the specified <paramref name="operand"/>.</returns>
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

        /// <summary>
        ///   <para>Determines whether this primitive comparator is equal to another specified primitive comparator.<br/>Build metadata is ignored and non-numeric version components are considered equal in this comparison. For build metadata-sensitive comparison, use <see cref="SemverComparer.IncludeBuild"/>, and for version component character-sensitive comparison, use <see cref="SemverComparer.DiffWildcards"/>.</para>
        /// </summary>
        /// <param name="other">The primitive comparator to compare with this primitive comparator.</param>
        /// <returns><see langword="true"/>, if this primitive comparator is equal to <paramref name="other"/>; otherwise, <see langword="false"/>.</returns>
        [Pure] public bool Equals(PrimitiveComparator? other)
        {
            if (ReferenceEquals(this, other)) return true;
            return other is not null && Operator.Normalize() == other.Operator.Normalize() && Operand.Equals(other.Operand);
        }
        /// <summary>
        ///   <para>Determines whether this primitive comparator is equal to the specified <paramref name="obj"/>.<br/>Build metadata is ignored and non-numeric version components and implicit/explicit equality operators are considered equal in this comparison. See <see cref="SemverComparer"/> for more options.</para>
        /// </summary>
        /// <param name="obj">The object to compare with this primitive comparator.</param>
        /// <returns><see langword="true"/>, if <paramref name="obj"/> is a <see cref="PrimitiveComparator"/> instance equal to this primitive comparator; otherwise, <see langword="false"/>.</returns>
        [Pure] public override bool Equals(object? obj)
            => Equals(obj as PrimitiveComparator);
        /// <summary>
        ///   <para>Returns a hash code for this primitive comparator.<br/>Build metadata is ignored and non-numeric version components and implicit/explicit equality operators are considered equal in this comparison. See <see cref="SemverComparer"/> for more options.</para>
        /// </summary>
        /// <returns>A hash code for this primitive comparator.</returns>
        [Pure] public override int GetHashCode()
        {
            // We won't add the type hashcode here for performance reasons, since
            // this one will get called the most, and adding the type hashcode in
            // advanced comparators should avoid collisions with primitives.
            return HashCode.Combine(Operand, Operator.Normalize());
        }

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

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
    public sealed class HyphenRangeComparator : AdvancedComparator, IEquatable<HyphenRangeComparator>
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
            PrimitiveComparator? from = ConvertFrom(From);
            PrimitiveComparator? to = ConvertTo(To);

            if (from is not null && to is not null && !RangeUtility.DoComparatorsComplement(to, from))
                return (null, None);

            return (from, to);

            static PrimitiveComparator? ConvertFrom(PartialVersion from)
            {
                // x.x.x    - ... ⇒ * ...
                // x.x.x-rc - ... ⇒ * ...
                // x.1.2    - ... ⇒ * ...
                // x.1.2-rc - ... ⇒ * ...
                // (Note: node-semver ignores minor, patch and pre-releases here)
                if (!from.Major.IsNumeric) return null;

                // 1.2.3    - ... ⇒ >=1.2.3    ...
                // 1.2.3-rc - ... ⇒ >=1.2.3-rc ...

                // (Note: node-semver ignores components and pre-releases after an unspecified component)
                // 1.2.x    - ... ⇒ >=1.2.0    ...
                // 1.2.x-rc - ... ⇒ >=1.2.0    ...
                // 1.x.x    - ... ⇒ >=1.0.0    ...
                // 1.x.x-rc - ... ⇒ >=1.0.0    ...

                return GreaterThanOrEqual(RangeUtility.NodeSemverTrim(from));
            }
            static PrimitiveComparator? ConvertTo(PartialVersion to)
            {
                // ... - x.x.x    ⇒ ... *
                // ... - x.x.x-rc ⇒ ... *
                // ... - x.1.2    ⇒ ... *
                // ... - x.1.2-rc ⇒ ... *
                // (Note: node-semver ignores minor, patch and pre-releases here)
                if (!to.Major.IsNumeric) return null;

                if (!to.Minor.IsNumeric)
                {
                    // ... - 1.x.x    ⇒ ... <2.0.0-0
                    // ... - 1.x.x-rc ⇒ ... <2.0.0-0

                    int major = to.Major.AsNumber;
                    if (major == int.MaxValue) throw new InvalidOperationException(Exceptions.MajorTooBig);
                    return LessThan(new SemanticVersion(major + 1, 0, 0, SemverPreRelease.ZeroArray, null, null, null));
                }

                if (!to.Patch.IsNumeric)
                {
                    // ... - 1.2.x    ⇒ ... <1.3.0-0
                    // ... - 1.2.x-rc ⇒ ... <1.3.0-0

                    int minor = to.Minor.AsNumber;
                    if (minor == int.MaxValue) throw new InvalidOperationException(Exceptions.MinorTooBig);
                    return LessThan(new SemanticVersion(to.Major.AsNumber, minor + 1, 0, SemverPreRelease.ZeroArray, null, null, null));
                }

                // ... - 1.2.3    ⇒ ... <=1.2.3
                // ... - 1.2.3-rc ⇒ ... <=1.2.3-rc

                return LessThanOrEqual(RangeUtility.NodeSemverTrim(to));
            }
        }

        [Pure] public bool Equals(HyphenRangeComparator? other)
        {
            if (ReferenceEquals(this, other)) return true;
            return other is not null && From.Equals(other.From) && To.Equals(other.To);
        }
        [Pure] public override bool Equals(object? obj)
            => Equals(obj as HyphenRangeComparator);
        [Pure] public override int GetHashCode()
        {
            // Add the type hashcode as well to avoid collisions between different types of comparators
            return HashCode.Combine(GetType(), From, To);
        }

        /// <inheritdoc/>
        [Pure] protected internal override int CalculateLength()
            => From.CalculateLength() + To.CalculateLength() + 3; // ' - '
        /// <inheritdoc/>
        protected internal override void BuildString(ref SpanBuilder sb)
        {
            From.BuildString(ref sb);
            sb.Append(" - ".AsSpan());
            To.BuildString(ref sb);
        }

    }
}

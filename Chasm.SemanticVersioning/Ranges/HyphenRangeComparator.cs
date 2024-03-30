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
            return (ConvertFrom(From), ConvertTo(To));

            static PrimitiveComparator? ConvertFrom(PartialVersion from)
            {
                // x.x.x    - ... ⇒ * ...
                // x.x.x-rc - ... ⇒ * ...
                // x.1.2    - ... ⇒ * ...
                // x.1.2-rc - ... ⇒ * ...
                // (TODO: what about unspecified minor and patch components and pre-releases here?)
                if (!from.Major.IsNumeric) return null;

                // 1.x.x    - ... ⇒ >=1.0.0    ...
                // 1.x.x-rc - ... ⇒ >=1.0.0-rc ... (TODO: node-semver ignores pre-releases if there are unspecified components)
                if (!from.Minor.IsNumeric)
                    return GreaterThanOrEqual(new SemanticVersion(from));

                // 1.2.x    - ... ⇒ >=1.2.0    ...
                // 1.2.x-rc - ... ⇒ >=1.2.0-rc ... (TODO: node-semver ignores pre-releases if there are unspecified components)
                if (!from.Patch.IsNumeric)
                    return GreaterThanOrEqual(new SemanticVersion(from));

                // 1.2.3    - ... ⇒ >=1.2.3    ...
                // 1.2.3-rc - ... ⇒ >=1.2.3-rc ...
                return GreaterThanOrEqual(new SemanticVersion(from));
            }
            static PrimitiveComparator? ConvertTo(PartialVersion to)
            {
                // ... - x.x.x    ⇒ ... *
                // ... - x.x.x-rc ⇒ ... *
                // ... - x.1.2    ⇒ ... *
                // ... - x.1.2-rc ⇒ ... *
                // (TODO: what about unspecified minor and patch components and pre-releases here?)
                if (!to.Major.IsNumeric) return null;

                // ... - 1.x.x    ⇒ ... <2.0.0-0
                // ... - 1.x.x-rc ⇒ ... <2.0.0-0
                if (!to.Minor.IsNumeric)
                {
                    int major = to.Major.AsNumber;
                    if (major == int.MaxValue) throw new InvalidOperationException(Exceptions.MajorTooBig);
                    return LessThan(new SemanticVersion(major + 1, 0, 0, SemverPreRelease.ZeroArray, null, default));
                }

                // ... - 1.2.x    ⇒ ... <1.3.0-0
                // ... - 1.2.x-rc ⇒ ... <1.3.0-0
                if (!to.Patch.IsNumeric)
                {
                    int minor = to.Minor.AsNumber;
                    if (minor == int.MaxValue) throw new InvalidOperationException(Exceptions.MinorTooBig);
                    return LessThan(new SemanticVersion(to.Major.AsNumber, minor + 1, 0, SemverPreRelease.ZeroArray, null, default));
                }

                // ... - 1.2.3    ⇒ ... <=1.2.3
                // ... - 1.2.3-rc ⇒ ... <=1.2.3-rc
                return LessThanOrEqual(new SemanticVersion(to));
            }
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

        [Pure] public bool Equals(HyphenRangeComparator? other)
            => other is not null && From.Equals(other.From) && To.Equals(other.To);
        [Pure] public override bool Equals(Comparator? comparator)
            => Equals(comparator as HyphenRangeComparator);
        [Pure] public override bool Equals(object? obj)
            => Equals(obj as HyphenRangeComparator);
        [Pure] public override int GetHashCode()
        {
            // Note: GetType() is not needed here, since GetHashCode() hashes two operands instead of just one.
            return HashCode.Combine(From, To);
        }

    }
}

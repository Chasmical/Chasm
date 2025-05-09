﻿using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Chasm.Formatting;
using JetBrains.Annotations;

namespace Chasm.SemanticVersioning.Ranges
{
    /// <summary>
    ///   <para>Represents a valid <c>node-semver</c> X-Range version comparator.</para>
    /// </summary>
    public sealed class XRangeComparator : AdvancedComparator, IEquatable<XRangeComparator>
    {
        /// <summary>
        ///   <para>Gets the X-Range comparator's operator.</para>
        /// </summary>
        public PrimitiveOperator Operator { get; }

        /// <summary>
        ///   <para>Initializes a new instance of the <see cref="XRangeComparator"/> class with the specified <paramref name="operand"/> and implicit equality operator.</para>
        /// </summary>
        /// <param name="operand">The X-Range comparator's operand.</param>
        /// <exception cref="ArgumentNullException"><paramref name="operand"/> is <see langword="null"/>.</exception>
        public XRangeComparator(PartialVersion operand) : this(operand, PrimitiveOperator.ImplicitEqual) { }
        /// <summary>
        ///   <para>Initializes a new instance of the <see cref="XRangeComparator"/> class with the specified <paramref name="primitiveComparator"/>'s operand and operator.</para>
        /// </summary>
        /// <param name="primitiveComparator">The primitive comparator to get the operand and operator from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="primitiveComparator"/> is <see langword="null"/>.</exception>
        public XRangeComparator(PrimitiveComparator primitiveComparator)
            : this((primitiveComparator ?? throw new ArgumentNullException(nameof(primitiveComparator))).Operand, primitiveComparator.Operator) { }
        /// <summary>
        ///   <para>Initializes a new instance of the <see cref="XRangeComparator"/> class with the specified <paramref name="operand"/> and comparison <paramref name="operator"/>.</para>
        /// </summary>
        /// <param name="operand">The X-Range comparator's operand.</param>
        /// <param name="operator">The X-Range comparator's operator.</param>
        /// <exception cref="ArgumentNullException"><paramref name="operand"/> is <see langword="null"/>.</exception>
        public XRangeComparator(PartialVersion operand, PrimitiveOperator @operator) : base(operand)
        {
            if (@operator > PrimitiveOperator.LessThanOrEqual)
                throw new InvalidEnumArgumentException(nameof(@operator), (int)@operator, typeof(PrimitiveOperator));
            Operator = @operator;
        }

        /// <summary>
        ///   <para>Defines an implicit conversion of a partial version to an X-Range comparator.</para>
        /// </summary>
        /// <param name="partialVersion">The partial version to convert.</param>
        [Pure] [return: NotNullIfNotNull(nameof(partialVersion))]
        public static implicit operator XRangeComparator?(PartialVersion? partialVersion)
            => partialVersion is null ? null : new XRangeComparator(partialVersion, PrimitiveOperator.ImplicitEqual);
        /// <summary>
        ///   <para>Defines an implicit conversion of a primitive comparator to an X-Range comparator.</para>
        /// </summary>
        /// <param name="primitiveComparator">The primitive comparator to convert.</param>
        [Pure] [return: NotNullIfNotNull(nameof(primitiveComparator))]
        public static implicit operator XRangeComparator?(PrimitiveComparator? primitiveComparator)
            => primitiveComparator is null ? null : new XRangeComparator(primitiveComparator);

        /// <summary>
        ///   <para>Defines an explicit conversion of an X-Range comparator to a primitive comparator, replacing wildcards in the operand's version components with zeroes.</para>
        /// </summary>
        /// <param name="xRangeComparator">The X-Range comparator to convert.</param>
        [Pure] [return: NotNullIfNotNull(nameof(xRangeComparator))]
        public static explicit operator PrimitiveComparator?(XRangeComparator? xRangeComparator)
            => xRangeComparator is null ? null : new PrimitiveComparator((SemanticVersion)xRangeComparator.Operand, xRangeComparator.Operator);

        /// <inheritdoc/>
        [Pure] protected override (PrimitiveComparator?, PrimitiveComparator?) ConvertToPrimitives()
        {
            if (!Operand.Major.IsNumeric)
            {
                // >=x.x.x, <=x.x.x, =x.x.x ⇒ *
                //  >x.x.x,  <x.x.x         ⇒ <0.0.0-0

                bool isExclusive = Operator is PrimitiveOperator.GreaterThan or PrimitiveOperator.LessThan;
                return (null, isExclusive ? PrimitiveComparator.None : null);
            }
            // simple primitive comparators
            if (Operand.Minor.IsNumeric && Operand.Patch.IsNumeric)
            {
                PrimitiveComparator primitive = new PrimitiveComparator(RangeUtility.NodeSemverTrim(Operand), Operator);
                return Operator is PrimitiveOperator.LessThan or PrimitiveOperator.LessThanOrEqual ? (null, primitive) : (primitive, null);
            }
            // at this point, major is numeric, and either minor or patch isn't numeric

            SemanticVersion version;
            int major = Operand.Major.AsNumber;
            int minor;
            // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
            switch (Operator)
            {
                case PrimitiveOperator.GreaterThan:

                    if (!Operand.Minor.IsNumeric)
                    {
                        // >1.x.3    ⇒ >=2.0.0
                        // >1.x.3-rc ⇒ >=2.0.0

                        if (major == int.MaxValue) throw new InvalidOperationException(Exceptions.MajorTooBig);
                        version = new SemanticVersion(major + 1, 0, 0, null, null, null, null);
                    }
                    else // if (!Operand.Patch.IsNumeric)
                    {
                        // >1.2.x    ⇒ >=1.3.0
                        // >1.2.x-rc ⇒ >=1.3.0

                        minor = Operand.Minor.AsNumber;
                        if (minor == int.MaxValue) throw new InvalidOperationException(Exceptions.MinorTooBig);
                        version = new SemanticVersion(major, minor + 1, 0, null, null, null, null);
                    }
                    return (PrimitiveComparator.GreaterThanOrEqual(version), null);

                case PrimitiveOperator.LessThanOrEqual:

                    if (!Operand.Minor.IsNumeric)
                    {
                        // (Note: node-semver ignores components and pre-releases after an unspecified component)
                        // <=1.x.3    ⇒ <2.0.0-0
                        // <=1.x.3-rc ⇒ <2.0.0-0

                        if (major == int.MaxValue) throw new InvalidOperationException(Exceptions.MajorTooBig);
                        version = new SemanticVersion(major + 1, 0, 0, SemverPreRelease.ZeroArray, null, null, null);
                    }
                    else // if (!Operand.Patch.IsNumeric)
                    {
                        // (Note: node-semver ignores components and pre-releases after an unspecified component)
                        // <=1.2.x    ⇒ <1.3.0-0
                        // <=1.2.x-rc ⇒ <1.3.0-0

                        minor = Operand.Minor.AsNumber;
                        if (minor == int.MaxValue) throw new InvalidOperationException(Exceptions.MinorTooBig);
                        version = new SemanticVersion(major, minor + 1, 0, SemverPreRelease.ZeroArray, null, null, null);
                    }
                    return (null, PrimitiveComparator.LessThan(version));

                case PrimitiveOperator.GreaterThanOrEqual:

                    // (Note: node-semver ignores components and pre-releases after an unspecified component)
                    // >=1.x.3    ⇒ >=1.0.0
                    // >=1.x.3-rc ⇒ >=1.0.0
                    // >=1.2.x    ⇒ >=1.2.0
                    // >=1.2.x-rc ⇒ >=1.2.0

                    version = new SemanticVersion(major, (int)Operand.Minor, 0, null, null, null, null);
                    return (PrimitiveComparator.GreaterThanOrEqual(version), null);

                case PrimitiveOperator.LessThan:

                    // (Note: node-semver ignores components and pre-releases after an unspecified component)
                    // <1.x.3    ⇒ <1.0.0-0
                    // <1.x.3-rc ⇒ <1.0.0-0
                    // <1.2.x    ⇒ <1.2.0-0
                    // <1.2.x-rc ⇒ <1.2.0-0

                    version = new SemanticVersion(major, (int)Operand.Minor, 0, SemverPreRelease.ZeroArray, null, null, null);
                    return (null, PrimitiveComparator.LessThan(version));

                default:
                // case PrimitiveOperator.Equal:
                // case PrimitiveOperator.ImplicitEqual:

                    SemanticVersion version2;
                    if (!Operand.Minor.IsNumeric)
                    {
                        // (Note: node-semver ignores components and pre-releases after an unspecified component)
                        // =1.x.3    ⇒ >=1.0.0    <2.0.0-0
                        // =1.x.3-rc ⇒ >=1.0.0    <2.0.0-0

                        if (major == int.MaxValue) throw new InvalidOperationException(Exceptions.MajorTooBig);
                        version = new SemanticVersion(major, 0, 0, null, null, null, null);
                        version2 = new SemanticVersion(major + 1, 0, 0, SemverPreRelease.ZeroArray, null, null, null);
                    }
                    else // if (!Operand.Patch.IsNumeric)
                    {
                        // (Note: node-semver ignores components and pre-releases after an unspecified component)
                        // =1.2.x    ⇒ >=1.2.0    <1.3.0-0
                        // =1.2.x-rc ⇒ >=1.2.0    <1.3.0-0

                        minor = Operand.Minor.AsNumber;
                        if (minor == int.MaxValue) throw new InvalidOperationException(Exceptions.MinorTooBig);
                        version = new SemanticVersion(major, minor, 0, null, null, null, null);
                        version2 = new SemanticVersion(major, minor + 1, 0, SemverPreRelease.ZeroArray, null, null, null);
                    }
                    return (PrimitiveComparator.GreaterThanOrEqual(version), PrimitiveComparator.LessThan(version2));

            }
        }

        /// <summary>
        ///   <para>Creates an implicit 'equal to' X-Range comparator with the specified <paramref name="operand"/>.</para>
        /// </summary>
        /// <param name="operand">The X-Range comparator's operand.</param>
        /// <returns>The new implicit 'equal to' X-Range comparator with the specified <paramref name="operand"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="operand"/> is <see langword="null"/>.</exception>
        [Pure] public static XRangeComparator ImplicitEqual(PartialVersion operand)
            => new XRangeComparator(operand, PrimitiveOperator.ImplicitEqual);
        /// <summary>
        ///   <para>Creates an 'equal to' X-Range comparator with the specified <paramref name="operand"/>.</para>
        /// </summary>
        /// <param name="operand">The X-Range comparator's operand.</param>
        /// <returns>The new 'equal to' X-Range comparator with the specified <paramref name="operand"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="operand"/> is <see langword="null"/>.</exception>
        [Pure] public static XRangeComparator Equal(PartialVersion operand)
            => new XRangeComparator(operand, PrimitiveOperator.Equal);
        /// <summary>
        ///   <para>Creates a 'greater than' X-Range comparator with the specified <paramref name="operand"/>.</para>
        /// </summary>
        /// <param name="operand">The X-Range comparator's operand.</param>
        /// <returns>The new 'greater than' X-Range comparator with the specified <paramref name="operand"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="operand"/> is <see langword="null"/>.</exception>
        [Pure] public static XRangeComparator GreaterThan(PartialVersion operand)
            => new XRangeComparator(operand, PrimitiveOperator.GreaterThan);
        /// <summary>
        ///   <para>Creates a 'less than' X-Range comparator with the specified <paramref name="operand"/>.</para>
        /// </summary>
        /// <param name="operand">The X-Range comparator's operand.</param>
        /// <returns>The new 'less than' X-Range comparator with the specified <paramref name="operand"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="operand"/> is <see langword="null"/>.</exception>
        [Pure] public static XRangeComparator LessThan(PartialVersion operand)
            => new XRangeComparator(operand, PrimitiveOperator.LessThan);
        /// <summary>
        ///   <para>Creates a 'greater than or equal to' X-Range comparator with the specified <paramref name="operand"/>.</para>
        /// </summary>
        /// <param name="operand">The X-Range comparator's operand.</param>
        /// <returns>The new 'greater than or equal to' X-Range comparator with the specified <paramref name="operand"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="operand"/> is <see langword="null"/>.</exception>
        [Pure] public static XRangeComparator GreaterThanOrEqual(PartialVersion operand)
            => new XRangeComparator(operand, PrimitiveOperator.GreaterThanOrEqual);
        /// <summary>
        ///   <para>Creates a 'less than or equal to' X-Range comparator with the specified <paramref name="operand"/>.</para>
        /// </summary>
        /// <param name="operand">The X-Range comparator's operand.</param>
        /// <returns>The new 'less than or equal to' X-Range comparator with the specified <paramref name="operand"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="operand"/> is <see langword="null"/>.</exception>
        [Pure] public static XRangeComparator LessThanOrEqual(PartialVersion operand)
            => new XRangeComparator(operand, PrimitiveOperator.LessThanOrEqual);

        /// <summary>
        ///   <para>Gets an X-Range comparator (<c>*</c>) that matches all non-pre-release versions (or all versions, with <c>includePreReleases</c> option).</para>
        /// </summary>
        public static XRangeComparator All { get; } = new XRangeComparator(PartialVersion.OneStar);

        /// <summary>
        ///   <para>Determines whether this X-Range comparator is equal to another specified X-Range comparator.<br/>Build metadata is ignored and non-numeric version components and implicit/explicit equality operators are considered equal in this comparison. See <see cref="SemverComparer"/> for more options.</para>
        /// </summary>
        /// <param name="other">The X-Range comparator to compare with this X-Range comparator.</param>
        /// <returns><see langword="true"/>, if this X-Range comparator is equal to <paramref name="other"/>; otherwise, <see langword="false"/>.</returns>
        [Pure] public bool Equals([NotNullWhen(true)] XRangeComparator? other)
        {
            if (ReferenceEquals(this, other)) return true;
            return other is not null && Operator.Normalize() == other.Operator.Normalize() && Operand.Equals(other.Operand);
        }
        /// <summary>
        ///   <para>Determines whether this X-Range comparator is equal to the specified <paramref name="obj"/>.<br/>Build metadata is ignored and non-numeric version components and implicit/explicit equality operators are considered equal in this comparison. See <see cref="SemverComparer"/> for more options.</para>
        /// </summary>
        /// <param name="obj">The object to compare with this X-Range comparator.</param>
        /// <returns><see langword="true"/>, if <paramref name="obj"/> is a <see cref="XRangeComparator"/> instance equal to this X-Range comparator; otherwise, <see langword="false"/>.</returns>
        [Pure] public override bool Equals([NotNullWhen(true)] object? obj)
            => Equals(obj as XRangeComparator);
        /// <summary>
        ///   <para>Returns a hash code for this X-Range comparator.<br/>Build metadata is ignored and non-numeric version components and implicit/explicit equality operators are considered equal in this comparison. See <see cref="SemverComparer"/> for more options.</para>
        /// </summary>
        /// <returns>A hash code for this X-Range comparator.</returns>
        [Pure] public override int GetHashCode()
        {
            // Add the type hashcode as well to avoid collisions between different types of comparators
            return HashCode.Combine(GetType(), Operand, Operator.Normalize());
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

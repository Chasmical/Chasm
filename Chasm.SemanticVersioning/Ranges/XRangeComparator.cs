using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Chasm.Formatting;
using JetBrains.Annotations;

namespace Chasm.SemanticVersioning.Ranges
{
    /// <summary>
    ///   <para>Represents a valid <c>node-semver</c> X-Range version comparator.</para>
    /// </summary>
    public sealed class XRangeComparator : AdvancedComparator
    {
        /// <summary>
        ///   <para>Gets the X-Range version comparator's operator.</para>
        /// </summary>
        public PrimitiveOperator Operator { get; }

        /// <summary>
        ///   <para>Initializes a new instance of the <see cref="XRangeComparator"/> class with the specified <paramref name="operand"/>.</para>
        /// </summary>
        /// <param name="operand">The X-Range version comparator's operand.</param>
        /// <exception cref="ArgumentNullException"><paramref name="operand"/> is <see langword="null"/>.</exception>
        public XRangeComparator(PartialVersion operand) : this(operand, PrimitiveOperator.Equal) { }
        /// <summary>
        ///   <para>Initializes a new instance of the <see cref="XRangeComparator"/> class with the specified <paramref name="primitiveComparator"/>'s operand and operator.</para>
        /// </summary>
        /// <param name="primitiveComparator">The primitive version comparator to get the operand and operator from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="primitiveComparator"/> is <see langword="null"/>.</exception>
        public XRangeComparator(PrimitiveComparator primitiveComparator)
            : this((primitiveComparator ?? throw new ArgumentNullException(nameof(primitiveComparator))).Operand, primitiveComparator.Operator) { }
        /// <summary>
        ///   <para>Initializes a new instance of the <see cref="XRangeComparator"/> class with the specified <paramref name="operand"/>.</para>
        /// </summary>
        /// <param name="operand">The X-Range version comparator's operand.</param>
        /// <param name="operator">The X-Range version comparator's operator.</param>
        /// <exception cref="ArgumentNullException"><paramref name="operand"/> is <see langword="null"/>.</exception>
        public XRangeComparator(PartialVersion operand, PrimitiveOperator @operator) : base(operand)
        {
            if (@operator > PrimitiveOperator.LessThanOrEqual)
                throw new InvalidEnumArgumentException(nameof(@operator), (int)@operator, typeof(PrimitiveOperator));
            Operator = @operator;
        }

        /// <summary>
        ///   <para>Defines an implicit conversion of a partial version to an X-Range version comparator.</para>
        /// </summary>
        /// <param name="partialVersion">The partial version to convert.</param>
        [Pure] [return: NotNullIfNotNull(nameof(partialVersion))]
        public static implicit operator XRangeComparator?(PartialVersion? partialVersion)
            => partialVersion is null ? null : new XRangeComparator(partialVersion, PrimitiveOperator.Equal);
        /// <summary>
        ///   <para>Defines an implicit conversion of a primitive version comparator to an X-Range version comparator.</para>
        /// </summary>
        /// <param name="primitiveComparator">The primitive version comparator to convert.</param>
        [Pure] [return: NotNullIfNotNull(nameof(primitiveComparator))]
        public static implicit operator XRangeComparator?(PrimitiveComparator? primitiveComparator)
            => primitiveComparator is null ? null : new XRangeComparator(primitiveComparator);

        /// <summary>
        ///   <para>Defines an explicit conversion of an X-Range version comparator to a primitive version comparator, replacing wildcards in the operand's version components with zeroes.</para>
        /// </summary>
        /// <param name="xRangeComparator">The X-Range version comparator to convert.</param>
        [Pure] [return: NotNullIfNotNull(nameof(xRangeComparator))]
        public static explicit operator PrimitiveComparator?(XRangeComparator? xRangeComparator)
            => xRangeComparator is null ? null : new PrimitiveComparator(new SemanticVersion(xRangeComparator.Operand), xRangeComparator.Operator);

        /// <inheritdoc/>
        [Pure] protected override (PrimitiveComparator?, PrimitiveComparator?) ConvertToPrimitives()
        {
            if (!Operand.Major.IsNumeric)
            {
                if (Operator is PrimitiveOperator.GreaterThan or PrimitiveOperator.LessThan)
                    return (PrimitiveComparator.LessThan(SemanticVersion.MinValue), null);
                return (null, null);
            }
            if (Operand.Minor.IsNumeric && Operand.Patch.IsNumeric)
                return (new PrimitiveComparator(new SemanticVersion(Operand), Operator), null);

            int major = Operand.Major.AsNumber;
            switch (Operator)
            {
                case PrimitiveOperator.GreaterThan:
                    // >1.2, >1.2.x ⇒ >=1.3.0-0
                    // >1.2.x-rc ⇒ >=1.3.0-0 (pre-releases don't matter)
                    // >1, >1.x.x ⇒ >=2.0.0-0
                    // >1.x.3 ⇒ >=2.0.0-0 (x can technically be greater than 2147483647)
                    if (Operand.Minor.IsNumeric)
                    {
                        int minor = Operand.Minor.AsNumber;
                        if (minor == int.MaxValue) throw new InvalidOperationException(Exceptions.MinorTooBig);
                        return (PrimitiveComparator.GreaterThanOrEqual(
                                    new SemanticVersion(major, minor + 1, 0, SemverPreRelease.ZeroArray, null, default)),
                                null);
                    }
                    if (major == int.MaxValue) throw new InvalidOperationException(Exceptions.MajorTooBig);
                    return (PrimitiveComparator.GreaterThanOrEqual(
                                new SemanticVersion(major + 1, 0, 0, SemverPreRelease.ZeroArray, null, default)),
                            null);

                case PrimitiveOperator.LessThanOrEqual:
                    // <=1.2, <=1.2.x ⇒ <1.3.0-0
                    // <=1.2.x-rc ⇒ <1.3.0-0 (pre-releases don't matter)
                    // <=1, <=1.x.x ⇒ <2.0.0-0
                    // <=1.x.3 ⇒ <2.0.0-0 (x can technically be greater than 2147483647)
                    if (Operand.Minor.IsNumeric)
                    {
                        int minor = Operand.Minor.AsNumber;
                        if (minor == int.MaxValue) throw new InvalidOperationException(Exceptions.MinorTooBig);
                        return (PrimitiveComparator.LessThan(
                                    new SemanticVersion(major, minor + 1, 0, SemverPreRelease.ZeroArray, null, default)),
                                null);
                    }
                    if (major == int.MaxValue) throw new InvalidOperationException(Exceptions.MajorTooBig);
                    return (PrimitiveComparator.LessThan(
                                new SemanticVersion(major + 1, 0, 0, SemverPreRelease.ZeroArray, null, default)),
                            null);

                case PrimitiveOperator.GreaterThanOrEqual:
                    // >=1.2, >=1.2.x ⇒ >=1.2.0
                    // >=1.2.x-rc ⇒ >=1.2.0-rc (pre-releases carry over)
                    // >=1, >=1.x.x ⇒ >=1.0.0
                    // >=1.x.x-rc ⇒ >=1.0.0-rc
                    // >=1.x.3 ⇒ >=1.0.3 TODO? (non-node-semver compliant)
                    if (Operand.Minor.IsNumeric)
                    {
                        return (PrimitiveComparator.GreaterThanOrEqual(
                                    new SemanticVersion(major, Operand.Minor.AsNumber, 0, Operand._preReleases, null, default)),
                                null);
                    }
                    return (PrimitiveComparator.GreaterThanOrEqual(
                                new SemanticVersion(major, 0, 0, Operand._preReleases, null, default)),
                            null);

                case PrimitiveOperator.LessThan:
                    // <1.2, <1.2.x ⇒ <1.2.0
                    // <1.2.x-rc ⇒ <1.2.0-rc (pre-releases carry over)
                    // <1, <1.x.x ⇒ <1.0.0
                    // <1.x.x-rc ⇒ <1.0.0-rc
                    // <1.x.3 ⇒ <1.0.3 TODO? (non-node-semver compliant)
                    if (Operand.Minor.IsNumeric)
                    {
                        return (PrimitiveComparator.LessThan(
                                    new SemanticVersion(major, Operand.Minor.AsNumber, 0, Operand._preReleases, null, default)),
                                null);
                    }
                    return (PrimitiveComparator.LessThan(
                                new SemanticVersion(major, 0, 0, Operand._preReleases, null, default)),
                            null);

                case PrimitiveOperator.Equal:
                    // =1.2, =1.2.x ⇒ >=1.2.0 <1.3.0-0
                    // =1.2.x-rc ⇒ >=1.2.0-rc <1.3.0-0 (mixed)
                    // =1, =1.x.x ⇒ >=1.0.0 <2.0.0-0
                    // =1.x.x-rc ⇒ >=1.0.0-rc <2.0.0-0
                    // =1.x.3 ⇒ ??? TODO (unimplementable?)
                    if (Operand.Minor.IsNumeric)
                    {
                        int minor = Operand.Minor.AsNumber;
                        if (minor == int.MaxValue) throw new InvalidOperationException(Exceptions.MinorTooBig);
                        return (
                            PrimitiveComparator.GreaterThanOrEqual(
                                new SemanticVersion(major, minor, 0, Operand._preReleases, null, default)),
                            PrimitiveComparator.LessThan(
                                new SemanticVersion(major, minor + 1, 0, SemverPreRelease.ZeroArray, null, default))
                        );
                    }
                    if (major == int.MaxValue) throw new InvalidOperationException(Exceptions.MajorTooBig);
                    return (
                        PrimitiveComparator.GreaterThanOrEqual(
                            new SemanticVersion(major, 0, 0, Operand._preReleases, null, default)),
                        PrimitiveComparator.LessThan(
                            new SemanticVersion(major + 1, 0, 0, SemverPreRelease.ZeroArray, null, default))
                    );

                default:
#if NET7_0_OR_GREATER
                    throw new System.Diagnostics.UnreachableException();
#else
                    throw new InvalidOperationException();
#endif
            }
        }

        /// <summary>
        ///   <para>Creates an 'equal to' X-Range version comparator with the specified <paramref name="operand"/>.</para>
        /// </summary>
        /// <param name="operand">The X-Range version comparator's operand.</param>
        /// <returns>The new 'equal to' X-Range version comparator with the specified <paramref name="operand"/>.</returns>
        [Pure] public static XRangeComparator Equal(PartialVersion operand)
            => new XRangeComparator(operand, PrimitiveOperator.Equal);
        /// <summary>
        ///   <para>Creates a 'greater than' X-Range version comparator with the specified <paramref name="operand"/>.</para>
        /// </summary>
        /// <param name="operand">The X-Range version comparator's operand.</param>
        /// <returns>The new 'greater than' X-Range version comparator with the specified <paramref name="operand"/>.</returns>
        [Pure] public static XRangeComparator GreaterThan(PartialVersion operand)
            => new XRangeComparator(operand, PrimitiveOperator.GreaterThan);
        /// <summary>
        ///   <para>Creates a 'less than' X-Range version comparator with the specified <paramref name="operand"/>.</para>
        /// </summary>
        /// <param name="operand">The X-Range version comparator's operand.</param>
        /// <returns>The new 'less than' X-Range version comparator with the specified <paramref name="operand"/>.</returns>
        [Pure] public static XRangeComparator LessThan(PartialVersion operand)
            => new XRangeComparator(operand, PrimitiveOperator.LessThan);
        /// <summary>
        ///   <para>Creates a 'greater than or equal to' X-Range version comparator with the specified <paramref name="operand"/>.</para>
        /// </summary>
        /// <param name="operand">The X-Range version comparator's operand.</param>
        /// <returns>The new 'greater than or equal to' X-Range version comparator with the specified <paramref name="operand"/>.</returns>
        [Pure] public static XRangeComparator GreaterThanOrEqual(PartialVersion operand)
            => new XRangeComparator(operand, PrimitiveOperator.GreaterThanOrEqual);
        /// <summary>
        ///   <para>Creates a 'less than or equal to' X-Range version comparator with the specified <paramref name="operand"/>.</para>
        /// </summary>
        /// <param name="operand">The X-Range version comparator's operand.</param>
        /// <returns>The new 'less than or equal to' X-Range version comparator with the specified <paramref name="operand"/>.</returns>
        [Pure] public static XRangeComparator LessThanOrEqual(PartialVersion operand)
            => new XRangeComparator(operand, PrimitiveOperator.LessThanOrEqual);

        /// <inheritdoc/>
        [Pure] protected internal override int CalculateLength()
            => Operand.CalculateLength() + (Operator > PrimitiveOperator.LessThan ? 2 : 1);
        /// <inheritdoc/>
        protected internal override void BuildString(ref SpanBuilder sb)
        {
            switch (Operator)
            {
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

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
    public sealed class XRangeComparator : AdvancedComparator
    {
        /// <summary>
        ///   <para>Gets the X-Range version comparator's operator.</para>
        /// </summary>
        public PrimitiveOperator Operator { get; }

        /// <summary>
        ///   <para>Initializes a new instance of the <see cref="XRangeComparator"/> class with the specified <paramref name="operand"/> and implicit equality operator.</para>
        /// </summary>
        /// <param name="operand">The X-Range version comparator's operand.</param>
        /// <exception cref="ArgumentNullException"><paramref name="operand"/> is <see langword="null"/>.</exception>
        public XRangeComparator(PartialVersion operand) : this(operand, PrimitiveOperator.ImplicitEqual) { }
        /// <summary>
        ///   <para>Initializes a new instance of the <see cref="XRangeComparator"/> class with the specified <paramref name="primitiveComparator"/>'s operand and operator.</para>
        /// </summary>
        /// <param name="primitiveComparator">The primitive version comparator to get the operand and operator from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="primitiveComparator"/> is <see langword="null"/>.</exception>
        public XRangeComparator(PrimitiveComparator primitiveComparator)
            : this((primitiveComparator ?? throw new ArgumentNullException(nameof(primitiveComparator))).Operand, primitiveComparator.Operator) { }
        /// <summary>
        ///   <para>Initializes a new instance of the <see cref="XRangeComparator"/> class with the specified <paramref name="operand"/> and comparison <paramref name="operator"/>.</para>
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
            => partialVersion is null ? null : new XRangeComparator(partialVersion, PrimitiveOperator.ImplicitEqual);
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
            //          >x.x.x,  <x.x.x ⇒ <0.0.0-0
            // =x.x.x, >=x.x.x, <=x.x.x ⇒ *
            if (!Operand.Major.IsNumeric)
            {
                if (Operator is PrimitiveOperator.GreaterThan or PrimitiveOperator.LessThan)
                    return (null, PrimitiveComparator.None);
                return (null, null);
            }
            // simple primitive comparators
            if (Operand.Minor.IsNumeric && Operand.Patch.IsNumeric)
            {
                PrimitiveComparator primitive = new PrimitiveComparator(new SemanticVersion(Operand), Operator);
                bool isLessThan = primitive.Operator is PrimitiveOperator.LessThan or PrimitiveOperator.LessThanOrEqual;
                return isLessThan ? (null, primitive) : (primitive, null);
            }
            // at this point, major is numeric, and either minor or patch isn't numeric

            SemanticVersion version;
            int major = Operand.Major.AsNumber;
            int minor;
            switch (Operator)
            {
                case PrimitiveOperator.GreaterThan:

                    if (!Operand.Minor.IsNumeric)
                    {
                        // >1.x.3    ⇒ >=2.0.0 (Note: don't include '-0' here, since that would opt in into 2.0.0 pre-releases)
                        // >1.x.3-rc ⇒ >=2.0.0 (Note: ^^^)
                        if (major == int.MaxValue) throw new InvalidOperationException(Exceptions.MajorTooBig);
                        version = new SemanticVersion(major + 1, 0, 0, SemverPreRelease.ZeroArray, null, default);
                    }
                    else // if (!Operand.Patch.IsNumeric)
                    {
                        // >1.2.x    ⇒ >=1.3.0 (Note: don't include '-0' here, since that would opt in into 1.3.0 pre-releases)
                        // >1.2.x-rc ⇒ >=1.3.0 (Note: ^^^)
                        minor = Operand.Minor.AsNumber;
                        if (minor == int.MaxValue) throw new InvalidOperationException(Exceptions.MinorTooBig);
                        version = new SemanticVersion(major, minor + 1, 0, SemverPreRelease.ZeroArray, null, default);
                    }
                    return (PrimitiveComparator.GreaterThanOrEqual(version), null);

                case PrimitiveOperator.LessThanOrEqual:

                    if (!Operand.Minor.IsNumeric)
                    {
                        // <=1.x.3    ⇒ <2.0.0-0
                        // <=1.x.3-rc ⇒ <2.0.0-0
                        if (major == int.MaxValue) throw new InvalidOperationException(Exceptions.MajorTooBig);
                        version = new SemanticVersion(major + 1, 0, 0, SemverPreRelease.ZeroArray, null, default);
                    }
                    else // if (!Operand.Patch.IsNumeric)
                    {
                        // <=1.2.x    ⇒ <1.3.0-0
                        // <=1.2.x-rc ⇒ <1.3.0-0
                        minor = Operand.Minor.AsNumber;
                        if (minor == int.MaxValue) throw new InvalidOperationException(Exceptions.MinorTooBig);
                        version = new SemanticVersion(major, minor + 1, 0, SemverPreRelease.ZeroArray, null, default);
                    }
                    return (null, PrimitiveComparator.LessThan(version));

                case PrimitiveOperator.GreaterThanOrEqual:

                    // >=1.x.3    ⇒ >=1.0.3    (TODO: node-semver ignores specified patch if minor is unspecified)
                    // >=1.x.3-rc ⇒ >=1.0.3-rc (TODO: node-semver ignores specified patch and pre-releases if minor is unspecified)
                    // >=1.2.x    ⇒ >=1.2.0
                    // >=1.2.x-rc ⇒ >=1.2.0-rc (TODO: node-semver ignores pre-releases if there are unspecified components)
                    version = new SemanticVersion(Operand);
                    return (PrimitiveComparator.GreaterThanOrEqual(version), null);

                case PrimitiveOperator.LessThan:

                    // <1.x.3    ⇒ <1.0.3    (TODO: node-semver ignores specified patch if minor is unspecified)
                    // <1.x.3-rc ⇒ <1.0.3-rc (TODO: node-semver ignores specified patch and pre-releases if minor is unspecified)
                    // <1.2.x    ⇒ <1.2.0
                    // <1.2.x-rc ⇒ <1.2.0-rc (TODO: node-semver ignores pre-releases if there are unspecified components)
                    version = new SemanticVersion(Operand);
                    return (null, PrimitiveComparator.LessThan(version));

                default:
                // case PrimitiveOperator.Equal:
                // case PrimitiveOperator.ImplicitEqual:

                    SemanticVersion version2;
                    if (!Operand.Minor.IsNumeric)
                    {
                        // =1.x.3    ⇒ >=1.0.3    <2.0.0-0 (TODO: node-semver ignores specified patch if minor is unspecified)
                        // =1.x.3-rc ⇒ >=1.0.3-rc <2.0.0-0 (TODO: node-semver ignores specified patch and pre-releases if minor is unspecified)
                        if (major == int.MaxValue) throw new InvalidOperationException(Exceptions.MajorTooBig);
                        version = new SemanticVersion(Operand);
                        version2 = new SemanticVersion(major + 1, 0, 0, SemverPreRelease.ZeroArray, null, default);
                    }
                    else // if (!Operand.Patch.IsNumeric)
                    {
                        // =1.2.x    ⇒ >=1.2.0    <1.3.0-0
                        // =1.2.x-rc ⇒ >=1.2.0-rc <1.3.0-0 (TODO: node-semver ignores pre-releases if there are unspecified components)
                        minor = Operand.Minor.AsNumber;
                        if (minor == int.MaxValue) throw new InvalidOperationException(Exceptions.MinorTooBig);
                        version = new SemanticVersion(Operand);
                        version2 = new SemanticVersion(major, minor + 1, 0, SemverPreRelease.ZeroArray, null, default);
                    }
                    return (PrimitiveComparator.GreaterThanOrEqual(version), PrimitiveComparator.LessThan(version2));

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
            => Operand.CalculateLength() + Utility.GetOperatorLength(Operator);
        /// <inheritdoc/>
        protected internal override void BuildString(ref SpanBuilder sb)
        {
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

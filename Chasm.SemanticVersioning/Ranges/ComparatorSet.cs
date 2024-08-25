using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Chasm.Formatting;
using JetBrains.Annotations;

namespace Chasm.SemanticVersioning.Ranges
{
    // Note: Do not implement IEnumerable.
    //   That would source-level break some methods due to ambiguous resolution
    //   caused by collection expressions and implicit operators. Json.NET will
    //   also serialize this as an array instead of using a TypeConverter.
    //   Having a GetEnumerator() method is good enough.

    /// <summary>
    ///   <para>Represents a valid <c>node-semver</c> version comparator set.</para>
    /// </summary>
    public sealed partial class ComparatorSet : ISpanBuildable, IEquatable<ComparatorSet>
#if NET7_0_OR_GREATER
                                              , System.Numerics.IEqualityOperators<ComparatorSet, ComparatorSet, bool>
#endif
    {
        internal readonly Comparator[] _comparators;
        internal ReadOnlyCollection<Comparator>? _comparatorsReadonly;
        /// <summary>
        ///   <para>Gets a read-only collection of the comparator set's comparators.</para>
        /// </summary>
        public ReadOnlyCollection<Comparator> Comparators
            => _comparatorsReadonly ??= _comparators.AsReadOnly();

        // ReSharper disable once UnusedParameter.Local
        internal ComparatorSet(Comparator[] comparators, bool _)
        {
            // Make sure the internal constructor isn't used with an invalid parameter
            Debug.Assert(Array.IndexOf(comparators, null) == -1);

            _comparators = comparators;
        }

        /// <summary>
        ///   <para>Initializes a new instance of the <see cref="ComparatorSet"/> class with the specified <paramref name="comparators"/>.</para>
        /// </summary>
        /// <param name="comparators">The comparator set's comparators.</param>
        /// <exception cref="ArgumentException"><paramref name="comparators"/> contains <see langword="null"/>.</exception>
        public ComparatorSet(params Comparator[]? comparators)
        {
            if (comparators?.Length > 0)
            {
                if (Array.IndexOf(comparators, null) >= 0) throw new ArgumentException(Exceptions.ComparatorsNull, nameof(comparators));
                _comparators = (Comparator[])comparators.Clone();
            }
            else _comparators = [];
        }
        /// <summary>
        ///   <para>Initializes a new instance of the <see cref="ComparatorSet"/> class with the specified <paramref name="comparators"/>.</para>
        /// </summary>
        /// <param name="comparators">The comparator set's comparators.</param>
        /// <exception cref="ArgumentException"><paramref name="comparators"/> contains <see langword="null"/>.</exception>
        public ComparatorSet([InstantHandle] IEnumerable<Comparator>? comparators)
            : this(comparators?.ToArray()) { }

        /// <summary>
        ///   <para>Defines an implicit conversion of a comparator to a comparator set.</para>
        /// </summary>
        /// <param name="comparator">The comparator to construct a comparator set from.</param>
        [Pure] [return: NotNullIfNotNull(nameof(comparator))]
        public static implicit operator ComparatorSet?(Comparator? comparator)
        {
            if (ReferenceEquals(comparator, PrimitiveComparator.None)) return None;
            if (ReferenceEquals(comparator, XRangeComparator.All)) return All;
            return comparator is null ? null : new ComparatorSet([comparator], default);
        }

        /// <summary>
        ///   <para>Determines whether this comparator set contains any advanced comparators.</para>
        /// </summary>
        public bool IsSugared
        {
            get
            {
                Comparator[] comparators = _comparators;
                for (int i = 0; i < comparators.Length; i++)
                    if (comparators[i].IsAdvanced)
                        return true;
                return false;
            }
        }

        // TODO: IsEmpty property would be nice to have

        /// <summary>
        ///   <para>Gets a read-only span of the comparators set's comparators.</para>
        /// </summary>
        /// <returns>A read-only span of the comparators set's comparators.</returns>
        [Pure] public ReadOnlySpan<Comparator> GetComparators()
            => _comparators;

        /// <summary>
        ///   <para>Determines whether the specified semantic <paramref name="version"/> satisfies this comparator set.</para>
        /// </summary>
        /// <param name="version">The semantic version to match.</param>
        /// <returns><see langword="true"/>, if the specified semantic <paramref name="version"/> satisfies this comparator set; otherwise, <see langword="false"/>.</returns>
        [Pure] public bool IsSatisfiedBy(SemanticVersion? version)
            => IsSatisfiedBy(version, false);
        /// <summary>
        ///   <para>Determines whether the specified semantic <paramref name="version"/> satisfies this comparator set.</para>
        /// </summary>
        /// <param name="version">The semantic version to match.</param>
        /// <param name="includePreReleases">Determines whether to treat pre-release versions like regular versions.</param>
        /// <returns><see langword="true"/>, if the specified semantic <paramref name="version"/> satisfies this comparator set; otherwise, <see langword="false"/>.</returns>
        [Pure] public bool IsSatisfiedBy(SemanticVersion? version, bool includePreReleases)
        {
            if (version is not null)
            {
                int i;
                Comparator[] comparators = _comparators;

                if (!includePreReleases && version.IsPreRelease)
                {
                    // at least one must match the component triple
                    for (i = 0; i < comparators.Length; i++)
                        if (comparators[i].CanMatchPreRelease(version.Major, version.Minor, version.Patch))
                            goto MATCH_COMPARATORS;
                    goto MATCH_FAIL;
                }

            MATCH_COMPARATORS:
                // all comparators must be satisfied by the version
                for (i = 0; i < comparators.Length; i++)
                    if (!comparators[i].IsSatisfiedByCore(version))
                        goto MATCH_FAIL;
                return true;
            }
        MATCH_FAIL:
            return false;
        }

        /// <summary>
        ///   <para>Returns a desugared copy of this comparator set, that is, with advanced comparators replaced by equivalent primitive comparators.</para>
        /// </summary>
        /// <returns>A desugared copy of this comparator set.</returns>
        [Pure] public ComparatorSet Desugar()
        {
            Comparator[] comparators = _comparators;
            if (comparators.Length > 0 && !IsSugared) return this;

            List<Comparator> desugared = new(comparators.Length);

            for (int i = 0; i < comparators.Length; i++)
            {
                Comparator comparator = comparators[i];
                if (comparator is AdvancedComparator advanced)
                {
                    (PrimitiveComparator? left, PrimitiveComparator? right) = advanced.ToPrimitives();
                    if (left is not null) desugared.Add(left);
                    if (right is not null) desugared.Add(right);
                }
                else
                {
                    desugared.Add(comparator);
                }
            }

            if (desugared.Count == 0)
                desugared.Add(PrimitiveComparator.All);

            return new ComparatorSet(desugared.ToArray(), default);
        }
        // TODO: make Normalize() public
        [Pure] internal ComparatorSet Normalize()
        {
            Comparator[] comparators = _comparators;

            // if there's only one comparator, normalize >x.x.x to <0.0.0-0, or just return this
            if (comparators.Length <= 1)
            {
                if (comparators.Length == 1 && comparators[0] is AdvancedComparator advanced)
                {
                    var (desugaredLeft, desugaredRight) = advanced.ToPrimitives();
                    if (desugaredLeft is null && PrimitiveComparator.None.Equals(desugaredRight))
                        return None;
                }
                return this;
            }

            var (low, high) = GetBounds();
            if (!RangeUtility.DoComparatorsIntersect(high, low)) return None;

            // if this set contains two comparators, and it's already normalized, just return it
            if (comparators.Length == 2 && ReferenceEquals(comparators[0], low) && ReferenceEquals(comparators[1], high))
                return this;

            // rearrange comparators in a normalized way
            if (low is null) return high is null ? All : high;
            if (high is null) return low;
            return new ComparatorSet([low, high], default);
        }

        /// <summary>
        ///   <para>Gets a comparator set (<c>&lt;0.0.0-0</c>) that doesn't match any versions.</para>
        /// </summary>
        public static ComparatorSet None { get; } = new ComparatorSet(PrimitiveComparator.None);
        /// <summary>
        ///   <para>Gets a comparator set (<c>*</c>) that matches all non-pre-release versions (or all versions, with <c>includePreReleases</c> option).</para>
        /// </summary>
        public static ComparatorSet All { get; } = new ComparatorSet(XRangeComparator.All);

        // Internal helper to minimize allocations during range operations
        [Pure] internal static ComparatorSet FromTuple((Comparator?, Comparator?) tuple)
        {
            (Comparator? resultLeft, Comparator? resultRight) = tuple;

            // if it's just a single comparator, use the implicit conversion operator
            if (resultRight is null) return resultLeft is null ? All : resultLeft;

            // combine both comparators in a set
            Debug.Assert(resultLeft is not null);
            return new ComparatorSet([resultLeft, resultRight], default);
        }

        [Pure] internal int CalculateLength()
        {
            Comparator[] comparators = _comparators;
            if (comparators.Length == 0) return 0;

            int length = comparators.Length - 1;
            for (int i = 0; i < comparators.Length; i++)
                length += comparators[i].CalculateLength();

            return length;
        }
        internal void BuildString(ref SpanBuilder sb)
        {
            Comparator[] comparators = _comparators;
            if (comparators.Length != 0)
            {
                comparators[0].BuildString(ref sb);
                for (int i = 1; i < comparators.Length; i++)
                {
                    sb.Append(' ');
                    comparators[i].BuildString(ref sb);
                }
            }
        }
        [Pure] int ISpanBuildable.CalculateLength() => CalculateLength();
        void ISpanBuildable.BuildString(ref SpanBuilder sb) => BuildString(ref sb);

        // TODO: make GetBounds() public
        [Pure] internal (PrimitiveComparator? Lower, PrimitiveComparator? Upper) GetBounds()
        {
            PrimitiveComparator? lower = null;
            PrimitiveComparator? upper = null;

            Comparator[] comparators = _comparators;
            for (int i = 0; i < comparators.Length; i++)
            {
                (PrimitiveComparator? left, PrimitiveComparator? right) = comparators[i].AsPrimitives();
                if (left?.Operator.IsEQ() == true)
                {
                    right = PrimitiveComparator.LessThanOrEqual(left.Operand);
                    left = PrimitiveComparator.GreaterThanOrEqual(left.Operand);
                }

                if (RangeUtility.CompareComparators(left, lower) > 0)
                    lower = left;
                if (RangeUtility.CompareComparators(right, upper, -1) < 0)
                    upper = right;
            }

            if (!RangeUtility.DoComparatorsIntersect(upper, lower))
                return (null, PrimitiveComparator.None);

            return (lower, upper);
        }
        [Pure] internal (PrimitiveOperator gt, SemanticVersion? gtOp, PrimitiveOperator lt, SemanticVersion? ltOp) GetBoundsCore()
        {
            PrimitiveOperator lowerOp = PrimitiveOperator.GreaterThanOrEqual;
            PrimitiveOperator upperOp = PrimitiveOperator.LessThanOrEqual;
            SemanticVersion? lower = null;
            SemanticVersion? upper = null;

            Comparator[] comparators = _comparators;
            for (int i = 0; i < comparators.Length; i++)
            {
                (PrimitiveComparator? leftC, PrimitiveComparator? rightC) = comparators[i].AsPrimitives();

                if (leftC is not null)
                {
                    PrimitiveOperator leftOp = leftC.Operator;
                    SemanticVersion left = leftC.Operand;

                    if (leftOp.IsEQ())
                    {
                        if (lower is null || left > lower)
                        {
                            lowerOp = PrimitiveOperator.GreaterThanOrEqual;
                            lower = left;
                        }
                        if (upper is null || left < upper)
                        {
                            upperOp = PrimitiveOperator.LessThanOrEqual;
                            upper = left;
                        }
                        continue;
                    }
                    if (lower is null || RangeUtility.CompareComparators(leftOp, left, lowerOp, lower) > 0)
                    {
                        lowerOp = leftOp;
                        lower = left;
                    }
                }
                if (rightC is not null)
                {
                    PrimitiveOperator rightOp = rightC.Operator;
                    SemanticVersion right = rightC.Operand;

                    if (upper is null || RangeUtility.CompareComparators(rightOp, right, upperOp, upper) < 0)
                    {
                        upperOp = rightOp;
                        upper = right;
                    }
                }
            }

            if (!RangeUtility.DoComparatorsIntersect(upperOp, upper, lowerOp, lower))
                return (default, null, PrimitiveOperator.LessThan, SemanticVersion.MinValue);

            return (lowerOp, lower, upperOp, upper);
        }

        /// <summary>
        ///   <para>Determines whether this comparator set contains the specified <paramref name="other"/> comparator set.</para>
        /// </summary>
        /// <param name="other">The comparator set to contain.</param>
        /// <returns><see langword="true"/>, if this comparator set contains <paramref name="other"/>; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="other"/> is <see langword="null"/>.</exception>
        [Pure] public bool Contains(ComparatorSet other)
        {
            if (other is null) throw new ArgumentNullException(nameof(other));

            var (lowOp1, low1, highOp1, high1) = GetBoundsCore();
            var (lowOp2, low2, highOp2, high2) = other.GetBoundsCore();

            if (highOp2 == PrimitiveOperator.LessThan && SemanticVersion.MinValue.Equals(high2))
                return true;

            return (low1 is null || low2 is not null && RangeUtility.CompareComparators(lowOp1, low1, lowOp2, low2) <= 0) &&
                   (high1 is null || high2 is not null && RangeUtility.CompareComparators(highOp1, high1, highOp2, high2) >= 0);
        }
        /// <summary>
        ///   <para>Determines whether this comparator set intersects the specified <paramref name="other"/> comparator set.</para>
        /// </summary>
        /// <param name="other">The comparator set to intersect with.</param>
        /// <returns><see langword="true"/>, if this comparator set intersects <paramref name="other"/>; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="other"/> is <see langword="null"/>.</exception>
        [Pure] public bool Intersects(ComparatorSet other)
        {
            if (other is null) throw new ArgumentNullException(nameof(other));

            var (lowOp1, low1, highOp1, high1) = GetBoundsCore();
            var (lowOp2, low2, highOp2, high2) = other.GetBoundsCore();

            if (highOp1 == PrimitiveOperator.LessThan && SemanticVersion.MinValue.Equals(high1))
                return highOp2 == PrimitiveOperator.LessThan && SemanticVersion.MinValue.Equals(high2);
            if (highOp2 == PrimitiveOperator.LessThan && SemanticVersion.MinValue.Equals(high2))
                return false;

            return RangeUtility.DoComparatorsIntersect(highOp1, high1, lowOp2, low2) &&
                   RangeUtility.DoComparatorsIntersect(highOp2, high2, lowOp1, low1);
        }
        /// <summary>
        ///   <para>Determines whether this comparator set touches the specified <paramref name="other"/> comparator set.</para>
        /// </summary>
        /// <param name="other">The comparator set to touch.</param>
        /// <returns><see langword="true"/>, if this comparator set touches <paramref name="other"/>; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="other"/> is <see langword="null"/>.</exception>
        [Pure] public bool Touches(ComparatorSet other)
        {
            if (other is null) throw new ArgumentNullException(nameof(other));

            var (lowOp1, low1, highOp1, high1) = GetBoundsCore();
            var (lowOp2, low2, highOp2, high2) = other.GetBoundsCore();

            if (highOp1 == PrimitiveOperator.LessThan && SemanticVersion.MinValue.Equals(high1))
                return highOp2 == PrimitiveOperator.LessThan && SemanticVersion.MinValue.Equals(high2);
            if (highOp2 == PrimitiveOperator.LessThan && SemanticVersion.MinValue.Equals(high2))
                return false;

            return RangeUtility.DoComparatorsComplement(highOp1, high1, lowOp2, low2) &&
                   RangeUtility.DoComparatorsComplement(highOp2, high2, lowOp1, low1);
        }

        /// <summary>
        ///   <para>Returns the string representation of this comparator set.</para>
        /// </summary>
        /// <returns>The string representation of this comparator set.</returns>
        [Pure] public override string ToString()
            => SpanBuilder.Format(this);

        /// <summary>
        ///   <para>Returns an enumerator that iterates through the comparator set's comparators.</para>
        /// </summary>
        /// <returns>An enumerator for the comparator set's comparators.</returns>
        [Pure] public IEnumerator<Comparator> GetEnumerator()
            => ((IEnumerable<Comparator>)_comparators).GetEnumerator();

        /// <summary>
        ///   <para>Gets the comparator at the specified <paramref name="index"/>.</para>
        /// </summary>
        /// <param name="index">The zero-based index of the comparator to get.</param>
        /// <returns>The comparator at the specified index.</returns>
        public Comparator this[int index] => _comparators[index];

        /// <summary>
        ///   <para>Determines whether this comparator set is equal to another specified comparator set.<br/>Build metadata is ignored and non-numeric version components and implicit/explicit equality operators are considered equal in this comparison. See <see cref="SemverComparer"/> for more options.</para>
        /// </summary>
        /// <param name="other">The comparator set to compare with this comparator set.</param>
        /// <returns><see langword="true"/>, if this comparator set is equal to <paramref name="other"/>; otherwise, <see langword="false"/>.</returns>
        [Pure] public bool Equals(ComparatorSet? other)
        {
            if (other is null) return false;
            return Utility.SequenceEqual(_comparators, other._comparators);
        }
        /// <summary>
        ///   <para>Determines whether this comparator set is equal to the specified <paramref name="obj"/>.<br/>Build metadata is ignored and non-numeric version components and implicit/explicit equality operators are considered equal in this comparison. See <see cref="SemverComparer"/> for more options.</para>
        /// </summary>
        /// <param name="obj">The object to compare with this comparator set.</param>
        /// <returns><see langword="true"/>, if <paramref name="obj"/> is a <see cref="ComparatorSet"/> instance equal to this comparator set; otherwise, <see langword="false"/>.</returns>
        [Pure] public override bool Equals(object? obj)
            => Equals(obj as ComparatorSet);
        /// <summary>
        ///   <para>Returns a hash code for this comparator set.<br/>Build metadata is ignored and non-numeric version components and implicit/explicit equality operators are considered equal in this comparison. See <see cref="SemverComparer"/> for more options.</para>
        /// </summary>
        /// <returns>A hash code for this comparator set.</returns>
        [Pure] public override int GetHashCode()
        {
            HashCode hash = new();
            Comparator[] comparators = _comparators;
            for (int i = 0; i < comparators.Length; i++)
                hash.Add(comparators[i]);
            return hash.ToHashCode();
        }

        /// <summary>
        ///   <para>Determines whether two specified comparator sets are equal.<br/>Build metadata is ignored and non-numeric version components and implicit/explicit equality operators are considered equal in this comparison. See <see cref="SemverComparer"/> for more options.</para>
        /// </summary>
        /// <param name="left">The first comparator set to compare.</param>
        /// <param name="right">The second comparator set to compare.</param>
        /// <returns><see langword="true"/>, if <paramref name="left"/> is equal to <paramref name="right"/>; otherwise, <see langword="false"/>.</returns>
        [Pure] public static bool operator ==(ComparatorSet? left, ComparatorSet? right)
            => left is null ? right is null : left.Equals(right);
        /// <summary>
        ///   <para>Determines whether two specified comparator sets are not equal.<br/>Build metadata is ignored and non-numeric version components and implicit/explicit equality operators are considered equal in this comparison. See <see cref="SemverComparer"/> for more options.</para>
        /// </summary>
        /// <param name="left">The first comparator set to compare.</param>
        /// <param name="right">The second comparator set to compare.</param>
        /// <returns><see langword="true"/>, if <paramref name="left"/> is not equal to <paramref name="right"/>; otherwise, <see langword="false"/>.</returns>
        [Pure] public static bool operator !=(ComparatorSet? left, ComparatorSet? right)
            => !(left == right);

        // TODO: Implement >, <, >=, <= operators

    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Chasm.Formatting;
using JetBrains.Annotations;

namespace Chasm.SemanticVersioning.Ranges
{
    /// <summary>
    ///   <para>Represents a valid <c>node-semver</c> version comparator set.</para>
    /// </summary>
    public sealed class ComparatorSet : ISpanBuildable
    {
        internal readonly Comparator[] _comparators;
        internal ReadOnlyCollection<Comparator>? _comparatorsReadonly;
        /// <summary>
        ///   <para>Gets a read-only collection of the version comparator set's version comparators.</para>
        /// </summary>
        public ReadOnlyCollection<Comparator> Comparators
            => _comparatorsReadonly ??= _comparators.AsReadOnly();

        // ReSharper disable once UnusedParameter.Local
        internal ComparatorSet(Comparator[] comparators, bool _)
            => _comparators = comparators;

        /// <summary>
        ///   <para>Initializes a new instance of the <see cref="ComparatorSet"/> class with the specified version <paramref name="comparators"/>.</para>
        /// </summary>
        /// <param name="comparators">The version comparator set's version comparators.</param>
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
        ///   <para>Initializes a new instance of the <see cref="ComparatorSet"/> class with the specified version <paramref name="comparators"/>.</para>
        /// </summary>
        /// <param name="comparators">The version comparator set's version comparators.</param>
        /// <exception cref="ArgumentException"><paramref name="comparators"/> contains <see langword="null"/>.</exception>
        public ComparatorSet([InstantHandle] IEnumerable<Comparator>? comparators)
            : this(comparators?.ToArray()) { }

        /// <summary>
        ///   <para>Defines an implicit conversion of a version comparator to a version comparator set.</para>
        /// </summary>
        /// <param name="comparator">The version comparator to construct a version comparator set from.</param>
        [Pure] [return: NotNullIfNotNull(nameof(comparator))]
        public static implicit operator ComparatorSet?(Comparator? comparator)
            => comparator is null ? null : new ComparatorSet([comparator], default);

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
        ///   <para>Determines whether the specified semantic <paramref name="version"/> satisfies this version comparator set.</para>
        /// </summary>
        /// <param name="version">The semantic version to match.</param>
        /// <returns><see langword="true"/>, if the specified semantic <paramref name="version"/> satisfies this version comparator set; otherwise, <see langword="false"/>.</returns>
        [Pure] public bool IsSatisfiedBy(SemanticVersion? version)
            => IsSatisfiedBy(version, false);
        /// <summary>
        ///   <para>Determines whether the specified semantic <paramref name="version"/> satisfies this version comparator set.</para>
        /// </summary>
        /// <param name="version">The semantic version to match.</param>
        /// <param name="includePreReleases">Determines whether to treat pre-release versions like regular versions.</param>
        /// <returns><see langword="true"/>, if the specified semantic <paramref name="version"/> satisfies this version comparator set; otherwise, <see langword="false"/>.</returns>
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
        ///   <para>Returns a desugared copy of this version comparator set, that is, with advanced version comparators replaced by equivalent primitive version comparators.</para>
        /// </summary>
        /// <returns>A desugared copy of this version comparator set.</returns>
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
                desugared.Add(PrimitiveComparator.AllNonPreRelease);

            return new ComparatorSet(desugared.ToArray(), default);
        }

        /// <summary>
        ///   <para>Gets a version comparator set (<c>&lt;0.0.0-0</c>) that doesn't match any versions.</para>
        /// </summary>
        public static ComparatorSet None { get; } = new ComparatorSet(PrimitiveComparator.None);
        /// <summary>
        ///   <para>Gets a version comparator set (<c>*</c>) that matches all non-pre-release versions (or all versions, with <c>includePreReleases</c> option).</para>
        /// </summary>
        public static ComparatorSet All { get; } = new ComparatorSet(XRangeComparator.All);

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

        /// <summary>
        ///   <para>Returns the string representation of this version comparator set.</para>
        /// </summary>
        /// <returns>The string representation of this version comparator set.</returns>
        [Pure] public override string ToString()
            => SpanBuilder.Format(this);

        // TODO: Implement Equals, GetHashCode, and ==, != operators

        // TODO: Implement >, <, >=, <= operators

        // TODO: Implement &, |, ~ operators

    }
}

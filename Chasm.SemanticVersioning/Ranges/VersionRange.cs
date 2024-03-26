using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Chasm.Collections;
using Chasm.Formatting;
using JetBrains.Annotations;

namespace Chasm.SemanticVersioning.Ranges
{
    /// <summary>
    ///   <para>Represents a valid <c>node-semver</c> version range.</para>
    /// </summary>
    public sealed partial class VersionRange : ISpanBuildable
    {
        private readonly ComparatorSet[] _comparatorSets;
        private ReadOnlyCollection<ComparatorSet>? _comparatorSetsReadonly;
        /// <summary>
        ///   <para>Gets a read-only collection of this version range's version comparator sets.</para>
        /// </summary>
        public ReadOnlyCollection<ComparatorSet> ComparatorSets
            => _comparatorSetsReadonly ??= _comparatorSets.AsReadOnly();

        // ReSharper disable once UnusedParameter.Local
        internal VersionRange(ComparatorSet[] comparatorSets, bool _)
            => _comparatorSets = comparatorSets;

        /// <summary>
        ///   <para>Initializes a new instance of the <see cref="VersionRange"/> class with the specified version <paramref name="comparatorSet"/>.</para>
        /// </summary>
        /// <param name="comparatorSet">The version range's version comparator sets.</param>
        /// <exception cref="ArgumentNullException"><paramref name="comparatorSet"/> is <see langword="null"/>.</exception>
        public VersionRange(ComparatorSet comparatorSet)
        {
            if (comparatorSet is null) throw new ArgumentNullException(nameof(comparatorSet));
            _comparatorSets = [comparatorSet];
        }
        /// <summary>
        ///   <para>Initializes a new instance of the <see cref="VersionRange"/> class with the specified version comparator sets.</para>
        /// </summary>
        /// <param name="firstComparatorSet">The version range's first version comparator set.</param>
        /// <param name="otherComparatorSets">The version range's subsequent version comparator sets.</param>
        /// <exception cref="ArgumentNullException"><paramref name="firstComparatorSet"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="otherComparatorSets"/> contains <see langword="null"/>.</exception>
        public VersionRange(ComparatorSet firstComparatorSet, params ComparatorSet[]? otherComparatorSets)
        {
            if (firstComparatorSet is null) throw new ArgumentNullException(nameof(firstComparatorSet));
            ComparatorSet[] array;
            if (otherComparatorSets?.Length > 0)
            {
                if (Array.IndexOf(otherComparatorSets, null) >= 0)
                    throw new ArgumentException(Exceptions.ComparatorSetsNull, nameof(otherComparatorSets));

                int otherLength = otherComparatorSets.Length;
                array = new ComparatorSet[otherLength + 1];
                array[0] = firstComparatorSet;
                Array.Copy(otherComparatorSets, 0, array, 1, otherLength);
            }
            else array = [firstComparatorSet];
            _comparatorSets = array;
        }
        /// <summary>
        ///   <para>Initializes a new instance of the <see cref="VersionRange"/> class with the specified version <paramref name="comparatorSets"/>.</para>
        /// </summary>
        /// <param name="comparatorSets">The version range's version comparator sets.</param>
        /// <exception cref="ArgumentNullException"><paramref name="comparatorSets"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="comparatorSets"/> is empty, or contains <see langword="null"/>.</exception>
        public VersionRange([InstantHandle] IEnumerable<ComparatorSet> comparatorSets)
        {
            if (comparatorSets is null) throw new ArgumentNullException(nameof(comparatorSets));
            ComparatorSet[] array = comparatorSets.ToArray();
            if (array.Length == 0) throw new ArgumentException(Exceptions.EmptyVersionRange, nameof(comparatorSets));
            if (Array.IndexOf(array, null) >= 0) throw new ArgumentException(Exceptions.ComparatorSetsNull, nameof(comparatorSets));
            _comparatorSets = array;
        }

        /// <summary>
        ///   <para>Defines an implicit conversion of a version comparator to a version range.</para>
        /// </summary>
        /// <param name="comparator">The version comparator to construct a version range from.</param>
        [Pure] [return: NotNullIfNotNull(nameof(comparator))]
        public static implicit operator VersionRange?(Comparator? comparator)
            => comparator is null ? null : new VersionRange(new ComparatorSet(comparator));
        /// <summary>
        ///   <para>Defines an implicit conversion of a version comparator set to a version range.</para>
        /// </summary>
        /// <param name="comparatorSet">The version comparator set to construct a version range from.</param>
        [Pure] [return: NotNullIfNotNull(nameof(comparatorSet))]
        public static implicit operator VersionRange?(ComparatorSet? comparatorSet)
            => comparatorSet is null ? null : new VersionRange(comparatorSet);

        // TODO: IsSugared is definitely a nice property to have, but I'm not sure about the name yet. Maybe ContainsSugar or sth?
        internal bool IsSugared => Array.Exists(_comparatorSets, static cs => cs.IsSugared);

        /// <summary>
        ///   <para>Determines whether this version range is empty, that is, doesn't match any versions.</para>
        /// </summary>
        public bool IsEmpty => Array.TrueForAll(_comparatorSets, static cs => cs.IsEmpty);

        /// <summary>
        ///   <para>Determines whether the specified semantic <paramref name="version"/> satisfies this version range.</para>
        /// </summary>
        /// <param name="version">The semantic version to match.</param>
        /// <returns><see langword="true"/>, if the specified semantic <paramref name="version"/> satisfies this version range otherwise, <see langword="false"/>.</returns>
        [Pure] public bool IsSatisfiedBy(SemanticVersion? version)
            => IsSatisfiedBy(version, false);
        /// <summary>
        ///   <para>Determines whether the specified semantic <paramref name="version"/> satisfies this version range.</para>
        /// </summary>
        /// <param name="version">The semantic version to match.</param>
        /// <param name="includePreReleases">Determines whether to treat pre-release versions like regular versions.</param>
        /// <returns><see langword="true"/>, if the specified semantic <paramref name="version"/> satisfies this version range otherwise, <see langword="false"/>.</returns>
        [Pure] public bool IsSatisfiedBy(SemanticVersion? version, bool includePreReleases)
            => version is not null && _comparatorSets.Exists(cs => cs.IsSatisfiedBy(version, includePreReleases));

        /// <summary>
        ///   <para>Returns a desugared copy of this version range, that is, with advanced version comparators replaced by equivalent primitive version comparators.</para>
        /// </summary>
        /// <returns>A desugared copy of this version range.</returns>
        [Pure] public VersionRange Desugar()
        {
            if (!IsSugared) return this;
            ComparatorSet[] desugared = Array.ConvertAll(_comparatorSets, static cs => cs.Desugar());
            return new VersionRange(desugared, default);
        }

        /// <summary>
        ///   <para>Gets a version range (<c>&lt;0.0.0-0</c>) that doesn't match any versions.</para>
        /// </summary>
        public static VersionRange None { get; } = new VersionRange(ComparatorSet.None);
        /// <summary>
        ///   <para>Gets a version range (<c>*</c>) that matches all non-pre-release versions (or all versions, with <c>includePreReleases</c> option).</para>
        /// </summary>
        public static VersionRange All { get; } = new VersionRange(ComparatorSet.All);

        [Pure] internal int CalculateLength()
        {
            ComparatorSet[] comparatorSets = _comparatorSets;
            int length = (comparatorSets.Length - 1) * 4; // ' || '
            for (int i = 0; i < comparatorSets.Length; i++)
                length += comparatorSets[i].CalculateLength();
            return length;
        }
        internal void BuildString(ref SpanBuilder sb)
        {
            ComparatorSet[] comparatorSets = _comparatorSets;
            comparatorSets[0].BuildString(ref sb);
            for (int i = 1; i < comparatorSets.Length; i++)
            {
                sb.Append(' ', '|', '|', ' ');
                comparatorSets[i].BuildString(ref sb);
            }
        }
        [Pure] int ISpanBuildable.CalculateLength() => CalculateLength();
        void ISpanBuildable.BuildString(ref SpanBuilder sb) => BuildString(ref sb);

        /// <summary>
        ///   <para>Returns the string representation of this version range.</para>
        /// </summary>
        /// <returns>The string representation of this version range.</returns>
        [Pure] public override string ToString()
            => SpanBuilder.Format(this);


    }
}

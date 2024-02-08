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
    ///   <para>Represents a valid <c>node-semver</c> version comparator set.</para>
    /// </summary>
    public sealed class ComparatorSet : ISpanBuildable
    {
        private readonly Comparator[] _comparators;
        private ReadOnlyCollection<Comparator>? _comparatorsReadonly;
        /// <summary>
        ///   <para>Gets a read-only collection of the version comparator set's version comparators.</para>
        /// </summary>
        public ReadOnlyCollection<Comparator> Comparators
            => _comparatorsReadonly ??= _comparators.AsReadOnly();

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
                _comparators = comparators.Copy();
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
            => comparator is null ? null : new ComparatorSet(comparator);

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
            if (version is null) return false;
            if (!includePreReleases && version.IsPreRelease)
            {
                bool canCompare = _comparators.Exists(c => c.CanMatchPreRelease(version.Major, version.Minor, version.Patch));
                if (!canCompare) return false;
            }
            return _comparators.TrueForAll(c => c.IsSatisfiedBy(version));
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
            if (comparators.Length == 0) return;

            comparators[0].BuildString(ref sb);
            for (int i = 1; i < comparators.Length; i++)
            {
                sb.Append(' ');
                comparators[i].BuildString(ref sb);
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

    }
}

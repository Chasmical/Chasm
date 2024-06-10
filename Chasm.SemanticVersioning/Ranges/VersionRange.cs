using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml;
using System.Xml.Serialization;
using Chasm.Formatting;
using JetBrains.Annotations;

namespace Chasm.SemanticVersioning.Ranges
{
    /// <summary>
    ///   <para>Represents a valid <c>node-semver</c> version range.</para>
    /// </summary>
    public sealed partial class VersionRange : ISpanBuildable, IXmlSerializable, IReadOnlyList<ComparatorSet>
    {
        internal readonly ComparatorSet[] _comparatorSets;
        internal ReadOnlyCollection<ComparatorSet>? _comparatorSetsReadonly;
        /// <summary>
        ///   <para>Gets a read-only collection of this version range's version comparator sets.</para>
        /// </summary>
        public ReadOnlyCollection<ComparatorSet> ComparatorSets
            => _comparatorSetsReadonly ??= _comparatorSets.AsReadOnly();

        // ReSharper disable once UnusedParameter.Local
        internal VersionRange(ComparatorSet[] comparatorSets, bool _)
        {
            // Make sure the internal constructor isn't used with an invalid parameter
            Debug.Assert(comparatorSets.Length >= 1);
            Debug.Assert(Array.IndexOf(comparatorSets, null) == -1);

            _comparatorSets = comparatorSets;
        }

        /// <summary>
        ///   <para>Initializes a new instance of the <see cref="VersionRange"/> class with the specified version <paramref name="comparatorSet"/>.</para>
        /// </summary>
        /// <param name="comparatorSet">The version range's version comparator set.</param>
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

                array = new ComparatorSet[otherComparatorSets.Length + 1];
                array[0] = firstComparatorSet;
                otherComparatorSets.CopyTo(array, 1);
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
            if (array.Length == 0) throw new ArgumentException(Exceptions.VersionRangeEmpty, nameof(comparatorSets));
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

        /// <summary>
        ///   <para>Determines whether this version range contains any advanced comparators.</para>
        /// </summary>
        public bool IsSugared
        {
            get
            {
                ComparatorSet[] comparatorSets = _comparatorSets;
                for (int i = 0; i < comparatorSets.Length; i++)
                    if (comparatorSets[i].IsSugared)
                        return true;
                return false;
            }
        }

        // TODO: IsEmpty property would be nice to have

        /// <summary>
        ///   <para>Gets a read-only span of the version range's comparator sets.</para>
        /// </summary>
        /// <returns>A read-only span of the version range's comparator sets.</returns>
        [Pure] public ReadOnlySpan<ComparatorSet> GetComparatorSets()
            => _comparatorSets;

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
        {
            if (version is not null)
            {
                ComparatorSet[] comparatorSets = _comparatorSets;
                for (int i = 0; i < comparatorSets.Length; i++)
                    if (comparatorSets[i].IsSatisfiedBy(version, includePreReleases))
                        return true;
            }
            return false;
        }

        /// <summary>
        ///   <para>Returns a desugared copy of this version range, that is, with advanced version comparators replaced by equivalent primitive version comparators.</para>
        /// </summary>
        /// <returns>A desugared copy of this version range.</returns>
        [Pure] public VersionRange Desugar()
        {
            if (!IsSugared) return this;
            ComparatorSet[] comparatorSets = _comparatorSets;
            ComparatorSet[] desugared = new ComparatorSet[comparatorSets.Length];
            for (int i = 0; i < desugared.Length; i++)
                desugared[i] = comparatorSets[i].Desugar();
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
                sb.Append(" || ".AsSpan());
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

        /// <summary>
        ///   <para>Returns an enumerator that iterates through the version range's comparator sets.</para>
        /// </summary>
        /// <returns>An enumerator for the version range's comparator sets.</returns>
        [Pure] public IEnumerator<ComparatorSet> GetEnumerator()
            => ((IEnumerable<ComparatorSet>)_comparatorSets).GetEnumerator();
        [Pure] IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        /// <summary>
        ///   <para>Gets the comparator set at the specified <paramref name="index"/>.</para>
        /// </summary>
        /// <param name="index">The zero-based index of the comparator set to get.</param>
        /// <returns>The comparator set at the specified index.</returns>
        public ComparatorSet this[int index] => _comparatorSets[index];
        int IReadOnlyCollection<ComparatorSet>.Count => _comparatorSets.Length;

        #region IXmlSerializable implementation
        System.Xml.Schema.XmlSchema? IXmlSerializable.GetSchema() => null;

#pragma warning disable CS8618
        [Obsolete] private VersionRange() { }
#pragma warning restore CS8618

        void IXmlSerializable.WriteXml(XmlWriter xml)
            => xml.WriteString(ToString());
        void IXmlSerializable.ReadXml(XmlReader xml)
        {
            if (_comparatorSets is not null) throw new InvalidOperationException();
            VersionRange range = Parse(xml.ReadElementContentAsString());
            Unsafe.AsRef(in _comparatorSets) = range._comparatorSets;
        }
        #endregion

        // TODO: Implement Equals, GetHashCode, and ==, != operators

        // TODO: Implement >, <, >=, <= operators

        // TODO: Implement &, |, ~ operators

    }
}

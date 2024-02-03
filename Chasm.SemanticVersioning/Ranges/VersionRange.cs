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
    public sealed class VersionRange : ISpanBuildable
    {
        private readonly ComparatorSet[] _comparatorSets;
        private ReadOnlyCollection<ComparatorSet>? _comparatorSetsReadonly;
        public ReadOnlyCollection<ComparatorSet> ComparatorSets
            => _comparatorSetsReadonly ??= _comparatorSets.AsReadOnly();

        public VersionRange(ComparatorSet comparatorSet)
        {
            if (comparatorSet is null) throw new ArgumentNullException(nameof(comparatorSet));
            _comparatorSets = [comparatorSet];
        }
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
        public VersionRange([InstantHandle] IEnumerable<ComparatorSet> comparatorSets)
        {
            if (comparatorSets is null) throw new ArgumentNullException(nameof(comparatorSets));
            ComparatorSet[] array = comparatorSets.ToArray();
            if (array.Length == 0) throw new ArgumentException(Exceptions.EmptyVersionRange, nameof(comparatorSets));
            if (Array.IndexOf(array, null) >= 0) throw new ArgumentException(Exceptions.ComparatorSetsNull, nameof(comparatorSets));
            _comparatorSets = array;
        }

        [Pure] [return: NotNullIfNotNull(nameof(comparator))]
        public static implicit operator VersionRange?(Comparator? comparator)
            => comparator is null ? null : new VersionRange(new ComparatorSet(comparator));
        [Pure] [return: NotNullIfNotNull(nameof(comparatorSet))]
        public static implicit operator VersionRange?(ComparatorSet? comparatorSet)
            => comparatorSet is null ? null : new VersionRange(comparatorSet);

        [Pure] public bool IsSatisfiedBy(SemanticVersion? version)
            => IsSatisfiedBy(version, false);
        [Pure] public bool IsSatisfiedBy(SemanticVersion? version, bool includePreReleases)
            => version is not null && _comparatorSets.Exists(cs => cs.IsSatisfiedBy(version, includePreReleases));

        [Pure] internal int CalculateLength()
        {
            // TODO: consider putting the array in a local variable
            int length = (_comparatorSets.Length - 1) * 4; // ' || '
            for (int i = 0, setsLength = _comparatorSets.Length; i < setsLength; i++)
                length += _comparatorSets[i].CalculateLength();
            return length;
        }
        internal void BuildString(ref SpanBuilder sb)
        {
            // TODO: consider putting the array in a local variable
            _comparatorSets[0].BuildString(ref sb);
            for (int i = 1, setsLength = _comparatorSets.Length; i < setsLength; i++)
            {
                sb.Append(' ', '|', '|', ' ');
                _comparatorSets[i].BuildString(ref sb);
            }
        }
        [Pure] int ISpanBuildable.CalculateLength() => CalculateLength();
        void ISpanBuildable.BuildString(ref SpanBuilder sb) => BuildString(ref sb);

        [Pure] public override string ToString()
            => SpanBuilder.Format(this);

    }
}

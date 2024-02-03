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
    public sealed class ComparatorSet : ISpanBuildable
    {
        private readonly Comparator[] _comparators;
        private ReadOnlyCollection<Comparator>? _comparatorsReadonly;
        public ReadOnlyCollection<Comparator> Comparators
            => _comparatorsReadonly ??= _comparators.AsReadOnly();

        public ComparatorSet(params Comparator[]? comparators)
        {
            if (comparators?.Length > 0)
            {
                if (Array.IndexOf(comparators, null) >= 0) throw new ArgumentException(Exceptions.ComparatorsNull, nameof(comparators));
                _comparators = comparators.Copy();
            }
            else _comparators = [];
        }
        public ComparatorSet([InstantHandle] IEnumerable<Comparator>? comparators)
            : this(comparators?.ToArray()) { }

        [Pure] [return: NotNullIfNotNull(nameof(comparator))]
        public static implicit operator ComparatorSet?(Comparator? comparator)
            => comparator is null ? null : new ComparatorSet(comparator);

        [Pure] public bool IsSatisfiedBy(SemanticVersion? version)
            => IsSatisfiedBy(version, false);
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
            // TODO: consider putting the array in a local variable
            int comparatorsLength = _comparators.Length;
            if (comparatorsLength == 0) return 0;

            int length = comparatorsLength - 1;
            for (int i = 0; i < comparatorsLength; i++)
                length += _comparators[i].CalculateLength();

            return length;
        }
        internal void BuildString(ref SpanBuilder sb)
        {
            // TODO: consider putting the array in a local variable
            int comparatorsLength = _comparators.Length;
            if (comparatorsLength == 0) return;

            _comparators[0].BuildString(ref sb);
            for (int i = 1; i < comparatorsLength; i++)
            {
                sb.Append(' ');
                _comparators[i].BuildString(ref sb);
            }
        }
        [Pure] int ISpanBuildable.CalculateLength() => CalculateLength();
        void ISpanBuildable.BuildString(ref SpanBuilder sb) => BuildString(ref sb);

        [Pure] public override string ToString()
            => SpanBuilder.Format(this);

    }
}

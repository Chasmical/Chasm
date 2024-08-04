using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Chasm.Formatting;
using Chasm.SemanticVersioning.Ranges;
using JetBrains.Annotations;

namespace Chasm.SemanticVersioning
{
    internal static class Utility
    {
        [Pure] public static bool IsValidCharacter(char c)
        {
            // Note: IsAsciiDigit and IsAsciiLetter are slightly slower, and also increase the assembly size
            return ((uint)c | ' ') - 'a' <= 'z' - 'a' || (uint)c - '0' <= '9' - '0' || c == '-';
        }
        [Pure] public static unsafe ReadOnlySpan<char> ReadPartialComponent(ref SpanParser parser)
        {
            char read = parser.Peek();
            if ((uint)read - '0' <= '9' - '0')
                return parser.ReadAsciiDigits();

            return parser.ReadWhile(&IsPartialChar);
        }
        [Pure] private static bool IsPartialChar(char c) => ((uint)c | ' ') == 'x' || c == '*';

        [Pure] public static bool IsNumeric(ReadOnlySpan<char> text)
        {
            // Note: ContainsAnyExceptInRange is pretty slow on small spans
            // Note: IsAsciiDigit is slightly slower, and also increases assembly size

            for (int i = 0; i < text.Length; i++)
            {
                if ((uint)text[i] - '0' >= 10u)
                    return false;
            }
            return true;
        }
        [Pure] public static bool AllValidCharacters(ReadOnlySpan<char> str)
        {
            for (int i = 0, length = str.Length; i < length; i++)
                if (!IsValidCharacter(str[i]))
                    return false;
            return true;
        }

        [Pure] public static ReadOnlyCollection<T> AsReadOnly<T>(this T[] array)
        {
            if (array.Length != 0) return new ReadOnlyCollection<T>(array);
#if NET8_0_OR_GREATER
            return ReadOnlyCollection<T>.Empty;
#else
            return Collection<T>.Empty;
#endif
        }

#if !NET8_0_OR_GREATER
        private static class Collection<T>
        {
            public static readonly ReadOnlyCollection<T> Empty = new([]);
        }
#endif

        /// <summary>
        ///   <para>Handles the conversion of a partial version to a semantic one the same way as <c>node-semver</c>: ignores components and pre-releases after an unspecified component, and removes build metadata. Major version component must be numeric at this point.</para>
        /// </summary>
        /// <param name="partial"></param>
        /// <returns></returns>
        public static SemanticVersion NodeSemverTrim(PartialVersion partial)
        {
            int major = (int)partial.Major._value;

            // Major is guaranteed to be numeric, since non-numerics are handled in AdvancedComparators
            Debug.Assert(major >= 0);

            int minor = (int)partial.Minor._value;
            int patch = minor >= 0 ? (int)partial.Patch._value : -1;

            SemverPreRelease[]? preReleases = null;
            ReadOnlyCollection<SemverPreRelease>? preReleasesReadonly = null;
            if (patch >= 0)
            {
                preReleases = partial._preReleases;
                preReleasesReadonly = partial._preReleasesReadonly;
            }
            else
            {
                patch = 0;
                if (minor < 0)
                    minor = 0;
            }

            return new SemanticVersion(major, minor, patch, preReleases, null, preReleasesReadonly, null);
        }

        [Pure] public static ReadOnlySpan<char> Trim(ReadOnlySpan<char> text, SemverOptions options)
        {
            const SemverOptions bothTrim = SemverOptions.AllowLeadingWhite | SemverOptions.AllowTrailingWhite;
            return (options & bothTrim) switch
            {
                bothTrim => text.Trim(),
                SemverOptions.AllowLeadingWhite => text.TrimStart(),
                SemverOptions.AllowTrailingWhite => text.TrimEnd(),
                _ => text,
            };
        }

        public static void ValidateBuildMetadataItem(string? identifier, [InvokerParameterName] string paramName)
        {
            if (identifier is null)
                throw new ArgumentException(Exceptions.BuildMetadataNull, paramName);
            if (identifier.Length == 0)
                throw new ArgumentException(Exceptions.BuildMetadataEmpty, paramName);
            if (!AllValidCharacters(identifier.AsSpan()))
                throw new ArgumentException(Exceptions.BuildMetadataInvalid, paramName);
        }

        [Pure] public static int CompareIdentifiers(string[] left, string[] right)
        {
            if (ReferenceEquals(left, right)) return 0;

            int leftLength = left.Length;
            int rightLength = right.Length;
            if (leftLength == 0 && rightLength > 0) return 1;
            if (leftLength > 0 && rightLength == 0) return -1;

            for (int i = 0; ; i++)
            {
                if (i == leftLength) return i == rightLength ? 0 : -1;
                if (i == rightLength) return 1;
                int res = string.CompareOrdinal(left[i], right[i]);
                if (res != 0) return res;
            }
        }
        [Pure] public static int CompareIdentifiers(SemverPreRelease[] left, SemverPreRelease[] right)
        {
            if (ReferenceEquals(left, right)) return 0;

            int thisLength = left.Length;
            int otherLength = right.Length;
            if (thisLength == 0 && otherLength > 0) return 1;
            if (thisLength > 0 && otherLength == 0) return -1;

            for (int i = 0; ; i++)
            {
                if (i == thisLength) return i == otherLength ? 0 : -1;
                if (i == otherLength) return 1;
                int res = left[i].CompareTo(right[i]);
                if (res != 0) return res;
            }
        }

        [Pure] public static bool SequenceEqual<T>(T[] left, T[] right) where T : IEquatable<T>
        {
            // Note: String and SemverPreRelease do ordinal comparison by default

#if NET6_0_OR_GREATER
            return left.AsSpan().SequenceEqual(right);
#else
            int length = left.Length;
            if (length != right.Length) return false;
            for (int i = 0; i < length; i++)
                if (!left[i].Equals(right[i]))
                    return false;
            return true;
#endif
        }

        [Pure] public static PrimitiveOperator Normalize(this PrimitiveOperator op)
            => (PrimitiveOperator)Math.Max((byte)op, (byte)1);

        [Pure] public static bool SameDirection(PrimitiveOperator a, PrimitiveOperator b)
            => a > PrimitiveOperator.Equal && b > PrimitiveOperator.Equal && (((byte)a + (byte)b) & 1) == 0;
        [Pure] public static bool IsGTOrGTE(this PrimitiveOperator op)
            => op is PrimitiveOperator.GreaterThan or PrimitiveOperator.GreaterThanOrEqual;
        [Pure] public static bool IsLTOrLTE(this PrimitiveOperator op)
            => op is PrimitiveOperator.LessThan or PrimitiveOperator.LessThanOrEqual;
        [Pure] public static bool IsEQ(this PrimitiveOperator op)
            => op <= PrimitiveOperator.Equal;
        [Pure] public static bool IsSthThanOrEqual(this PrimitiveOperator op)
            => op is PrimitiveOperator.GreaterThanOrEqual or PrimitiveOperator.LessThanOrEqual;
        [Pure] public static PrimitiveOperator Invert(this PrimitiveOperator op)
        {
            // 7 - 2 (>) = 5 (<=)
            // 7 - 3 (<) = 4 (>=)
            // 7 - 4 (>=) = 3 (<)
            // 7 - 5 (<=) = 2 (>)
            return 7 - op;
        }

        [Pure] public static int CompareComparators(PrimitiveComparator? left, PrimitiveComparator? right)
        {
            if (left is null) return right is null ? 0 : -1;
            if (right is null) return 1;
            return CompareComparators(left.Operator, left.Operand, right.Operator, right.Operand);
        }

        [Pure] public static int CompareComparators(
            PrimitiveOperator leftOperator, SemanticVersion leftOperand,
            PrimitiveOperator rightOperator, SemanticVersion rightOperand
        )
        {
            Debug.Assert(!leftOperator.IsEQ() && !rightOperator.IsEQ());

            int cmp = leftOperand.CompareTo(rightOperand);
            // >1.2.3 is greater than >=1.2.3, since it doesn't include =1.2.3
            // <=1.2.3 is greater than <1.2.3, since it also includes =1.2.3
            if (cmp == 0 && leftOperator != rightOperator)
                cmp = leftOperator is PrimitiveOperator.GreaterThan or PrimitiveOperator.LessThanOrEqual ? 1 : -1;
            return cmp;
        }

        /// <summary>
        ///   <para>Determines whether the union of the specified comparators is equivalent to <c>*</c>.</para>
        /// </summary>
        /// <param name="lessThan"></param>
        /// <param name="greaterThan"></param>
        /// <returns></returns>
        [Pure] public static bool DoComparatorsComplement(PrimitiveComparator lessThan, PrimitiveComparator greaterThan)
        {
            Debug.Assert(lessThan.Operator.IsLTOrLTE());
            Debug.Assert(greaterThan.Operator.IsGTOrGTE());

            int cmp = lessThan.Operand.CompareTo(greaterThan.Operand);
            //  <1.3.0 | >=1.3.0 ⇒ *
            // <=1.3.0 |  >1.3.0 ⇒ *
            // <=1.3.0 | >=1.3.0 ⇒ *
            if (cmp == 0) return lessThan.Operator.IsSthThanOrEqual() || greaterThan.Operator.IsSthThanOrEqual();

            // <=1.3.0 | >=1.4.0 ⇒ <=1.3.0 || >=1.4.0
            // <=1.3.0 | >=1.2.0 ⇒ *
            return cmp > 0;
        }

    }
}

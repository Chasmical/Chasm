﻿using System;
using System.Collections.ObjectModel;
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

        public static ReadOnlySpan<char> ReadSemverIdentifier(this scoped ref SpanParser parser)
        {
            int start = parser.position;
            while (parser.position < parser.length && IsValidCharacter(parser.source[parser.position]))
                parser.position++;
            return parser.source.Slice(start, parser.position - start);
        }
        public static ReadOnlySpan<char> ReadPartialComponent(this scoped ref SpanParser parser)
        {
            int start = parser.position;
            while (parser.position < parser.length)
            {
                char next = parser.source[parser.position];
                if ((uint)next - '0' >= 10u && next is not ('x' or 'X' or '*')) break;
                parser.position++;
            }
            return parser.source.Slice(start, parser.position - start);
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
            if (!AllValidCharacters(identifier))
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

        [Pure] public static int GetOperatorLength(PrimitiveOperator op)
        {
            // ImplicitEqual      = (0 + 2) / 3 = 0
            // Equal              = (1 + 2) / 3 = 1, '-'
            // GreaterThan        = (2 + 2) / 3 = 1, '>'
            // LessThan           = (3 + 2) / 3 = 1, '<'
            // GreaterThanOrEqual = (4 + 2) / 3 = 2, '>='
            // LessThanOrEqual    = (5 + 2) / 3 = 2, '<='
            return (int)(op + 2) / 3;
        }
        [Pure] public static PrimitiveOperator Normalize(this PrimitiveOperator op)
            => (PrimitiveOperator)Math.Min((byte)op, (byte)1);

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

    }
}

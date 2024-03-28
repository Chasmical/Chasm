using System;
using Chasm.Formatting;
using Chasm.SemanticVersioning.Ranges;
using JetBrains.Annotations;

namespace Chasm.SemanticVersioning
{
    internal static class Utility
    {
        [Pure] public static bool IsValidCharacter(char c)
        {
#if NET7_0_OR_GREATER
            return char.IsAsciiLetter(c) || char.IsAsciiDigit(c) || c == '-';
#else
            return ((uint)c | ' ') - 'a' <= 'z' - 'a' || (uint)c - '0' <= '9' - '0' || c == '-';
#endif
        }

        [Pure] public static bool IsNumeric(ReadOnlySpan<char> text)
        {
            for (int i = 0, length = text.Length; i < length; i++)
            {
#if NET7_0_OR_GREATER
                if (!char.IsAsciiDigit(text[i]))
#else
                if ((uint)text[i] - '0' >= 10u)
#endif
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

            int thisLength = left.Length;
            int otherLength = right.Length;
            if (thisLength == 0 && otherLength > 0) return 1;
            if (thisLength > 0 && otherLength == 0) return -1;

            for (int i = 0; ; i++)
            {
                if (i == thisLength) return i == otherLength ? 0 : -1;
                if (i == otherLength) return 1;
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

        [Pure] public static bool EqualsIdentifiers(string[] left, string[] right)
        {
            if (ReferenceEquals(left, right)) return true;

            int preReleasesLength = left.Length;
            if (preReleasesLength != right.Length) return false;
            for (int i = 0; i < preReleasesLength; i++)
                if (!left[i].Equals(right[i]))
                    return false;
            return true;
        }
        [Pure] public static bool EqualsIdentifiers(SemverPreRelease[] left, SemverPreRelease[] right)
        {
            if (ReferenceEquals(left, right)) return true;

            int preReleasesLength = left.Length;
            if (preReleasesLength != right.Length) return false;
            for (int i = 0; i < preReleasesLength; i++)
                if (!left[i].Equals(right[i]))
                    return false;
            return true;
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

        [Pure] public static bool SameDirection(PrimitiveOperator left, PrimitiveOperator right)
            => left > PrimitiveOperator.Equal && right > PrimitiveOperator.Equal && ((byte)left & 1) == ((byte)right & 1);
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

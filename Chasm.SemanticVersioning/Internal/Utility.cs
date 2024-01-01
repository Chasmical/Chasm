using System;
using Chasm.Formatting;
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

        [Pure] public static ReadOnlySpan<char> ReadSemverIdentifier(this scoped ref SpanParser parser)
        {
            int start = parser.position;
            while (parser.position < parser.length && IsValidCharacter(parser.source[parser.position]))
                parser.position++;
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

        public static bool TryCopyTo(this string text, Span<char> destination, out int charsWritten)
        {
            if (text.Length > destination.Length)
            {
                charsWritten = 0;
                return false;
            }
            charsWritten = text.Length;
            text
#if !NET6_0_OR_GREATER
                .AsSpan()
#endif
                .CopyTo(destination);
            return true;
        }

    }
}

using JetBrains.Annotations;
using System;

namespace Chasm.SemanticVersioning
{
    internal static class Utility
    {
        [Pure] public static bool IsValidCharacter(char c) => (uint)c - '0' < 10u || (uint)c - 'A' < 26u || (uint)c - 'a' < 26u || c == '-';
        [Pure] public static bool IsLetter(char c) => (uint)c - 'A' < 26u || (uint)c - 'a' < 26u;
        [Pure] public static bool IsDigit(char c) => (uint)c - '0' < 10u;

        [Pure] public static bool IsNumeric(ReadOnlySpan<char> text)
        {
            for (int i = 0, length = text.Length; i < length; i++)
                if ((uint)text[i] - '0' >= 10u)
                    return false;
            return true;
        }
        [Pure] public static bool AllValidCharacters(ReadOnlySpan<char> str)
        {
            for (int i = 0, length = str.Length; i < length; i++)
                if (!IsValidCharacter(str[i]))
                    return false;
            return true;
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

        [Pure] public static int ParseNonNegativeInt32(ReadOnlySpan<char> text)
        {
            int result = text[0] - '0';
            for (int i = 1, length = text.Length; i < length; i++)
            {
                if (result > int.MaxValue / 10) return -1;
                result = result * 10 + (text[i] - '0');
                if (result < 0) return -1;
            }
            return result;
        }

    }
}

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

        public static void ValidateBuildMetadataItem(string? identifier, [InvokerParameterName] string paramName)
        {
            if (identifier is null)
                throw new ArgumentException(Exceptions.BuildMetadataNull, paramName);
            if (identifier.Length == 0)
                throw new ArgumentException(Exceptions.BuildMetadataEmpty, paramName);
            if (!AllValidCharacters(identifier))
                throw new ArgumentException(Exceptions.BuildMetadataInvalid, paramName);
        }

    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Chasm.Formatting;
using JetBrains.Annotations;

namespace Chasm.SemanticVersioning.Ranges
{
    public sealed partial class VersionRange
#if NET7_0_OR_GREATER
        : ISpanParsable<VersionRange>
#endif
    {
        [Pure] internal static SemverErrorCode ParseLoose(ReadOnlySpan<char> text, SemverOptions options, out VersionRange? range)
        {
            SpanParser parser = new SpanParser(text);
            return ParseLoose(ref parser, options, out range);
        }
        [Pure] internal static SemverErrorCode ParseLoose(ref SpanParser parser, SemverOptions options, out VersionRange? range)
        {
            range = null;

            if ((options & SemverOptions.AllowLeadingWhite) != 0)
                parser.SkipWhitespaces();

            bool innerWhite = (options & SemverOptions.AllowInnerWhite) != 0;

            List<Comparator> comparators = [];
            List<List<Comparator>> comparatorSets = [];
            // Note: collection expression here results in some perf/size overhead for some reason
            // ReSharper disable once AppendToCollectionExpression
            comparatorSets.Add(comparators);

            const SemverOptions removeOptions = SemverOptions.AllowEqualsPrefix // '=' is used by comparison comparators
                                              | SemverOptions.AllowInnerWhite // interferes with the hyphen range syntax
                                              | SemverOptions.AllowLeadingWhite // whitespace is handled in this method
                                              | SemverOptions.AllowTrailingWhite;
            // allow leftovers, so that we can continue parsing
            SemverOptions comparatorOptions = (options & ~removeOptions) | SemverOptions.AllowLeftovers;

            while (parser.CanRead())
            {
                SemverErrorCode code = ParseComparator(ref parser, comparatorOptions, innerWhite, out Comparator? comparator);
                if (code is not SemverErrorCode.Success) return code;
                comparators.Add(comparator!);

                // expect separator whitespaces
                if (innerWhite)
                {
                    int beforeSkippingWhitePos = parser.position;
                    parser.SkipWhitespaces();
                    // any amount of whitespaces between comparator sets is fine
                    if (parser.Skip('|', '|'))
                    {
                        parser.SkipWhitespaces();
                        comparatorSets.Add(comparators = []);
                    }
                    // if there's supposed to be whitespace between comparators but there isn't any, exit the parsing cycle
                    else if (parser.position == beforeSkippingWhitePos)
                        break;

                    // at this point, some whitespace was skipped, and we can move on to the next comparator
                }
                else
                {
                    // a space is necessary to continue parsing subsequent comparators
                    bool canPrecedeMoreComparators = parser.Skip(' ');

                    int beforeSkippingWhitePos = parser.position;
                    parser.SkipAll(' ');
                    // determine whether extra spaces were skipped (that's valid only between comparator sets)
                    bool skippedExtraSpaces = parser.position != beforeSkippingWhitePos;

                    // any amount of spaces between comparator sets is fine
                    if (parser.Skip('|', '|'))
                    {
                        parser.SkipAll(' ');
                        comparatorSets.Add(comparators = []);
                    }
                    // if a space wasn't skipped, or extra spaces were skipped (not in between comparator sets), then exit the parsing cycle
                    else if (!canPrecedeMoreComparators || skippedExtraSpaces)
                    {
                        parser.position = beforeSkippingWhitePos;
                        break;
                    }
                    // at this point, a valid number of whitespaces was skipped, and we can move on to the next comparator
                }

            }

            if ((options & SemverOptions.AllowTrailingWhite) == 0)
                parser.UndoSkippingWhitespace();
            else
                parser.SkipWhitespaces();

            if ((options & SemverOptions.AllowLeftovers) == 0 && parser.CanRead())
                return SemverErrorCode.Leftovers;

            ComparatorSet[] createdComparatorSets = new ComparatorSet[comparatorSets.Count];
            // TODO: Is it more efficient to use the array's length here instead?
            // I feel like there's a better chance of JIT optimizing the list enumeration if the list's Count is used
            for (int i = 0; i < comparatorSets.Count; i++)
                createdComparatorSets[i] = new ComparatorSet(comparatorSets[i].ToArray(), default);

            range = new VersionRange(createdComparatorSets, default);
            return SemverErrorCode.Success;
        }

        [Pure] internal static SemverErrorCode ParseComparator(ref SpanParser parser, SemverOptions options, bool innerWhite, out Comparator? comparator)
        {
            comparator = null;
            SemverErrorCode code;
            PartialVersion? partial;

            int onFirstChar = parser.position;
            char next = parser.Read();
            switch (next)
            {
                case '^': // read it as a caret comparator
                    if (innerWhite) parser.SkipWhitespaces();
                    code = PartialVersion.ParseLoose(ref parser, options, out partial);
                    if (code is not SemverErrorCode.Success) return code;
                    comparator = new CaretComparator(partial!);
                    break;
                case '~': // read it as a tilde comparator
                    if (innerWhite) parser.SkipWhitespaces();
                    code = PartialVersion.ParseLoose(ref parser, options, out partial);
                    if (code is not SemverErrorCode.Success) return code;
                    comparator = new TildeComparator(partial!);
                    break;
                case '>' or '<': // read it as a comparison comparator

                    // GreaterThanOrEqual or GreaterThan = 100 or 010
                    // LessThanOrEqual or LessThan       = 101 or 011

                    PrimitiveOperator op = parser.Skip('=') ? PrimitiveOperator.GreaterThanOrEqual : PrimitiveOperator.GreaterThan;
                    // ReSharper disable once BitwiseOperatorOnEnumWithoutFlags
                    if (next == '<') op |= (PrimitiveOperator)1;

                    if (innerWhite) parser.SkipWhitespaces();
                    code = PartialVersion.ParseLoose(ref parser, options, out partial);
                    if (code is not SemverErrorCode.Success) return code;

                    comparator = partial!.IsPartial
                        ? (Comparator)new XRangeComparator(partial, op)
                        : new PrimitiveComparator((SemanticVersion)partial, op);
                    break;
                case '=': // read it as an equality '=' comparator
                    if (innerWhite) parser.SkipWhitespaces();
                    code = PartialVersion.ParseLoose(ref parser, options, out partial);
                    if (code is not SemverErrorCode.Success) return code;

                    comparator = partial!.IsPartial
                        ? (Comparator)new XRangeComparator(partial, PrimitiveOperator.Equal)
                        : new PrimitiveComparator((SemanticVersion)partial, PrimitiveOperator.Equal);
                    break;
                default: // read it as either an implicit '=' comparator, or a hyphen range comparator

                    // un-skip the first character
                    parser.position = onFirstChar;
                    code = PartialVersion.ParseLoose(ref parser, options, out partial);
                    if (code is not SemverErrorCode.Success) return code;

                    // try skipping the hyphen separator
                    if (innerWhite ? SkipHyphenInnerWhite(ref parser) : parser.Skip(' ', '-', ' '))
                    {
                        // read it as a hyphen range comparator
                        code = PartialVersion.ParseLoose(ref parser, options, out PartialVersion? partial2);
                        if (code is not SemverErrorCode.Success) return code;
                        comparator = new HyphenRangeComparator(partial!, partial2!);
                    }
                    else
                    {
                        // read it as an implicit '=' comparator
                        comparator = partial!.IsPartial
                            ? (Comparator)new XRangeComparator(partial)
                            : new PrimitiveComparator((SemanticVersion)partial);
                    }
                    break;
            }

            return SemverErrorCode.Success;
        }

        [Pure] private static bool SkipHyphenInnerWhite(ref SpanParser parser)
        {
            int initialPos = parser.position;
            parser.SkipWhitespaces();
            // the first character can't be a hyphen, so it's safe to omit the position check
            // (the hyphen would get parsed along with the partial versions)
            if (parser.Skip('-'))
            {
                int afterHyphenPos = parser.position;
                parser.SkipWhitespaces();
                // if a whitespace was skipped, succeed
                if (parser.position != afterHyphenPos)
                    return true;
            }
            // revert the parser's position
            parser.position = initialPos;
            return false;
        }

        /// <summary>
        ///   <para>Converts the specified string representation of a version range to an equivalent <see cref="VersionRange"/> instance.</para>
        /// </summary>
        /// <param name="text">The string containing a version range to convert.</param>
        /// <returns>The <see cref="VersionRange"/> instance equivalent to the version range specified in the <paramref name="text"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="text"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="text"/> is not a valid version range.</exception>
        [Pure] public static VersionRange Parse(string text)
            => Parse(text, SemverOptions.Strict);
        /// <summary>
        ///   <para>Converts the specified read-only span of characters representing a version range to an equivalent <see cref="VersionRange"/> instance.</para>
        /// </summary>
        /// <param name="text">The read-only span of characters containing a version range to convert.</param>
        /// <returns>The <see cref="VersionRange"/> instance equivalent to the version range specified in the <paramref name="text"/>.</returns>
        /// <exception cref="ArgumentException"><paramref name="text"/> is not a valid version range.</exception>
        [Pure] public static VersionRange Parse(ReadOnlySpan<char> text)
            => Parse(text, SemverOptions.Strict);
        /// <summary>
        ///   <para>Tries to convert the specified string representation of a version range to an equivalent <see cref="VersionRange"/> instance, and returns a value indicating whether the conversion was successful.</para>
        /// </summary>
        /// <param name="text">The string containing a version range to convert.</param>
        /// <param name="range">When this method returns, contains the <see cref="VersionRange"/> instance equivalent to the version range specified in the <paramref name="text"/>, if the conversion succeeded, or <see langword="null"/> if the conversion failed.</param>
        /// <returns><see langword="true"/>, if the conversion was successful; otherwise, <see langword="false"/>.</returns>
        [Pure] public static bool TryParse(string? text, [NotNullWhen(true)] out VersionRange? range)
        {
            if (text is not null) return TryParse(text, SemverOptions.Strict, out range);
            range = null;
            return false;
        }
        /// <summary>
        ///   <para>Tries to convert the specified read-only span of characters representing a version range to an equivalent <see cref="VersionRange"/> instance, and returns a value indicating whether the conversion was successful.</para>
        /// </summary>
        /// <param name="text">The read-only span of characters containing a version range to convert.</param>
        /// <param name="range">When this method returns, contains the <see cref="VersionRange"/> instance equivalent to the version range specified in the <paramref name="text"/>, if the conversion succeeded, or <see langword="null"/> if the conversion failed.</param>
        /// <returns><see langword="true"/>, if the conversion was successful; otherwise, <see langword="false"/>.</returns>
        [Pure] public static bool TryParse(ReadOnlySpan<char> text, [NotNullWhen(true)] out VersionRange? range)
            => TryParse(text, SemverOptions.Strict, out range);

        /// <summary>
        ///   <para>Converts the specified string representation of a version range to an equivalent <see cref="VersionRange"/> instance, using the specified parsing <paramref name="options"/>.</para>
        /// </summary>
        /// <param name="text">The string containing a version range to convert.</param>
        /// <param name="options">The version range parsing options to use.</param>
        /// <returns>The <see cref="VersionRange"/> instance equivalent to the version range specified in the <paramref name="text"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="text"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="text"/> is not a valid version range.</exception>
        [Pure] public static VersionRange Parse(string text, SemverOptions options)
        {
            ANE.ThrowIfNull(text);
            return Parse(text.AsSpan(), options);
        }
        /// <summary>
        ///   <para>Converts the specified read-only span of characters representing a version range to an equivalent <see cref="VersionRange"/> instance, using the specified parsing <paramref name="options"/>.</para>
        /// </summary>
        /// <param name="text">The read-only span of characters containing a version range to convert.</param>
        /// <param name="options">The version range parsing options to use.</param>
        /// <returns>The <see cref="VersionRange"/> instance equivalent to the version range specified in the <paramref name="text"/>.</returns>
        /// <exception cref="ArgumentException"><paramref name="text"/> is not a valid version range.</exception>
        [Pure] public static VersionRange Parse(ReadOnlySpan<char> text, SemverOptions options)
            => ParseLoose(text, options, out VersionRange? range).ReturnOrThrow(range, nameof(text));
        /// <summary>
        ///   <para>Tries to convert the specified string representation of a version range to an equivalent <see cref="VersionRange"/> instance, using the specified parsing <paramref name="options"/>, and returns a value indicating whether the conversion was successful.</para>
        /// </summary>
        /// <param name="text">The string containing a version range to convert.</param>
        /// <param name="options">The version range parsing options to use.</param>
        /// <param name="range">When this method returns, contains the <see cref="VersionRange"/> instance equivalent to the version range specified in the <paramref name="text"/>, if the conversion succeeded, or <see langword="null"/> if the conversion failed.</param>
        /// <returns><see langword="true"/>, if the conversion was successful; otherwise, <see langword="false"/>.</returns>
        [Pure] public static bool TryParse(string? text, SemverOptions options, [NotNullWhen(true)] out VersionRange? range)
        {
            if (text is not null) return TryParse(text.AsSpan(), options, out range);
            range = null;
            return false;
        }
        /// <summary>
        ///   <para>Tries to convert the specified read-only span of characters representing a version range to an equivalent <see cref="VersionRange"/> instance, using the specified parsing <paramref name="options"/>, and returns a value indicating whether the conversion was successful.</para>
        /// </summary>
        /// <param name="text">The read-only span of characters containing a version range to convert.</param>
        /// <param name="options">The version range parsing options to use.</param>
        /// <param name="range">When this method returns, contains the <see cref="VersionRange"/> instance equivalent to the version range specified in the <paramref name="text"/>, if the conversion succeeded, or <see langword="null"/> if the conversion failed.</param>
        /// <returns><see langword="true"/>, if the conversion was successful; otherwise, <see langword="false"/>.</returns>
        [Pure] public static bool TryParse(ReadOnlySpan<char> text, SemverOptions options, [NotNullWhen(true)] out VersionRange? range)
            => ParseLoose(text, options, out range) is SemverErrorCode.Success;

#if NET7_0_OR_GREATER
        [Pure] static VersionRange IParsable<VersionRange>.Parse(string s, IFormatProvider? _)
            => Parse(s);
        [Pure] static bool IParsable<VersionRange>.TryParse(string? s, IFormatProvider? _, [NotNullWhen(true)] out VersionRange? range)
            => TryParse(s, out range);
        [Pure] static VersionRange ISpanParsable<VersionRange>.Parse(ReadOnlySpan<char> s, IFormatProvider? _)
            => Parse(s);
        [Pure] static bool ISpanParsable<VersionRange>.TryParse(ReadOnlySpan<char> s, IFormatProvider? _, [NotNullWhen(true)] out VersionRange? range)
            => TryParse(s, out range);
#endif

    }
}

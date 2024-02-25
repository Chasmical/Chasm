using System;
using System.Collections.Generic;
using Chasm.Formatting;
using JetBrains.Annotations;

namespace Chasm.SemanticVersioning.Ranges
{
    public sealed partial class VersionRange
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
            List<List<Comparator>> comparatorSets = [comparators];

            const SemverOptions removeOptions = SemverOptions.AllowEqualsPrefix // '=' is used by comparison comparators
                                              | SemverOptions.AllowInnerWhite // interferes with the hyphen range syntax
                                              | SemverOptions.AllowLeadingWhite // whitespace is handled in this method
                                              | SemverOptions.AllowTrailingWhite;
            // allow leftovers, so that we can continue parsing
            SemverOptions comparatorOptions = (options & ~removeOptions) | SemverOptions.AllowLeftovers;

            while (parser.CanRead())
            {
                SemverErrorCode code = ParseComparator(ref parser, comparatorOptions, out Comparator? comparator);
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

        internal static SemverErrorCode ParseComparator(ref SpanParser parser, SemverOptions options, out Comparator? comparator)
        {
            comparator = null;
            SemverErrorCode code;
            PartialVersion? partial;
            PrimitiveOperator op;

            char next = parser.Read();
            switch (next)
            {
                case '^': // read it as a caret comparator
                    code = PartialVersion.ParseLoose(ref parser, options, out partial);
                    if (code is not SemverErrorCode.Success) return code;
                    comparator = new CaretComparator(partial!);
                    break;
                case '~': // read it as a tilde comparator
                    code = PartialVersion.ParseLoose(ref parser, options, out partial);
                    if (code is not SemverErrorCode.Success) return code;
                    comparator = new TildeComparator(partial!);
                    break;
                case '>': // read it as a '>' or '>=' comparator
                    op = parser.Skip('=') ? PrimitiveOperator.GreaterThanOrEqual : PrimitiveOperator.GreaterThan;
                    code = PartialVersion.ParseLoose(ref parser, options, out partial);
                    if (code is not SemverErrorCode.Success) return code;

                    comparator = partial!.IsPartial
                        ? (Comparator)new XRangeComparator(partial, op)
                        : new PrimitiveComparator(new SemanticVersion(partial), op);
                    break;
                case '<': // read it as a '<' or '<=' comparator
                    op = parser.Skip('=') ? PrimitiveOperator.LessThanOrEqual : PrimitiveOperator.LessThan;
                    code = PartialVersion.ParseLoose(ref parser, options, out partial);
                    if (code is not SemverErrorCode.Success) return code;

                    comparator = partial!.IsPartial
                        ? (Comparator)new XRangeComparator(partial, op)
                        : new PrimitiveComparator(new SemanticVersion(partial), op);
                    break;
                case '=': // read it as an '=' comparator
                    code = PartialVersion.ParseLoose(ref parser, options, out partial);
                    if (code is not SemverErrorCode.Success) return code;

                    comparator = partial!.IsPartial
                        ? (Comparator)new XRangeComparator(partial, PrimitiveOperator.Equal)
                        : new PrimitiveComparator(new SemanticVersion(partial), PrimitiveOperator.Equal);
                    break;
                default: // read it as either an implicit '=' comparator, or a hyphen range comparator

                    // un-skip the first character
                    parser.position--;
                    code = PartialVersion.ParseLoose(ref parser, options, out partial);
                    if (code is not SemverErrorCode.Success) return code;

                    // try skipping the hyphen separator
                    if ((options & SemverOptions.AllowInnerWhite) != 0 ? SkipHyphenInnerWhite(ref parser) : parser.Skip(' ', '-', ' '))
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
                            ? (Comparator)new XRangeComparator(partial, PrimitiveOperator.Equal)
                            : new PrimitiveComparator(new SemanticVersion(partial), PrimitiveOperator.Equal);
                    }
                    break;
            }

            return SemverErrorCode.Success;
        }

        private static bool SkipHyphenInnerWhite(ref SpanParser parser)
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



    }
}

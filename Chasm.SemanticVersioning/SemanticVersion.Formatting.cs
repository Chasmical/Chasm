using System;
using System.Globalization;
using Chasm.Formatting;
using JetBrains.Annotations;

namespace Chasm.SemanticVersioning
{
    public sealed partial class SemanticVersion
        : ISpanBuildable, ISpanBuildableFormat
#if NET6_0_OR_GREATER
        , ISpanFormattable
#else
        , IFormattable
#endif
    {
        [Pure] internal int CalculateLength()
        {
            int length = 2 + SpanBuilder.CalculateLength((uint)Major)
                           + SpanBuilder.CalculateLength((uint)Minor)
                           + SpanBuilder.CalculateLength((uint)Patch);

            SemverPreRelease[] preReleases = _preReleases;
            if (preReleases.Length != 0)
            {
                length += preReleases.Length;
                for (int i = 0; i < preReleases.Length; i++)
                    length += preReleases[i].CalculateLength();
            }
            string[] buildMetadata = _buildMetadata;
            if (buildMetadata.Length != 0)
            {
                length += buildMetadata.Length;
                for (int i = 0; i < buildMetadata.Length; i++)
                    length += buildMetadata[i].Length;
            }
            return length;
        }
        internal void BuildString(ref SpanBuilder sb)
        {
            sb.Append((uint)Major);
            sb.Append('.');
            sb.Append((uint)Minor);
            sb.Append('.');
            sb.Append((uint)Patch);

            SemverPreRelease[] preReleases = _preReleases;
            if (preReleases.Length != 0)
            {
                sb.Append('-');
                preReleases[0].BuildString(ref sb);
                for (int i = 1; i < preReleases.Length; i++)
                {
                    sb.Append('.');
                    preReleases[i].BuildString(ref sb);
                }
            }
            string[] buildMetadata = _buildMetadata;
            if (buildMetadata.Length != 0)
            {
                sb.Append('+');
                sb.Append(buildMetadata[0]);
                for (int i = 1; i < buildMetadata.Length; i++)
                {
                    sb.Append('.');
                    sb.Append(buildMetadata[i]);
                }
            }
        }
        [Pure] int ISpanBuildable.CalculateLength() => CalculateLength();
        void ISpanBuildable.BuildString(ref SpanBuilder sb) => BuildString(ref sb);

        [Pure] internal int CalculateLength(ReadOnlySpan<char> format)
        {
            if (format.IsEmpty) return CalculateLength();

            // NOTE: See the BuildString method for comments.

            SpanParser parser = new SpanParser(format);

            SemverPreRelease[] preReleases = _preReleases;
            string[] buildMetadata = _buildMetadata;

            int preReleaseIndex = 0;
            int buildMetadataIndex = 0;
            char separator = '\0';

            static void FlushSeparator(ref int length, char separator)
            {
                if (separator == '\0') return;
                length++;
            }

            int length = 0;
            while (parser.CanRead())
            {
                char read = parser.Read();
                switch (read)
                {
                    case 'M':
                        if (parser.Skip('M')) throw new FormatException();
                        FlushSeparator(ref length, separator);
                        length += SpanBuilder.CalculateLength((uint)Major);
                        break;
                    case 'm':
                        if (parser.Skip('m') && Minor == 0 && Patch == 0) break;
                        FlushSeparator(ref length, separator);
                        length += SpanBuilder.CalculateLength((uint)Minor);
                        break;
                    case 'p':
                        if (parser.Skip('p') && Patch == 0) break;
                        FlushSeparator(ref length, separator);
                        length += SpanBuilder.CalculateLength((uint)Patch);
                        break;
                    case 'r':
                        if (parser.Skip('r'))
                        {
                            if (preReleaseIndex < preReleases.Length)
                            {
                                FlushSeparator(ref length, separator);
                                length += preReleases[preReleaseIndex++].CalculateLength();
                                length += preReleases.Length - preReleaseIndex; // '.' separators
                                while (preReleaseIndex < preReleases.Length)
                                    length += preReleases[preReleaseIndex++].CalculateLength();
                            }
                            break;
                        }
                        if (parser.OnAsciiDigit)
                        {
                            ReadOnlySpan<char> digits = parser.ReadAsciiDigits();
                            preReleaseIndex = int.Parse(digits, NumberStyles.None);
                        }
                        if (preReleaseIndex < preReleases.Length)
                        {
                            FlushSeparator(ref length, separator);
                            length += preReleases[preReleaseIndex++].CalculateLength();
                        }
                        break;
                    case 'd':
                        if (parser.Skip('d'))
                        {
                            if (buildMetadataIndex < buildMetadata.Length)
                            {
                                FlushSeparator(ref length, separator);
                                length += buildMetadata[buildMetadataIndex++].Length;
                                length += buildMetadata.Length - buildMetadataIndex; // '.' separators
                                while (buildMetadataIndex < buildMetadata.Length)
                                    length += buildMetadata[buildMetadataIndex++].Length;
                            }
                            break;
                        }
                        if (parser.OnAsciiDigit)
                        {
                            ReadOnlySpan<char> digits = parser.ReadAsciiDigits();
                            buildMetadataIndex = int.Parse(digits, NumberStyles.None);
                        }
                        if (buildMetadataIndex < buildMetadata.Length)
                        {
                            FlushSeparator(ref length, separator);
                            length += buildMetadata[buildMetadataIndex++].Length;
                        }
                        break;
                    default:
                        FlushSeparator(ref length, separator);
                        switch (read)
                        {
                            case '\'' or '"':
                                ReadOnlySpan<char> escaped = parser.ReadUntil(read);
                                if (!parser.Skip(read)) throw new FormatException();
                                length += escaped.Length;
                                break;
                            case '.' or '-' or '+' or '_' or ' ':
                                separator = read;
                                continue;
                            case '\\':
                                parser.Skip();
                                goto default;
                            default:
                                length++;
                                break;
                        }
                        break;
                }
                separator = '\0';
            }

            FlushSeparator(ref length, separator);

            return length;
        }
        internal void BuildString(ref SpanBuilder sb, ReadOnlySpan<char> format)
        {
            if (format.IsEmpty)
            {
                BuildString(ref sb);
                return;
            }

            SpanParser parser = new SpanParser(format);

            SemverPreRelease[] preReleases = _preReleases;
            string[] buildMetadata = _buildMetadata;

            int preReleaseIndex = 0;
            int buildMetadataIndex = 0;
            char separator = '\0';

            static void FlushSeparator(ref SpanBuilder sb, char separator)
            {
                if (separator == '\0') return;
                sb.Append(separator);
            }

            while (parser.CanRead())
            {
                char read = parser.Read();
                switch (read)
                {
                    case 'M':
                        // write the major version component
                        if (parser.Skip('M')) throw new FormatException();
                        FlushSeparator(ref sb, separator);
                        sb.Append((uint)Major);
                        break;
                    case 'm':
                        // write the minor version component (maybe optional)
                        if (parser.Skip('m') && Minor == 0 && Patch == 0) break;
                        FlushSeparator(ref sb, separator);
                        sb.Append((uint)Minor);
                        break;
                    case 'p':
                        // write the patch version component (maybe optional)
                        if (parser.Skip('p') && Patch == 0) break;
                        FlushSeparator(ref sb, separator);
                        sb.Append((uint)Patch);
                        break;
                    case 'r':
                        if (parser.Skip('r'))
                        {
                            // write all remaining pre-releases after the last one written
                            if (preReleaseIndex < preReleases.Length)
                            {
                                FlushSeparator(ref sb, separator);
                                preReleases[preReleaseIndex++].BuildString(ref sb);
                                while (preReleaseIndex < preReleases.Length)
                                {
                                    sb.Append('.');
                                    preReleases[preReleaseIndex++].BuildString(ref sb);
                                }
                            }
                            break;
                        }
                        // optionally set the next pre-release index
                        if (parser.OnAsciiDigit)
                        {
                            ReadOnlySpan<char> digits = parser.ReadAsciiDigits();
                            preReleaseIndex = int.Parse(digits, NumberStyles.None);
                        }
                        // write the next pre-release
                        if (preReleaseIndex < preReleases.Length)
                        {
                            FlushSeparator(ref sb, separator);
                            preReleases[preReleaseIndex++].BuildString(ref sb);
                        }
                        break;
                    case 'd':
                        if (parser.Skip('d'))
                        {
                            // write all remaining build metadata after the last one written
                            if (buildMetadataIndex < buildMetadata.Length)
                            {
                                FlushSeparator(ref sb, separator);
                                sb.Append(buildMetadata[buildMetadataIndex++]);
                                while (buildMetadataIndex < buildMetadata.Length)
                                {
                                    sb.Append('.');
                                    sb.Append(buildMetadata[buildMetadataIndex++]);
                                }
                            }
                            break;
                        }
                        // optionally set the next build metadata index
                        if (parser.OnAsciiDigit)
                        {
                            ReadOnlySpan<char> digits = parser.ReadAsciiDigits();
                            buildMetadataIndex = int.Parse(digits, NumberStyles.None);
                        }
                        // write the next build metadata
                        if (buildMetadataIndex < buildMetadata.Length)
                        {
                            FlushSeparator(ref sb, separator);
                            sb.Append(buildMetadata[buildMetadataIndex++]);
                        }
                        break;
                    default:
                        // flush separator (the following format characters don't remove preceding separators)
                        FlushSeparator(ref sb, separator);
                        switch (read)
                        {
                            case '\'' or '"':
                                // append the text enclosed in quotes (quote characters can't be escaped inside)
                                ReadOnlySpan<char> escaped = parser.ReadUntil(read);
                                if (!parser.Skip(read)) throw new FormatException();
                                sb.Append(escaped);
                                break;
                            case '\\':
                                // append the following character as is
                                sb.Append(parser.Read());
                                break;
                            case '.' or '-' or '+' or '_' or ' ':
                                // set the separator character and continue (skipping separator reset)
                                separator = read;
                                continue;
                            default:
                                // not a format character, append as is
                                sb.Append(read);
                                break;
                        }
                        break;
                }
                // reset separator
                separator = '\0';
            }

            FlushSeparator(ref sb, separator);
        }
        [Pure] int ISpanBuildableFormat.CalculateLength(ReadOnlySpan<char> format) => CalculateLength(format);
        void ISpanBuildableFormat.BuildString(ref SpanBuilder sb, ReadOnlySpan<char> format) => BuildString(ref sb, format);

        /// <summary>
        ///   <para>Returns the SemVer 2.0.0 compliant string representation of this semantic version.</para>
        /// </summary>
        /// <returns>The SemVer 2.0.0 compliant string representation of this semantic version.</returns>
        [Pure] public override string ToString() => SpanBuilder.Format(this);

        [Pure] public string ToString(string? format) => ToString(format.AsSpan());
        [Pure] public string ToString(ReadOnlySpan<char> format) => SpanBuilder.Format(this, format);

        public bool TryFormat(Span<char> destination, out int charsWritten)
            => SpanBuilder.TryFormat(this, destination, out charsWritten);
        public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format)
            => SpanBuilder.TryFormat(this, destination, out charsWritten, format);

        string IFormattable.ToString(string? format, IFormatProvider? _)
            => ToString(format);
#if NET6_0_OR_GREATER
        bool ISpanFormattable.TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? _)
            => TryFormat(destination, out charsWritten, format);
#endif

#if NOT_PUBLISHING_PACKAGE
        // Note: This method is only used in benchmarks.
        // See here: /Chasm.SemanticVersioning.Benchmarks/SpanBuilderVsStringBuilder.cs

        [Pure, System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        internal string ToStringWithStringBuilder()
        {
            System.Text.StringBuilder sb = new();
            sb.Append((uint)Major).Append('.').Append((uint)Minor).Append('.').Append((uint)Patch);

            SemverPreRelease[] preReleases = _preReleases;
            if (preReleases.Length != 0)
            {
                sb.Append('-').Append(preReleases[0]);
                for (int i = 1; i < preReleases.Length; i++)
                    sb.Append('.').Append(preReleases[i]);
            }
            string[] buildMetadata = _buildMetadata;
            if (buildMetadata.Length != 0)
            {
                sb.Append('+').Append(buildMetadata[0]);
                for (int i = 1; i < buildMetadata.Length; i++)
                    sb.Append('.').Append(buildMetadata[i]);
            }

            return sb.ToString();
        }
        [Pure, System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        internal string ToStringWithStringBuilder(ReadOnlySpan<char> format)
        {
            if (format.IsEmpty) return ToString();

            System.Text.StringBuilder sb = new();

            SpanParser parser = new SpanParser(format);

            SemverPreRelease[] preReleases = _preReleases;
            string[] buildMetadata = _buildMetadata;

            int preReleaseIndex = 0;
            int buildMetadataIndex = 0;
            char separator = '\0';

            static void FlushSeparator(System.Text.StringBuilder sb, char separator)
            {
                if (separator == '\0') return;
                sb.Append(separator);
            }

            while (parser.CanRead())
            {
                char read = parser.Read();
                switch (read)
                {
                    case 'M':
                        // write the major version component
                        if (parser.Skip('M')) throw new FormatException();
                        FlushSeparator(sb, separator);
                        sb.Append((uint)Major);
                        break;
                    case 'm':
                        // write the minor version component (maybe optional)
                        if (parser.Skip('m') && Minor == 0 && Patch == 0) break;
                        FlushSeparator(sb, separator);
                        sb.Append((uint)Minor);
                        break;
                    case 'p':
                        // write the patch version component (maybe optional)
                        if (parser.Skip('p') && Patch == 0) break;
                        FlushSeparator(sb, separator);
                        sb.Append((uint)Patch);
                        break;
                    case 'r':
                        if (parser.Skip('r'))
                        {
                            // write all remaining pre-releases after the last one written
                            if (preReleaseIndex < preReleases.Length)
                            {
                                FlushSeparator(sb, separator);
                                sb.Append(preReleases[preReleaseIndex++]);
                                while (preReleaseIndex < preReleases.Length)
                                    sb.Append('.').Append(preReleases[preReleaseIndex++]);
                            }
                            break;
                        }
                        // optionally set the next pre-release index
                        if (parser.OnAsciiDigit)
                        {
                            ReadOnlySpan<char> digits = parser.ReadAsciiDigits();
                            preReleaseIndex = int.Parse(digits, NumberStyles.None);
                        }
                        // write the next pre-release
                        if (preReleaseIndex < preReleases.Length)
                        {
                            FlushSeparator(sb, separator);
                            sb.Append(preReleases[preReleaseIndex++]);
                        }
                        break;
                    case 'd':
                        if (parser.Skip('d'))
                        {
                            // write all remaining build metadata after the last one written
                            if (buildMetadataIndex < buildMetadata.Length)
                            {
                                FlushSeparator(sb, separator);
                                sb.Append(buildMetadata[buildMetadataIndex++]);
                                while (buildMetadataIndex < buildMetadata.Length)
                                {
                                    sb.Append('.');
                                    sb.Append(buildMetadata[buildMetadataIndex++]);
                                }
                            }
                            break;
                        }
                        // optionally set the next build metadata index
                        if (parser.OnAsciiDigit)
                        {
                            ReadOnlySpan<char> digits = parser.ReadAsciiDigits();
                            buildMetadataIndex = int.Parse(digits, NumberStyles.None);
                        }
                        // write the next build metadata
                        if (buildMetadataIndex < buildMetadata.Length)
                        {
                            FlushSeparator(sb, separator);
                            sb.Append(buildMetadata[buildMetadataIndex++]);
                        }
                        break;
                    default:
                        // flush separator (the following format characters don't remove preceding separators)
                        FlushSeparator(sb, separator);
                        switch (read)
                        {
                            case '\'' or '"':
                                // append the text enclosed in quotes (quote characters can't be escaped inside)
                                ReadOnlySpan<char> escaped = parser.ReadUntil(read);
                                if (!parser.Skip(read)) throw new FormatException();
                                sb.Append(escaped);
                                break;
                            case '\\':
                                // append the following character as is
                                sb.Append(parser.Read());
                                break;
                            case '.' or '-' or '+' or '_' or ' ':
                                // set the separator character and continue (skipping separator reset)
                                separator = read;
                                continue;
                            default:
                                // not a format character, append as is
                                sb.Append(read);
                                break;
                        }
                        break;
                }
                // reset separator
                separator = '\0';
            }

            FlushSeparator(sb, separator);

            return sb.ToString();
        }
#endif

    }
}

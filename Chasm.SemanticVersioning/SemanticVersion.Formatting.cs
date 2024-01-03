using Chasm.Formatting;
using JetBrains.Annotations;

namespace Chasm.SemanticVersioning
{
    public sealed partial class SemanticVersion : ISpanBuildable
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

        [Pure] public override string ToString() => SpanBuilder.Format(this);

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
#endif

    }
}

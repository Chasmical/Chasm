using System;
using Chasm.Formatting;
using JetBrains.Annotations;

namespace Chasm.SemanticVersioning.Ranges
{
    public sealed partial class PartialVersion : ISpanBuildable
    {
        [Pure] internal int CalculateLength()
        {
            int length = Major.CalculateLength();

            int componentLength = Minor.CalculateLength();
            if (componentLength != 0) length += componentLength + 1;
            componentLength = Patch.CalculateLength();
            if (componentLength != 0) length += componentLength + 1;

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
            Major.BuildString(ref sb);
            if (!Minor.IsOmitted)
            {
                sb.Append('.');
                Minor.BuildString(ref sb);
                if (!Patch.IsOmitted)
                {
                    sb.Append('.');
                    Patch.BuildString(ref sb);
                }
            }

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
                sb.Append(buildMetadata[0].AsSpan());
                for (int i = 1; i < buildMetadata.Length; i++)
                {
                    sb.Append('.');
                    sb.Append(buildMetadata[i].AsSpan());
                }
            }
        }
        [Pure] int ISpanBuildable.CalculateLength() => CalculateLength();
        void ISpanBuildable.BuildString(ref SpanBuilder sb) => BuildString(ref sb);

        /// <summary>
        ///   <para>Returns the string representation of this partial version.</para>
        /// </summary>
        /// <returns>The string representation of this partial version.</returns>
        [Pure] public override string ToString() => SpanBuilder.Format(this);

    }
}

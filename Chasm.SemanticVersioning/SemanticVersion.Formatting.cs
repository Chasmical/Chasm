using System.Text;
using JetBrains.Annotations;

namespace Chasm.SemanticVersioning
{
    public sealed partial class SemanticVersion
    {
        [Pure] public override string ToString()
        {
            StringBuilder sb = new();
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

    }
}

using System;

namespace Chasm.SemanticVersioning
{
    [Flags]
    public enum SemverComparison : byte
    {
        Default                = 0,

        IncludeBuildMetadata   = 1 << 0,
        DifferentiateWildcards = 1 << 1,

    }
}

using System;
// ReSharper disable InconsistentNaming IdentifierTypo

namespace Chasm.SemanticVersioning
{
    [Flags]
    internal enum SemverErrorCode : byte
    {
        Success = 0,

        IdentifierMask = 0b_0000_1111,

        COMPONENT      = 0b_0000_0001,
        MAJOR          = 0b_0000_0010,
        MINOR          = 0b_0000_0011,
        PATCH          = 0b_0000_0100,
        PRERELEASE     = 0b_0000_0101,
        BUILD_METADATA = 0b_0000_0110,

        ErrorTypeMask  = 0b_1111_0000,

        NOT_FOUND      = 0b_0001_0000,
        LEADING_ZEROES = 0b_0010_0000,
        NEGATIVE       = 0b_0011_0000,
        TOO_BIG        = 0b_0100_0000,
        EMPTY          = 0b_0101_0000,
        INVALID        = 0b_0110_0000,
        LEFTOVERS      = 0b_0111_0000,

        MajorNotFound           = MAJOR          | NOT_FOUND,
        MinorNotFound           = MINOR          | NOT_FOUND,
        PatchNotFound           = PATCH          | NOT_FOUND,

        ComponentLeadingZeroes  = COMPONENT      | LEADING_ZEROES,
        MajorLeadingZeroes      = MAJOR          | LEADING_ZEROES,
        MinorLeadingZeroes      = MINOR          | LEADING_ZEROES,
        PatchLeadingZeroes      = PATCH          | LEADING_ZEROES,
        PreReleaseLeadingZeroes = PRERELEASE     | LEADING_ZEROES,

        ComponentNegative       = COMPONENT      | NEGATIVE,
        MajorNegative           = MAJOR          | NEGATIVE,
        MinorNegative           = MINOR          | NEGATIVE,
        PatchNegative           = PATCH          | NEGATIVE,
        PreReleaseNegative      = PRERELEASE     | NEGATIVE,

        ComponentTooBig         = COMPONENT      | TOO_BIG,
        MajorTooBig             = MAJOR          | TOO_BIG,
        MinorTooBig             = MINOR          | TOO_BIG,
        PatchTooBig             = PATCH          | TOO_BIG,
        PreReleaseTooBig        = PRERELEASE     | TOO_BIG,

        PreReleaseEmpty         = PRERELEASE     | EMPTY,
        BuildMetadataEmpty      = BUILD_METADATA | EMPTY,

        PreReleaseInvalid       = PRERELEASE     | INVALID,
        BuildMetadataInvalid    = BUILD_METADATA | INVALID,

        Leftovers               = LEFTOVERS,

    }
}

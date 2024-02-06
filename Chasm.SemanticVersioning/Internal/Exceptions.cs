using System;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Chasm.SemanticVersioning
{
    internal static class Exceptions
    {
        public const string MajorNotFound = "The major version component could not be found.";
        public const string MinorNotFound = "The minor version component could not be found.";
        public const string PatchNotFound = "The patch version component could not be found.";

        public const string ComponentLeadingZeroes = "The version component cannot contain leading zeroes.";
        public const string MajorLeadingZeroes = "The major version component cannot contain leading zeroes.";
        public const string MinorLeadingZeroes = "The minor version component cannot contain leading zeroes.";
        public const string PatchLeadingZeroes = "The patch version component cannot contain leading zeroes.";
        public const string PreReleaseLeadingZeroes = "The numeric pre-release identifier cannot contain leading zeroes.";

        public const string ComponentNegative = "The version component cannot be less than 0.";
        public const string MajorNegative = "The major version component cannot be less than 0.";
        public const string MinorNegative = "The minor version component cannot be less than 0.";
        public const string PatchNegative = "The patch version component cannot be less than 0.";
        public const string PreReleaseNegative = "The numeric pre-release identifier cannot be less than 0.";

        public const string ComponentTooBig = "The version component cannot be greater than 2147483647.";
        public const string MajorTooBig = "The major version component cannot be greater than 2147483647.";
        public const string MinorTooBig = "The minor version component cannot be greater than 2147483647.";
        public const string PatchTooBig = "The patch version component cannot be greater than 2147483647.";
        public const string PreReleaseTooBig = "The numeric pre-release identifier cannot be greater than 2147483647.";

        public const string PreReleaseEmpty = "The pre-release identifier cannot be empty.";
        public const string BuildMetadataEmpty = "The build metadata identifier cannot be empty.";
        public const string BuildMetadataNull = "The build metadata identifiers cannot be null.";

        public const string ComponentInvalid = "The partial version component must be either numeric or a wildcard character.";
        public const string PreReleaseInvalid = "The pre-release identifier must only contain [A-Za-z0-9-] characters.";
        public const string BuildMetadataInvalid = "The build metadata identifier must only contain [A-Za-z0-9-] characters.";

        public const string ComponentNotNumeric = "The version component is not numeric.";
        public const string ComponentNotWildcard = "The version component is not a wildcard.";
        public const string PreReleaseNotNumeric = "The pre-release identifier is not numeric.";

        public const string Leftovers = "Encountered an invalid semantic version character during parsing.";

        public const string ComparatorsNull = "The version comparators cannot be null.";
        public const string ComparatorSetsNull = "The version comparator sets cannot be null.";
        public const string EmptyVersionRange = "The version range must contain at least one comparator set.";

        public const string MajorOmitted = "The major version component cannot be omitted.";
        public const string MinorOmitted = "The minor version component cannot be omitted, if the patch component isn't.";

        [Pure] public static string GetMessage(this SemverErrorCode code) => code switch
        {
            SemverErrorCode.MajorNotFound => MajorNotFound,
            SemverErrorCode.MinorNotFound => MinorNotFound,
            SemverErrorCode.PatchNotFound => PatchNotFound,

            SemverErrorCode.ComponentLeadingZeroes => ComponentLeadingZeroes,
            SemverErrorCode.MajorLeadingZeroes => MajorLeadingZeroes,
            SemverErrorCode.MinorLeadingZeroes => MinorLeadingZeroes,
            SemverErrorCode.PatchLeadingZeroes => PatchLeadingZeroes,
            SemverErrorCode.PreReleaseLeadingZeroes => PreReleaseLeadingZeroes,

            SemverErrorCode.ComponentNegative => ComponentNegative,
            SemverErrorCode.MajorNegative => MajorNegative,
            SemverErrorCode.MinorNegative => MinorNegative,
            SemverErrorCode.PatchNegative => PatchNegative,
            SemverErrorCode.PreReleaseNegative => PreReleaseNegative,

            SemverErrorCode.ComponentTooBig => ComponentTooBig,
            SemverErrorCode.MajorTooBig => MajorTooBig,
            SemverErrorCode.MinorTooBig => MinorTooBig,
            SemverErrorCode.PatchTooBig => PatchTooBig,
            SemverErrorCode.PreReleaseTooBig => PreReleaseTooBig,

            SemverErrorCode.PreReleaseEmpty => PreReleaseEmpty,
            SemverErrorCode.BuildMetadataEmpty => BuildMetadataEmpty,

            SemverErrorCode.PreReleaseInvalid => PreReleaseInvalid,
            SemverErrorCode.BuildMetadataInvalid => BuildMetadataInvalid,

            SemverErrorCode.Leftovers => Leftovers,

            _ => throw new ArgumentException($"{code} error code is not supposed to have a message."),
        };

        [Pure, MustUseReturnValue, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TReturn ReturnOrThrow<TReturn>(this SemverErrorCode code, TReturn? returnValue, [InvokerParameterName] string parameterName)
        {
            if (code is not SemverErrorCode.Success)
                throw new ArgumentException(code.GetMessage(), parameterName);
            return returnValue!;
        }

    }
}

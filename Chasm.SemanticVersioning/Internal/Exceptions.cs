namespace Chasm.SemanticVersioning
{
    internal static class Exceptions
    {
        public const string MajorLeadingZeroes = "The major version component cannot contain leading zeroes.";
        public const string MinorLeadingZeroes = "The minor version component cannot contain leading zeroes.";
        public const string PatchLeadingZeroes = "The patch version component cannot contain leading zeroes.";
        public const string PreReleaseLeadingZeroes = "The numeric pre-release identifier cannot contain leading zeroes.";

        public const string MajorNegative = "The major version component cannot be less than 0.";
        public const string MinorNegative = "The minor version component cannot be less than 0.";
        public const string PatchNegative = "The patch version component cannot be less than 0.";
        public const string PreReleaseNegative = "The numeric pre-release identifier cannot be less than 0.";

        public const string MajorTooBig = "The major version component cannot be greater than 2147483647.";
        public const string MinorTooBig = "The minor version component cannot be greater than 2147483647.";
        public const string PatchTooBig = "The patch version component cannot be greater than 2147483647.";
        public const string PreReleaseTooBig = "The numeric pre-release identifier cannot be greater than 2147483647.";

        public const string PreReleaseEmpty = "The pre-release identifier cannot be empty.";
        public const string BuildMetadataEmpty = "The build metadata identifier cannot be empty.";
        public const string BuildMetadataNull = "The build metadata identifiers cannot be null.";

        public const string PreReleaseInvalid = "The pre-release identifier must only contain [A-Za-z0-9-] characters.";
        public const string BuildMetadataInvalid = "The build metadata identifier must only contain [A-Za-z0-9-] characters.";

    }
}

using System;

namespace Chasm.SemanticVersioning
{
    /// <summary>
    ///   <para>Defines semantic version parsing options.</para>
    /// </summary>
    [Flags]
    public enum SemverOptions
    {
        /// <summary>
        ///   <para>The strict semantic version parsing mode, as per the SemVer 2.0.0 specification.</para>
        /// </summary>
        Strict = 0,

        /// <summary>
        ///   <para>Allows leading zeroes in version components and numeric pre-release identifiers. Example: <c>1.02.5-alpha.007</c>.</para>
        /// </summary>
        AllowLeadingZeroes          = 1 << 0,
        /// <summary>
        ///   <para>Allows <c>'='</c> as a prefix to the semantic version. Example: <c>=3.0.0</c>.</para>
        /// </summary>
        AllowEqualsPrefix           = 1 << 1,
        /// <summary>
        ///   <para>Allows <c>'v'</c> or <c>'V'</c> as a prefix to the semantic version. Example: <c>v2.7.5</c>.</para>
        /// </summary>
        AllowVersionPrefix          = 1 << 2,
        /// <summary>
        ///   <para>Allows leading whitespace characters in the string to parse.</para>
        /// </summary>
        AllowLeadingWhite           = 1 << 3,
        /// <summary>
        ///   <para>Allows trailing whitespace characters in the string to parse.</para>
        /// </summary>
        AllowTrailingWhite          = 1 << 4,
        /// <summary>
        ///   <para>Allows whitespace characters between (but not inside) version components and identifiers. Example: <c>= 1 .4. 6- alpha</c>.</para>
        /// </summary>
        AllowInnerWhite             = 1 << 5,
        /// <summary>
        ///   <para>Allows omitting both the minor and patch version components (but not <b>just</b> the patch version component). Example: <c>1-beta.4</c> will be parsed, but not <c>2.3-alpha.2</c>.</para>
        /// </summary>
        OptionalMinor               = 1 << 6,
        /// <summary>
        ///   <para>Allows omitting the patch version component. Example: <c>1.2-nightly.456</c>.</para>
        /// </summary>
        OptionalPatch               = 1 << 7,
        /// <summary>
        ///   <para>Specifies that there could be unrelated to semantic versioning characters at some point in the string. Parsing is stopped at first such character, and the result is determined by the contents of the string preceding it.</para>
        /// </summary>
        AllowLeftovers              = 1 << 8,
        /// <summary>
        ///   <para>Allows specifying pre-release identifiers in an old-style fashion. For example, <c>4.0.0alpha2beta7</c> is resolved as having <c>alpha</c>, <c>2</c>, <c>beta</c> and <c>7</c> as pre-release identifiers.</para>
        /// </summary>
        OptionalPreReleaseSeparator = 1 << 9,

        /// <summary>
        ///   <para>Specifies all parsing options, to be able to parse normally invalid semantic versions.</para>
        /// </summary>
        Loose = ~0,
    }
}

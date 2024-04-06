using System;

namespace Chasm.SemanticVersioning
{
    /// <summary>
    ///   <para>Defines semantic version comparison rules to be used by <see cref="SemverComparer.FromComparison"/>.</para>
    /// </summary>
    [Flags]
    public enum SemverComparison : byte
    {
        /// <summary>
        ///   <para>Default SemVer 2.0.0 comparison, that ignores the build metadata, and, when comparing partial versions, considers wildcard characters and omitted components equal.</para>
        /// </summary>
        Default = 0,

        /// <summary>
        ///   <para>Compare semantic versions with build metadata included in the comparison.</para>
        /// </summary>
        IncludeBuild  = 1 << 0,
        /// <summary>
        ///   <para>Compare partial versions, differentiating between different wildcard characters and omitted components.</para>
        /// </summary>
        DiffWildcards = 1 << 1,
        /// <summary>
        ///   <para>Compare primitive and X-Range comparators, differentiating between implicit and explicit equality operators.</para>
        /// </summary>
        DiffEquality  = 1 << 2,

        /// <summary>
        ///   <para>Compare semantic versions, and <c>node-semver</c> objects exactly: including build metadata, and differentiating wildcards and equality operators.</para>
        /// </summary>
        Exact = IncludeBuild | DiffWildcards | DiffEquality,

    }
}

﻿namespace Chasm.SemanticVersioning
{
    /// <summary>
    ///   <para>Defines semantic version increment types.</para>
    /// </summary>
    public enum IncrementType : byte
    {
        /// <summary>
        ///   <para>Specifies no changes.</para>
        /// </summary>
        None,

        /// <summary>
        ///   <para>Specifies incrementing to the next major version.</para>
        /// </summary>
        Major,
        /// <summary>
        ///   <para>Specifies incrementing to the next minor version.</para>
        /// </summary>
        Minor,
        /// <summary>
        ///   <para>Specifies incrementing to the next patch version.</para>
        /// </summary>
        Patch,

        /// <summary>
        ///   <para>Specifies incrementing to a pre-release of the next major version.</para>
        /// </summary>
        PreMajor,
        /// <summary>
        ///   <para>Specifies incrementing to a pre-release of the next minor version.</para>
        /// </summary>
        PreMinor,
        /// <summary>
        ///   <para>Specifies incrementing to a pre-release of the next patch version.</para>
        /// </summary>
        PrePatch,

        /// <summary>
        ///   <para>Specifies incrementing to the next pre-release.</para>
        /// </summary>
        PreRelease,

        // TODO (v3): It would have been nice if PreRelease was put right after Major, Minor and Patch.
        // Then it'd be possible to combine values: PreRelease (0b100) | Minor (0b010) = PreMinor (0b110)
    }
}

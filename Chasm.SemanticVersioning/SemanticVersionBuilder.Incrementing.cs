﻿using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Chasm.SemanticVersioning
{
    public sealed partial class SemanticVersionBuilder
    {
        /// <summary>
        ///   <para>Bumps the semantic version to the next major version.</para>
        /// </summary>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="InvalidOperationException"><see cref="Major"/> is equal to <see cref="int.MaxValue"/>.</exception>
        public SemanticVersionBuilder IncrementMajor()
        {
            // 1.2.3   → 2.0.0
            // 1.0.0-0 → 1.0.0 | pre-release of a major release
            if (_minor != 0 || _patch != 0 || _preReleases.Count == 0)
            {
                if (_major == int.MaxValue) throw new InvalidOperationException(Exceptions.MajorTooBig);
                _major++;
            }
            _minor = 0;
            _patch = 0;
            _preReleases.Clear();
            return this;
        }
        /// <summary>
        ///   <para>Bumps the semantic version to the next minor version.</para>
        /// </summary>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="InvalidOperationException"><see cref="Minor"/> is equal to <see cref="int.MaxValue"/>.</exception>
        public SemanticVersionBuilder IncrementMinor()
        {
            // 1.2.3   → 1.3.0
            // 1.2.0-0 → 1.2.0 | pre-release of a minor release
            if (_patch != 0 || _preReleases.Count == 0)
            {
                if (_minor == int.MaxValue) throw new InvalidOperationException(Exceptions.MinorTooBig);
                _minor++;
            }
            _patch = 0;
            _preReleases.Clear();
            return this;
        }
        /// <summary>
        ///   <para>Bumps the semantic version to the next patch version.</para>
        /// </summary>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="InvalidOperationException"><see cref="Patch"/> is equal to <see cref="int.MaxValue"/>.</exception>
        public SemanticVersionBuilder IncrementPatch()
        {
            // 1.2.3   → 1.2.4
            // 1.2.3-0 → 1.2.3 | pre-release of a patch release
            if (_preReleases.Count == 0)
            {
                if (_patch == int.MaxValue) throw new InvalidOperationException(Exceptions.PatchTooBig);
                _patch++;
            }
            _preReleases.Clear();
            return this;
        }

        private SemanticVersionBuilder SetPreRelease(SemverPreRelease preRelease)
        {
            List<SemverPreRelease> preReleases = _preReleases;
            preReleases.Clear();
            preReleases.Add(preRelease);
            if (preRelease != SemverPreRelease.Zero)
                preReleases.Add(SemverPreRelease.Zero);
            return this;
        }

        /// <summary>
        ///   <para>Bumps the semantic version to the first pre-release of the next major version.</para>
        /// </summary>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="InvalidOperationException"><see cref="Major"/> is equal to <see cref="int.MaxValue"/>.</exception>
        public SemanticVersionBuilder IncrementPreMajor()
            => IncrementPreMajor(SemverPreRelease.Zero);
        /// <summary>
        ///   <para>Bumps the semantic version to the first specified <paramref name="preRelease"/> of the next major version.</para>
        /// </summary>
        /// <param name="preRelease">The pre-release identifier of the next major version to bump to.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="InvalidOperationException"><see cref="Major"/> is equal to <see cref="int.MaxValue"/>.</exception>
        public SemanticVersionBuilder IncrementPreMajor(SemverPreRelease preRelease)
        {
            if (_major == int.MaxValue) throw new InvalidOperationException(Exceptions.MajorTooBig);
            _major++;
            _minor = 0;
            _patch = 0;
            // 1.2.3 →   (0)   → 2.0.0-0       | 0 specifies not to use an extra identifier
            // 1.2.3 →   (1)   → 2.0.0-1.0
            // 1.2.3 → (alpha) → 2.0.0-alpha.0
            return SetPreRelease(preRelease);
        }
        /// <summary>
        ///   <para>Bumps the semantic version to the first pre-release of the next minor version.</para>
        /// </summary>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="InvalidOperationException"><see cref="Minor"/> is equal to <see cref="int.MaxValue"/>.</exception>
        public SemanticVersionBuilder IncrementPreMinor()
            => IncrementPreMinor(SemverPreRelease.Zero);
        /// <summary>
        ///   <para>Bumps the semantic version to the first specified <paramref name="preRelease"/> of the next minor version.</para>
        /// </summary>
        /// <param name="preRelease">The pre-release identifier of the next minor version to bump to.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="InvalidOperationException"><see cref="Minor"/> is equal to <see cref="int.MaxValue"/>.</exception>
        public SemanticVersionBuilder IncrementPreMinor(SemverPreRelease preRelease)
        {
            if (_minor == int.MaxValue) throw new InvalidOperationException(Exceptions.MinorTooBig);
            _minor++;
            _patch = 0;
            // 1.2.3 →   (0)   → 1.3.0-0       | 0 specifies not to use an extra identifier
            // 1.2.3 →   (1)   → 1.3.0-1.0
            // 1.2.3 → (alpha) → 1.3.0-alpha.0
            return SetPreRelease(preRelease);
        }
        /// <summary>
        ///   <para>Bumps the semantic version to the first pre-release of the next patch version.</para>
        /// </summary>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="InvalidOperationException"><see cref="Patch"/> is equal to <see cref="int.MaxValue"/>.</exception>
        public SemanticVersionBuilder IncrementPrePatch()
            => IncrementPrePatch(SemverPreRelease.Zero);
        /// <summary>
        ///   <para>Bumps the semantic version to the first specified <paramref name="preRelease"/> of the next patch version.</para>
        /// </summary>
        /// <param name="preRelease">The pre-release identifier of the next patch version to bump to.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="InvalidOperationException"><see cref="Patch"/> is equal to <see cref="int.MaxValue"/>.</exception>
        public SemanticVersionBuilder IncrementPrePatch(SemverPreRelease preRelease)
        {
            if (_patch == int.MaxValue) throw new InvalidOperationException(Exceptions.PatchTooBig);
            _patch++;
            // 1.2.3 →   (0)   → 1.2.4-0       | 0 specifies not to use an extra identifier
            // 1.2.3 →   (1)   → 1.2.4-1.0
            // 1.2.3 → (alpha) → 1.2.4-alpha.0
            return SetPreRelease(preRelease);
        }

        /// <summary>
        ///   <para>Bumps the semantic version to the next pre-release.</para>
        /// </summary>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="InvalidOperationException">The right-most numeric pre-release identifier is equal to <see cref="int.MaxValue"/>.</exception>
        public SemanticVersionBuilder IncrementPreRelease()
            => IncrementPreRelease(SemverPreRelease.Zero);
        /// <summary>
        ///   <para>Bumps the semantic version to the next specified <paramref name="preRelease"/> of the semantic version.</para>
        /// </summary>
        /// <param name="preRelease">The pre-release identifier of the next pre-release to bump to.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="InvalidOperationException">The right-most numeric pre-release identifier (or <see cref="Patch"/>, if there are no pre-releases) is equal to <see cref="int.MaxValue"/>.</exception>
        public SemanticVersionBuilder IncrementPreRelease(SemverPreRelease preRelease)
        {
#if NET5_0_OR_GREATER
            // optimize access to the list, if possible
            Span<SemverPreRelease> preReleases = System.Runtime.InteropServices.CollectionsMarshal.AsSpan(_preReleases);
            int length = preReleases.Length;
#else
            List<SemverPreRelease> preReleases = _preReleases;
            int length = preReleases.Count;
#endif

            if (length == 0)
            {
                // increment patch and add 'pre.0' or '0'
                return IncrementPrePatch(preRelease);
            }
            if (preRelease == SemverPreRelease.Zero || length > 1 && preReleases[0] == preRelease && preReleases[1].IsNumeric)
            {
                // try to increment the right-most numeric identifier
                int i = length;
                while (--i >= 0)
                {
                    SemverPreRelease identifier = preReleases[i];
                    if (identifier.IsNumeric)
                    {
                        int number = identifier.AsNumber;
                        if (number == int.MaxValue) throw new InvalidOperationException(Exceptions.PreReleaseTooBig);
                        preReleases[i] = new SemverPreRelease(number + 1);
                        return this;
                    }
                }
                // couldn't find a numeric identifier
                _preReleases.Add(SemverPreRelease.Zero);
                return this;
            }
            // replace the pre-releases with 'pre.0'
            return SetPreRelease(preRelease);
        }

        /// <summary>
        ///   <para>Increments the semantic version using the specified increment <paramref name="type"/>.</para>
        /// </summary>
        /// <param name="type">The increment type to use.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="InvalidEnumArgumentException"><paramref name="type"/> is not a valid increment type.</exception>
        /// <exception cref="InvalidOperationException"><see cref="Major"/>, <see cref="Minor"/>, <see cref="Patch"/> or the right-most numeric pre-release identifier is equal to <see cref="int.MaxValue"/>.</exception>
        public SemanticVersionBuilder Increment(IncrementType type)
            => Increment(type, SemverPreRelease.Zero);
        /// <summary>
        ///   <para>Increments the semantic version using the specified increment <paramref name="type"/> with the specified <paramref name="preRelease"/> identifier.</para>
        /// </summary>
        /// <param name="type">The increment type to use.</param>
        /// <param name="preRelease">The pre-release identifier to increment with.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="InvalidEnumArgumentException"><paramref name="type"/> is not a valid increment type.</exception>
        /// <exception cref="InvalidOperationException"><see cref="Major"/>, <see cref="Minor"/>, <see cref="Patch"/> or the right-most numeric pre-release identifier is equal to <see cref="int.MaxValue"/>.</exception>
        public SemanticVersionBuilder Increment(IncrementType type, SemverPreRelease preRelease) => type switch
        {
            IncrementType.None => this,

            IncrementType.Major => IncrementMajor(),
            IncrementType.Minor => IncrementMinor(),
            IncrementType.Patch => IncrementPatch(),

            IncrementType.PreMajor => IncrementPreMajor(preRelease),
            IncrementType.PreMinor => IncrementPreMinor(preRelease),
            IncrementType.PrePatch => IncrementPrePatch(preRelease),

            IncrementType.PreRelease => IncrementPreRelease(preRelease),

            _ => throw new InvalidEnumArgumentException(nameof(type), (int)type, typeof(IncrementType)),
        };

    }
}

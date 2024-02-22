using Chasm.Formatting;
using JetBrains.Annotations;

namespace Chasm.SemanticVersioning.Ranges
{
    /// <summary>
    ///   <para>Represents a valid <c>node-semver</c> version comparator.</para>
    /// </summary>
    public abstract class Comparator : ISpanBuildable
    {
        /// <summary>
        ///   <para>Determines whether this version comparator is a <see cref="PrimitiveComparator"/>.</para>
        /// </summary>
        // Note: Apparently, type-checking is quicker than calling an overriden getter
        public bool IsPrimitive => this is PrimitiveComparator;

        /// <summary>
        ///   <para>Determines whether this version comparator can match a pre-release version with the specified <paramref name="major"/>, <paramref name="minor"/> and <paramref name="patch"/> version components.</para>
        /// </summary>
        /// <param name="major">The major version component of a pre-release version.</param>
        /// <param name="minor">The minor version component of a pre-release version.</param>
        /// <param name="patch">The patch version component of a pre-release version.</param>
        /// <returns><see langword="true"/>, if this version comparator can match a pre-release version with the specified <paramref name="major"/>, <paramref name="minor"/> and <paramref name="patch"/> version components; otherwise, <see langword="false"/>.</returns>
        [Pure] public abstract bool CanMatchPreRelease(int major, int minor, int patch);
        /// <summary>
        ///   <para>Determines whether the specified semantic <paramref name="version"/> satisfies this version comparator.</para>
        /// </summary>
        /// <param name="version">The semantic version to match.</param>
        /// <returns><see langword="true"/>, if the specified semantic <paramref name="version"/> satisfies this version comparator; otherwise, <see langword="false"/>.</returns>
        [Pure] public abstract bool IsSatisfiedBy(SemanticVersion? version);

        /// <summary>
        ///   <para>Determines whether the specified semantic <paramref name="version"/> is a pre-release version, and has the specified <paramref name="major"/>, <paramref name="minor"/> and <paramref name="patch"/> version components.</para>
        /// </summary>
        /// <param name="version">The semantic version to match.</param>
        /// <param name="major">The major version component of a pre-release version.</param>
        /// <param name="minor">The minor version component of a pre-release version.</param>
        /// <param name="patch">The patch version component of a pre-release version.</param>
        /// <returns><see langword="true"/>, if the specified semantic <paramref name="version"/> is a pre-release version, and has the specified <paramref name="major"/>, <paramref name="minor"/> and <paramref name="patch"/> version components; otherwise, <see langword="false"/>.</returns>
        [Pure] protected static bool CanMatchPreRelease(SemanticVersion version, int major, int minor, int patch)
            => version.IsPreRelease && version.Major == major && version.Minor == minor && version.Patch == patch;
        /// <summary>
        ///   <para>Determines whether the specified partial <paramref name="version"/> is a pre-release version, and matches the specified <paramref name="major"/>, <paramref name="minor"/> and <paramref name="patch"/> version components (either not numeric, or equal numerically).</para>
        /// </summary>
        /// <param name="version">The partial version to match.</param>
        /// <param name="major">The major version component of a pre-release version.</param>
        /// <param name="minor">The minor version component of a pre-release version.</param>
        /// <param name="patch">The patch version component of a pre-release version.</param>
        /// <returns><see langword="true"/>, if the specified partial <paramref name="version"/> is a pre-release version, and has the specified <paramref name="major"/>, <paramref name="minor"/> and <paramref name="patch"/> version components (either not numeric, or equal numerically); otherwise, <see langword="false"/>.</returns>
        [Pure] protected static bool CanMatchPreRelease(PartialVersion version, int major, int minor, int patch)
            => version.IsPreRelease
            && (!version.Major.IsNumeric || version.Major.GetValueOrZero() == major)
            && (!version.Minor.IsNumeric || version.Minor.GetValueOrZero() == minor)
            && (!version.Patch.IsNumeric || version.Patch.GetValueOrZero() == patch);

        /// <inheritdoc cref="ISpanBuildable.CalculateLength"/>
        [Pure] protected internal abstract int CalculateLength();
        /// <inheritdoc cref="ISpanBuildable.BuildString"/>
        protected internal abstract void BuildString(ref SpanBuilder sb);

        [Pure] int ISpanBuildable.CalculateLength() => CalculateLength();
        void ISpanBuildable.BuildString(ref SpanBuilder sb) => BuildString(ref sb);

        /// <summary>
        ///   <para>Returns the string representation of this version comparator.</para>
        /// </summary>
        /// <returns>The string representation of this version comparator.</returns>
        [Pure] public override string ToString()
            => SpanBuilder.Format(this);

    }
}

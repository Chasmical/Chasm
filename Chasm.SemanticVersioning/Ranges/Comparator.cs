using System;
using Chasm.Formatting;
using JetBrains.Annotations;

namespace Chasm.SemanticVersioning.Ranges
{
    /// <summary>
    ///   <para>Represents a valid <c>node-semver</c> version comparator.</para>
    /// </summary>
    public abstract class Comparator : ISpanBuildable, IEquatable<Comparator>
#if NET7_0_OR_GREATER
                                     , System.Numerics.IEqualityOperators<Comparator, Comparator, bool>
#endif
    {
        /// <summary>
        ///   <para>Determines whether this version comparator is a <see cref="PrimitiveComparator"/>.</para>
        /// </summary>
        // Note: Apparently, type-checking is quicker than calling an overridden getter
        public bool IsPrimitive => this is PrimitiveComparator;
        /// <summary>
        ///   <para>Determines whether this version comparator is an <see cref="AdvancedComparator"/>.</para>
        /// </summary>
        public bool IsAdvanced => !IsPrimitive;

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
        [Pure] protected static bool CanMatchPreRelease(SemanticVersion? version, int major, int minor, int patch)
            => version?.IsPreRelease == true && version.Major == major && version.Minor == minor && version.Patch == patch;

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

        [Pure] public abstract bool Equals(Comparator? comparator);
        [Pure] public abstract override bool Equals(object? obj);
        [Pure] public abstract override int GetHashCode();

        [Pure] public static bool operator ==(Comparator? left, Comparator? right)
            => left is null ? right is null : left.Equals(right);
        [Pure] public static bool operator !=(Comparator? left, Comparator? right)
            => !(left == right);

        // TODO: Add &, |, ~ operators

    }
}

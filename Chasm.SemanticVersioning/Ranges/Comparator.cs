using System;
using Chasm.Formatting;
using JetBrains.Annotations;

namespace Chasm.SemanticVersioning.Ranges
{
    /// <summary>
    ///   <para>Represents a valid <c>node-semver</c> version comparator.</para>
    /// </summary>
    public abstract partial class Comparator : ISpanBuildable, IEquatable<Comparator>
#if NET7_0_OR_GREATER
                                             , System.Numerics.IEqualityOperators<Comparator, Comparator, bool>
#endif
    {
        /// <summary>
        ///   <para>Determines whether this comparator is a <see cref="PrimitiveComparator"/>.</para>
        /// </summary>
        // Note: Apparently, type-checking is quicker than calling an overridden getter
        public bool IsPrimitive => this is PrimitiveComparator;
        /// <summary>
        ///   <para>Determines whether this comparator is an <see cref="AdvancedComparator"/>.</para>
        /// </summary>
        public bool IsAdvanced => !IsPrimitive;

        /// <summary>
        ///   <para>Determines whether this comparator can match a pre-release version with the specified <paramref name="major"/>, <paramref name="minor"/> and <paramref name="patch"/> version components.</para>
        /// </summary>
        /// <param name="major">The major version component of a pre-release version.</param>
        /// <param name="minor">The minor version component of a pre-release version.</param>
        /// <param name="patch">The patch version component of a pre-release version.</param>
        /// <returns><see langword="true"/>, if this comparator can match a pre-release version with the specified <paramref name="major"/>, <paramref name="minor"/> and <paramref name="patch"/> version components; otherwise, <see langword="false"/>.</returns>
        [Pure] public abstract bool CanMatchPreRelease(int major, int minor, int patch);
        /// <summary>
        ///   <para>Determines whether the specified semantic <paramref name="version"/> satisfies this comparator.</para>
        /// </summary>
        /// <param name="version">The semantic version to match.</param>
        /// <returns><see langword="true"/>, if the specified semantic <paramref name="version"/> satisfies this comparator; otherwise, <see langword="false"/>.</returns>
        [Pure] public bool IsSatisfiedBy(SemanticVersion? version)
            => IsSatisfiedBy(version, false);
        /// <summary>
        ///   <para>Determines whether the specified semantic <paramref name="version"/> satisfies this comparator.</para>
        /// </summary>
        /// <param name="version">The semantic version to match.</param>
        /// <param name="includePreReleases">Determines whether to treat pre-release versions like regular versions.</param>
        /// <returns><see langword="true"/>, if the specified semantic <paramref name="version"/> satisfies this comparator; otherwise, <see langword="false"/>.</returns>
        [Pure] public bool IsSatisfiedBy(SemanticVersion? version, bool includePreReleases)
        {
            if (version is not null)
            {
                if (includePreReleases || !version.IsPreRelease || CanMatchPreRelease(version.Major, version.Minor, version.Patch))
                    return IsSatisfiedByCore(version);
            }
            return false;
        }

        /// <summary>
        ///   <para>Determines whether this comparator matches the specified semantic <paramref name="version"/>. At this point, <paramref name="version"/> has been confirmed to not be <see langword="null"/>, and pre-release versions should be treated like regular versions.</para>
        /// </summary>
        /// <param name="version">The semantic version to match.</param>
        /// <returns><see langword="true"/>, if the specified semantic <paramref name="version"/> satisfies this comparator; otherwise, <see langword="false"/>.</returns>
        [Pure] protected internal abstract bool IsSatisfiedByCore(SemanticVersion version);

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

        // Internal helper method to arrange the primitives properly, for the use of some methods
        // TODO: maybe expose AsPrimitives() as public API somehow
        [Pure] internal (PrimitiveComparator?, PrimitiveComparator?) AsPrimitives()
        {
            if (this is PrimitiveComparator primitive)
                return primitive.Operator.IsLTOrLTE() ? (null, primitive) : (primitive, null);
            return ((AdvancedComparator)this).ToPrimitives();
        }

        /// <inheritdoc cref="ISpanBuildable.CalculateLength"/>
        [Pure] protected internal abstract int CalculateLength();
        /// <inheritdoc cref="ISpanBuildable.BuildString"/>
        protected internal abstract void BuildString(ref SpanBuilder sb);

        [Pure] int ISpanBuildable.CalculateLength() => CalculateLength();
        void ISpanBuildable.BuildString(ref SpanBuilder sb) => BuildString(ref sb);

        /// <summary>
        ///   <para>Returns the string representation of this comparator.</para>
        /// </summary>
        /// <returns>The string representation of this comparator.</returns>
        [Pure] public override string ToString()
            => SpanBuilder.Format(this);

        [Pure] bool IEquatable<Comparator>.Equals(Comparator? other)
            => Equals(other);
        [Pure] public abstract override bool Equals(object? obj);
        [Pure] public abstract override int GetHashCode();

        [Pure] public static bool operator ==(Comparator? left, Comparator? right)
            => left is null ? right is null : left.Equals(right);
        [Pure] public static bool operator !=(Comparator? left, Comparator? right)
            => !(left == right);

        // TODO: Implement >, <, >=, <= operators

    }
}

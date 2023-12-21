using System;
using JetBrains.Annotations;

namespace Chasm.SemanticVersioning
{
    /// <summary>
    ///   <para>Represents a valid semantic version pre-release identifier, compliant to the SemVer 2.0.0 specification.</para>
    /// </summary>
    public readonly partial struct SemverPreRelease : IEquatable<SemverPreRelease>, IComparable, IComparable<SemverPreRelease>
#if NET7_0_OR_GREATER
                                                    , System.Numerics.IComparisonOperators<SemverPreRelease, SemverPreRelease, bool>
#endif
    {
        private readonly string? text;
        private readonly int number;

        /// <summary>
        ///   <para>Initializes a new instance of the <see cref="SemverPreRelease"/> structure with the specified numeric <paramref name="identifier"/>.</para>
        /// </summary>
        /// <param name="identifier">The numeric pre-release identifier.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="identifier"/> is less than 0.</exception>
        public SemverPreRelease(int identifier)
        {
            if (identifier < 0) throw new ArgumentOutOfRangeException(nameof(identifier), identifier, Exceptions.PreReleaseNegative);
            number = identifier;
        }
        /// <summary>
        ///   <para>Initializes a new instance of the <see cref="SemverPreRelease"/> structure with the specified <paramref name="identifier"/>.</para>
        /// </summary>
        /// <param name="identifier">The string containing a semantic version pre-release identifier.</param>
        /// <exception cref="ArgumentException"><paramref name="identifier"/> is not a valid semantic version pre-release identifier.</exception>
        public SemverPreRelease(string identifier)
            => this = Parse(identifier);
        /// <summary>
        ///   <para>Initializes a new instance of the <see cref="SemverPreRelease"/> structure with the specified <paramref name="identifier"/>.</para>
        /// </summary>
        /// <param name="identifier">The string containing a semantic version pre-release identifier.</param>
        /// <exception cref="ArgumentException"><paramref name="identifier"/> is not a valid semantic version pre-release identifier.</exception>
        public SemverPreRelease(ReadOnlySpan<char> identifier)
            => this = Parse(identifier);

        /// <summary>
        ///   <para>Defines an implicit conversion of a 32-bit signed integer to a numeric pre-release identifier.</para>
        /// </summary>
        /// <param name="identifier">The 32-bit signed integer to convert.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="identifier"/> is less than 0.</exception>
        [Pure] public static implicit operator SemverPreRelease(int identifier) => new SemverPreRelease(identifier);
        /// <summary>
        ///   <para>Defines an implicit conversion of a string to an alphanumeric pre-release identifier.</para>
        /// </summary>
        /// <param name="identifier">The string to convert.</param>
        /// <exception cref="ArgumentNullException"><paramref name="identifier"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="identifier"/> is not a valid semantic version pre-release identifier.</exception>
        [Pure] public static implicit operator SemverPreRelease(string identifier) => new SemverPreRelease(identifier);
        /// <summary>
        ///   <para>Defines an implicit conversion of a read-only span of characters to an alphanumeric pre-release identifier.</para>
        /// </summary>
        /// <param name="identifier">The read-only span of characters to convert.</param>
        /// <exception cref="ArgumentException"><paramref name="identifier"/> is not a valid semantic version pre-release identifier.</exception>
        [Pure] public static implicit operator SemverPreRelease(ReadOnlySpan<char> identifier) => new SemverPreRelease(identifier);
        /// <summary>
        ///   <para>Defines an explicit conversion of a numeric pre-release identifier to a 32-bit signed integer.</para>
        /// </summary>
        /// <param name="preRelease">The numeric pre-release identifier to convert.</param>
        /// <exception cref="InvalidOperationException"><paramref name="preRelease"/> is not numeric.</exception>
        [Pure] public static explicit operator int(SemverPreRelease preRelease) => preRelease.AsNumber;
        /// <summary>
        ///   <para>Defines an explicit conversion of a pre-release identifier to a string.</para>
        /// </summary>
        /// <param name="preRelease">The pre-release identifier to convert.</param>
        [Pure] public static explicit operator string(SemverPreRelease preRelease) => preRelease.ToString();
        /// <summary>
        ///   <para>Defines an explicit conversion of a pre-release identifier to a read-only span of characters.</para>
        /// </summary>
        /// <param name="preRelease">The pre-release identifier to convert.</param>
        [Pure] public static explicit operator ReadOnlySpan<char>(SemverPreRelease preRelease) => preRelease.ToString();

        /// <summary>
        ///   <para>Determines whether the pre-release identifier is numeric.</para>
        /// </summary>
        public bool IsNumeric => text is null;
        /// <summary>
        ///   <para>Gets the numeric value of the pre-release identifier, if it's numeric; otherwise, throws an exception.</para>
        /// </summary>
        /// <exception cref="InvalidOperationException">This pre-release identifier is not numeric.</exception>
        public int AsNumber => text is null ? number : throw new InvalidOperationException(Exceptions.PreReleaseNotNumeric);

        /// <summary>
        ///   <para>Gets the numeric pre-release identifier with the value of 0.</para>
        /// </summary>
        public static SemverPreRelease Zero => default;
        internal static readonly SemverPreRelease[] ZeroArray = { default };

        /// <summary>
        ///   <para>Determines whether this pre-release identifier is equal to another specified pre-release identifier.</para>
        /// </summary>
        /// <param name="other">The pre-release identifier to compare with this pre-release identifier.</param>
        /// <returns><see langword="true"/>, if this pre-release identifier is equal to <paramref name="other"/>; otherwise, <see langword="false"/>.</returns>
        [Pure] public bool Equals(SemverPreRelease other)
            => text is null ? other.text is null && number == other.number : text.Equals(other.text);
        /// <summary>
        ///   <para>Determines whether this pre-release identifier is equal to the specified <paramref name="obj"/>.</para>
        /// </summary>
        /// <param name="obj">The object to compare with this pre-release identifier.</param>
        /// <returns><see langword="true"/>, if <paramref name="obj"/> is a <see cref="SemverPreRelease"/> instance equal to this pre-release identifier; otherwise, <see langword="false"/>.</returns>
        [Pure] public override bool Equals(object? obj)
            => obj is SemverPreRelease other && Equals(other);
        /// <summary>
        ///   <para>Returns a hash code for this pre-release identifier.</para>
        /// </summary>
        /// <returns>A hash code for this pre-release identifier.</returns>
        [Pure] public override int GetHashCode()
            => text?.GetHashCode() ?? number;

        /// <summary>
        ///   <para>Compares this pre-release identifier with another specified pre-release identifier and returns an integer that indicates whether this pre-release identifier precedes, follows or occurs in the same position in the sort order as the <paramref name="other"/> pre-release identifier.</para>
        /// </summary>
        /// <param name="other">The pre-release identifier to compare with this pre-release identifier.</param>
        /// <returns>&lt;0, if this pre-release identifier precedes <paramref name="other"/> in the sort order;<br/>=0, if this pre-release identifier occurs in the same position in the sort order as <paramref name="other"/>;<br/>&gt;0, if this pre-release identifier follows <paramref name="other"/> in the sort order.</returns>
        [Pure] public int CompareTo(SemverPreRelease other)
        {
            bool isNumeric = text is null;
            if (isNumeric != other.text is null) return isNumeric ? -1 : 1;
            return isNumeric ? number - other.number : string.CompareOrdinal(text, other.text);
        }
        [Pure] int IComparable.CompareTo(object? obj)
        {
            if (obj is null) return 1;
            if (obj is SemverPreRelease other) return CompareTo(other);
            throw new ArgumentException($"Object must be of type {nameof(SemverPreRelease)}.", nameof(obj));
        }

        /// <summary>
        ///   <para>Determines whether two specified pre-release identifiers are equal.</para>
        /// </summary>
        /// <param name="left">The first pre-release identifier to compare.</param>
        /// <param name="right">The second pre-release identifier to compare.</param>
        /// <returns><see langword="true"/>, if <paramref name="left"/> is equal to <paramref name="right"/>; otherwise, <see langword="false"/>.</returns>
        [Pure] public static bool operator ==(SemverPreRelease left, SemverPreRelease right)
            => left.Equals(right);
        /// <summary>
        ///   <para>Determines whether two specified pre-release identifiers are not equal.</para>
        /// </summary>
        /// <param name="left">The first pre-release identifier to compare.</param>
        /// <param name="right">The second pre-release identifier to compare.</param>
        /// <returns><see langword="true"/>, if <paramref name="left"/> is not equal to <paramref name="right"/>; otherwise, <see langword="false"/>.</returns>
        [Pure] public static bool operator !=(SemverPreRelease left, SemverPreRelease right)
            => !left.Equals(right);

        /// <summary>
        ///   <para>Determines whether a specified pre-release identifier is greater than another specified pre-release identifier.</para>
        /// </summary>
        /// <param name="left">The first pre-release identifier to compare.</param>
        /// <param name="right">The second pre-release identifier to compare.</param>
        /// <returns><see langword="true"/>, if <paramref name="left"/> is greater than <paramref name="right"/>; otherwise, <see langword="false"/>.</returns>
        [Pure] public static bool operator >(SemverPreRelease left, SemverPreRelease right)
            => left.CompareTo(right) > 0;
        /// <summary>
        ///   <para>Determines whether a specified pre-release identifier is less than another specified pre-release identifier.</para>
        /// </summary>
        /// <param name="left">The first pre-release identifier to compare.</param>
        /// <param name="right">The second pre-release identifier to compare.</param>
        /// <returns><see langword="true"/>, if <paramref name="left"/> is less than <paramref name="right"/>; otherwise, <see langword="false"/>.</returns>
        [Pure] public static bool operator <(SemverPreRelease left, SemverPreRelease right)
            => left.CompareTo(right) < 0;
        /// <summary>
        ///   <para>Determines whether a specified pre-release identifier is greater than or equal to another specified pre-release identifier.</para>
        /// </summary>
        /// <param name="left">The first pre-release identifier to compare.</param>
        /// <param name="right">The second pre-release identifier to compare.</param>
        /// <returns><see langword="true"/>, if <paramref name="left"/> is greater than or equal to <paramref name="right"/>; otherwise, <see langword="false"/>.</returns>
        [Pure] public static bool operator >=(SemverPreRelease left, SemverPreRelease right)
            => left.CompareTo(right) >= 0;
        /// <summary>
        ///   <para>Determines whether a specified pre-release identifier is less than or equal to another specified pre-release identifier.</para>
        /// </summary>
        /// <param name="left">The first pre-release identifier to compare.</param>
        /// <param name="right">The second pre-release identifier to compare.</param>
        /// <returns><see langword="true"/>, if <paramref name="left"/> is less than or equal to <paramref name="right"/>; otherwise, <see langword="false"/>.</returns>
        [Pure] public static bool operator <=(SemverPreRelease left, SemverPreRelease right)
            => left.CompareTo(right) <= 0;

    }
}

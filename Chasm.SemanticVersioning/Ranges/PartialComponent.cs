using System;
using JetBrains.Annotations;

namespace Chasm.SemanticVersioning.Ranges
{
    public readonly partial struct PartialComponent : IEquatable<PartialComponent>, IComparable, IComparable<PartialComponent>
#if NET7_0_OR_GREATER
                                                    , System.Numerics.IEqualityOperators<PartialComponent, PartialComponent, bool>
#endif
    {
        internal readonly int _value;

        /// <summary>
        ///   <para>Initializes a new instance of the <see cref="PartialComponent"/> structure with the specified numeric <paramref name="value"/>.</para>
        /// </summary>
        /// <param name="value">The partial version component's numeric value.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="value"/> is less than 0.</exception>
        public PartialComponent(int value)
        {
            if (value < 0) throw new ArgumentOutOfRangeException(nameof(value), value, Exceptions.ComponentNegative);
            _value = value;
        }
        /// <summary>
        ///   <para>Initializes a new instance of the <see cref="PartialComponent"/> structure with the specified wildcard or numeric <paramref name="character"/>.</para>
        /// </summary>
        /// <param name="character">The wildcard or numeric character representing a partial version component.</param>
        /// <exception cref="ArgumentException"><paramref name="character"/> does not represent a valid partial version component.</exception>
        public PartialComponent(char character)
            => this = Parse(character);

        /// <summary>
        ///   <para>Defines an implicit conversion of a 21-bit signed integer to a numeric partial version component.</para>
        /// </summary>
        /// <param name="value">The partial version component's numeric value.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="value"/> is less than 0.</exception>
        [Pure] public static implicit operator PartialComponent(int value) => new PartialComponent(value);
        /// <summary>
        ///   <para>Defines an implicit conversion of a wildcard or numeric <paramref name="character"/> to a partial version component.</para>
        /// </summary>
        /// <param name="character">The wildcard or numeric character representing a partial version component.</param>
        /// <exception cref="ArgumentException"><paramref name="character"/> does not represent a valid partial version component.</exception>
        [Pure] public static implicit operator PartialComponent(char character) => Parse(character);
        /// <summary>
        ///   <para>Defines an explicit conversion of a partial version component to a 32-bit signed integer.</para>
        /// </summary>
        /// <param name="component">The partial version component to convert.</param>
        [Pure] public static explicit operator int(PartialComponent component) => component.GetValueOrZero();

        /// <summary>
        ///   <para>Determines whether the partial version component is numeric.</para>
        /// </summary>
        public bool IsNumeric => _value > -1;
        /// <summary>
        ///   <para>Determines whether the partial version component is omitted.</para>
        /// </summary>
        public bool IsOmitted => _value == -1;
        /// <summary>
        ///   <para>Determines whether the partial version component is a wildcard.</para>
        /// </summary>
        public bool IsWildcard => _value < -1;

        /// <summary>
        ///   <para>Gets the partial version component's numeric value, if it's numeric; otherwise, throws an exception.</para>
        /// </summary>
        /// <exception cref="InvalidOperationException">The partial version component is not numeric.</exception>
        public int AsNumber => _value >= 0 ? _value : throw new InvalidOperationException(Exceptions.ComponentNotNumeric);

        /// <summary>
        ///   <para>Returns the partial version component's numeric value, if it's numeric; otherwise, returns <c>0</c>.</para>
        /// </summary>
        /// <returns>The partial version component's numeric value, if it's numeric; otherwise, <c>0</c>.</returns>
        [Pure] public int GetValueOrZero()
            => Math.Max(_value, 0);
        // TODO: Should GetValueOrMinusOne be public?
        [Pure] internal int GetValueOrMinusOne()
            => Math.Max(_value, -1);

        /// <summary>
        ///   <para>Gets the partial version component with the value of <c>0</c>.</para>
        /// </summary>
        public static PartialComponent Zero => default;
        /// <summary>
        ///   <para>Gets the lowercase X (<c>x</c>) wildcard partial version component.</para>
        /// </summary>
        public static PartialComponent LowerX { get; } = new PartialComponent(-'x', default);
        /// <summary>
        ///   <para>Gets the uppercase X (<c>X</c>) wildcard partial version component.</para>
        /// </summary>
        public static PartialComponent UpperX { get; } = new PartialComponent(-'X', default);
        /// <summary>
        ///   <para>Gets the star/asterisk (<c>*</c>) wildcard partial version component.</para>
        /// </summary>
        public static PartialComponent Star { get; } = new PartialComponent(-'*', default);
        /// <summary>
        ///   <para>Gets the omitted/unspecified partial version component, which will not be displayed or output.</para>
        /// </summary>
        public static PartialComponent Omitted { get; } = new PartialComponent(-1, default);

        /// <summary>
        ///   <para>Determines whether this partial version component is equal to another specified partial version component.<br/>Non-numeric version components are considered equal in this comparison. For character-sensitive comparison, use <see cref="WildcardComponentComparer"/>.</para>
        /// </summary>
        /// <param name="other">The partial version component to compare with this partial version component.</param>
        /// <returns><see langword="true"/>, if this partial version component is equal to <paramref name="other"/>; otherwise, <see langword="false"/>.</returns>
        [Pure] public bool Equals(PartialComponent other)
            => _value < 0 ? other._value < 0 : _value == other._value;
        /// <summary>
        ///   <para>Determines whether this partial version component is equal to the specified <paramref name="obj"/>.<br/>Non-numeric version components are considered equal in this comparison. For character-sensitive comparison, use <see cref="WildcardComponentComparer"/>.</para>
        /// </summary>
        /// <param name="obj">The object to compare with this partial version component.</param>
        /// <returns><see langword="true"/>, if <paramref name="obj"/> is a <see cref="PartialComponent"/> instance equal to this partial version component; otherwise, <see langword="false"/>.</returns>
        [Pure] public override bool Equals(object? obj)
            => obj is PartialComponent other && Equals(other);
        /// <summary>
        ///   <para>Returns a hash code for this partial version component.<br/>Non-numeric version components are considered equal in this comparison. For character-sensitive comparison, use <see cref="WildcardComponentComparer"/>.</para>
        /// </summary>
        /// <returns>A hash code for this partial version component.</returns>
        [Pure] public override int GetHashCode()
            => Math.Max(_value, -1);

        /// <summary>
        ///   <para>Compares this partial version component with another specified partial version component and returns an integer that indicates whether this partial version component precedes, follows or occurs in the same position in the sort order as the <paramref name="other"/> partial version component.<br/>Non-numeric version components are considered equal in this comparison. For character-sensitive comparison, use <see cref="WildcardComponentComparer"/>.</para>
        /// </summary>
        /// <param name="other">The partial version component to compare with this partial version component.</param>
        /// <returns>&lt;0, if this partial version component precedes <paramref name="other"/> in the sort order;<br/>=0, if this partial version component occurs in the same position in the sort order as <paramref name="other"/>;<br/>&gt;0, if this partial version component follows <paramref name="other"/> in the sort order.</returns>
        [Pure] public int CompareTo(PartialComponent other)
        {
            if (_value < 0) return other._value < 0 ? 0 : -1;
            return other._value < 0 ? 1 : _value - other._value;
        }
        [Pure] int IComparable.CompareTo(object? obj)
        {
            if (obj is null) return 1;
            if (obj is PartialComponent other) return CompareTo(other);
            throw new ArgumentException($"Object must be of type {nameof(PartialComponent)}.", nameof(obj));
        }

        /// <summary>
        ///   <para>Determines whether two specified partial version components are equal.<br/>Non-numeric version components are considered equal in this comparison. For character-sensitive comparison, use <see cref="WildcardComponentComparer"/>.</para>
        /// </summary>
        /// <param name="left">The first partial version component to compare.</param>
        /// <param name="right">The second partial version component to compare.</param>
        /// <returns><see langword="true"/>, if <paramref name="left"/> is equal to <paramref name="right"/>; otherwise, <see langword="false"/>.</returns>
        [Pure] public static bool operator ==(PartialComponent left, PartialComponent right) => left.Equals(right);
        /// <summary>
        ///   <para>Determines whether two specified partial version components are not equal.<br/>Non-numeric version components are considered equal in this comparison. For character-sensitive comparison, use <see cref="WildcardComponentComparer"/>.</para>
        /// </summary>
        /// <param name="left">The first partial version component to compare.</param>
        /// <param name="right">The second partial version component to compare.</param>
        /// <returns><see langword="true"/>, if <paramref name="left"/> is not equal to <paramref name="right"/>; otherwise, <see langword="false"/>.</returns>
        [Pure] public static bool operator !=(PartialComponent left, PartialComponent right) => !left.Equals(right);

    }
}

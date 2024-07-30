﻿#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
using System.Runtime.CompilerServices;
[assembly: TypeForwardedTo(typeof(System.Range))]
#else
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#if NETCOREAPP1_0_OR_GREATER || NETSTANDARD1_0_OR_GREATER || NET45_OR_GREATER
using System.Runtime.CompilerServices;
#endif

// ReSharper disable CommentTypo GrammarMistakeInComment
// ReSharper disable once CheckNamespace
namespace System
{
    /// <summary>Represent a range has start and end indexes.</summary>
    /// <remarks>
    /// Range is used by the C# compiler to support the range syntax.
    /// <code>
    /// int[] someArray = new int[5] { 1, 2, 3, 4, 5 };
    /// int[] subArray1 = someArray[0..2]; // { 1, 2 }
    /// int[] subArray2 = someArray[1..^0]; // { 2, 3, 4, 5 }
    /// </code>
    /// </remarks>
    public readonly struct Range : IEquatable<Range>
    {
        /// <summary>Represent the inclusive start index of the Range.</summary>
        public Index Start { get; }

        /// <summary>Represent the exclusive end index of the Range.</summary>
        public Index End { get; }

        /// <summary>Construct a Range object using the start and end indexes.</summary>
        /// <param name="start">Represent the inclusive start index of the range.</param>
        /// <param name="end">Represent the exclusive end index of the range.</param>
        public Range(Index start, Index end)
        {
            Start = start;
            End = end;
        }

        /// <summary>Indicates whether the current Range object is equal to another object of the same type.</summary>
        /// <param name="value">An object to compare with this object</param>
        public override bool Equals(
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            [System.Diagnostics.CodeAnalysis.NotNullWhen(true)]
#endif
            object? value
        ) => value is Range r && r.Start.Equals(Start) && r.End.Equals(End);

        /// <summary>Indicates whether the current Range object is equal to another Range object.</summary>
        /// <param name="other">An object to compare with this object</param>
        public bool Equals(Range other) => other.Start.Equals(Start) && other.End.Equals(End);

        /// <summary>Returns the hash code for this instance.</summary>
        public override int GetHashCode()
        {
            // This is the inlined contents of System.HashCode.Combine(,)

            uint hash = hashSeed + 374761393U + 8U;

            hash = RotateLeft_17(hash + (uint)Start.GetHashCode() * 3266489917U) * 668265263U;
            hash = RotateLeft_17(hash + (uint)End.GetHashCode() * 3266489917U) * 668265263U;

            hash ^= hash >> 15;
            hash *= 2246822519U;
            hash ^= hash >> 13;
            hash *= 3266489917U;
            hash ^= hash >> 16;

            return (int)hash;
        }

        private static readonly uint hashSeed = GenerateHashSeed();
        private static uint GenerateHashSeed()
        {
            byte[] buffer = new byte[sizeof(uint)];
#if NETCOREAPP1_0_OR_GREATER || NETSTANDARD1_3_OR_GREATER || NET11_OR_GREATER
            using (var rng = Security.Cryptography.RandomNumberGenerator.Create())
                rng.GetBytes(buffer);
#else
            new Random().NextBytes(buffer);
#endif
            return BitConverter.ToUInt32(buffer, 0);
        }

#if NETCOREAPP1_0_OR_GREATER || NETSTANDARD1_0_OR_GREATER || NET45_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private static uint RotateLeft_17(uint value)
            => (value << 17) | (value >> (32 - 17));

        /// <summary>Converts the value of the current Range object to its equivalent string representation.</summary>
        public override string ToString() => Start + ".." + End;

        /// <summary>Create a Range object starting from start index to the end of the collection.</summary>
        public static Range StartAt(Index start) => new Range(start, Index.End);

        /// <summary>Create a Range object starting from first element in the collection to the end Index.</summary>
        public static Range EndAt(Index end) => new Range(Index.Start, end);

        /// <summary>Create a Range object starting from first element to the end.</summary>
        public static Range All => new Range(Index.Start, Index.End);

        /// <summary>Calculate the start offset and length of range object using a collection length.</summary>
        /// <param name="length">The length of the collection that the range will be used with. length has to be a positive value.</param>
        /// <remarks>
        /// For performance reason, we don't validate the input length parameter against negative values.
        /// It is expected Range will be used with collections which always have non negative length/count.
        /// We validate the range is inside the length scope though.
        /// </remarks>
#if NETCOREAPP1_0_OR_GREATER || NETSTANDARD1_0_OR_GREATER || NET45_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public (int Offset, int Length) GetOffsetAndLength(int length)
        {
            int start = Start.GetOffset(length);
            int end = End.GetOffset(length);

            if ((uint)end > (uint)length || (uint)start > (uint)end)
                ThrowArgumentOutOfRangeException();

            return (start, end - start);
        }

        private static void ThrowArgumentOutOfRangeException()
            // ReSharper disable once NotResolvedInText
            => throw new ArgumentOutOfRangeException("length");

    }
}
#endif

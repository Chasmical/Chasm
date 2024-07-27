#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
using System.Runtime.CompilerServices;
[assembly: TypeForwardedTo(typeof(System.Range))]
#else
// Adapted from .NET's source code
#if NETCOREAPP1_0_OR_GREATER || NETSTANDARD1_0_OR_GREATER || NET45_OR_GREATER
using System.Runtime.CompilerServices;
#endif

// ReSharper disable once CheckNamespace
namespace System
{
    public readonly struct Range : IEquatable<Range>
    {
        public Index Start { get; }
        public Index End { get; }

        public Range(Index start, Index end)
        {
            Start = start;
            End = end;
        }

        public override bool Equals(object? value)
            => value is Range r && r.Start.Equals(Start) && r.End.Equals(End);

        public bool Equals(Range other)
            => other.Start.Equals(Start) && other.End.Equals(End);

        #region GetHashCode()
        private static readonly uint hashSeed = GenerateHashSeed();
        private static uint GenerateHashSeed()
        {
            byte[] buffer = new byte[sizeof(uint)];
            new Random().NextBytes(buffer);
            return BitConverter.ToUInt32(buffer, 0);
        }

#if NETCOREAPP1_0_OR_GREATER || NETSTANDARD1_0_OR_GREATER || NET45_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private static uint RotateLeft_17(uint value)
            => (value << 17) | (value >> (32 - 17));

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
        #endregion

        public override string ToString()
            => Start + ".." + End;

        public static Range StartAt(Index start) => new Range(start, Index.End);
        public static Range EndAt(Index end) => new Range(Index.Start, end);

        public static Range All => new Range(Index.Start, Index.End);

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
            => throw new ArgumentOutOfRangeException("length");

    }
}
#endif

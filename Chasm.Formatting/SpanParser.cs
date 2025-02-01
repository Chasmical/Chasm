using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using JetBrains.Annotations;

namespace Chasm.Formatting
{
    /// <summary>
    ///   <para>Represents a parser, that sequentially reads characters from a read-only span of characters.</para>
    /// </summary>
    [DebuggerDisplay($"{{{nameof(DebuggerDisplay)},nq}}")]
#if NET5_0_OR_GREATER
    [SkipLocalsInit]
#endif
#if NETSTANDARD1_0 // System.Memory for netstandard1.0 doesn't have annotations
    [System.Diagnostics.CodeAnalysis.SuppressMessage("ReSharper", "PossiblyImpureMethodCallOnReadonlyVariable")]
#endif
    public ref struct SpanParser
    {
        /// <summary>
        ///   <para>The current position of the parser.</para>
        /// </summary>
        public int position;
        /// <summary>
        ///   <para>The read-only span of characters that the parser is reading.</para>
        /// </summary>
        public readonly ReadOnlySpan<char> source;
        /// <summary>
        ///   <para>The length of the span of characters that the parser is reading.</para>
        /// </summary>
        public readonly int length;

        /// <summary>
        ///   <para>Initializes a new instance of the <see cref="SpanParser"/> structure with the specified <paramref name="text"/>.</para>
        /// </summary>
        /// <param name="text">The read-only span of characters to read.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SpanParser(ReadOnlySpan<char> text)
        {
            source = text;
            length = text.Length;
        }

        /// <summary>
        ///   <para>Gets the character the parser is currently on, if any; otherwise, gets <see langword="default"/>.</para>
        /// </summary>
        public readonly char Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => position < length ? source[position] : default;
        }
        /// <summary>
        ///   <para>Determines whether the current character is an ASCII digit.</para>
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public readonly bool OnAsciiDigit
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (uint)Current - '0' < 10u;
        }
        /// <summary>
        ///   <para>Determines whether the current character is a lowercase or uppercase ASCII letter.</para>
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public readonly bool OnAsciiLetter
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ((uint)Current | ' ') - 'a' <= 'z' - 'a';
        }

        /// <summary>
        ///   <para>Determines whether the parser has not finished reading the span of characters.</para>
        /// </summary>
        /// <returns><see langword="true"/>, if the parser has not finished reading the span of characters; otherwise, <see langword="false"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool CanRead()
            => position < length;
        /// <summary>
        ///   <para>Moves forward a single character, if the parser has not finished reading.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Skip()
        {
            if (position < length)
                position++;
        }
        /// <summary>
        ///   <para>Reads the current character and moves forward, if the parser has not finished reading; otherwise, returns <see langword="default"/>.</para>
        /// </summary>
        /// <returns>The read character, if any; otherwise, <see langword="default"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public char Read()
            => position < length ? source[position++] : default;
        /// <summary>
        ///   <para>Tries to read the current character and move forward, and returns a value indicating whether the character was successfully read.</para>
        /// </summary>
        /// <param name="read">When this method returns, contains the read character, if one was successfully read, or <see langword="default"/> otherwise.</param>
        /// <returns><see langword="true"/>, if a character was read and the parser moved forward; otherwise, <see langword="false"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryRead(out char read)
        {
            if (position < length)
            {
                read = source[position++];
                return true;
            }
            read = default;
            return false;
        }

        /// <summary>
        ///   <para>Returns the character the parser is currently on, if any; otherwise, returns <see langword="default"/>.</para>
        /// </summary>
        /// <returns>The character the parser is currently on, if any; otherwise, <see langword="default"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly char Peek()
            => position < length ? source[position] : default;
        /// <summary>
        ///   <para>Returns the character at the specified <paramref name="offset"/> from the one the parser is currently on, if any; otherwise, returns <see langword="default"/>.</para>
        /// </summary>
        /// <param name="offset">The offset from the current position to get a character from.</param>
        /// <returns>The character at the specified <paramref name="offset"/> from the one the parser is currently on, if any; otherwise, <see langword="default"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly char Peek(int offset)
        {
            int pos = position + offset;
            return pos >= 0 && pos < length ? source[pos] : default;
        }
        /// <summary>
        ///   <para>Tries to return the character the parser is currently on, and returns a value indicating whether the character was successfully peeked at.</para>
        /// </summary>
        /// <param name="peeked">When this method returns, contains the character the parser is currently on, if the parser has not finished reading, or <see langword="default"/> otherwise.</param>
        /// <returns><see langword="true"/>, if the parser has not finished reading; otherwise, <see langword="false"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool TryPeek(out char peeked)
        {
            if (position < length)
            {
                peeked = source[position];
                return true;
            }
            peeked = default;
            return false;
        }
        /// <summary>
        ///   <para>Returns the character before the one the parser is currently on, of any; otherwise, returns <see langword="default"/>.</para>
        /// </summary>
        /// <returns>The character before the one the parser is currently on, of any; otherwise, <see langword="default"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly char PeekBack()
            => position > 0 ? source[position - 1] : default;
        /// <summary>
        ///   <para>Tries to return the character before the one the parser is currently on, and returns a value indicating whether the character was successfully peeked at.</para>
        /// </summary>
        /// <param name="peeked">When this method returns, contains the character before the one the parser is currently on, if there is one, or <see langword="default"/> otherwise.</param>
        /// <returns><see langword="true"/>, if there is a character before the one the parser is currently on; otherwise, <see langword="false"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool TryPeekBack(out char peeked)
        {
            if (position > 0)
            {
                peeked = source[position - 1];
                return true;
            }
            peeked = default;
            return false;
        }

        /// <summary>
        ///   <para>Tries to read the specified character, and if successful, moves forward past the read character.</para>
        /// </summary>
        /// <param name="c">The character to try to read.</param>
        /// <returns><see langword="true"/>, if the specified character was successfully read; otherwise, <see langword="false"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Skip(char c)
        {
            if (position < length && source[position] == c)
            {
                position++;
                return true;
            }
            return false;
        }
        /// <summary>
        ///   <para>Tries to read two specified characters in a sequence, and if successful, moves forward past the read characters.</para>
        /// </summary>
        /// <param name="a">The first character to try to read.</param>
        /// <param name="b">The second character to try to read.</param>
        /// <returns><see langword="true"/>, if the two specified characters were successfully read; otherwise, <see langword="false"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Skip(char a, char b)
        {
            if (position + 1 < length && source[position] == a && source[position + 1] == b)
            {
                position += 2;
                return true;
            }
            return false;
        }
        /// <summary>
        ///   <para>Tries to read three specified characters in a sequence, and if successful, moves forward past the read characters.</para>
        /// </summary>
        /// <param name="a">The first character to try to read.</param>
        /// <param name="b">The second character to try to read.</param>
        /// <param name="c">The third character to try to read.</param>
        /// <returns><see langword="true"/>, if the three specified characters were successfully read; otherwise, <see langword="false"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Skip(char a, char b, char c)
        {
            if (position + 2 < length && source[position] == a && source[position + 1] == b && source[position + 2] == c)
            {
                position += 3;
                return true;
            }
            return false;
        }
        /// <summary>
        ///   <para>Tries to read four specified characters in a sequence, and if successful, moves forward past the read characters.</para>
        /// </summary>
        /// <param name="a">The first character to try to read.</param>
        /// <param name="b">The second character to try to read.</param>
        /// <param name="c">The third character to try to read.</param>
        /// <param name="d">The fourth character to try to read.</param>
        /// <returns><see langword="true"/>, if the four specified characters were successfully read; otherwise, <see langword="false"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Skip(char a, char b, char c, char d)
        {
            if (position + 3 < length && source[position] == a && source[position + 1] == b && source[position + 2] == c && source[position + 3] == d)
            {
                position += 4;
                return true;
            }
            return false;
        }
        /// <summary>
        ///   <para>Tries to read the specified <paramref name="sequence"/> of characters, and if successful, moves forward past the read characters.</para>
        /// </summary>
        /// <param name="sequence">The sequence of characters to try to read.</param>
        /// <returns><see langword="true"/>, if the specified <paramref name="sequence"/> of characters was successfully read; otherwise, <see langword="false"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Skip(ReadOnlySpan<char> sequence)
        {
            if (position + sequence.Length <= length && source.Slice(position, sequence.Length).SequenceEqual(sequence))
            {
                position += sequence.Length;
                return true;
            }
            return false;
        }

        /// <summary>
        ///   <para>Tries to read any of the specified two characters, and if successful, moves forward past the read character.</para>
        /// </summary>
        /// <param name="a">The first character to try to read.</param>
        /// <param name="b">The second character to try to read.</param>
        /// <returns><see langword="true"/>, if any of the two specified characters was read; otherwise, <see langword="false"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool SkipAny(char a, char b)
        {
            if (position < length && (source[position] == a || source[position] == b))
            {
                position++;
                return true;
            }
            return false;
        }
        /// <summary>
        ///   <para>Tries to read any of the specified three characters, and if successful, moves forward past the read character.</para>
        /// </summary>
        /// <param name="a">The first character to try to read.</param>
        /// <param name="b">The second character to try to read.</param>
        /// <param name="c">The third character to try to read.</param>
        /// <returns><see langword="true"/>, if any of the three specified characters was read; otherwise, <see langword="false"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool SkipAny(char a, char b, char c)
        {
            if (position < length && (source[position] == a || source[position] == b || source[position] == c))
            {
                position++;
                return true;
            }
            return false;
        }
        /// <summary>
        ///   <para>Tries to read any of the specified four characters, and if successful, moves forward past the read character.</para>
        /// </summary>
        /// <param name="a">The first character to try to read.</param>
        /// <param name="b">The second character to try to read.</param>
        /// <param name="c">The third character to try to read.</param>
        /// <param name="d">The fourth character to try to read.</param>
        /// <returns><see langword="true"/>, if any of the four specified characters was read; otherwise, <see langword="false"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool SkipAny(char a, char b, char c, char d)
        {
            if (position < length && (source[position] == a || source[position] == b || source[position] == c || source[position] == d))
            {
                position++;
                return true;
            }
            return false;
        }
        /// <summary>
        ///   <para>Tries to read any of the specified <paramref name="characters"/>, and if successful, moves forward past the read character.</para>
        /// </summary>
        /// <param name="characters">The characters to try to read.</param>
        /// <returns><see langword="true"/>, if any of the specified <paramref name="characters"/> was read; otherwise, <see langword="false"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool SkipAny(ReadOnlySpan<char> characters)
        {
#if NETCOREAPP3_0_OR_GREATER
            if (position < length && characters.Contains(source[position]))
#else
            if (position < length && characters.IndexOf(source[position]) >= 0)
#endif
            {
                position++;
                return true;
            }
            return false;
        }

        /// <summary>
        ///   <para>Moves past all characters equal to the specified character.</para>
        /// </summary>
        /// <param name="c">The character to move past.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SkipAll(char c)
        {
            while (position < length && source[position] == c)
                position++;
        }

        /// <summary>
        ///   <para>Moves past all whitespace characters.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SkipWhitespaces()
        {
            while (position < length && char.IsWhiteSpace(source[position]))
                position++;
        }
        /// <summary>
        ///   <para>Moves back to the character after the last non-whitespace character.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UndoSkippingWhitespace()
        {
            while (position > 0 && char.IsWhiteSpace(source[position - 1]))
                position--;
        }

        /// <summary>
        ///   <para>Reads a sequence of ASCII digits, and moves past the read sequence.</para>
        /// </summary>
        /// <returns>The read sequence of ASCII digits.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlySpan<char> ReadAsciiDigits()
        {
            int start = position;
            while (position < length && (uint)source[position] - '0' < 10u)
                position++;
            return source.Slice(start, position - start);
        }
        /// <summary>
        ///   <para>Reads a sequence of lowercase and uppercase ASCII letters, and moves past the read sequence.</para>
        /// </summary>
        /// <returns>The read sequence of lowercase and uppercase ASCII letters.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlySpan<char> ReadAsciiLetters()
        {
            int start = position;
            while (position < length && ((uint)source[position] | ' ') - 'a' <= 'z' - 'a')
                position++;
            return source.Slice(start, position - start);
        }

        /// <summary>
        ///   <para>Reads a sequence of characters not equal to the specified <paramref name="character"/>, and moves past the read sequence.</para>
        /// </summary>
        /// <param name="character">The character to stop reading at.</param>
        /// <returns>The read sequence of characters.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlySpan<char> ReadUntil(char character)
        {
            int start = position;
            while (position < length && source[position] != character)
                position++;
            return source.Slice(start, position - start);
        }

        /// <summary>
        ///   <para>Reads a sequence of characters satisfying the specified <paramref name="predicate"/>, and moves past the read sequence.</para>
        /// </summary>
        /// <param name="predicate">The function that defines the characters to read.</param>
        /// <returns>The read sequence of characters.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlySpan<char> ReadWhile([InstantHandle] Func<char, bool> predicate)
        {
            int start = position;
            while (position < length && predicate(source[position]))
                position++;
            return source.Slice(start, position - start);
        }
        /// <inheritdoc cref="ReadWhile(Func{char,bool})"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe ReadOnlySpan<char> ReadWhile([InstantHandle] delegate*<char, bool> predicate)
        {
            int start = position;
            while (position < length && predicate(source[position]))
                position++;
            return source.Slice(start, position - start);
        }
        /// <summary>
        ///   <para>Reads a sequence of characters not satisfying the specified <paramref name="predicate"/>, and moves past the read sequence.</para>
        /// </summary>
        /// <param name="predicate">The function that defines the characters to stop reading at.</param>
        /// <returns>The read sequence of characters.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlySpan<char> ReadUntil([InstantHandle] Func<char, bool> predicate)
        {
            int start = position;
            while (position < length && !predicate(source[position]))
                position++;
            return source.Slice(start, position - start);
        }
        /// <inheritdoc cref="ReadUntil(Func{char,bool})"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe ReadOnlySpan<char> ReadUntil([InstantHandle] delegate*<char, bool> predicate)
        {
            int start = position;
            while (position < length && !predicate(source[position]))
                position++;
            return source.Slice(start, position - start);
        }

        /// <summary>
        ///   <para>Reads the remaining sequence of characters, and moves to the end.</para>
        /// </summary>
        /// <returns>The remaining sequence of characters.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlySpan<char> ReadRemaining()
        {
            int start = position;
            position = length;
            return source.Slice(start, length - start);
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly string DebuggerDisplay
        {
            // ReSharper disable CommentTypo GrammarMistakeInComment
            get
            {
                // Debugger display format:
                // 
                //  (15 chars behind) (15  chars ahead)
                //   vvvvvvvvvvvvvvv   vvvvvvvvvvvvvvv
                // "…012345678901234⟨5⟩678901234567890…" (pos=25/60)
                //                  ^^^                  ^^^^^^^^^^^
                //               (pointer)             (position info)
                // 
                // Examples:
                // 
                // - "" (pos=0/0, finished)
                // - "123" (pos=3/3, finished)
                // - "⟨1⟩23" (pos=1/3)
                // - "⟨0⟩123456789012345…" (pos=0/60)
                // - "01234⟨5⟩678901234567890…" (pos=5/60)
                // - "…901234567890123⟨4⟩56789" (pos=54/60)
                // 

                int pos = position;
                ReadOnlySpan<char> src = source;

                const int peekDistance = 15;
                // Initial capacity is 64 chars. The StringBuilder can, of course, expand as needed.
                // (15-char peek distance & 4-digit length - max 63 chars)
                StringBuilder sb = new(64);

                sb.Append('"');
                {
                    int leftBound = Math.Max(pos - peekDistance, 0);
                    int rightBound = Math.Min(pos + peekDistance + 1, src.Length);

                    // Append the left peeked side
                    {
                        if (leftBound > 0) sb.Append('…');
#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                        sb.Append(src.Slice(leftBound, pos - leftBound));
#else
                        sb.Append(src.Slice(leftBound, pos - leftBound).ToString());
#endif
                    }

                    if (pos < src.Length)
                    {
                        // Append the current char pointer
                        sb.Append('⟨').Append(src[pos]).Append('⟩');

                        // Append the right peeked side
                        {
#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                            sb.Append(src.Slice(pos + 1, rightBound - (pos + 1)));
#else
                            sb.Append(src.Slice(pos + 1, rightBound - (pos + 1)).ToString());
#endif
                            if (rightBound < src.Length) sb.Append('…');
                        }
                    }
                }
                sb.Append('"');

                // Append position information
                sb.Append(" (pos=").Append(pos).Append('/').Append(src.Length);
                if (pos == src.Length) sb.Append(", finished");
                sb.Append(')');

                return sb.ToString();
            }
        }

    }
}

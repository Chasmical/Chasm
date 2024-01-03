# Chasm.Formatting Changelog

### v2.1.1
- 🚑️ Fixed IndexOutOfRangeException when formatting a number at the end;

### v2.1.0
- ✨ Added `SpanParser.ReadUntil(char)`;
- ✨ Added `SpanParser.ReadWhile(delegate*<char, bool>)`;
- ✨ Added `SpanParser.ReadUntil(delegate*<char, bool>)`;
- ⚡️ Improved ASCII letter reading performance for `SpanParser`;

### v2.0.2
- 🧩 Retargeted to: `net8.0`, `net6.0`, `net5.0`, `netcoreapp2.1`, `netstandard2.1`;

### v2.0.1
- 🧩 Retargeted to: `net8.0`, `net6.0`, `netcoreapp2.1`, `netstandard2.1`;
- 🐛 Removed unnecessary dependency on `System.Runtime.CompilerServices.Unsafe`;

### v2.0.0
- 🧩 Targets: `net7.0`, `net6.0`, `netcoreapp2.1`, `netstandard2.1`;
- ✨ Added `ref struct SpanBuilder`;
- ✨ Added `SpanBuilder.Append(char)`;
- ✨ Added `SpanBuilder.Append(char, char)`;
- ✨ Added `SpanBuilder.Append(char, char, char)`;
- ✨ Added `SpanBuilder.Append(char, char, char, char)`;
- ✨ Added `SpanBuilder.Append(ReadOnlySpan<char>)`;
- ✨ Added `SpanBuilder.Append(ISpanBuildable)`;
- ✨ Added `SpanBuilder.Append(uint)`;
- ✨ Added `SpanBuilder.Append(int)`;
- ✨ Added `static SpanBuilder.CalculateLength(uint)`;
- ✨ Added `static SpanBuilder.CalculateLength(int)`;
- ✨ Added `static SpanBuilder.Format(ISpanBuildable)`;
- ✨ Added `static SpanBuilder.Format(ISpanBuildableFormat, ReadOnlySpan<char>)`;
- ✨ Added `static SpanBuilder.Format(int, SpanBuilderAction)`;
- ✨ Added `delegate SpanBuilder.SpanBuilderAction(ref SpanBuilder)`;
- ✨ Added `interface ISpanBuildable`: `CalculateLength()`, `BuildString(ref SpanBuilder)`;
- ✨ Added `interface ISpanBuildableFormat`: `CalculateLength(ReadOnlySpan<char>)`, `BuildString(ref SpanBuilder, ReadOnlySpan<char>)`;
- ✨ Added `ref struct SpanParser`;
- ✨ Added `SpanParser.position`;
- ✨ Added `SpanParser.source`;
- ✨ Added `SpanParser.length`;
- ✨ Added `SpanParser(ReadOnlySpan<char>)`;
- ✨ Added `SpanParser.Current`;
- ✨ Added `SpanParser.OnAsciiDigit`;
- ✨ Added `SpanParser.OnAsciiLetter`;
- ✨ Added `SpanParser.CanRead()`;
- ✨ Added `SpanParser.Skip()`;
- ✨ Added `SpanParser.Read()`;
- ✨ Added `SpanParser.TryRead(out char)`;
- ✨ Added `SpanParser.Peek()`;
- ✨ Added `SpanParser.Peek(int)`;
- ✨ Added `SpanParser.TryPeek(out char)`;
- ✨ Added `SpanParser.PeekBack()`;
- ✨ Added `SpanParser.TryPeekBack(out char)`;
- ✨ Added `SpanParser.Skip(char)`;
- ✨ Added `SpanParser.Skip(char, char)`;
- ✨ Added `SpanParser.Skip(char, char, char)`;
- ✨ Added `SpanParser.Skip(char, char, char, char)`;
- ✨ Added `SpanParser.SkipAny(char, char)`;
- ✨ Added `SpanParser.SkipAny(char, char, char)`;
- ✨ Added `SpanParser.SkipAny(char, char, char, char)`;
- ✨ Added `SpanParser.SkipAll(char)`;
- ✨ Added `SpanParser.SkipWhitespaces()`;
- ✨ Added `SpanParser.UndoSkippingWhitespace()`;
- ✨ Added `SpanParser.ReadAsciiDigits()`;
- ✨ Added `SpanParser.ReadAsciiLetters()`;
- ✨ Added `SpanParser.ReadWhile(Func<char, bool>)`;
- ✨ Added `SpanParser.ReadUntil(Func<char, bool>)`;

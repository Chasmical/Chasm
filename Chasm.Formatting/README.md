# Chasm.Formatting

[![Latest NuGet version](https://img.shields.io/nuget/v/Chasm.Formatting)](https://www.nuget.org/packages/Chasm.Formatting/)
[![MIT License](https://img.shields.io/github/license/Chasmical/Chasm)](../LICENSE)

Provides various formatting and parsing utility types and methods.



## `SpanParser`

A helper structure designed to quickly and efficiently parse text.

```cs
string text = """
$variable = [some value];
DoSomething();
""";

SpanParser parser = new SpanParser(text);

// returns true and moves forward, if the next character is '$' or '@'
if (parser.SkipAny('$', '@'))
{
    // reads "variable", moves past it and returns it
    ReadOnlySpan<char> name = parser.ReadAsciiLetters();
    parser.SkipWhitespaces();
    // returns true and moves forward, if the next character is '='
    if (parser.Skip('='))
    {
        parser.SkipWhitespaces();
        // reads "[some value]", moves past it and returns it
        ReadOnlySpan<char> value = parser.ReadUntil(c => c == ';');

        // at this point, the parser either reached the end, or is at ';'
        /* ... */
    }
}
// returns the current character, or '\0' if the parser reached the end
else if (parser.Peek() is >= 'A' and <= 'Z')
{
    char read = parser.Read(); // reads a character (or '\0') and moves forward
    /* ... */
}
```

> [!NOTE]
> There's a very similar type in .NET called [`SequenceReader<T>`](https://learn.microsoft.com/en-us/dotnet/api/system.buffers.sequencereader-1), but it only works on `Memory<T>`, and not on `Span<T>`. I don't really know how it compares to my `SpanParser` though.
> 
> - [ ] TODO: do a feature comparison and some benchmarks between `SequenceReader<T>` and `SpanParser`.



## `SpanBuilder`

A helper structure designed to minimize formatting's memory allocation by allocating the exact required amount of memory and then formatting directly into it.

```cs
public readonly struct MyStruct : ISpanBuildable
{
    private readonly string? text;
    private readonly int number;

    public override string ToString()
        => SpanBuilder.Format(this);

    // Of course, on this scale, using SpanBuilder would be detrimental to performance.
    // You should use it in more complex scenarios, with lots of nesting.

    public void CalculateLength()
        => text?.Length ?? SpanBuilder.CalculateLength(number);

    public void BuildString(ref SpanBuilder sb)
    {
        if (text is not null)
            sb.Append(text);
        else
            sb.Append((uint)number);
    }
}
```

> [!NOTE]
> The performance benefit isn't that great, and sometimes `SpanBuilder` is slightly slower than a regular `StringBuilder`. RAM is considered a relatively cheap resource in .NET for a reason, after all.

> [!CAUTION]
> **Implementations of `ISpanBuildable` are extremely prone to errors!**
> 
> `SpanBuilder` will throw an exception if the sizes don't match. Make sure that it's all thoroughly tested.



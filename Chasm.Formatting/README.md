# Chasm.Formatting

Provides various formatting and parsing utility types and methods.

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
    // reads "variable" and returns it
    ReadOnlySpan<char> name = parser.ReadAsciiLetters();
    parser.SkipWhitespaces();
    // returns true and moves forward, if the next character is '='
    if (parser.Skip('='))
    {
        parser.SkipWhitespaces();
        // reads "[some value]" and returns it
        ReadOnlySpan<char> value = parser.ReadUntil(c => c == ';');
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

# Chasm.Utilities

Provides various utility types and methods.

## `Util`

Contains `Fail` and `Catch` utility methods, that can be used to shorten common branch code. *(or... you could also just not use the auto-formatting, and write everything in a single line, but that's not my style)*

`Util.Fail` sets all `out` parameters to `default` and returns either `false` or a specified return value. Especially useful for parsing.

```cs
// Commonly written as:
if (stringIsInvalid) { result = null; return ErrorCode.InvalidString; }

// Using Util.Fail:
if (stringIsInvalid) return Util.Fail(ErrorCode.InvalidString, out result);
```

`Util.Catch` catches an exception thrown by the specified delegate. If the delegate returned a value, it can be read through an `out` parameter.

```cs
// Commonly written as:
object? result; Exception? ex;
try { result = doSomething(); ex = null; }
catch (Exception caught) { result = null; ex = caught; }

// Using Util.Catch:
var ex = Util.Catch(doSomething, out object? result);
```

# Chasm.Utilities

[![Latest NuGet version](https://img.shields.io/nuget/v/Chasm.Utilities)](https://www.nuget.org/packages/Chasm.Utilities/)
[![MIT License](https://img.shields.io/github/license/Chasmical/Chasm)](../LICENSE)

Provides various utility types and methods, for code that needs to be written quickly, and not necessarily efficiently.

> [!WARNING]
> ████ **Don't use this package in performance-critical scenarios!** ████  
> Many methods don't get inlined properly, and even if they do, there's still some overhead.  
> As such, appropriate uses of this library would be: writing quick tests, proofs-of-concept and mock-ups.



## `Util`

Contains various utility methods, that can be used to shorten common branch code. *(or... you could also just not use the auto-formatting, and write everything in a single line, but that's not my style)*

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

`Util.Is` can be used to assign a type-casted value to an existing variable.

```cs
// Commonly written as:
if (a is BigInteger bigInt_1) { /* ... */ }
else if (b is BigInteger bigInt_2) { /* ... */ }
else if (c is BigInteger bigInt_3) { /* ... */ }

// Using Util.Is:
if (a is BigInteger bigInt) { /* ... */ }
else if (Util.Is(b, out bigInt)) { /* ... */ }
else if (Util.Is(c, out bigInt)) { /* ... */ }
```

`Util.With` can be used to invoke functions while `using` a `IDisposable`:

```cs
// Commonly written as:
string? firstLine;
using (StreamReader reader = File.OpenText(path))
    firstLine = reader.ReadLine();

// Using Util.With:
string? firstLine = Util.With(File.OpenText(path), reader => reader.ReadLine());
```

`Util.Swap` just swaps two values. Useful when the tuple swap is too long or prone to errors.

```cs
// Commonly written as:
(firstVariable, secondVariable) = (secondVariable, firstVariable);

// Using Util.Swap:
Util.Swap(ref firstVariable, ref secondVariable);
```



## `DelegateDisposable`

`DelegateDisposable` is an `IDisposable` interface implementation that invokes an action on disposal.

```cs
store.Subscribe(listenerFunc);
var listener = new DelegateDisposable(() => store.Unsubscribe(listenerFunc));

// Or like this, functional programming style:
var listener = DelegateDisposable.Create(
    () => store.Subscribe(listenerFunc),
    () => store.Unsubscribe(listenerFunc)
);
```



## `ReaderWriterLockSlimExtensions`

`ReaderWriterLockSlimExtensions` makes use of specialized variants of `DelegateDisposable` to enter and exit locks.

```cs
ReaderWriterLockSlim rwl = new();

// Commonly written as:
try
{
    rwl.EnterReadLock();
    /* ... */
}
finally
{
    rwl.ExitReadLock();
}

// Using ReaderWriterLockSlimExtensions:
using (rwl.WithReaderLock())
    /* ... */
```



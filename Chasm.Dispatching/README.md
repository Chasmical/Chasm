# Chasm.Dispatching

[![Latest NuGet version](https://img.shields.io/nuget/v/Chasm.Dispatching)](https://www.nuget.org/packages/Chasm.Dispatching/)
[![MIT License](https://img.shields.io/github/license/Chasmical/Chasm)](../LICENSE)

Provides a `CompiledDispatch<T>` class, that pre-compiles the invocation delegate to use static dispatch instead of dynamic dispatch.



## `CompiledDispatch<T>`

Many frameworks tend to use the following pattern:

```csharp
public interface ISystem {
    void Update(GameTime time);
}

foreach (System system in GetSystems())
    system.Update();
```

This often results in hundreds, or even thousands, of virtual calls. It's fine if these systems aren't called often, but if they are (e.g. in a game with ECS), it could cost the program entire seconds of CPU time! `CompiledDispatch<T>` aims to alleviate this problem, by compiling the methods statically (compilation only takes a fraction of a millisecond).

```csharp
CompiledDispatch<GameTime> dispatch = new();

// all the ways methods can be added:
dispatch.Add(clockSystem.Update); // ← preferred
dispatch.Add(timerSystem, "Update");
dispatch.Add(npcSystem, npcSystemUpdateMethod);

// you can even add static methods into the mix!
dispatch.Add(SomeStaticUpdateMethod);
// or even just regular lambdas:
dispatch.Add(time => Console.WriteLine(time));

dispatch.Compile();
dispatch.Dispatch(new GameTime(1000));
```

The compiled method would essentially look like this:

```csharp
[MethodImpl(MethodImplOptions.NoInlining)]
static void Dispatch_Generated(GameTime time)
{
    clockSystem.Update(time);
    timerSystem.Update(time);
    npcSystem.Update(time);

    SomeStaticUpdateMethod();
    (time => Console.WriteLine(time))();
}
```

All of these method calls will have the ability to be inlined or even completely optimized away!

> [!CAUTION]
>
> **Don't update the collection of dispatched methods too often!**  
> Otherwise, the performance gain won't be worth the extra compilation time.  
> Ideally, you'd populate the collection just once, and then only call the compiled method.



## Benchmarks

Benchmarking: 100 objects with virtual no-op (but not inlinable) methods.

`Compiled` uses `CompiledDispatch<…>`, and calls each method statically. `Virtual` uses a specialized vtable (for class methods), to find the overridden methods more quickly. `Interfaces` have to look up their own vtable in the type's method table, so it's a bit harder to find the overridden methods. `Delegates` invoke `Action<…>` delegates, providing more flexibility to users, but also requiring more runtime checks.

```
| Method           | Mean        | Error     | StdDev    |
|----------------- |------------:|----------:|----------:|
| Compiled         |    163.9 ns |   1.57 ns |   1.47 ns |
| Virtual          |    180.5 ns |   0.43 ns |   0.38 ns |
| Interfaces       |    230.2 ns |   0.93 ns |   0.87 ns |
| Delegates        |    257.8 ns |   1.11 ns |   1.04 ns |
|----------------- |------------:|----------:|----------:|
| Create           |    11.32 us |  0.123 us |  0.115 us |
| CreateAndCompile |    91.21 us |  0.704 us |  0.588 us |
| ^ Compile        |    79.89 us |
```

- `Compiled` vs. `Virtual`: 16.6 ns difference [40s @120fps]
- `Compiled` vs. `Interfaces`: 66.3 ns difference [10s @120fps]
- `Compiled` vs. `Delegates`: 93.9 ns difference [7s @120fps]

Time in square brackets is the amount of time running a game with ECS on 120fps, after which this optimization's benefits become worthwhile after compiling the `CompiledDispatch<…>`'s methods once.

### Conclusion

While virtual class methods can be almost just as efficient, the `CompiledDispatch<…>` class really shines when it comes to systems relying on interfaces or delegates. And another conclusion: abstract classes > interfaces > delegates.



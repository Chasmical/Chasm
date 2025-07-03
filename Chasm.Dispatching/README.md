# Chasm.Dispatching

[![Latest NuGet version](https://img.shields.io/nuget/v/Chasm.Dispatching)](https://www.nuget.org/packages/Chasm.Dispatching/)
[![MIT License](https://img.shields.io/github/license/Chasmical/Chasm)](../LICENSE)

Provides a `CompiledDispatch<T>` class, that pre-compiles the invocation delegate to use static dispatch instead of dynamic dispatch.



## `CompiledDispatch<T>`

Many frameworks tend to use the following pattern:

```csharp
public abstract class System {
    public abstract void Update(GameTime time);
}

foreach (System system in GetSystems())
    system.Update();
```

This often results in hundreds, or even thousands, of virtual calls. It's fine if these systems aren't called often, but if they are (e.g. in a game with ECS), it could cost the program entire seconds of CPU time! `CompiledDispatch<T>` aims to alleviate this problem, by compiling the methods statically (compilation only takes a few dozen microseconds).

```csharp
CompiledDispatch<GameTime> dispatch = new();

// all the ways methods can be added:
dispatch.Add(clockSystem.Update); // ← preferred
dispatch.Add(timerSystem, "Update");
dispatch.Add(npcSystem, npcSystemUpdateMethod);

dispatch.Compile();
dispatch.Dispatch(new GameTime(1000));
```

The compiled method would essentially look like this:

```csharp
[MethodImpl(MethodImplOptions.NoInlining)]
static void Dispatch_Generated(GameTime time)
{
    ClockSystem clockSystem = /*…*/;
    clockSystem.Update(time);
    TimerSystem timerSystem = /*…*/;
    timerSystem.Update(time);
    NpcSystem npcSystem = /*…*/;
    npcSystem.Update(time);
}
```

All of the static `Update` calls will have the ability to be inlined or even completely optimized away!

> [!CAUTION]
>
> **Don't update the collection of dispatched methods too often!**  
> Otherwise, the performance gain won't be worth the extra compilation time.  
> Ideally, you'd populate the collection just once, and then only call the compiled method.



TODO: BENCHMARKS



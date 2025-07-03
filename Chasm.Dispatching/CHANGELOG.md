# Chasm.Dispatching Changelog

### v1.1.0
- 🧩 Added `netcoreapp3.0`, `netstandard2.1` and `net461` targets. Now targets: `net9.0`, `netcoreapp3.0`, `netcoreapp2.0`, `netstandard2.1`, `netstandard2.0`, `net461`, `net45`;
- 🐛 Replaced `System.Memory` with `System.Runtime.CompilerServices.Unsafe` dependency;
- ⚡️ Slightly improved `CompiledDispatch<TArg>`'s code gen, eliminating unnecessary bounds-checks;
- ⚡️ Fixed inlining options of `CompiledDispatch<TArg>`'s `Compile` and `Dispatch` methods;
- 📝 Added XML docs, added benchmarks to README;

### v1.0.0
- 🧩 Targets: `net9.0`, `netcoreapp2.0`, `netstandard2.0`, `net45`;
- ✨ Added `sealed class CompiledDispatch<TArg>`;
- ✨ Added `CompiledDispatch<TArg>()`;
- ✨ Added `CompiledDispatch<TArg>.IsCompiled`;
- ✨ Added `CompiledDispatch<TArg>.Count`;
- ✨ Added `CompiledDispatch<TArg>.Add(object?, MethodInfo)`;
- ✨ Added `CompiledDispatch<TArg>.Add(object, string)`;
- ✨ Added `CompiledDispatch<TArg>.Add(Action<TArg>)`;
- ✨ Added `CompiledDispatch<TArg>.Remove(object?, MethodInfo)`;
- ✨ Added `CompiledDispatch<TArg>.Remove(Action<TArg>)`;
- ✨ Added `CompiledDispatch<TArg>.Compile()`;
- ✨ Added `CompiledDispatch<TArg>.Dispatch(TArg)`;
- ✨ Added `CompiledDispatch<TArg>.Dispatch(TArg, bool)`;

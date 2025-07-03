# Chasm.Dispatching Changelog

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

# Chasm.Utilities Changelog

### v2.5.0
- ✨ Added `static class HashCodeExtensions`;
- ✨ Added `HashCodeExtensions.AddRange<T>(this HashCode, ReadOnlySpan<T>)`;
- ✨ Added `HashCodeExtensions.AddRange<T>(this HashCode, ReadOnlySpan<T>, IEqualityComparer<T>?)`;
- ✨ Added `HashCodeExtensions.AddRange<T>(this HashCode, IEnumerable<T>)`;
- ✨ Added `HashCodeExtensions.AddRange<T>(this HashCode, IEnumerable<T>, IEqualityComparer<T>?)`;
- 🧩 Added `netcoreapp2.1` and `netstandard2.0` targets. Now targets: `net8.0`, `netcoreapp3.0`, `netcoreapp2.1`, `netcoreapp1.0`, `netstandard2.1`, `netstandard2.0`, `netstandard1.0`, `net45`, `net35`;
- ⚡️ Microoptimized IL code size of `DelegateDisposable`'s constructor;

### v2.4.0
- ✨ Added `static class WeakReferenceExtensions`;
- ✨ Added `WeakReferenceExtensions.TryGetTarget(this WeakReference, out object?)`;
- ✨ Added `WeakReferenceExtensions.GetTargetOrDefault(this WeakReference)`;
- ✨ Added `WeakReferenceExtensions.GetTargetOrDefault<T>(this WeakReference<T>)`;

### v2.3.7
- ♻️ Refactored shimmed attributes;

### v2.3.6
- 🧩 Added `net35` target. Now targets: `net8.0`, `netcoreapp3.0`, `netcoreapp1.0`, `netstandard2.1`, `netstandard1.0`, `net45`, `net35`;

### v2.3.5
- 📄 Updated license information;

### v2.3.4
- ✨ Added `static Util.Swap<T>(ref T, ref T)`;

### v2.3.3
- ✨ Added `static DelegateDisposable.Create<TState>(Func<TState>, Action<TState>)`;

### v2.3.2
- 🧩 Retargeted to: `net8.0`, `netcoreapp3.0`, `netcoreapp1.0`, `netstandard2.1`, `netstandard1.0`, `net45`;

### v2.3.1
- ⬆️ Upgraded `JetBrains.Annotations` from 2023.2.0 to 2023.3.0;
- 🧑‍💻 Added `[MustDisposeResource]`, `[HandlesResourceDisposal]` code analysis attributes;

### v2.3.0
- ✨ Added `class DelegateDisposable : IDisposable`;
- ✨ Added `DelegateDisposable(Action)`;
- ✨ Added `DelegateDisposable.Dispose()`;
- ✨ Added `DelegateDisposable.Dispose(bool)`;
- ✨ Added `static DelegateDisposable.Create(Action, Action)`;
- ✨ Added `static class ReaderWriterLockSlimExtensions`;
- ✨ Added `ReaderWriterLockSlimExtensions.WithReaderLock(this ReaderWriterLockSlim)`;
- ✨ Added `ReaderWriterLockSlimExtensions.WithUpgradeableReaderLock(this ReaderWriterLockSlim)`;
- ✨ Added `ReaderWriterLockSlimExtensions.WithWriterLock(this ReaderWriterLockSlim)`;

### v2.2.0
- 🧩 Retargeted to: `net7.0`, `netcoreapp3.0`, `netcoreapp1.0`, `netstandard2.1`, `netstandard1.0`, `net45`;
- ✨ Added `Util.With<TResult>(IDisposable, Func<TResult>)`;
- ✨ Added `Util.With<T>(T, Func<T, TResult>)`;

### v2.1.0
- ✨ Added `Util.Catch(Action)`;
- ✨ Added `Util.Catch<TResult>(Func<TResult>, out TResult?)`;
- ✨ Added `Util.Catch<TException>(Action)`;
- ✨ Added `Util.Catch<TException, TResult>(Func<TResult>, out TResult?)`;
- ✨ Added `Util.Is<T>(object?, out T?)`;

### v2.0.0
- 🧩 Targets: `net7.0`, `netcoreapp3.1`, `netstandard2.1`;
- ✨ Added `static class Util`;
- ✨ Added `Util.Fail<T>(out T?)`;
- ✨ Added `Util.Fail<T1, T2>(out T1?, out T2?)`;
- ✨ Added `Util.Fail<T1, T2, T3>(out T1?, out T2?, out T3?)`;
- ✨ Added `Util.Fail<TReturn, T>(TReturn, out T?)`;
- ✨ Added `Util.Fail<TReturn, T1, T2>(TReturn, out T1?, out T2?)`;
- ✨ Added `Util.Fail<TReturn, T1, T2, T3>(TReturn, out T1?, out T2?, out T3?)`;

# Chasm.Compatibility.HashCode Changelog

### v2.1.2
- 🧩 Removed `net8.0` target. Now targets: `netcoreapp2.1`, `netcoreapp2.0`, `netcoreapp1.0`, `netstandard2.1`, `netstandard2.0`, `netstandard1.3`, `netstandard1.0`, `net461`, `net45`, `net35`;

### v2.1.1
- 🐛 Fixed usage of `RandomNumberGenerator` for .NET Framework targets;

### v2.1.0
- ♻️ Refactored shimmed attributes;

### v2.0.0
- 🧩 Targets: `net8.0`, `netcoreapp2.1`, `netcoreapp2.0`, `netcoreapp1.0`, `netstandard2.1`, `netstandard2.0`, `netstandard1.3`, `netstandard1.0`, `net461`, `net45`, `net35`;
- ✨ Added `System.HashCode` polyfill;

# Chasm.Compatibility.IndexRange Changelog

### v2.2.2
- 🧩 Removed `net8.0` target. Now targets: `netcoreapp3.0`, `netcoreapp2.0`, `netcoreapp1.0`, `netstandard2.1`, `netstandard2.0`, `netstandard1.0`, `net47`, `net45`, `net40`, `net35`;

### v2.2.1
- 🐛 Fixed usage of `RandomNumberGenerator` for hashing for .NET Framework targets;

### v2.2.0
- 🧩 Added `netcoreapp2.0`, `netstandard2.0`, `net47`, `net40` targets. Now targets: `net8.0`, `netcoreapp3.0`, `netcoreapp2.0`, `netcoreapp1.0`, `netstandard2.1`, `netstandard2.0`, `netstandard1.0`, `net47`, `net45`, `net40`, `net35`;
- ♻️ Refactored shimmed attributes;

### v2.1.0
- 🧩 Added `netstandard1.3` target. Now targets: `net8.0`, `netcoreapp3.0`, `netcoreapp1.0`, `netstandard2.1`, `netstandard1.0`, `net45`, `net35`;
- 🐛 Fixed random seed generation in `Range.GetHashCode()`;
- ♻️ Refactored source code;
- 📝 Added XML documentation;

### v2.0.0
- 🧩 Targets: `net8.0`, `netcoreapp3.0`, `netcoreapp1.0`, `netstandard2.1`, `netstandard1.0`, `net45`, `net35`;
- ✨ Added `System.Index` polyfill;
- ✨ Added `System.Range` polyfill;

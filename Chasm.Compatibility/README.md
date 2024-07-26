# Chasm.Compatibility

[![Latest NuGet version](https://img.shields.io/nuget/v/Chasm.Compatibility)](https://www.nuget.org/packages/Chasm.Compatibility/)
[![MIT License](https://img.shields.io/github/license/Chasmical/Chasm)](../LICENSE)

Provides a bunch of compatibility stuff, polyfills and shims.



> [!CAUTION]
> **If you're developing your own package, make sure you're targeting the same framework versions as this package!**  
> **Otherwise, your users may end up with ambiguous type reference errors!**
> 
> **For example:**  
> Your package targets Standard 2.0, and uses this package's target for Standard 2.0, which polyfills `AllowNullAttribute` (that was introduced in Standard 2.1). Then, the user of your package targets Standard 2.1 (which already has an `AllowNullAttribute`) and uses your library for Standard 2.0, which in turn references this package (with polyfilled `AllowNullAttribute`).
> 
> Alternatively, you could set `PrivateAssets="all"` in the `<PackageReference>` item. Doing this **will** introduce some Reflection issues, so make sure you know what you're doing.



## How does this compare to PolySharp?

PolySharp only polyfills types that are actually used (which is a good thing) and puts them in the project's own DLL file. But it doesn't work well in multi-project setups, with `<InternalsVisibleTo>` exposing one project's PolySharp generated files to another, causing ambiguous type errors.

Chasm.Compatibility, on the other hand, includes all of the types in a separate DLL. Nothing fancy, but it works fine with multiple projects, as long as you're careful with their targets.



# Chasm.Compatibility

[![Latest NuGet version](https://img.shields.io/nuget/v/Chasm.Compatibility)](https://www.nuget.org/packages/Chasm.Compatibility/)
[![MIT License](https://img.shields.io/github/license/Chasmical/Chasm)](../LICENSE)

Provides a bunch of compatibility stuff, polyfills and shims.

Uses officially provided packages when possible, and uses its own when official ones aren't available.

`Chasm.Compatibility` extends support down to `netcoreapp1.0`, `netstandard1.0` and `net35`.



## DO NOT use this for other packages!

> [!CAUTION]
>
> But if, for some reason, you need to, **make sure you're targeting the same framework versions as this package!**  
> Otherwise, your users may end up with ambiguous type reference errors!
> 
> **For example:**  
> Your package targets Standard 2.0, and uses this package's target for Standard 2.0, which polyfills `AllowNullAttribute` (that was introduced in Standard 2.1). Then, the user of your package targets Standard 2.1 (which already has an `AllowNullAttribute`) and uses your library for Standard 2.0, which in turn references this package (with polyfilled `AllowNullAttribute`).



## How does this compare to PolySharp?

PolySharp only polyfills types that are actually used (which is a good thing) and puts them in the project's own DLL file. But it doesn't work well in multi-project setups, with `<InternalsVisibleTo>` exposing one project's PolySharp generated files to another, causing ambiguous type errors.

Chasm.Compatibility, on the other hand, includes all of the types in a separate DLL. Nothing fancy, but it works fine with multiple projects, as long as you're careful with their targets.



## Compatibility Table

<table>
  <thead>
    <tr>
      <th rowspan="2">Features</th>
      <th rowspan="2">Compatibility type</th>
      <th colspan="3">Framework support</th>
    </tr>
    <tr>
      <th>.NET / .NET Core</th>
      <th>.NET Standard</th>
      <th>.NET Framework</th>
    </tr>
  </thead>
  <tbody>
    <!-- System.ValueTuple comparison -->
    <tr>
      <td rowspan="3">
        <code>System.ValueTuple</code>
      </td>
      <td>Core libraries</td>
      <td><code>netcoreapp2.0</code></td><td><code>netstandard2.0</code></td><td><code>net47</code></td>
    </tr>
    <tr>
      <td>Official packages</td>
      <td><code>netcoreapp1.0</code></td><td><code>netstandard1.0</code></td><td><code>net45</code></td>
    </tr>
    <tr>
      <td>Chasm.Compatibility</td>
      <td><code>netcoreapp1.0</code></td><td><code>netstandard1.0</code></td><td><code>net35</code></td>
    </tr>
    <!-- System.Index and System.Range comparison -->
    <tr><td colspan="5"></td></tr>
    <tr>
      <td rowspan="4">
        <code>System.Index</code> and <code>System.Range</code>
      </td>
      <td>Core libraries</td>
      <td><code>netcoreapp3.0</code></td><td><code>netstandard2.1</code></td><td>—</td>
    </tr>
    <tr>
      <td>Official packages</td>
      <td>—</td><td>—</td><td>—</td>
    </tr>
    <tr>
      <td>IndexRange package</td>
      <td><code>netcoreapp2.0</code></td><td><code>netstandard2.0</code></td><td><code>net35</code></td>
    </tr>
    <tr>
      <td>Chasm.Compatibility</td>
      <td><code>netcoreapp1.0</code></td><td><code>netstandard1.0</code></td><td><code>net35</code></td>
    </tr>
    <!-- System.Runtime.CompilerServices.Unsafe comparison -->
    <tr></tr> <!-- fix striped colors -->
    <tr><td colspan="5"></td></tr>
    <tr>
      <td rowspan="4">
        <code>System.Runtime.CompilerServices.Unsafe</code>
      </td>
      <td>Core libraries</td>
      <td><code>netcoreapp3.0</code></td><td>—</td><td>—</td>
    </tr>
    <tr>
      <td>Official packages (v6)</td>
      <td><code>netcoreapp2.0</code></td><td><code>netstandard2.0</code></td><td><code>net461</code></td>
    </tr>
    <tr>
      <td>Official packages (v5)</td>
      <td><code>netcoreapp1.0</code></td><td><code>netstandard1.0</code></td><td><code>net45</code></td>
    </tr>
    <tr>
      <td>Chasm.Compatibility</td>
      <td><code>netcoreapp1.0</code></td><td><code>netstandard1.0</code></td><td><code>net35</code></td>
    </tr>
    <!-- System.HashCode comparison -->
    <tr></tr> <!-- fix striped colors -->
    <tr><td colspan="5"></td></tr>
    <tr>
      <td rowspan="3">
        <code>System.HashCode</code>
      </td>
      <td>Core libraries</td>
      <td><code>netcoreapp2.1</code></td><td><code>netstandard2.1</code></td><td>—</td>
    </tr>
    <tr>
      <td>Official packages</td>
      <td><code>netcoreapp2.0</code></td><td><code>netstandard2.0</code></td><td><code>net461</code></td>
    </tr>
    <tr>
      <td>Chasm.Compatibility</td>
      <td><code>netcoreapp1.0</code></td><td><code>netstandard1.0</code></td><td><code>net35</code></td>
    </tr>
  </tbody>
</table>



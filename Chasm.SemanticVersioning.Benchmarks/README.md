
## Benchmarked libraries

- `Chasm` - [Chasm.SemanticVersioning](https://github.com/Chasmical/Chasm/tree/main/Chasm.SemanticVersioning#readme) ([NuGet](https://www.nuget.org/packages/Chasm.SemanticVersioning)), this project, v2.4.0 (Jun 2024);
- `McSherry` - [McSherry.SemanticVersioning](https://github.com/McSherry/McSherry.SemanticVersioning) ([NuGet](https://www.nuget.org/packages/McSherry.SemanticVersioning)) v1.4.1 (Jan 2021);
- `Reeve` - [SemanticVersioning](https://github.com/adamreeve/semver.net) ([NuGet](https://www.nuget.org/packages/SemanticVersioning)) v3.0.0-beta2 (Nov 2023);
- `Hauser` - [Semver](https://github.com/maxhauser/semver) ([NuGet](https://www.nuget.org/packages/Semver)) v3.0.0-beta.1 (Aug 2023);
- `NuGet` - [NuGet.Versioning](https://github.com/NuGet/NuGet.Client/tree/dev/src/NuGet.Core/NuGet.Versioning) ([NuGet](https://www.nuget.org/packages/NuGet.Versioning)) v6.11.0-preview.2 (Jun 2024);



## Benchmark results

```

BenchmarkDotNet v0.13.12, Windows 11 (10.0.22631.3672/23H2/2023Update/SunValley3)
AMD Ryzen 5 3500X, 1 CPU, 6 logical and 6 physical cores
.NET SDK 8.0.300
  [Host]     : .NET 8.0.5 (8.0.524.21615), X64 RyuJIT AVX2
  DefaultJob : .NET 8.0.5 (8.0.524.21615), X64 RyuJIT AVX2


```
| Method    | Categories | Mean       | Error    | StdDev   | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
|---------- |----------- |-----------:|---------:|---------:|------:|--------:|-------:|----------:|------------:|
| Chasm1    | Sample1    |   212.0 ns |  0.63 ns |  0.55 ns |  1.00 |    0.00 | 0.0381 |     320 B |        1.00 |
| McSherry1 | Sample1    | 1,345.3 ns | 18.12 ns | 16.95 ns |  6.34 |    0.08 | 0.3986 |    3344 B |       10.45 |
| Reeve1    | Sample1    | 3,023.1 ns | 12.68 ns | 10.59 ns | 14.26 |    0.06 | 0.6523 |    5464 B |       17.07 |
| Hauser1   | Sample1    | 1,795.7 ns | 15.12 ns | 13.40 ns |  8.47 |    0.07 | 0.3414 |    2864 B |        8.95 |
| NuGet1    | Sample1    |   753.7 ns | 10.62 ns |  9.42 ns |  3.56 |    0.04 | 0.1650 |    1384 B |        4.33 |
|           |            |            |          |          |       |         |        |           |             |
| Chasm2    | Sample2    |   782.3 ns | 11.32 ns | 10.59 ns |  1.00 |    0.00 | 0.1802 |    1512 B |        1.00 |
| McSherry2 | Sample2    | 2,691.2 ns | 32.99 ns | 27.55 ns |  3.44 |    0.04 | 0.6104 |    5136 B |        3.40 |
| Reeve2    | Sample2    | 4,671.6 ns | 53.50 ns | 47.42 ns |  5.98 |    0.07 | 1.0147 |    8536 B |        5.65 |
| Hauser2   | Sample2    | 3,217.7 ns | 17.85 ns | 15.83 ns |  4.12 |    0.07 | 0.6409 |    5368 B |        3.55 |
| NuGet2    | Sample2    | 1,198.9 ns | 17.78 ns | 14.85 ns |  1.53 |    0.02 | 0.3109 |    2616 B |        1.73 |
|           |            |            |          |          |       |         |        |           |             |
| Chasm3    | Sample3    | 1,596.9 ns | 28.44 ns | 25.21 ns |  1.00 |    0.00 | 0.3662 |    3064 B |        1.00 |
| McSherry3 | Sample3    | 4,587.4 ns | 71.39 ns | 59.61 ns |  2.87 |    0.06 | 0.9537 |    8032 B |        2.62 |
| Reeve3    | Sample3    | 5,836.4 ns | 69.77 ns | 61.85 ns |  3.66 |    0.07 | 1.1520 |    9672 B |        3.16 |
| Hauser3   | Sample3    | 5,100.3 ns | 54.82 ns | 48.60 ns |  3.19 |    0.06 | 0.9003 |    7592 B |        2.48 |
| NuGet3    | Sample3    | 1,897.6 ns | 11.14 ns |  9.30 ns |  1.19 |    0.02 | 0.4864 |    4072 B |        1.33 |



## Conclusion

`Chasm` outperforms all of the benchmarked alternatives. It's closely followed by `NuGet` (ratios: 1.19, 1.53, 3.56), then by `McSherry` (ratios: 2.87, 3.44, 6.34) and `Hauser` (ratios: 3.19, 4.12, 8.47). Additionally, `Chasm` allocates as little unnecessary memory as possible during parsing through the use of read-only spans, available on newer framework versions.



## Lowest targets comparison

| Library    | .NET Core       | .NET Standard       | .NET Framework         |
|------------|-----------------|---------------------|------------------------|
| McSherry   | .NET Core 1.0   | .NET Standard 1.0   | .NET Framework 4.5     |
| Chasm      | .NET Core 2.0   | .NET Standard 2.0   | .NET Framework 4.6.1   |
| Reeve      | *.NET Core 2.0* | .NET Standard 2.0   | *.NET Framework 4.6.1* |
| Hauser     | *.NET Core 2.0* | .NET Standard 2.0   | *.NET Framework 4.6.1* |
| NuGet      | *.NET Core 2.0* | .NET Standard 2.0   | *.NET Framework 4.6.1* |

- explicitly included target framework (in package)
- *compatible target framework* (through .NET Standard)



## Library size comparison

| Library    | Raw total |  Packaged |
|------------|----------:|----------:|
| Chasm      |  82.5 KiB |  31.4 KiB |
| McSherry   |  50.0 KiB |  19.8 KiB |
| Reeve      |  34.0 KiB |  14.1 KiB |
| Hauser     |  74.5 KiB |  28.4 KiB |
| NuGet      |  67.0 KiB |  33.4 KiB |

The size differences are negligible, they're all roughly the size of a low quality meme PNG.



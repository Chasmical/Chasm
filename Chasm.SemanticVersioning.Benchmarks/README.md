## Benchmarked libraries

- `Chasm` - [Chasm.SemanticVersioning](https://github.com/Chasmical/Chasm/tree/main/Chasm.SemanticVersioning#readme) ([NuGet](https://www.nuget.org/packages/Chasm.SemanticVersioning)), this project, v2.7.4 (Sep 2024);
- `McSherry` - [McSherry.SemanticVersioning](https://github.com/McSherry/McSherry.SemanticVersioning) ([NuGet](https://www.nuget.org/packages/McSherry.SemanticVersioning)) v1.4.1 (Jan 2021);
- `Reeve` - [SemanticVersioning](https://github.com/adamreeve/semver.net) ([NuGet](https://www.nuget.org/packages/SemanticVersioning)) v3.0.0-beta2 (Nov 2023);
- `Hauser` - [Semver](https://github.com/maxhauser/semver) ([NuGet](https://www.nuget.org/packages/Semver)) v3.0.0-beta.1 (Aug 2023);
- `NuGet` - [NuGet.Versioning](https://github.com/NuGet/NuGet.Client/tree/dev/src/NuGet.Core/NuGet.Versioning) ([NuGet](https://www.nuget.org/packages/NuGet.Versioning)) v6.11.0 (Aug 2024);



## Overview of all benchmarks

TODO



## Version parsing benchmarks

```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.22631.4169/23H2/2023Update/SunValley3)
AMD Ryzen 5 3500X, 1 CPU, 6 logical and 6 physical cores
.NET SDK 9.0.100-rc.1.24452.12
  [Host]     : .NET 8.0.8 (8.0.824.36612), X64 RyuJIT AVX2
  DefaultJob : .NET 8.0.8 (8.0.824.36612), X64 RyuJIT AVX2


```
| Method    | Categories | Mean       | Error    | StdDev   | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
|---------- |----------- |-----------:|---------:|---------:|------:|--------:|-------:|----------:|------------:|
| Chasm1    | Sample1    |   187.4 ns |  1.97 ns |  1.84 ns |  1.00 |    0.01 | 0.0381 |     320 B |        1.00 |
| McSherry1 | Sample1    | 1,289.2 ns |  8.40 ns |  7.86 ns |  6.88 |    0.08 | 0.3986 |    3344 B |       10.45 |
| Reeve1    | Sample1    | 2,918.7 ns | 13.02 ns | 12.18 ns | 15.58 |    0.16 | 0.6523 |    5464 B |       17.07 |
| Hauser1   | Sample1    | 1,755.9 ns | 16.91 ns | 14.12 ns |  9.37 |    0.12 | 0.3414 |    2864 B |        8.95 |
| NuGet1    | Sample1    |   685.1 ns |  3.27 ns |  3.06 ns |  3.66 |    0.04 | 0.1650 |    1384 B |        4.33 |
|           |            |            |          |          |       |         |        |           |             |
| Chasm2    | Sample2    |   640.8 ns |  4.19 ns |  3.92 ns |  1.00 |    0.01 | 0.1802 |    1512 B |        1.00 |
| McSherry2 | Sample2    | 2,558.0 ns | 20.87 ns | 19.52 ns |  3.99 |    0.04 | 0.6104 |    5136 B |        3.40 |
| Reeve2    | Sample2    | 4,462.9 ns | 21.47 ns | 20.08 ns |  6.97 |    0.05 | 1.0147 |    8536 B |        5.65 |
| Hauser2   | Sample2    | 3,005.5 ns | 12.79 ns | 11.96 ns |  4.69 |    0.03 | 0.6409 |    5368 B |        3.55 |
| NuGet2    | Sample2    | 1,158.3 ns | 12.68 ns | 11.86 ns |  1.81 |    0.02 | 0.3109 |    2616 B |        1.73 |
|           |            |            |          |          |       |         |        |           |             |
| Chasm3    | Sample3    | 1,090.6 ns |  8.96 ns |  6.99 ns |  1.00 |    0.01 | 0.3662 |    3064 B |        1.00 |
| McSherry3 | Sample3    | 4,389.9 ns | 35.66 ns | 33.36 ns |  4.03 |    0.04 | 0.9537 |    8032 B |        2.62 |
| Reeve3    | Sample3    | 5,500.0 ns | 14.61 ns | 12.95 ns |  5.04 |    0.03 | 1.1520 |    9672 B |        3.16 |
| Hauser3   | Sample3    | 4,086.5 ns | 21.08 ns | 19.72 ns |  3.75 |    0.03 | 0.9003 |    7592 B |        2.48 |
| NuGet3    | Sample3    | 1,920.9 ns | 15.01 ns | 14.04 ns |  1.76 |    0.02 | 0.4864 |    4072 B |        1.33 |

### Results

`Chasm` significantly outperforms all of the benchmarked alternatives. It's closely followed by `NuGet` (ratios: 1.76, 1.81, 3.66), then by `McSherry` (ratios: 4.03, 3.99, 6.88) and `Hauser` (ratios: 3.75, 4.69, 9.37). Additionally, `Chasm` allocates as little unnecessary memory as possible during parsing through the use of read-only spans, available on newer framework versions.



## Version formatting benchmarks

```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.22631.4169/23H2/2023Update/SunValley3)
AMD Ryzen 5 3500X, 1 CPU, 6 logical and 6 physical cores
.NET SDK 9.0.100-rc.1.24452.12
  [Host]     : .NET 8.0.8 (8.0.824.36612), X64 RyuJIT AVX2
  DefaultJob : .NET 8.0.8 (8.0.824.36612), X64 RyuJIT AVX2


```
| Method    | Categories | Mean         | Error      | StdDev     | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
|---------- |----------- |-------------:|-----------:|-----------:|------:|--------:|-------:|----------:|------------:|
| Chasm1    | Sample1    |   119.972 ns |  1.5447 ns |  1.3694 ns |  1.00 |    0.02 | 0.0248 |     208 B |        1.00 |
| McSherry1 | Sample1    |   441.908 ns |  4.9343 ns |  4.3741 ns |  3.68 |    0.05 | 0.1469 |    1232 B |        5.92 |
| Reeve1    | Sample1    |     7.492 ns |  0.0149 ns |  0.0132 ns |  0.06 |    0.00 |      - |         - |        0.00 |
| Hauser1   | Sample1    |   484.191 ns |  2.9618 ns |  2.7705 ns |  4.04 |    0.05 | 0.0954 |     800 B |        3.85 |
| NuGet1    | Sample1    |   379.446 ns |  1.9825 ns |  1.7575 ns |  3.16 |    0.04 | 0.0248 |     208 B |        1.00 |
|           |            |              |            |            |       |         |        |           |             |
| Chasm2    | Sample2    |   203.332 ns |  1.6338 ns |  1.5282 ns |  1.00 |    0.01 | 0.0439 |     368 B |        1.00 |
| McSherry2 | Sample2    |   903.403 ns |  6.7064 ns |  5.6002 ns |  4.44 |    0.04 | 0.2193 |    1840 B |        5.00 |
| Reeve2    | Sample2    |     7.440 ns |  0.0193 ns |  0.0171 ns |  0.04 |    0.00 |      - |         - |        0.00 |
| Hauser2   | Sample2    |   516.083 ns |  3.7395 ns |  3.4979 ns |  2.54 |    0.03 | 0.1297 |    1088 B |        2.96 |
| NuGet2    | Sample2    |   561.246 ns |  5.5950 ns |  5.2336 ns |  2.76 |    0.03 | 0.0715 |     600 B |        1.63 |
|           |            |              |            |            |       |         |        |           |             |
| Chasm3    | Sample3    |   294.951 ns |  5.7919 ns |  5.6885 ns |  1.00 |    0.03 | 0.0715 |     600 B |        1.00 |
| McSherry3 | Sample3    | 1,518.798 ns | 14.4458 ns | 12.8059 ns |  5.15 |    0.10 | 0.3281 |    2744 B |        4.57 |
| Reeve3    | Sample3    |     7.650 ns |  0.0440 ns |  0.0411 ns |  0.03 |    0.00 |      - |         - |        0.00 |
| Hauser3   | Sample3    |   576.248 ns | 11.4104 ns | 11.7177 ns |  1.95 |    0.05 | 0.1822 |    1528 B |        2.55 |
| NuGet3    | Sample3    |   579.960 ns |  9.3017 ns |  8.7008 ns |  1.97 |    0.05 | 0.0772 |     648 B |        1.08 |

### Results

`Reeve` stores the original string, resulting in a simple field read. Then, `Chasm` is in second, followed by `Hauser` (ratios: 1.95, 2.54, 4.04) and `NuGet` (ratios: 1.97, 2.76, 3.16). `McSherry` is far behind. Additionally, `Chasm` allocates as little unnecessary memory as possible; `NuGet` behaves similarly, but it's not ideal.



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
| Chasm      |  94.5 KiB |  36.3 KiB |
| McSherry   |  50.0 KiB |  19.8 KiB |
| Reeve      |  34.0 KiB |  14.1 KiB |
| Hauser     |  74.5 KiB |  28.4 KiB |
| NuGet      |  67.0 KiB |  33.4 KiB |

The size differences are negligible, they're all roughly the size of a low quality meme PNG.



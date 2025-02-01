## Benchmarked libraries

- `Chasm` - [Chasm.SemanticVersioning](https://github.com/Chasmical/Chasm/tree/main/Chasm.SemanticVersioning#readme) ([NuGet](https://www.nuget.org/packages/Chasm.SemanticVersioning)), this project, v2.8.1 (Feb 2025);
- `McSherry` - [McSherry.SemanticVersioning](https://github.com/McSherry/McSherry.SemanticVersioning) ([NuGet](https://www.nuget.org/packages/McSherry.SemanticVersioning)) v1.4.1 (Jan 2021);
- `Reeve` - [SemanticVersioning](https://github.com/adamreeve/semver.net) ([NuGet](https://www.nuget.org/packages/SemanticVersioning)) v3.0.0 (Jan 2025);
- `Hauser` - [Semver](https://github.com/maxhauser/semver) ([NuGet](https://www.nuget.org/packages/Semver)) v3.0.0 (Oct 2024);
- `NuGet` - [NuGet.Versioning](https://github.com/NuGet/NuGet.Client/tree/dev/src/NuGet.Core/NuGet.Versioning) ([NuGet](https://www.nuget.org/packages/NuGet.Versioning)) v6.12.1 (Nov 2024);

```
BenchmarkDotNet v0.14.0, Windows 11 (10.0.26100.3037)
AMD Ryzen 5 3500X, 1 CPU, 6 logical and 6 physical cores
.NET SDK 9.0.200-preview.0.25057.12
  [Host]     : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2
  DefaultJob : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2
```



## Version parsing benchmarks

| Method    | Categories | Mean       | Error     | StdDev    | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
|---------- |----------- |-----------:|----------:|----------:|------:|--------:|-------:|----------:|------------:|
| Chasm1    | Sample1    |   208.0 ns |   2.21 ns |   2.07 ns |  1.00 |    0.01 | 0.0381 |     320 B |        1.00 |
| McSherry1 | Sample1    | 1,231.9 ns |  22.82 ns |  46.10 ns |  5.92 |    0.23 | 0.3986 |    3344 B |       10.45 |
| Reeve1    | Sample1    | 2,859.8 ns |  53.82 ns |  47.71 ns | 13.75 |    0.26 | 0.6523 |    5464 B |       17.07 |
| Hauser1   | Sample1    | 1,827.4 ns |  26.52 ns |  24.81 ns |  8.79 |    0.14 | 0.1545 |    1304 B |        4.08 |
| NuGet1    | Sample1    |   766.6 ns |  15.12 ns |  18.00 ns |  3.69 |    0.09 | 0.1650 |    1384 B |        4.33 |
|           |            |            |           |           |       |         |        |           |             |
| Chasm2    | Sample2    |   572.9 ns |  11.02 ns |  17.48 ns |  1.00 |    0.04 | 0.1802 |    1512 B |        1.00 |
| McSherry2 | Sample2    | 2,154.9 ns |  26.13 ns |  24.44 ns |  3.76 |    0.12 | 0.5646 |    4736 B |        3.13 |
| Reeve2    | Sample2    | 4,253.1 ns |  46.94 ns |  43.91 ns |  7.43 |    0.23 | 0.9918 |    8312 B |        5.50 |
| Hauser2   | Sample2    | 3,288.7 ns |  31.66 ns |  29.61 ns |  5.75 |    0.18 | 0.3777 |    3184 B |        2.11 |
| NuGet2    | Sample2    | 1,321.5 ns |  25.62 ns |  25.16 ns |  2.31 |    0.08 | 0.3109 |    2616 B |        1.73 |
|           |            |            |           |           |       |         |        |           |             |
| Chasm3    | Sample3    | 1,146.4 ns |  18.61 ns |  16.50 ns |  1.00 |    0.02 | 0.3662 |    3064 B |        1.00 |
| McSherry3 | Sample3    | 3,866.3 ns |  71.89 ns |  67.25 ns |  3.37 |    0.07 | 0.8545 |    7168 B |        2.34 |
| Reeve3    | Sample3    | 5,087.8 ns | 100.74 ns | 123.72 ns |  4.44 |    0.12 | 1.1139 |    9360 B |        3.05 |
| Hauser3   | Sample3    | 4,410.9 ns |  86.47 ns | 118.36 ns |  3.85 |    0.11 | 0.6332 |    5304 B |        1.73 |
| NuGet3    | Sample3    | 2,023.3 ns |  40.39 ns |  90.34 ns |  1.77 |    0.08 | 0.4845 |    4072 B |        1.33 |

### Results

`Chasm` significantly outperforms all of the benchmarked alternatives. It's closely followed by `NuGet` (ratios: 1.77, 2.31, 3.69), then by `McSherry` (ratios: 3.37, 3.76, 5.92) and `Hauser` (ratios: 3.85, 5.75, 8.79). Additionally, `Chasm` allocates as little unnecessary memory as possible during parsing through the use of read-only spans, available on newer framework versions.



## Range parsing benchmarks

| Method    | Categories | Mean         | Error       | StdDev      | Ratio  | RatioSD | Gen0    | Allocated | Alloc Ratio |
|---------- |----------- |-------------:|------------:|------------:|-------:|--------:|--------:|----------:|------------:|
| Chasm1    | Sample1    |     520.6 ns |     7.33 ns |     6.50 ns |   1.00 |    0.02 |  0.2098 |   1.72 KB |        1.00 |
| McSherry1 | Sample1    |   3,450.2 ns |    32.57 ns |    30.47 ns |   6.63 |    0.10 |  0.8698 |   7.12 KB |        4.14 |
| Reeve1    | Sample1    |  71,406.0 ns |   589.03 ns |   550.98 ns | 137.17 |    2.00 |  9.2773 |  77.32 KB |       44.99 |
| Hauser1   | Sample1    |   1,799.5 ns |    11.57 ns |    10.82 ns |   3.46 |    0.05 |  0.2403 |   1.98 KB |        1.15 |
|           |            |              |             |             |        |         |         |           |             |
| Chasm2    | Sample2    |     558.1 ns |     5.80 ns |     5.43 ns |   1.00 |    0.01 |  0.2270 |   1.86 KB |        1.00 |
| McSherry2 | Sample2    |   4,341.9 ns |    10.56 ns |     8.82 ns |   7.78 |    0.08 |  1.1749 |   9.62 KB |        5.17 |
| Reeve2    | Sample2    |  82,737.1 ns |   513.85 ns |   455.51 ns | 148.27 |    1.61 | 11.2305 |  92.38 KB |       49.68 |
| Hauser2   | Sample2    |   2,626.0 ns |    15.04 ns |    11.74 ns |   4.71 |    0.05 |  0.3090 |   2.53 KB |        1.36 |
|           |            |              |             |             |        |         |         |           |             |
| Chasm3    | Sample3    |     763.6 ns |     5.02 ns |     4.69 ns |   1.00 |    0.01 |  0.3109 |   2.55 KB |        1.00 |
| McSherry3 | Sample3    |   6,415.8 ns |    15.51 ns |    14.51 ns |   8.40 |    0.05 |  1.7471 |  14.29 KB |        5.61 |
| Reeve3    | Sample3    | 150,100.6 ns | 1,006.82 ns |   941.78 ns | 196.57 |    1.68 | 20.0195 | 164.57 KB |       64.62 |
| Hauser3   | Sample3    |   4,466.0 ns |    22.92 ns |    21.44 ns |   5.85 |    0.04 |  0.4501 |    3.7 KB |        1.45 |
|           |            |              |             |             |        |         |         |           |             |
| Chasm4    | Sample4    |   2,096.0 ns |    19.48 ns |    17.27 ns |   1.00 |    0.01 |  0.7629 |   6.26 KB |        1.00 |
| McSherry4 | Sample4    |  16,489.3 ns |   148.90 ns |   139.28 ns |   7.87 |    0.09 |  4.4556 |   36.6 KB |        5.85 |
| Reeve4    | Sample4    | 351,562.4 ns | 2,418.91 ns | 2,019.90 ns | 167.74 |    1.64 | 44.9219 | 377.77 KB |       60.37 |
| Hauser4   | Sample4    |  12,782.5 ns |    80.67 ns |    75.46 ns |   6.10 |    0.06 |  1.1902 |   9.82 KB |        1.57 |

### Results

Firstly, some libraries don't fully support `node-semver`'s syntax. `McSherry` doesn't recognize wildcards (e.g. `=1.2.x`, `1.2 - 3.4`), and both `Hauser` and `Reeve` don't parse hyphen ranges in complex sets (e.g. `>1.2.0 1.5 - 1.7`). `Chasm` though, as far as I'm aware, follows `node-semver`'s specifications exactly.

`Chasm` outperforms other libraries once again, with `Hauser` (ratios: 3.46, 4.71, 5.85, 6.10) and `McSherry` (ratios: 6.63, 7.78, 8.40, 7.87) pretty far behind. `Reeve`'s performance is particularly bad, with ratios: 137, 148, 196, 167. Both `Chasm` and `Hauser` make little allocations, with `Hauser` allocating slightly more.



## Version formatting benchmarks

| Method    | Categories | Mean         | Error      | StdDev     | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
|---------- |----------- |-------------:|-----------:|-----------:|------:|--------:|-------:|----------:|------------:|
| Chasm1    | Sample1    |   120.782 ns |  1.0064 ns |  0.9413 ns |  1.00 |    0.01 | 0.0248 |     208 B |        1.00 |
| McSherry1 | Sample1    |   385.063 ns |  4.6027 ns |  4.3054 ns |  3.19 |    0.04 | 0.1469 |    1232 B |        5.92 |
| Reeve1    | Sample1    |     7.424 ns |  0.0206 ns |  0.0193 ns |  0.06 |    0.00 |      - |         - |        0.00 |
| Hauser1   | Sample1    |   518.714 ns |  3.0363 ns |  2.8401 ns |  4.29 |    0.04 | 0.0954 |     800 B |        3.85 |
| NuGet1    | Sample1    |   278.223 ns |  0.9988 ns |  0.8854 ns |  2.30 |    0.02 | 0.0248 |     208 B |        1.00 |
|           |            |              |            |            |       |         |        |           |             |
| Chasm2    | Sample2    |   176.107 ns |  3.5648 ns |  6.1491 ns |  1.00 |    0.05 | 0.0439 |     368 B |        1.00 |
| McSherry2 | Sample2    |   769.424 ns | 15.1324 ns | 24.0017 ns |  4.37 |    0.20 | 0.2193 |    1840 B |        5.00 |
| Reeve2    | Sample2    |     7.555 ns |  0.0440 ns |  0.0412 ns |  0.04 |    0.00 |      - |         - |        0.00 |
| Hauser2   | Sample2    |   551.615 ns |  6.2294 ns |  5.8269 ns |  3.14 |    0.11 | 0.1297 |    1088 B |        2.96 |
| NuGet2    | Sample2    |   424.241 ns |  8.2521 ns |  7.7190 ns |  2.41 |    0.09 | 0.0715 |     600 B |        1.63 |
|           |            |              |            |            |       |         |        |           |             |
| Chasm3    | Sample3    |   300.364 ns |  4.6458 ns |  4.3456 ns |  1.00 |    0.02 | 0.0715 |     600 B |        1.00 |
| McSherry3 | Sample3    | 1,252.509 ns |  5.8264 ns |  5.1649 ns |  4.17 |    0.06 | 0.3281 |    2744 B |        4.57 |
| Reeve3    | Sample3    |     7.534 ns |  0.0448 ns |  0.0419 ns |  0.03 |    0.00 |      - |         - |        0.00 |
| Hauser3   | Sample3    |   583.903 ns |  6.7078 ns |  5.9463 ns |  1.94 |    0.03 | 0.1822 |    1528 B |        2.55 |
| NuGet3    | Sample3    |   446.683 ns |  8.5906 ns |  8.0356 ns |  1.49 |    0.03 | 0.0772 |     648 B |        1.08 |

### Results

`Reeve` stores the original string, resulting in a simple field read. Then, `Chasm` is in second, followed by `NuGet` (ratios: 1.49, 2.41, 2.30) and `Hauser` (ratios: 1.94, 3.14, 4.29). `McSherry` is far behind. Additionally, `Chasm` allocates as little unnecessary memory as possible; `NuGet` behaves similarly, but it's not ideal.



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
| Chasm      |  96.0 KiB |  37.7 KiB |
| McSherry   |  50.0 KiB |  19.8 KiB |
| Reeve      |  34.0 KiB |  13.8 KiB |
| Hauser     |  71.0 KiB |  27.3 KiB |
| NuGet      |  62.6 KiB |  31.3 KiB |

The size differences are negligible, they're all roughly the size of a low quality meme PNG.



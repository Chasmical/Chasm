using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
// ReSharper disable ReturnValueOfPureMethodIsNotUsed

namespace Chasm.SemanticVersioning.Benchmarks
{
    [MemoryDiagnoser]
    [GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByMethod)]
    [SimpleJob(RuntimeMoniker.NetCoreApp31)]
    [SimpleJob(RuntimeMoniker.Net50)]
    [SimpleJob(RuntimeMoniker.Net60)]
    [SimpleJob(RuntimeMoniker.Net70)]
    [SimpleJob(RuntimeMoniker.Net80)]
    public class SpanBuilderVsStringBuilderBenchmarks
    {
        public SemanticVersion Sample1 = new(1, 2, 3);
        public SemanticVersion Sample2 = new(1, 20, 34, [4]);
        public SemanticVersion Sample3 = new(1, 17, 20, ["alpha", 125]);
        public SemanticVersion Sample4 = new(120, 3392, 2011, ["alpha", 7, "beta", 448]);
        public SemanticVersion Sample5 = new(1128504242, 391237410, 289013141, ["alpha", 2323787, "beta", 4122248], ["dev", "007-BUILD"]);

        [Benchmark, BenchmarkCategory("SpanBuilder")]
        public void SpanBuilder1() { for (int i = 0; i < 1000; i++) Sample1.ToString(); }
        [Benchmark, BenchmarkCategory("StringBuilder")]
        public void StringBuilder1() { for (int i = 0; i < 1000; i++) Sample1.ToStringWithStringBuilder(); }
        [Benchmark, BenchmarkCategory("SpanBuilder")]
        public void SpanBuilder2() { for (int i = 0; i < 1000; i++) Sample2.ToString(); }
        [Benchmark, BenchmarkCategory("StringBuilder")]
        public void StringBuilder2() { for (int i = 0; i < 1000; i++) Sample2.ToStringWithStringBuilder(); }
        [Benchmark, BenchmarkCategory("SpanBuilder")]
        public void SpanBuilder3() { for (int i = 0; i < 1000; i++) Sample3.ToString(); }
        [Benchmark, BenchmarkCategory("StringBuilder")]
        public void StringBuilder3() { for (int i = 0; i < 1000; i++) Sample3.ToStringWithStringBuilder(); }
        [Benchmark, BenchmarkCategory("SpanBuilder")]
        public void SpanBuilder4() { for (int i = 0; i < 1000; i++) Sample4.ToString(); }
        [Benchmark, BenchmarkCategory("StringBuilder")]
        public void StringBuilder4() { for (int i = 0; i < 1000; i++) Sample4.ToStringWithStringBuilder(); }
        [Benchmark, BenchmarkCategory("SpanBuilder")]
        public void SpanBuilder5() { for (int i = 0; i < 1000; i++) Sample5.ToString(); }
        [Benchmark, BenchmarkCategory("StringBuilder")]
        public void StringBuilder5() { for (int i = 0; i < 1000; i++) Sample5.ToStringWithStringBuilder(); }

    }
}

/*

```

BenchmarkDotNet v0.13.11, Windows 11 (10.0.22631.2861/23H2/2023Update/SunValley3)
AMD Ryzen 5 3500X, 1 CPU, 6 logical and 6 physical cores
.NET SDK 8.0.100
  [Host]        : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
  .NET 5.0      : .NET 5.0.17 (5.0.1722.21314), X64 RyuJIT AVX2
  .NET 6.0      : .NET 6.0.25 (6.0.2523.51912), X64 RyuJIT AVX2
  .NET 7.0      : .NET 7.0.14 (7.0.1423.51910), X64 RyuJIT AVX2
  .NET 8.0      : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
  .NET Core 3.1 : .NET Core 3.1.32 (CoreCLR 4.700.22.55902, CoreFX 4.700.22.56512), X64 RyuJIT AVX2


```
| Method         | Job           | Runtime       | Mean      | Error    | StdDev   | Gen0     | Allocated |
|--------------- |-------------- |-------------- |----------:|---------:|---------:|---------:|----------:|
| SpanBuilder1   | .NET 5.0      | .NET 5.0      |  25.28 μs | 0.091 μs | 0.085 μs |   3.8147 |  31.25 KB |
| SpanBuilder1   | .NET 6.0      | .NET 6.0      |  37.14 μs | 0.730 μs | 1.278 μs |   3.8147 |  31.25 KB |
| SpanBuilder1   | .NET 7.0      | .NET 7.0      |  20.17 μs | 0.098 μs | 0.092 μs |   3.8147 |  31.25 KB |
| SpanBuilder1   | .NET 8.0      | .NET 8.0      |  17.61 μs | 0.073 μs | 0.064 μs |   3.8147 |  31.25 KB |
| SpanBuilder1   | .NET Core 3.1 | .NET Core 3.1 |  29.85 μs | 0.298 μs | 0.279 μs |   3.8147 |  31.25 KB |
|                |               |               |           |          |          |          |           |
| StringBuilder1 | .NET 5.0      | .NET 5.0      |  38.94 μs | 0.260 μs | 0.244 μs |  16.2354 | 132.81 KB |
| StringBuilder1 | .NET 6.0      | .NET 6.0      |  35.34 μs | 0.217 μs | 0.192 μs |  16.2354 | 132.81 KB |
| StringBuilder1 | .NET 7.0      | .NET 7.0      |  31.32 μs | 0.170 μs | 0.151 μs |  16.2354 | 132.81 KB |
| StringBuilder1 | .NET 8.0      | .NET 8.0      |  27.76 μs | 0.377 μs | 0.353 μs |  16.2354 | 132.81 KB |
| StringBuilder1 | .NET Core 3.1 | .NET Core 3.1 |  79.99 μs | 0.330 μs | 0.275 μs |  16.2354 | 132.81 KB |
|                |               |               |           |          |          |          |           |
| SpanBuilder2   | .NET 5.0      | .NET 5.0      |  34.02 μs | 0.196 μs | 0.184 μs |   4.7607 |  39.06 KB |
| SpanBuilder2   | .NET 6.0      | .NET 6.0      |  28.05 μs | 0.428 μs | 0.401 μs |   4.7607 |  39.06 KB |
| SpanBuilder2   | .NET 7.0      | .NET 7.0      |  28.43 μs | 0.498 μs | 0.466 μs |   4.7607 |  39.06 KB |
| SpanBuilder2   | .NET 8.0      | .NET 8.0      |  22.83 μs | 0.208 μs | 0.194 μs |   4.7607 |  39.06 KB |
| SpanBuilder2   | .NET Core 3.1 | .NET Core 3.1 |  37.58 μs | 0.455 μs | 0.425 μs |   4.7607 |  39.06 KB |
|                |               |               |           |          |          |          |           |
| StringBuilder2 | .NET 5.0      | .NET 5.0      |  64.35 μs | 0.900 μs | 0.842 μs |  20.9961 | 171.88 KB |
| StringBuilder2 | .NET 6.0      | .NET 6.0      |  48.52 μs | 0.674 μs | 0.630 μs |  20.9961 | 171.88 KB |
| StringBuilder2 | .NET 7.0      | .NET 7.0      |  45.06 μs | 0.765 μs | 0.639 μs |  20.9961 | 171.88 KB |
| StringBuilder2 | .NET 8.0      | .NET 8.0      |  38.20 μs | 0.482 μs | 0.451 μs |  20.9961 | 171.88 KB |
| StringBuilder2 | .NET Core 3.1 | .NET Core 3.1 | 109.60 μs | 0.433 μs | 0.383 μs |  20.9961 | 171.88 KB |
|                |               |               |           |          |          |          |           |
| SpanBuilder3   | .NET 5.0      | .NET 5.0      |  45.82 μs | 0.486 μs | 0.455 μs |   6.6528 |  54.69 KB |
| SpanBuilder3   | .NET 6.0      | .NET 6.0      |  42.70 μs | 0.523 μs | 0.489 μs |   6.6528 |  54.69 KB |
| SpanBuilder3   | .NET 7.0      | .NET 7.0      |  36.69 μs | 0.701 μs | 0.808 μs |   6.6528 |  54.69 KB |
| SpanBuilder3   | .NET 8.0      | .NET 8.0      |  28.60 μs | 0.130 μs | 0.122 μs |   6.6833 |  54.69 KB |
| SpanBuilder3   | .NET Core 3.1 | .NET Core 3.1 |  46.79 μs | 0.636 μs | 0.595 μs |   6.6528 |  54.69 KB |
|                |               |               |           |          |          |          |           |
| StringBuilder3 | .NET 5.0      | .NET 5.0      | 126.55 μs | 0.917 μs | 0.857 μs |  42.9688 | 351.56 KB |
| StringBuilder3 | .NET 6.0      | .NET 6.0      | 105.57 μs | 1.249 μs | 1.168 μs |  42.9688 | 351.56 KB |
| StringBuilder3 | .NET 7.0      | .NET 7.0      |  87.12 μs | 1.237 μs | 1.157 μs |  42.9688 | 351.56 KB |
| StringBuilder3 | .NET 8.0      | .NET 8.0      |  65.79 μs | 0.394 μs | 0.369 μs |  39.1846 | 320.31 KB |
| StringBuilder3 | .NET Core 3.1 | .NET Core 3.1 | 170.50 μs | 1.338 μs | 1.186 μs |  42.9688 | 351.56 KB |
|                |               |               |           |          |          |          |           |
| SpanBuilder4   | .NET 5.0      | .NET 5.0      |  80.08 μs | 0.817 μs | 0.724 μs |  10.4980 |  85.94 KB |
| SpanBuilder4   | .NET 6.0      | .NET 6.0      |  65.08 μs | 0.960 μs | 0.898 μs |  10.4980 |  85.94 KB |
| SpanBuilder4   | .NET 7.0      | .NET 7.0      |  56.07 μs | 0.458 μs | 0.429 μs |  10.4980 |  85.94 KB |
| SpanBuilder4   | .NET 8.0      | .NET 8.0      |  44.14 μs | 0.176 μs | 0.147 μs |  10.4980 |  85.94 KB |
| SpanBuilder4   | .NET Core 3.1 | .NET Core 3.1 |  67.54 μs | 0.912 μs | 0.809 μs |  10.4980 |  85.94 KB |
|                |               |               |           |          |          |          |           |
| StringBuilder4 | .NET 5.0      | .NET 5.0      | 178.53 μs | 1.731 μs | 1.620 μs |  54.4434 | 445.31 KB |
| StringBuilder4 | .NET 6.0      | .NET 6.0      | 140.66 μs | 2.214 μs | 2.071 μs |  54.4434 | 445.31 KB |
| StringBuilder4 | .NET 7.0      | .NET 7.0      | 115.22 μs | 1.513 μs | 1.264 μs |  54.4434 | 445.31 KB |
| StringBuilder4 | .NET 8.0      | .NET 8.0      |  95.32 μs | 1.229 μs | 1.150 μs |  54.4434 | 445.31 KB |
| StringBuilder4 | .NET Core 3.1 | .NET Core 3.1 | 232.94 μs | 1.906 μs | 1.782 μs |  54.4434 | 445.31 KB |
|                |               |               |           |          |          |          |           |
| SpanBuilder5   | .NET 5.0      | .NET 5.0      | 107.81 μs | 0.531 μs | 0.471 μs |  20.0195 | 164.06 KB |
| SpanBuilder5   | .NET 6.0      | .NET 6.0      |  96.05 μs | 0.473 μs | 0.443 μs |  20.0195 | 164.06 KB |
| SpanBuilder5   | .NET 7.0      | .NET 7.0      |  92.48 μs | 1.014 μs | 0.949 μs |  20.0195 | 164.06 KB |
| SpanBuilder5   | .NET 8.0      | .NET 8.0      |  87.96 μs | 0.960 μs | 0.898 μs |  20.0195 | 164.06 KB |
| SpanBuilder5   | .NET Core 3.1 | .NET Core 3.1 | 115.45 μs | 0.365 μs | 0.323 μs |  20.0195 | 164.06 KB |
|                |               |               |           |          |          |          |           |
| StringBuilder5 | .NET 5.0      | .NET 5.0      | 309.37 μs | 1.968 μs | 1.745 μs | 114.7461 |  937.5 KB |
| StringBuilder5 | .NET 6.0      | .NET 6.0      | 277.80 μs | 1.866 μs | 1.746 μs | 114.7461 |  937.5 KB |
| StringBuilder5 | .NET 7.0      | .NET 7.0      | 220.81 μs | 0.828 μs | 0.775 μs | 114.7461 |  937.5 KB |
| StringBuilder5 | .NET 8.0      | .NET 8.0      | 199.47 μs | 2.116 μs | 1.875 μs | 114.7461 |  937.5 KB |
| StringBuilder5 | .NET Core 3.1 | .NET Core 3.1 | 381.01 μs | 1.979 μs | 1.851 μs | 114.7461 |  937.5 KB |

*/

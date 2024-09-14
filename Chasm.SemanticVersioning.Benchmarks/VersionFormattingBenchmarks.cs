// ReSharper disable IdentifierTypo
using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;

namespace Chasm.SemanticVersioning.Benchmarks
{
    using static VersionSamples; // See the samples here

    [MemoryDiagnoser, GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory), CategoriesColumn]
    public class VersionFormattingBenchmarks
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void Use<T>(T _) { }

        [Benchmark(Baseline = true), BenchmarkCategory(nameof(Sample1))]
        public void Chasm1() { foreach (var ver in ChasmSample1) Use(ver.ToString()); }
        [Benchmark(Baseline = true), BenchmarkCategory(nameof(Sample2))]
        public void Chasm2() { foreach (var ver in ChasmSample2) Use(ver.ToString()); }
        [Benchmark(Baseline = true), BenchmarkCategory(nameof(Sample3))]
        public void Chasm3() { foreach (var ver in ChasmSample3) Use(ver.ToString()); }

        [Benchmark, BenchmarkCategory(nameof(Sample1))]
        public void McSherry1() { foreach (var ver in McSherrySample1) Use(ver.ToString()); }
        [Benchmark, BenchmarkCategory(nameof(Sample2))]
        public void McSherry2() { foreach (var ver in McSherrySample2) Use(ver.ToString()); }
        [Benchmark, BenchmarkCategory(nameof(Sample3))]
        public void McSherry3() { foreach (var ver in McSherrySample3) Use(ver.ToString()); }

        [Benchmark, BenchmarkCategory(nameof(Sample1))]
        public void Reeve1() { foreach (var ver in ReeveSample1) Use(ver.ToString()); }
        [Benchmark, BenchmarkCategory(nameof(Sample2))]
        public void Reeve2() { foreach (var ver in ReeveSample2) Use(ver.ToString()); }
        [Benchmark, BenchmarkCategory(nameof(Sample3))]
        public void Reeve3() { foreach (var ver in ReeveSample3) Use(ver.ToString()); }

        [Benchmark, BenchmarkCategory(nameof(Sample1))]
        public void Hauser1() { foreach (var ver in HauserSample1) Use(ver.ToString()); }
        [Benchmark, BenchmarkCategory(nameof(Sample2))]
        public void Hauser2() { foreach (var ver in HauserSample2) Use(ver.ToString()); }
        [Benchmark, BenchmarkCategory(nameof(Sample3))]
        public void Hauser3() { foreach (var ver in HauserSample3) Use(ver.ToString()); }

        [Benchmark, BenchmarkCategory(nameof(Sample1))]
        public void NuGet1() { foreach (var ver in NuGetSample1) Use(ver.ToString()); }
        [Benchmark, BenchmarkCategory(nameof(Sample2))]
        public void NuGet2() { foreach (var ver in NuGetSample2) Use(ver.ToString()); }
        [Benchmark, BenchmarkCategory(nameof(Sample3))]
        public void NuGet3() { foreach (var ver in NuGetSample3) Use(ver.ToString()); }

    }
}

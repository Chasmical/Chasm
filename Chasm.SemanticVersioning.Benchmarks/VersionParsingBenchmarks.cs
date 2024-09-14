// ReSharper disable IdentifierTypo
using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;

namespace Chasm.SemanticVersioning.Benchmarks
{
    using static VersionSamples; // See the samples here

    [MemoryDiagnoser, GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory), CategoriesColumn]
    public class VersionParsingBenchmarks
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void Use<T>(T _) { }

        [Benchmark(Baseline = true), BenchmarkCategory(nameof(Sample1))]
        public void Chasm1() { foreach (string text in Sample1) Use(ChasmVersion.Parse(text)); }
        [Benchmark(Baseline = true), BenchmarkCategory(nameof(Sample2))]
        public void Chasm2() { foreach (string text in Sample2) Use(ChasmVersion.Parse(text)); }
        [Benchmark(Baseline = true), BenchmarkCategory(nameof(Sample3))]
        public void Chasm3() { foreach (string text in Sample3) Use(ChasmVersion.Parse(text)); }

        [Benchmark, BenchmarkCategory(nameof(Sample1))]
        public void McSherry1() { foreach (string text in Sample1) Use(McSherryVersion.Parse(text)); }
        [Benchmark, BenchmarkCategory(nameof(Sample2))]
        public void McSherry2() { foreach (string text in Sample2) Use(McSherryVersion.Parse(text)); }
        [Benchmark, BenchmarkCategory(nameof(Sample3))]
        public void McSherry3() { foreach (string text in Sample3) Use(McSherryVersion.Parse(text)); }

        [Benchmark, BenchmarkCategory(nameof(Sample1))]
        public void Reeve1() { foreach (string text in Sample1) Use(ReeveVersion.Parse(text)); }
        [Benchmark, BenchmarkCategory(nameof(Sample2))]
        public void Reeve2() { foreach (string text in Sample2) Use(ReeveVersion.Parse(text)); }
        [Benchmark, BenchmarkCategory(nameof(Sample3))]
        public void Reeve3() { foreach (string text in Sample3) Use(ReeveVersion.Parse(text)); }

        [Benchmark, BenchmarkCategory(nameof(Sample1))]
        public void Hauser1() { foreach (string text in Sample1) Use(HauserVersion.Parse(text, Semver.SemVersionStyles.Strict)); }
        [Benchmark, BenchmarkCategory(nameof(Sample2))]
        public void Hauser2() { foreach (string text in Sample2) Use(HauserVersion.Parse(text, Semver.SemVersionStyles.Strict)); }
        [Benchmark, BenchmarkCategory(nameof(Sample3))]
        public void Hauser3() { foreach (string text in Sample3) Use(HauserVersion.Parse(text, Semver.SemVersionStyles.Strict)); }

        [Benchmark, BenchmarkCategory(nameof(Sample1))]
        public void NuGet1() { foreach (string text in Sample1) Use(NuGetVersion.Parse(text)); }
        [Benchmark, BenchmarkCategory(nameof(Sample2))]
        public void NuGet2() { foreach (string text in Sample2) Use(NuGetVersion.Parse(text)); }
        [Benchmark, BenchmarkCategory(nameof(Sample3))]
        public void NuGet3() { foreach (string text in Sample3) Use(NuGetVersion.Parse(text)); }

    }
}

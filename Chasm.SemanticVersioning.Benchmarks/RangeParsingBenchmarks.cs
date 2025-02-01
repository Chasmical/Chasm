using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;

namespace Chasm.SemanticVersioning.Benchmarks
{
    using static RangeSamples;

    [MemoryDiagnoser, GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory), CategoriesColumn]
    public class RangeParsingBenchmarks
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void Use<T>(T _) { }

        // Since not all libraries support node-semver ranges fully, use simplified samples instead
        private static string[] Sample2 = SimplifiedSample2;
        private static string[] Sample3 = SimplifiedSample3;
        private static string[] Sample4 = SimplifiedSample4;

        [Benchmark(Baseline = true), BenchmarkCategory(nameof(Sample1))]
        public void Chasm1() { foreach (string text in Sample1) Use(ChasmRange.Parse(text)); }
        [Benchmark(Baseline = true), BenchmarkCategory(nameof(Sample2))]
        public void Chasm2() { foreach (string text in Sample2) Use(ChasmRange.Parse(text)); }
        [Benchmark(Baseline = true), BenchmarkCategory(nameof(Sample3))]
        public void Chasm3() { foreach (string text in Sample3) Use(ChasmRange.Parse(text)); }
        [Benchmark(Baseline = true), BenchmarkCategory(nameof(Sample4))]
        public void Chasm4() { foreach (string text in Sample4) Use(ChasmRange.Parse(text)); }

        [Benchmark, BenchmarkCategory(nameof(Sample1))]
        public void McSherry1() { foreach (string text in Sample1) Use(McSherryRange.Parse(text)); }
        [Benchmark, BenchmarkCategory(nameof(Sample2))]
        public void McSherry2() { foreach (string text in Sample2) Use(McSherryRange.Parse(text)); }
        [Benchmark, BenchmarkCategory(nameof(Sample3))]
        public void McSherry3() { foreach (string text in Sample3) Use(McSherryRange.Parse(text)); }
        [Benchmark, BenchmarkCategory(nameof(Sample4))]
        public void McSherry4() { foreach (string text in Sample4) Use(McSherryRange.Parse(text)); }

        [Benchmark, BenchmarkCategory(nameof(Sample1))]
        public void Reeve1() { foreach (string text in Sample1) Use(ReeveRange.Parse(text)); }
        [Benchmark, BenchmarkCategory(nameof(Sample2))]
        public void Reeve2() { foreach (string text in Sample2) Use(ReeveRange.Parse(text)); }
        [Benchmark, BenchmarkCategory(nameof(Sample3))]
        public void Reeve3() { foreach (string text in Sample3) Use(ReeveRange.Parse(text)); }
        [Benchmark, BenchmarkCategory(nameof(Sample4))]
        public void Reeve4() { foreach (string text in Sample4) Use(ReeveRange.Parse(text)); }

        [Benchmark, BenchmarkCategory(nameof(Sample1))]
        public void Hauser1() { foreach (string text in Sample1) Use(HauserRange.ParseNpm(text)); }
        [Benchmark, BenchmarkCategory(nameof(Sample2))]
        public void Hauser2() { foreach (string text in Sample2) Use(HauserRange.ParseNpm(text)); }
        [Benchmark, BenchmarkCategory(nameof(Sample3))]
        public void Hauser3() { foreach (string text in Sample3) Use(HauserRange.ParseNpm(text)); }
        [Benchmark, BenchmarkCategory(nameof(Sample4))]
        public void Hauser4() { foreach (string text in Sample4) Use(HauserRange.ParseNpm(text)); }

    }
}

// ReSharper disable IdentifierTypo
using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using ChasmVersion = Chasm.SemanticVersioning.SemanticVersion;
using McSherryVersion = McSherry.SemanticVersioning.SemanticVersion;
using ReeveVersion = SemanticVersioning.Version;
using HauserVersion = Semver.SemVersion;
using NuGetVersion = NuGet.Versioning.SemanticVersion;

namespace Chasm.SemanticVersioning.Benchmarks
{
    [MemoryDiagnoser, GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory), CategoriesColumn]
    public class VersionParsingBenchmarks
    {
        public static readonly string[] Sample1 =
        [
            "0.0.0",
            "1.0.0",
            "2.3.4",
            "12.34.56",
            "99999.333444.767676767",
        ];
        public static readonly string[] Sample2 =
        [
            "1.0.0-0",
            "1.2.3-alpha.5.test",
            "4.7.2-nightly-beta.542",
            "7.21.8-pre.2.beta.7",
            "1212122.43434.465643565-pre-beta-test.70.pre-alpha",
        ];
        public static readonly string[] Sample3 =
        [
            "0.0.1-pre.67+DEV.BUILD",
            "1.2.3-alpha.beta.2.theta.70+BUILD.METADATA.0090.DEV",
            "8.12.5-nightly-beta.7.pre+TEST-BUILD-NOT-FOR-USE",
            "67.2.50-beta-test.07t.5+DEV-TEST.05.03.2023",
            "12222223.5545454.7-alpha.34.beta.23+TEST.BUILD-METADATA.0123456789000.230",
        ];

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

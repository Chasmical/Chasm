using System.Reflection;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using Chasm.Utilities;

namespace Chasm.SemanticVersioning.Benchmarks
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            // Quickly test what semver range syntaxes are supported by libraries
            TestRangeParsingMethods();

            IConfig config = DefaultConfig.Instance;

            BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args, config);

            Console.ReadKey();
        }
        private static void TestRangeParsingMethods()
        {
            RangeParsingBenchmarks b = new();
            foreach (MethodInfo method in b.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance))
            {
                if (method.DeclaringType != b.GetType()) continue;
                if (Util.Catch(() => method.Invoke(b, [])) is { } ex)
                    Console.WriteLine($"{method.Name}: {ex.GetBaseException().Message}");
            }
        }
    }
}

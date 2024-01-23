using System;
using BenchmarkDotNet.Running;

namespace Chasm.SemanticVersioning.Benchmarks
{
    public static class Program
    {
        public static void Main()
        {
            BenchmarkRunner.Run<SpanBuilderVsStringBuilderBenchmarks>();

            Console.ReadKey();
        }
    }
}

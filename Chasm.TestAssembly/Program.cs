using System;

namespace Chasm.TestAssembly
{
    public class Program
    {
        private readonly string myProp = "434";
        public string MyProp => myProp;

        public string AutoProp { get; set; } = "323232333";
        public static string StaticAutoProp { get; set; } = "32323ddd2333";

        public static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            ThisIsALocalMethod();
            Console.ReadKey();

            static void ThisIsALocalMethod() { }
        }

        private struct PrivateStruct
        {
            public int Value { get; }
        }
    }
}

using System;
using Mono.Cecil;

namespace Chasm.AssemblyOptimizer
{
    public sealed class ObfuscateInternals : OptimizationAction
    {
        public override void Execute(OptimizationContext context)
        {
            if (!context.HasFlag("ObfuscateInternals")) return;

            int i = 0;
            foreach (TypeDefinition type in context.EnumerateTypes())
            {
                if (type.IsNotPublic)
                    type.Name = GetObfuscatedName(i++);

                int j = 0;
                foreach (FieldDefinition field in type.Fields)
                    if (field.IsPrivate || field.IsAssembly)
                        field.Name = GetObfuscatedName(j++);

                foreach (MethodDefinition method in type.Methods)
                    if (method.IsPrivate || method.IsAssembly)
                        method.Name = GetObfuscatedName(j++);

                foreach (TypeDefinition nestedType in type.NestedTypes)
                    if (nestedType.IsNestedPrivate || nestedType.IsNestedAssembly)
                        nestedType.Name = GetObfuscatedName(j++);

            }

        }

        private const string letters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private static readonly string[] obfuscatedNames = Array.ConvertAll(letters.ToCharArray(), static c => c.ToString());

        public static string GetObfuscatedName(int index)
            => obfuscatedNames[index];

    }
}

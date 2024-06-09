using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Mono.Cecil;

namespace Chasm.AssemblyOptimizer
{
    public class OptimizeTask : Task
    {
        [Required]
        public string AssemblyPath { get; set; } = null!;
        [Required]
        public string? FeatureFlags { get; set; }


        public override bool Execute()
        {
            List<string> flags = (FeatureFlags ?? "").Split([',', ';', ' '], StringSplitOptions.RemoveEmptyEntries)
                                                     .Select(static f => f.Trim().ToUpperInvariant()).ToList();

            Log.LogWarning($"Patching assembly: \"{AssemblyPath}\".");
            Log.LogWarning($"Patching flags: [{string.Join(", ", flags)}].");



            byte[] data = File.ReadAllBytes(AssemblyPath);
            AssemblyDefinition assembly = AssemblyDefinition.ReadAssembly(new MemoryStream(data));

            OptimizationContext context = new(assembly, flags);

            foreach (Type type in typeof(OptimizeTask).Assembly.GetTypes())
            {
                if (type.IsSubclassOf(typeof(OptimizationAction)))
                {
                    OptimizationAction action = (OptimizationAction)Activator.CreateInstance(type);
                    action.Execute(context);
                }
            }



            using (MemoryStream memory = new())
            {
                assembly.Write(memory);
                data = memory.ToArray();
            }
            File.WriteAllBytes(AssemblyPath, data);

            Log.LogWarning("Finished patching.");

            return true;
        }

    }
    public sealed class OptimizationContext(AssemblyDefinition assembly, IList<string> flags)
    {
        public AssemblyDefinition Assembly { get; } = assembly;

        public ReadOnlyCollection<string> Flags { get; } = new(flags);

        public bool HasFlag(string flag) => Flags.Contains(flag.ToUpperInvariant());

        public IEnumerable<TypeDefinition> EnumerateTypes() => Assembly.Modules.SelectMany(static m => m.Types);
        public IEnumerable<MethodDefinition> EnumerateMethods() => EnumerateTypes().SelectMany(static t => t.Methods);
        public IEnumerable<FieldDefinition> EnumerateFields() => EnumerateTypes().SelectMany(static t => t.Fields);

    }
    public abstract class OptimizationAction
    {
        public abstract void Execute(OptimizationContext context);
    }
}

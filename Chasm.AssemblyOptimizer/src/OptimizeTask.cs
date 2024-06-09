using System;
using System.Collections.Generic;
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
            string[] flags = (FeatureFlags ?? "").Split([',', ';', ' '], StringSplitOptions.RemoveEmptyEntries);

            Log.LogWarning($"Patching assembly: \"{AssemblyPath}\".");
            Log.LogWarning($"Patching flags: [{string.Join(", ", flags)}].");

            string newAssemblyPath = AssemblyPath + ".patched";
            try
            {
                using (FileStream fileStream = File.OpenRead(AssemblyPath))
                using (AssemblyDefinition assembly = AssemblyDefinition.ReadAssembly(fileStream))
                {
                    PrePackageAction[] actions = GetActions();

                    foreach (PrePackageAction action in actions)
                    {
                        string name = action.GetType().Name;
                        if (name.EndsWith("Action")) name = name.Substring(0, name.Length - "Action".Length);

                        if (flags.Contains(name, StringComparer.OrdinalIgnoreCase))
                        {
                            Log.LogWarning($"Applying action {name}");
                            action.Execute(assembly);
                        }
                    }

                    using (FileStream writeStream = File.Create(newAssemblyPath))
                        assembly.Write(writeStream);
                }

                File.Copy(newAssemblyPath, AssemblyPath, true);
            }
            finally
            {
                File.Delete(newAssemblyPath);
            }

            Log.LogWarning("Finished patching.");
            return true;
        }

        private PrePackageAction[] GetActions()
        {
            List<PrePackageAction> actions = [];

            foreach (Type actionType in GetType().Assembly.GetTypes())
                if (!actionType.IsAbstract && actionType.IsSubclassOf(typeof(PrePackageAction)))
                {
                    PrePackageAction action = (PrePackageAction)Activator.CreateInstance(actionType);
                    action.Log = Log;
                    actions.Add(action);
                }

            actions.Sort();
            return actions.ToArray();
        }

    }
    public abstract class PrePackageAction : IComparable<PrePackageAction>
    {
        public TaskLoggingHelper Log { get; internal set; } = null!;

        public virtual int ExecutionOrder => 1000;
        public abstract void Execute(AssemblyDefinition assembly);

        public int CompareTo(PrePackageAction other)
            => ExecutionOrder.CompareTo(other.ExecutionOrder);

    }
}

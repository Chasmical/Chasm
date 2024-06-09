using System;
using System.Linq;
using System.Text.RegularExpressions;
using Mono.Cecil;
using Mono.Collections.Generic;

namespace Chasm.AssemblyOptimizer
{
    public sealed class NormalizeNames : OptimizationAction
    {
        public override void Execute(OptimizationContext context)
        {
            if (!context.HasFlag("NormalizeNames")) return;

            foreach (FieldDefinition field in context.EnumerateFields())
                NormalizeField(field);

            foreach (MethodDefinition method in context.EnumerateMethods())
                NormalizeMethod(method);

        }

        private static readonly Regex autoPropertyFieldRegex = new Regex("^<([^>]*)>k__BackingField$");

        public static void NormalizeField(FieldDefinition field)
        {
            Match match = autoPropertyFieldRegex.Match(field.Name);
            if (match.Success)
            {
                // Replace "<AutoProp>k__BackingField" with "autoProp"
                string newName = match.Groups[1].Value;
                field.Name = char.ToLower(newName[0]) + newName.Substring(1);

                RemoveCompilerGenerated(field.CustomAttributes);
            }
            if (field.Name.StartsWith("_", StringComparison.Ordinal))
            {
                // Replace "_value" with "value"
                field.Name = field.Name.Substring(1);
            }
        }

        private static readonly Regex localStaticMethodRegex = new Regex(@"^<([^>]*)>g__([^|]*)\|\d*_\d*$");

        public static void NormalizeMethod(MethodDefinition method)
        {
            Match match = localStaticMethodRegex.Match(method.Name);
            if (match.Success)
            {
                // Replace "<SomeMethod>g__LocalStaticMethod|53_0" with "LocalStaticMethod"
                string newName = match.Groups[2].Value;
                if (method.DeclaringType.Methods.Any(m => m.Name == newName))
                {
                    int suffix = 0;
                    while (method.DeclaringType.Methods.Any(m => m.Name == newName + suffix))
                        suffix++;
                    newName += suffix;
                }
                method.Name = newName;

                RemoveCompilerGenerated(method.CustomAttributes);
            }
        }

        private static void RemoveCompilerGenerated(Collection<CustomAttribute> attributes)
        {
            const string attrName = "System.Runtime.CompilerServices.CompilerGeneratedAttribute";
            CustomAttribute? attr = attributes.FirstOrDefault(static a => a.AttributeType.FullName == attrName);
            attributes.Remove(attr);
        }

    }
}

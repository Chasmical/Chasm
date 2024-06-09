using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Mono.Cecil;

namespace Chasm.AssemblyOptimizer
{
    public sealed class NormalizeNamesAction : PrePackageAction
    {
        public override void Execute(AssemblyDefinition assembly)
        {
            foreach (TypeDefinition type in assembly.EnumerateAllTypes())
            {
                foreach (PropertyDefinition property in type.Properties)
                    ExecuteOnProperty(property);
                foreach (FieldDefinition field in type.Fields)
                    ExecuteOnField(field);
                foreach (MethodDefinition method in type.Methods)
                    ExecuteOnMethod(method);
            }
        }

        private void ExecuteOnProperty(PropertyDefinition property)
        {
            MethodDefinition? getter = property.GetMethod;
            MethodDefinition? setter = property.SetMethod;

            if (getter?.HasAttribute<CompilerGeneratedAttribute>() == true)
            {
                // Find a matching auto-generated field
                string fieldName = $"<{property.Name}>k__BackingField";
                FieldDefinition? field = property.DeclaringType.Fields.FirstOrDefault(f => f.Name == fieldName);

                if (field is not null)
                {
                    // Rename "<AutoProp>k__BackingField" to "autoProp"
                    string newName = property.Name;
                    field.Name = char.ToLower(newName[0]) + newName.Substring(1);

                    // Remove [CompilerGenerated] attribute from the field, getter and setter
                    field.RemoveAttribute<CompilerGeneratedAttribute>();
                    getter.RemoveAttribute<CompilerGeneratedAttribute>();
                    setter?.RemoveAttribute<CompilerGeneratedAttribute>();
                }
                else
                {
                    Log.LogWarning($"Could not find the field for auto-property \"{property.FullName}\"");
                }

            }
        }

        private static void ExecuteOnField(FieldDefinition field)
        {
            if (field.Name.StartsWith("_", StringComparison.Ordinal))
            {
                // Rename "_value" to "value"
                field.Name = field.Name.Substring(1);
            }
        }

        private static readonly Regex localStaticMethodRegex = new Regex(@"^<([^>]+)>g__([^|]+)\|\d+_\d+$");

        private static void ExecuteOnMethod(MethodDefinition method)
        {
            Match match = localStaticMethodRegex.Match(method.Name);
            if (match.Success)
            {
                // Rename "<SomeMethod>g__LocalStaticMethod|53_0" to "LocalStaticMethod"
                method.Name = match.Groups[2].Value;

                // Remove [CompilerGenerated] attribute from the method
                method.RemoveAttribute<CompilerGeneratedAttribute>();
            }
        }

    }
}

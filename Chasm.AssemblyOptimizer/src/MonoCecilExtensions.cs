using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Collections.Generic;

namespace Chasm.AssemblyOptimizer
{
    internal static class MonoCecilExtensions
    {
        public static IEnumerable<TypeDefinition> EnumerateAllTypes(this AssemblyDefinition assembly)
            => assembly.Modules.SelectMany(m => EnumerateRecursive(m.Types));

        private static IEnumerable<TypeDefinition> EnumerateRecursive(Collection<TypeDefinition> types)
        {
            foreach (TypeDefinition type in types)
            {
                yield return type;
                if (!type.HasNestedTypes) continue;

                foreach (TypeDefinition nestedType in EnumerateRecursive(type.NestedTypes))
                    yield return nestedType;
            }
        }

        public static CustomAttribute? GetAttribute<T>(this IMemberDefinition member) where T : Attribute
        {
            string attrName = typeof(T).FullName!;
            return member.CustomAttributes.FirstOrDefault(a => a.AttributeType.FullName == attrName);
        }
        public static bool HasAttribute<T>(this IMemberDefinition member) where T : Attribute
            => GetAttribute<T>(member) is not null;
        public static bool RemoveAttribute<T>(this IMemberDefinition member) where T : Attribute
            => member.CustomAttributes.Remove(GetAttribute<T>(member));

    }
}

﻿#if NET5_0_OR_GREATER
using System.Runtime.CompilerServices;
[assembly: TypeForwardedTo(typeof(System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes))]
#else
// ReSharper disable once CheckNamespace
namespace System.Diagnostics.CodeAnalysis
{
    [Flags] public enum DynamicallyAccessedMemberTypes
    {
        None = 0,
        PublicParameterlessConstructor = 0x0001,
        PublicConstructors = 0x0002 | PublicParameterlessConstructor,
        NonPublicConstructors = 0x0004,
        PublicMethods = 0x0008,
        NonPublicMethods = 0x0010,
        PublicFields = 0x0020,
        NonPublicFields = 0x0040,
        PublicNestedTypes = 0x0080,
        NonPublicNestedTypes = 0x0100,
        PublicProperties = 0x0200,
        NonPublicProperties = 0x0400,
        PublicEvents = 0x0800,
        NonPublicEvents = 0x1000,
        Interfaces = 0x2000,
        NonPublicConstructorsWithInherited = NonPublicConstructors | 0x4000,
        NonPublicMethodsWithInherited = NonPublicMethods | 0x8000,
        NonPublicFieldsWithInherited = NonPublicFields | 0x10000,
        NonPublicNestedTypesWithInherited = NonPublicNestedTypes | 0x20000,
        NonPublicPropertiesWithInherited = NonPublicProperties | 0x40000,
        NonPublicEventsWithInherited = NonPublicEvents | 0x80000,
        PublicConstructorsWithInherited = PublicConstructors | 0x100000,
        PublicNestedTypesWithInherited = PublicNestedTypes | 0x200000,
        AllConstructors = PublicConstructorsWithInherited | NonPublicConstructorsWithInherited,
        AllMethods = PublicMethods | NonPublicMethodsWithInherited,
        AllFields = PublicFields | NonPublicFieldsWithInherited,
        AllNestedTypes = PublicNestedTypesWithInherited | NonPublicNestedTypesWithInherited,
        AllProperties = PublicProperties | NonPublicPropertiesWithInherited,
        AllEvents = PublicEvents | NonPublicEventsWithInherited,
        All = ~None
    }
}
#endif

﻿#if NET5_0_OR_GREATER
using System.Runtime.CompilerServices;
[assembly: TypeForwardedTo(typeof(SkipLocalsInitAttribute))]
#else
// ReSharper disable once CheckNamespace
namespace System.Runtime.CompilerServices
{
    [AttributeUsage(AttributeTargets.Module
                  | AttributeTargets.Class
                  | AttributeTargets.Struct
                  | AttributeTargets.Interface
                  | AttributeTargets.Constructor
                  | AttributeTargets.Method
                  | AttributeTargets.Property
                  | AttributeTargets.Event, Inherited = false)]
    public sealed class SkipLocalsInitAttribute : Attribute;
}
#endif

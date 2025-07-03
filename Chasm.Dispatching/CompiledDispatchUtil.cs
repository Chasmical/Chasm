using System;
using System.Reflection;

namespace Chasm.Dispatching
{
    internal static class CompiledDispatchUtil
    {
        public static void ValidateMethod(object? target, MethodInfo method, Type argType)
        {
            if (!IsCompatible(target, method, argType))
                ThrowNotCompatible(method, nameof(method));
        }
        private static bool IsCompatible(object? target, MethodInfo method, Type argType)
        {
            ParameterInfo[] pars = method.GetParameters();
            Type? instancePar = null, argumentPar = null;

            if (method.IsStatic)
            {
                // Update() | Update(arg) | Update(instance, arg)
                if (pars.Length > 0)
                {
                    if (pars.Length == 1)
                        argumentPar = pars[0].ParameterType;
                    else
                    {
                        instancePar = pars[0].ParameterType;
                        argumentPar = pars[1].ParameterType;
                        if (pars.Length > 2) return false;
                    }
                }
            }
            else
            {
                // this.Update() | this.Update(arg)
                instancePar = method.DeclaringType!;
                if (pars.Length > 0)
                {
                    argumentPar = pars[0].ParameterType;
                    if (pars.Length > 1) return false;
                }
            }

            return (instancePar is null || !instancePar.IsValueType && instancePar.IsInstanceOfType(target)) &&
                   (argumentPar is null || argumentPar.IsAssignableFrom(argType));
        }
        private static void ThrowNotCompatible(MethodInfo method, string paramName)
        {
            string msg = $"Method's signature {method} is not compatible with CompiledDispatch.";
            throw new ArgumentException(msg, paramName);
        }

        public static MethodInfo FindMethod(Type type, object? target, string methodName, Type argType)
        {
            foreach (MethodInfo method in type.GetMethods())
                if (method.Name == methodName && IsCompatible(target, method, argType))
                    return method;

            throw new ArgumentException($"No compatible method {methodName} could be found in {type}.");
        }

    }
}

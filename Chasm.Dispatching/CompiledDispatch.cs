using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Chasm.Dispatching
{
    using Entry = CompiledDispatchImpl.Entry;

    public sealed class CompiledDispatch<TArg>
    {
        private CompiledDispatchImpl impl = new();
        private Action<Entry[], TArg>? _dispatch;

        public bool IsCompiled => _dispatch is not null;
        public int Count => impl._count;

        public void Add(object? element, MethodInfo method)
        {
            ANE.ThrowIfNull(method);
            CompiledDispatchUtil.ValidateMethod(element, method, typeof(TArg));

            impl.Add(element, method);
            _dispatch = null;
        }
        public void Add(object element, string methodName)
        {
            ANE.ThrowIfNull(element);
            ANE.ThrowIfNull(methodName);

            if (element is Type type) element = null!;
            else type = element.GetType();

            MethodInfo method = CompiledDispatchUtil.FindMethod(type, element, methodName, typeof(TArg));
            impl.Add(element, method);
            _dispatch = null;
        }

        public bool Remove(object? element, MethodInfo method)
        {
            bool res = impl.Remove(element, method);
            if (res) _dispatch = null;
            return res;
        }

        public void Add(Action<TArg> action)
        {
            ANE.ThrowIfNull(action);
            impl.Add(action.Target, action.Method);
            _dispatch = null;
        }
        public bool Remove(Action<TArg> action)
        {
            bool res = impl.Remove(action.Target, action.Method);
            if (res) _dispatch = null;
            return res;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void Compile()
        {
            if (_dispatch is not null) return;
            int version = 0;
            var dispatch = impl.CompileDispatch(typeof(Action<Entry[], TArg>), typeof(TArg), ref version, version)!;
            _dispatch = Unsafe.As<Delegate, Action<Entry[], TArg>>(ref dispatch);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispatch(TArg arg)
        {
            if (_dispatch is null) Compile();
            _dispatch!(impl._entries, arg);
        }
        public void Dispatch(TArg arg, bool forceCompile)
        {
            if (_dispatch is null)
            {
                if (!forceCompile)
                {
                    impl.DispatchNaiveDynamic([arg]);
                    return;
                }
                Compile();
            }
            _dispatch!(impl._entries, arg);
        }

    }
}

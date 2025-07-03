using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Chasm.Dispatching
{
    using Entry = CompiledDispatchImpl.Entry;

    /// <summary>
    ///   <para>Represents a collection of methods, that can be compiled and then statically dispatched sequentially.</para>
    /// </summary>
    /// <typeparam name="TArg">The dispatch parameter, received by the collection's methods.</typeparam>
    public sealed class CompiledDispatch<TArg>
    {
        private CompiledDispatchImpl impl = new();
        private Action<Entry[], TArg>? _dispatch;

        /// <summary>
        ///   <para>Determines whether the methods have been compiled into a delegate.</para>
        /// </summary>
        public bool IsCompiled => _dispatch is not null;
        /// <summary>
        ///   <para>Gets the number of methods contained in the <see cref="CompiledDispatch{TArg}"/>.</para>
        /// </summary>
        public int Count => impl._count;

        /// <summary>
        ///   <para>Adds the specified target-method pair to the end of the <see cref="CompiledDispatch{TArg}"/>.</para>
        /// </summary>
        /// <param name="target">The target that the method is invoked on. Can be <see langword="null"/>, if the method is static.</param>
        /// <param name="method">The method to be added to the end of the <see cref="CompiledDispatch{TArg}"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="method"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="method"/>'s signature is not compatible with the specified <paramref name="target"/>'s type or <typeparamref name="TArg"/> type.</exception>
        public void Add(object? target, MethodInfo method)
        {
            ANE.ThrowIfNull(method);
            CompiledDispatchUtil.ValidateMethod(target, method, typeof(TArg));

            impl.Add(target, method);
            _dispatch = null;
        }
        /// <summary>
        ///   <para>Adds the specified target-method pair to the end of the <see cref="CompiledDispatch{TArg}"/>.</para>
        /// </summary>
        /// <param name="target">The target that the method is invoked on.</param>
        /// <param name="methodName">The name of the method to be added to the end of the <see cref="CompiledDispatch{TArg}"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="target"/> or <paramref name="methodName"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">A method, that is compatible with the specified <paramref name="target"/>'s type and <typeparamref name="TArg"/> type, could not be found.</exception>
        public void Add(object target, string methodName)
        {
            ANE.ThrowIfNull(target);
            ANE.ThrowIfNull(methodName);

            if (target is Type type) target = null!;
            else type = target.GetType();

            MethodInfo method = CompiledDispatchUtil.FindMethod(type, target, methodName, typeof(TArg));
            impl.Add(target, method);
            _dispatch = null;
        }
        /// <summary>
        ///   <para>Adds the specified delegate to the end of the <see cref="CompiledDispatch{TArg}"/>.</para>
        /// </summary>
        /// <param name="action">The delegate to be added to the end of the <see cref="CompiledDispatch{TArg}"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null"/>.</exception>
        public void Add(Action<TArg> action)
        {
            ANE.ThrowIfNull(action);
            impl.Add(action.Target, action.Method);
            _dispatch = null;
        }

        /// <summary>
        ///   <para>Removes the specified target-method pair from the <see cref="CompiledDispatch{TArg}"/>.</para>
        /// </summary>
        /// <param name="target">The target that the method is invoked on. Can be <see langword="null"/> if the method is static.</param>
        /// <param name="method">The method to be removed from the <see cref="CompiledDispatch{TArg}"/>.</param>
        /// <returns><see langword="true"/>, if the specified target-method pair was successfully removed from the <see cref="CompiledDispatch{TArg}"/>; otherwise, <see langword="false"/>.</returns>
        public bool Remove(object? target, MethodInfo? method)
        {
            bool res = impl.Remove(target, method);
            if (res) _dispatch = null;
            return res;
        }
        /// <summary>
        ///   <para>Removes the specified delegate from the <see cref="CompiledDispatch{TArg}"/>.</para>
        /// </summary>
        /// <param name="action">The delegate to be removed from the <see cref="CompiledDispatch{TArg}"/>.</param>
        /// <returns><see langword="true"/>, if the specified delegate was successfully removed from the <see cref="CompiledDispatch{TArg}"/>; otherwise, <see langword="false"/>.</returns>
        public bool Remove(Action<TArg>? action)
        {
            if (action is null) return false;
            bool res = impl.Remove(action.Target, action.Method);
            if (res) _dispatch = null;
            return res;
        }

        /// <summary>
        ///   <para>Compiles the methods contained in the collection into a delegate.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void Compile()
        {
            if (_dispatch is not null) return;
            int version = 0;
            var dispatch = impl.CompileDispatch(typeof(Action<Entry[], TArg>), typeof(TArg), ref version, version)!;
            _dispatch = Unsafe.As<Delegate, Action<Entry[], TArg>>(ref dispatch);
        }

        /// <summary>
        ///   <para>Compiles and invokes all the methods contained in the collection with the specified <paramref name="arg"/>.</para>
        /// </summary>
        /// <param name="arg">The argument to invoke all the methods contained in the collection with.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispatch(TArg arg)
        {
            if (_dispatch is null) Compile();
            _dispatch!(impl._entries, arg);
        }
        /// <summary>
        ///   <para>Invokes all the methods contained in the collection with the specified <paramref name="arg"/>, optionally skipping compilation and falling back to unoptimized reflection calls.</para>
        /// </summary>
        /// <param name="arg">The argument to invoke all the methods contained in the collection with.</param>
        /// <param name="forceCompile">Determines whether the methods should be compiled before invocation.</param>
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

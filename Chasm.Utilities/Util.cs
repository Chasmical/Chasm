using System;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Chasm.Utilities
{
    /// <summary>
    ///   <para>Provides a set of static methods that can be used to shorten common branch code.</para>
    /// </summary>
    public static class Util
    {
        /// <summary>
        ///   <para>Sets <paramref name="result"/> to <see langword="default"/> and returns <see langword="false"/>.</para>
        /// </summary>
        /// <typeparam name="T">The type to set the default value of to the <paramref name="result"/> parameter.</typeparam>
        /// <param name="result">When this method returns, contains the default value of type <typeparamref name="T"/>.</param>
        /// <returns><see langword="false"/>.</returns>
        [Pure, MustUseReturnValue, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Fail<T>(out T? result)
        {
            result = default;
            return false;
        }
        /// <summary>
        ///   <para>Sets <paramref name="result1"/> and <paramref name="result2"/> to <see langword="default"/> and returns <see langword="false"/>.</para>
        /// </summary>
        /// <typeparam name="T1">The type to set the default value of to the <paramref name="result1"/> parameter.</typeparam>
        /// <typeparam name="T2">The type to set the default value of to the <paramref name="result2"/> parameter.</typeparam>
        /// <param name="result1">When this method returns, contains the default value of type <typeparamref name="T1"/>.</param>
        /// <param name="result2">When this method returns, contains the default value of type <typeparamref name="T2"/>.</param>
        /// <returns><see langword="false"/>.</returns>
        [Pure, MustUseReturnValue, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Fail<T1, T2>(out T1? result1, out T2? result2)
        {
            result1 = default;
            result2 = default;
            return false;
        }
        /// <summary>
        ///   <para>Sets <paramref name="result1"/>, <paramref name="result2"/> and <paramref name="result3"/> to <see langword="default"/> and returns <see langword="false"/>.</para>
        /// </summary>
        /// <typeparam name="T1">The type to set the default value of to the <paramref name="result1"/> parameter.</typeparam>
        /// <typeparam name="T2">The type to set the default value of to the <paramref name="result2"/> parameter.</typeparam>
        /// <typeparam name="T3">The type to set the default value of to the <paramref name="result3"/> parameter.</typeparam>
        /// <param name="result1">When this method returns, contains the default value of type <typeparamref name="T1"/>.</param>
        /// <param name="result2">When this method returns, contains the default value of type <typeparamref name="T2"/>.</param>
        /// <param name="result3">When this method returns, contains the default value of type <typeparamref name="T3"/>.</param>
        /// <returns><see langword="false"/>.</returns>
        [Pure, MustUseReturnValue, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Fail<T1, T2, T3>(out T1? result1, out T2? result2, out T3? result3)
        {
            result1 = default;
            result2 = default;
            result3 = default;
            return false;
        }

        /// <summary>
        ///   <para>Sets <paramref name="result"/> to <see langword="default"/> and returns the specified <paramref name="returnValue"/>.</para>
        /// </summary>
        /// <typeparam name="TReturn">The type of the return value.</typeparam>
        /// <typeparam name="T">The type to set the default value of to the <paramref name="result"/> parameter.</typeparam>
        /// <param name="returnValue">The value to return.</param>
        /// <param name="result">When this method returns, contains the default value of type <typeparamref name="T"/>.</param>
        /// <returns>The specified <paramref name="returnValue"/>.</returns>
        [Pure, MustUseReturnValue, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TReturn Fail<TReturn, T>(TReturn returnValue, out T? result)
        {
            result = default;
            return returnValue;
        }
        /// <summary>
        ///   <para>Sets <paramref name="result1"/> and <paramref name="result2"/> to <see langword="default"/> and returns the specified <paramref name="returnValue"/>.</para>
        /// </summary>
        /// <typeparam name="TReturn">The type of the return value.</typeparam>
        /// <typeparam name="T1">The type to set the default value of to the <paramref name="result1"/> parameter.</typeparam>
        /// <typeparam name="T2">The type to set the default value of to the <paramref name="result2"/> parameter.</typeparam>
        /// <param name="returnValue">The value to return.</param>
        /// <param name="result1">When this method returns, contains the default value of type <typeparamref name="T1"/>.</param>
        /// <param name="result2">When this method returns, contains the default value of type <typeparamref name="T2"/>.</param>
        /// <returns>The specified <paramref name="returnValue"/>.</returns>
        [Pure, MustUseReturnValue, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TReturn Fail<TReturn, T1, T2>(TReturn returnValue, out T1? result1, out T2? result2)
        {
            result1 = default;
            result2 = default;
            return returnValue;
        }
        /// <summary>
        ///   <para>Sets <paramref name="result1"/>, <paramref name="result2"/> and <paramref name="result3"/> to <see langword="default"/> and returns the specified <paramref name="returnValue"/>.</para>
        /// </summary>
        /// <typeparam name="TReturn">The type of the return value.</typeparam>
        /// <typeparam name="T1">The type to set the default value of to the <paramref name="result1"/> parameter.</typeparam>
        /// <typeparam name="T2">The type to set the default value of to the <paramref name="result2"/> parameter.</typeparam>
        /// <typeparam name="T3">The type to set the default value of to the <paramref name="result3"/> parameter.</typeparam>
        /// <param name="returnValue">The value to return.</param>
        /// <param name="result1">When this method returns, contains the default value of type <typeparamref name="T1"/>.</param>
        /// <param name="result2">When this method returns, contains the default value of type <typeparamref name="T2"/>.</param>
        /// <param name="result3">When this method returns, contains the default value of type <typeparamref name="T3"/>.</param>
        /// <returns>The specified <paramref name="returnValue"/>.</returns>
        [Pure, MustUseReturnValue, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TReturn Fail<TReturn, T1, T2, T3>(TReturn returnValue, out T1? result1, out T2? result2, out T3? result3)
        {
            result1 = default;
            result2 = default;
            result3 = default;
            return returnValue;
        }

        /// <summary>
        ///   <para>Catches and returns an exception, thrown by invoking the specified <paramref name="action"/>.</para>
        /// </summary>
        /// <param name="action">The delegate that can throw an exception.</param>
        /// <returns>The caught exception, if the specified <paramref name="action"/> caused one; otherwise, <see langword="null"/>.</returns>
        [Pure] public static Exception? Catch([InstantHandle] Action action)
            => Catch<Exception>(action);
        /// <summary>
        ///   <para>Catches and returns an exception of the <typeparamref name="TException"/> type, thrown by invoking the specified <paramref name="action"/>.</para>
        /// </summary>
        /// <typeparam name="TException">The type of the exception to catch.</typeparam>
        /// <param name="action">The delegate that can throw an exception of the <typeparamref name="TException"/> type.</param>
        /// <returns>The caught <typeparamref name="TException"/>, if the specified <paramref name="action"/> caused one; otherwise, <see langword="null"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null"/>.</exception>
        [Pure] public static TException? Catch<TException>([InstantHandle] Action action) where TException : Exception
        {
            if (action is null) throw new ArgumentNullException(nameof(action));
            try
            {
                action();
                return null;
            }
            catch (TException exception)
            {
                return exception;
            }
        }

        /// <summary>
        ///   <para>Catches and returns an exception, thrown by invoking the specified <paramref name="function"/>. If no exception is thrown, stores the result in the <paramref name="result"/> parameter.</para>
        /// </summary>
        /// <typeparam name="TResult">The return type of the delegate to invoke.</typeparam>
        /// <param name="function">The delegate that can throw an exception.</param>
        /// <param name="result">When this method returns, contains the return value of the specified <paramref name="function"/>, if no exception was thrown, or <see langword="default"/> otherwise.</param>
        /// <returns>The caught exception, if the specified <paramref name="function"/> caused one; otherwise, <see langword="null"/>.</returns>
        [Pure] public static Exception? Catch<TResult>([InstantHandle] Func<TResult> function, out TResult? result)
            => Catch<Exception, TResult>(function, out result);
        /// <summary>
        ///   <para>Catches and returns an exception of the <typeparamref name="TException"/> type, thrown by invoking the specified <paramref name="function"/>. If no exception is thrown, stores the result in the <paramref name="result"/> parameter.</para>
        /// </summary>
        /// <typeparam name="TException">The type of the exception to catch.</typeparam>
        /// <typeparam name="TResult">The return type of the delegate to invoke.</typeparam>
        /// <param name="function">The delegate that can throw an exception.</param>
        /// <param name="result">When this method returns, contains the return value of the specified <paramref name="function"/>, if no exception was thrown, or <see langword="default"/> otherwise.</param>
        /// <returns>The caught <typeparamref name="TException"/>, if the specified <paramref name="function"/> caused one; otherwise, <see langword="null"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="function"/> is <see langword="null"/>.</exception>
        [Pure] public static TException? Catch<TException, TResult>([InstantHandle] Func<TResult> function, out TResult? result) where TException : Exception
        {
            if (function is null) throw new ArgumentNullException(nameof(function));
            try
            {
                result = function();
                return null;
            }
            catch (TException exception)
            {
                result = default;
                return exception;
            }
        }

    }
}

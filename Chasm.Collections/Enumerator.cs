using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Chasm.Collections
{
    /// <summary>
    ///   <para>Provides static methods for getting instances of empty enumerators.</para>
    /// </summary>
    public static class Enumerator
    {
        /// <summary>
        ///   <para>Returns an empty enumerator.</para>
        /// </summary>
        /// <returns>An empty enumerator.</returns>
        [Pure] public static IEnumerator Empty()
            => EmptyEnumerator.Instance;
        /// <summary>
        ///   <para>Returns an empty enumerator.</para>
        /// </summary>
        /// <typeparam name="T">The type of the elements to enumerate.</typeparam>
        /// <returns>An empty enumerator.</returns>
        [Pure] public static IEnumerator<T> Empty<T>()
            => EmptyEnumerator<T>.Instance;

        private sealed class EmptyEnumerator : IEnumerator
        {
            public static readonly IEnumerator Instance = new EmptyEnumerator();

            private EmptyEnumerator() { }
            object? IEnumerator.Current => null;
            bool IEnumerator.MoveNext() => false;
            void IEnumerator.Reset() { }
        }
        private sealed class EmptyEnumerator<T> : IEnumerator<T>
        {
            public static readonly IEnumerator<T> Instance = new EmptyEnumerator<T>();

            private EmptyEnumerator() { }
            object? IEnumerator.Current => null;
            T IEnumerator<T>.Current => default!;
            bool IEnumerator.MoveNext() => false;
            void IEnumerator.Reset() { }
            void IDisposable.Dispose() { }
        }

#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        /// <summary>
        ///   <para>Returns an empty asynchronous enumerator.</para>
        /// </summary>
        /// <typeparam name="T">The type of the elements to enumerate.</typeparam>
        /// <returns>An empty asynchronous enumerator.</returns>
        [Pure] public static IAsyncEnumerator<T> EmptyAsync<T>()
            => EmptyAsyncEnumerator<T>.Instance;

        private sealed class EmptyAsyncEnumerator<T> : IAsyncEnumerator<T>
        {
            public static readonly IAsyncEnumerator<T> Instance = new EmptyAsyncEnumerator<T>();

            private EmptyAsyncEnumerator() { }
            T IAsyncEnumerator<T>.Current => default!;
            ValueTask IAsyncDisposable.DisposeAsync()
                => new ValueTask(Task.CompletedTask);
            ValueTask<bool> IAsyncEnumerator<T>.MoveNextAsync()
                => new ValueTask<bool>(false);
        }
#endif

    }
}

using System.Collections;
using System.Collections.Generic;
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
using System.Threading.Tasks;
#endif
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
            public object? Current => null;
            public bool MoveNext() => false;
            public void Reset() { }
        }
        private sealed class EmptyEnumerator<T> : IEnumerator<T>
        {
            public static readonly IEnumerator<T> Instance = new EmptyEnumerator<T>();

            private EmptyEnumerator() { }
            public T Current => default!;
            object? IEnumerator.Current => null;
            public bool MoveNext() => false;
            public void Reset() { }
            public void Dispose() { }
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
            public T Current => default!;
            public ValueTask DisposeAsync() => default; // equivalent to ValueTask.CompletedTask
            public ValueTask<bool> MoveNextAsync() => default; // equivalent to new ValueTask<bool>(false)
        }
#endif

    }
}

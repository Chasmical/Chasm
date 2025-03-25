using System;
using System.Runtime.CompilerServices;
using System.Threading;
using JetBrains.Annotations;

namespace Chasm.Utilities
{
    /// <summary>
    ///   <para>Represents a <see cref="IDisposable"/>, that invokes an action when disposed.</para>
    /// </summary>
    [MustDisposeResource]
    public class DelegateDisposable : IDisposable
    {
        private Action? disposeAction;

        /// <summary>
        ///   <para>Initializes a new instance of the <see cref="DelegateDisposable"/> class with the specified <paramref name="dispose"/> action.</para>
        /// </summary>
        /// <param name="dispose">The action to invoke when the <see cref="DelegateDisposable"/> is disposed.</param>
        public DelegateDisposable(Action dispose)
        {
            ANE.ThrowIfNull(dispose);
            disposeAction = dispose;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        /// <inheritdoc cref="Dispose()"/>
        /// <param name="disposing"><see langword="true"/> to release both managed and unmanaged resources; <see langword="false"/> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing) Interlocked.Exchange(ref disposeAction, null)?.Invoke();
        }

        /// <summary>
        ///   <para>Runs the <paramref name="setup"/> action, and returns a <see cref="DelegateDisposable"/>, that will call <paramref name="dispose"/> when it's disposed.</para>
        /// </summary>
        /// <param name="setup">The action that is invoked immediately.</param>
        /// <param name="dispose">The action that is invoked when the returned <see cref="DelegateDisposable"/> is disposed.</param>
        /// <returns>A <see cref="DelegateDisposable"/> that will call <paramref name="dispose"/> when it's disposed.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="setup"/> or <paramref name="dispose"/> is <see langword="null"/>.</exception>
        [MustDisposeResource, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DelegateDisposable Create([InstantHandle] Action setup, Action dispose)
        {
            ANE.ThrowIfNull(setup);
            ANE.ThrowIfNull(dispose);
            setup();
            return new DelegateDisposable(dispose);
        }
        /// <summary>
        ///   <para>Runs the <paramref name="setup"/> action, and returns a <see cref="DelegateDisposable"/>, that will call <paramref name="dispose"/> with a state argument returned by <paramref name="setup"/> when it's disposed.</para>
        /// </summary>
        /// <param name="setup">The action that is invoked immediately and returns a state argument.</param>
        /// <param name="dispose">The action that is invoked with a state argument returned by <paramref name="setup"/> when the returned <see cref="DelegateDisposable"/> is disposed.</param>
        /// <returns>A <see cref="DelegateDisposable"/> that will call <paramref name="dispose"/> with a state argument returned by <paramref name="setup"/> when it's disposed.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="setup"/> or <paramref name="dispose"/> is <see langword="null"/>.</exception>
        [MustDisposeResource, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DelegateDisposable Create<TState>([InstantHandle] Func<TState> setup, Action<TState> dispose)
        {
            ANE.ThrowIfNull(setup);
            ANE.ThrowIfNull(dispose);
            TState arg = setup();
            return new DelegateDisposable(() => dispose(arg));
        }

    }
}

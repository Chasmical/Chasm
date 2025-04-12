using System;
using System.Threading;
using JetBrains.Annotations;

namespace Chasm.Utilities
{
    /// <summary>
    ///   <para>Provides a set of extension methods for the <see cref="ReaderWriterLockSlim"/> class.</para>
    /// </summary>
    public static class ReaderWriterLockSlimExtensions
    {
        /// <summary>
        ///   <para>Tries to enter the lock in read mode, and returns a <see cref="ReaderLockDisposable"/>, that exits read mode when disposed.</para>
        /// </summary>
        /// <param name="rwl">The <see cref="ReaderWriterLockSlim"/> instance.</param>
        /// <returns>A <see cref="ReaderLockDisposable"/>, that exits read mode when disposed.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="rwl"/> is <see langword="null"/>.</exception>
        /// <inheritdoc cref="ReaderWriterLockSlim.EnterReadLock" path="exception"/>
        [MustDisposeResource]
        public static ReaderLockDisposable WithReaderLock(this ReaderWriterLockSlim rwl)
        {
            ANE.ThrowIfNull(rwl);
            rwl.EnterReadLock();
            return new ReaderLockDisposable(rwl);
        }
        /// <summary>
        ///   <para>Tries to enter the lock in upgradeable mode, and returns a <see cref="UpgradeableReaderLockDisposable"/>, that exits upgradeable mode when disposed.</para>
        /// </summary>
        /// <param name="rwl">The <see cref="ReaderWriterLockSlim"/> instance.</param>
        /// <returns>A <see cref="UpgradeableReaderLockDisposable"/>, that exits upgradeable mode when disposed.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="rwl"/> is <see langword="null"/>.</exception>
        /// <inheritdoc cref="ReaderWriterLockSlim.EnterUpgradeableReadLock" path="exception"/>
        [MustDisposeResource]
        public static UpgradeableReaderLockDisposable WithUpgradeableReaderLock(this ReaderWriterLockSlim rwl)
        {
            ANE.ThrowIfNull(rwl);
            rwl.EnterUpgradeableReadLock();
            return new UpgradeableReaderLockDisposable(rwl);
        }
        /// <summary>
        ///   <para>Tries to enter the lock in write mode, and returns a <see cref="WriterLockDisposable"/>, that exits write mode when disposed.</para>
        /// </summary>
        /// <param name="rwl">The <see cref="ReaderWriterLockSlim"/> instance.</param>
        /// <returns>A <see cref="WriterLockDisposable"/>, that exits write mode when disposed.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="rwl"/> is <see langword="null"/>.</exception>
        /// <inheritdoc cref="ReaderWriterLockSlim.EnterWriteLock" path="exception"/>
        [MustDisposeResource]
        public static WriterLockDisposable WithWriterLock(this ReaderWriterLockSlim rwl)
        {
            ANE.ThrowIfNull(rwl);
            rwl.EnterWriteLock();
            return new WriterLockDisposable(rwl);
        }

        public struct ReaderLockDisposable : IDisposable
        {
            private ReaderWriterLockSlim? _rwl;
            internal ReaderLockDisposable(ReaderWriterLockSlim? rwl) => _rwl = rwl;
            /// <inheritdoc/>
            public void Dispose() { if (_rwl is not { } rwl) return; rwl.ExitReadLock(); _rwl = null; }
        }
        public struct UpgradeableReaderLockDisposable : IDisposable
        {
            private ReaderWriterLockSlim? _rwl;
            internal UpgradeableReaderLockDisposable(ReaderWriterLockSlim? rwl) => _rwl = rwl;
            /// <inheritdoc/>
            public void Dispose() { if (_rwl is not { } rwl) return; rwl.ExitUpgradeableReadLock(); _rwl = null; }
        }
        public struct WriterLockDisposable : IDisposable
        {
            private ReaderWriterLockSlim? _rwl;
            internal WriterLockDisposable(ReaderWriterLockSlim? rwl) => _rwl = rwl;
            /// <inheritdoc/>
            public void Dispose() { if (_rwl is not { } rwl) return; rwl.ExitWriteLock(); _rwl = null; }
        }

    }
}

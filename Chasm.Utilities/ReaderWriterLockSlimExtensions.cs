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
        ///   <para>Tries to enter the lock in read mode, and returns a <see cref="DelegateDisposable"/>, that exits read mode when disposed.</para>
        /// </summary>
        /// <param name="rwl">The <see cref="ReaderWriterLockSlim"/> instance.</param>
        /// <returns>A <see cref="DelegateDisposable"/>, that exits read mode when disposed.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="rwl"/> is <see langword="null"/>.</exception>
        /// <inheritdoc cref="ReaderWriterLockSlim.EnterReadLock" path="exception"/>
        [MustDisposeResource]
        public static DelegateDisposable WithReaderLock(this ReaderWriterLockSlim rwl)
        {
            if (rwl is null) throw new ArgumentNullException(nameof(rwl));
            return DelegateDisposable.Create(rwl.EnterReadLock, rwl.ExitReadLock);
        }
        /// <summary>
        ///   <para>Tries to enter the lock in upgradeable mode, and returns a <see cref="DelegateDisposable"/>, that exits upgradeable mode when disposed.</para>
        /// </summary>
        /// <param name="rwl">The <see cref="ReaderWriterLockSlim"/> instance.</param>
        /// <returns>A <see cref="DelegateDisposable"/>, that exits upgradeable mode when disposed.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="rwl"/> is <see langword="null"/>.</exception>
        /// <inheritdoc cref="ReaderWriterLockSlim.EnterUpgradeableReadLock" path="exception"/>
        [MustDisposeResource]
        public static DelegateDisposable WithUpgradeableReaderLock(this ReaderWriterLockSlim rwl)
        {
            if (rwl is null) throw new ArgumentNullException(nameof(rwl));
            return DelegateDisposable.Create(rwl.EnterUpgradeableReadLock, rwl.ExitUpgradeableReadLock);
        }
        /// <summary>
        ///   <para>Tries to enter the lock in write mode, and returns a <see cref="DelegateDisposable"/>, that exits write mode when disposed.</para>
        /// </summary>
        /// <param name="rwl">The <see cref="ReaderWriterLockSlim"/> instance.</param>
        /// <returns>A <see cref="DelegateDisposable"/>, that exits write mode when disposed.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="rwl"/> is <see langword="null"/>.</exception>
        /// <inheritdoc cref="ReaderWriterLockSlim.EnterWriteLock" path="exception"/>
        [MustDisposeResource]
        public static DelegateDisposable WithWriterLock(this ReaderWriterLockSlim rwl)
        {
            if (rwl is null) throw new ArgumentNullException(nameof(rwl));
            return DelegateDisposable.Create(rwl.EnterWriteLock, rwl.ExitWriteLock);
        }

    }
}

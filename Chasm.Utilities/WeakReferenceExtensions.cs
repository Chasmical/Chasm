using System;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace Chasm.Utilities
{
    /// <summary>
    ///   <para>Provides a set of extension methods for <see cref="WeakReference"/> and <see cref="WeakReference{T}"/> classes.</para>
    /// </summary>
    public static class WeakReferenceExtensions
    {
        /// <summary>
        ///   <para>Tries to retrieve the target object referenced by the specified <paramref name="weakReference"/>.</para>
        /// </summary>
        /// <param name="weakReference">The weak reference to try to get the target object of.</param>
        /// <param name="target">When this method returns, contains the target object, if it is available, or <see langword="null"/> if it has been garbage collected.</param>
        /// <returns><see langword="true"/>, if the target object was retrieved; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="weakReference"/> is <see langword="null"/>.</exception>
        [Pure] public static bool TryGetTarget(this WeakReference weakReference, [NotNullWhen(true)] out object? target)
        {
            ANE.ThrowIfNull(weakReference);
            return (target = weakReference.Target) is not null;
        }

        /// <summary>
        ///   <para>Returns the target object referenced by the specified <paramref name="weakReference"/>.</para>
        /// </summary>
        /// <param name="weakReference">The weak reference to get the target object of.</param>
        /// <returns>The target object referenced by the specified <paramref name="weakReference"/>, or <see langword="null"/>, if it has been garbage collected.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="weakReference"/> is <see langword="null"/>.</exception>
        [Pure] public static object? GetTargetOrDefault(this WeakReference weakReference)
        {
            ANE.ThrowIfNull(weakReference);
            return weakReference.Target;
        }
#if NETCOREAPP1_0_OR_GREATER || NETSTANDARD1_0_OR_GREATER || NET45_OR_GREATER
        /// <summary>
        ///   <para>Returns the target object referenced by the specified <paramref name="weakReference"/>.</para>
        /// </summary>
        /// <typeparam name="T">The type of the object referenced.</typeparam>
        /// <param name="weakReference">The weak reference to get the target object of.</param>
        /// <returns>The target object referenced by the specified <paramref name="weakReference"/>, or <see langword="null"/>, if it has been garbage collected.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="weakReference"/> is <see langword="null"/>.</exception>
        [Pure] public static T? GetTargetOrDefault<T>(this WeakReference<T> weakReference) where T : class
        {
            ANE.ThrowIfNull(weakReference);
            return weakReference.TryGetTarget(out T? target) ? target : null;
        }
#endif

    }
}

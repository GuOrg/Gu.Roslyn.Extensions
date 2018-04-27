namespace Gu.Roslyn.AnalyzerExtensions
{
    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using Microsoft.CodeAnalysis;

    /// <summary>
    /// A <see cref="HashSet{T}"/> for re-use.
    /// </summary>
    /// <typeparam name="T">The type of items in the use.</typeparam>
    [DebuggerTypeProxy(typeof(PooledSetDebugView<>))]
    [DebuggerDisplay("Count = {this.Count}, refCount = {this.refCount}")]
    public sealed class PooledSet<T> : IDisposable, IReadOnlyCollection<T>
    {
        private static readonly ConcurrentQueue<PooledSet<T>> Cache = new ConcurrentQueue<PooledSet<T>>();
        private readonly HashSet<T> inner = new HashSet<T>(GetComparer());

        private int refCount;

        private PooledSet()
        {
        }

        /// <inheritdoc />
        public int Count => this.inner.Count;

        /// <summary>
        /// The result from this call is meant to be used in a using.
        /// </summary>
        /// <returns>A <see cref="PooledSet{T}"/></returns>
        public static PooledSet<T> Borrow()
        {
            if (Cache.TryDequeue(out var set))
            {
                Debug.Assert(set.refCount == 0, $"{nameof(Borrow)} set.refCount == {set.refCount}");
                set.refCount = 1;
                return set;
            }

            return new PooledSet<T> { refCount = 1 };
        }

        /// <summary>
        /// The result from this call is meant to be used in a using.
        /// </summary>
        /// <param name="set">A previously borrowed set or null.</param>
        /// <returns>A newly borrowed set or the same instance with incremented ref count.</returns>
        public static PooledSet<T> BorrowOrIncrementUsage(PooledSet<T> set)
        {
            if (set == null)
            {
                return Borrow();
            }

            var current = Interlocked.Increment(ref set.refCount);
            Debug.Assert(current >= 1, $"{nameof(BorrowOrIncrementUsage)} set.refCount == {current}");
            return set;
        }

        /// <summary>
        /// <see cref="HashSet{T}.Add"/>
        /// </summary>
        /// <param name="item">The item</param>
        /// <returns>True if the item was added.</returns>
        public bool Add(T item)
        {
            this.ThrowIfDisposed();
            return this.inner.Add(item);
        }

        /// <summary>
        /// <see cref="HashSet{T}.Remove"/>
        /// </summary>
        /// <param name="item">The item</param>
        /// <returns>True if the item was Removeed.</returns>
        public bool Remove(T item)
        {
            this.ThrowIfDisposed();
            return this.inner.Remove(item);
        }

        /// <summary>
        /// <see cref="HashSet{T}.Contains"/>
        /// </summary>
        /// <param name="item">The item</param>
        /// <returns>True if the item was added.</returns>
        public bool Contains(T item)
        {
            this.ThrowIfDisposed();
            return this.inner.Contains(item);
        }

        /// <summary>
        /// <see cref="HashSet{T}.SetEquals"/>
        /// </summary>
        /// <param name="items">The item</param>
        /// <returns>True if the item was added.</returns>
        public bool SetEquals(IEnumerable<T> items) => this.inner.SetEquals(items);

        /// <summary> <see cref="HashSet{T}.UnionWith"/> </summary>
        /// <param name="other">The items to union with.</param>
        public void UnionWith(IEnumerable<T> other) => this.inner.UnionWith(other);

        /// <summary> <see cref="HashSet{T}.IntersectWith"/> </summary>
        /// <param name="other">The items to union with.</param>
        public void IntersectWith(IEnumerable<T> other) => this.inner.IntersectWith(other);

        /// <summary> <see cref="HashSet{T}.ExceptWith"/> </summary>
        /// <param name="other">The items to union with.</param>
        public void ExceptWith(IEnumerable<T> other) => this.inner.ExceptWith(other);

        /// <summary>
        /// <see cref="HashSet{T}.Clear"/>
        /// </summary>
        public void Clear() => this.inner.Clear();

        /// <inheritdoc />
        public IEnumerator<T> GetEnumerator() => this.inner.GetEnumerator();

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        /// <inheritdoc />
        void IDisposable.Dispose()
        {
            if (Interlocked.Decrement(ref this.refCount) == 0)
            {
                Debug.Assert(!Cache.Contains(this), "!Cache.Contains(this)");
                this.inner.Clear();
                Cache.Enqueue(this);
            }
        }

        private static IEqualityComparer<T> GetComparer()
        {
            if (typeof(T) == typeof(IAssemblySymbol))
            {
                return (IEqualityComparer<T>)AssemblySymbolComparer.Default;
            }

            if (typeof(T) == typeof(IEventSymbol))
            {
                return (IEqualityComparer<T>)EventSymbolComparer.Default;
            }

            if (typeof(T) == typeof(IFieldSymbol))
            {
                return (IEqualityComparer<T>)FieldSymbolComparer.Default;
            }

            if (typeof(T) == typeof(ILocalSymbol))
            {
                return (IEqualityComparer<T>)LocalSymbolComparer.Default;
            }

            if (typeof(T) == typeof(IMethodSymbol))
            {
                return (IEqualityComparer<T>)MethodSymbolComparer.Default;
            }

            if (typeof(T) == typeof(INamedTypeSymbol))
            {
                return (IEqualityComparer<T>)NamedTypeSymbolComparer.Default;
            }

            if (typeof(T) == typeof(INamespaceSymbol))
            {
                return (IEqualityComparer<T>)NamespaceSymbolComparer.Default;
            }

            if (typeof(T) == typeof(IParameterSymbol))
            {
                return (IEqualityComparer<T>)ParameterSymbolComparer.Default;
            }

            if (typeof(T) == typeof(IPropertySymbol))
            {
                return (IEqualityComparer<T>)PropertySymbolComparer.Default;
            }

            if (typeof(T) == typeof(ISymbol))
            {
                return (IEqualityComparer<T>)SymbolComparer.Default;
            }

            if (typeof(T) == typeof(ITypeSymbol))
            {
                return (IEqualityComparer<T>)TypeSymbolComparer.Default;
            }

            return EqualityComparer<T>.Default;
        }

        [Conditional("DEBUG")]
        private void ThrowIfDisposed()
        {
            if (this.refCount <= 0)
            {
                Debug.Assert(this.refCount == 0, $"{nameof(this.ThrowIfDisposed)} set.refCount == {this.refCount}");
                throw new ObjectDisposedException(this.GetType().FullName);
            }
        }
    }
}

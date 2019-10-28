namespace Gu.Roslyn.AnalyzerExtensions
{
    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;

    /// <summary>
    /// A <see cref="HashSet{T}"/> for re-use.
    /// </summary>
    /// <typeparam name="T">The type of items in the set.</typeparam>
    [DebuggerTypeProxy(typeof(PooledSetDebugView<>))]
    [DebuggerDisplay("Count = {this.Count}, refCount = {this.refCount}")]
#pragma warning disable CA1710 // Identifiers should have correct suffix
    public sealed class PooledSet<T> : IDisposable, IReadOnlyCollection<T>
#pragma warning restore CA1710 // Identifiers should have correct suffix
    {
        private static readonly ConcurrentQueue<PooledSet<T>> Cache = new ConcurrentQueue<PooledSet<T>>();
        private readonly HashSet<T> inner = new HashSet<T>(GetComparer());

        private int refCount;

        private PooledSet()
        {
        }

        /// <inheritdoc />
        public int Count => this.inner.Count;

        /// <summary>Gets the <see cref="System.Collections.Generic.IEqualityComparer{T}" /> object that is used to determine equality for the values in the set.</summary>
        /// <returns>The <see cref="System.Collections.Generic.IEqualityComparer{T}" /> object that is used to determine equality for the values in the set.</returns>
        public IEqualityComparer<T> Comparer => this.inner.Comparer;

#pragma warning disable CA1000 // Do not declare static members on generic types
        /// <summary>
        /// The result from this call is meant to be used in a using.
        /// </summary>
        /// <returns>A <see cref="PooledSet{T}"/>.</returns>
        public static PooledSet<T> Borrow()
#pragma warning restore CA1000 // Do not declare static members on generic types
        {
            if (Cache.TryDequeue(out var set))
            {
                Debug.Assert(set.refCount == 0, $"{nameof(Borrow)} set.refCount == {set.refCount}");
                set.refCount = 1;
                return set;
            }

            return new PooledSet<T> { refCount = 1 };
        }

#pragma warning disable IDISP015 // Member should not return created and cached instance.
#pragma warning disable CA1000 // Do not declare static members on generic types
        /// <summary>
        /// The result from this call is meant to be used in a using.
        /// </summary>
        /// <param name="set">A previously borrowed set or null.</param>
        /// <returns>A newly borrowed set or the same instance with incremented ref count.</returns>
        public static PooledSet<T> BorrowOrIncrementUsage(PooledSet<T> set)
#pragma warning restore CA1000 // Do not declare static members on generic types
#pragma warning restore IDISP015 // Member should not return created and cached instance.
        {
            if (set == null)
            {
                return Borrow();
            }

            // ReSharper disable once RedundantAssignment
            var current = Interlocked.Increment(ref set.refCount);
            Debug.Assert(current >= 1, $"{nameof(BorrowOrIncrementUsage)} set.refCount == {current}");
            return set;
        }

        /// <summary>
        /// <see cref="HashSet{T}.Add"/>.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>True if the item was added.</returns>
        public bool Add(T item)
        {
            this.ThrowIfDisposed();
            return this.inner.Add(item);
        }

        /// <summary>
        /// <see cref="HashSet{T}.Remove"/>.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>True if the item was removed.</returns>
        public bool Remove(T item)
        {
            this.ThrowIfDisposed();
            return this.inner.Remove(item);
        }

        /// <summary>
        /// <see cref="HashSet{T}.Contains"/>.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>True if the item was added.</returns>
        public bool Contains(T item)
        {
            this.ThrowIfDisposed();
            return this.inner.Contains(item);
        }

        /// <summary>
        /// <see cref="HashSet{T}.SetEquals"/>.
        /// </summary>
        /// <param name="items">The item.</param>
        /// <returns>True if the item was added.</returns>
        public bool SetEquals(IEnumerable<T> items) => this.inner.SetEquals(items);

        /// <summary> <see cref="HashSet{T}.UnionWith"/>. </summary>
        /// <param name="other">The items to union with.</param>
        public void UnionWith(IEnumerable<T> other) => this.inner.UnionWith(other);

        /// <summary> <see cref="HashSet{T}.IntersectWith"/>. </summary>
        /// <param name="other">The items to union with.</param>
        public void IntersectWith(IEnumerable<T> other) => this.inner.IntersectWith(other);

        /// <summary> <see cref="HashSet{T}.ExceptWith"/>. </summary>
        /// <param name="other">The items to union with.</param>
        public void ExceptWith(IEnumerable<T> other) => this.inner.ExceptWith(other);

        /// <summary>
        /// <see cref="HashSet{T}.Clear"/>.
        /// </summary>
        public void Clear() => this.inner.Clear();

        /// <inheritdoc />
        public IEnumerator<T> GetEnumerator() => this.inner.GetEnumerator();

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        /// <inheritdoc />
        public void Dispose()
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
            return PooledSet.SymbolComparers.OfType<IEqualityComparer<T>>().FirstOrDefault() ??
                   EqualityComparer<T>.Default;
        }

        [Conditional("DEBUG")]
        private void ThrowIfDisposed()
        {
            if (this.refCount <= 0)
            {
                Debug.Assert(this.refCount == 0, $"{nameof(this.ThrowIfDisposed)} set.refCount == {this.refCount}");
                throw new ObjectDisposedException(typeof(PooledSet<T>).FullName);
            }
        }
    }
}

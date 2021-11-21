#pragma warning disable CA1000 // Do not declare static members on generic types
namespace Gu.Roslyn.AnalyzerExtensions
{
    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;

    /// <summary>
    /// A <see cref="List{T}"/> for re-use.
    /// </summary>
    /// <typeparam name="T">The type of items.</typeparam>
    public sealed class PooledList<T> : IList<T>, IDisposable
    {
        private static readonly ConcurrentQueue<PooledList<T>> Cache = new();

        private readonly List<T> inner = new List<T>();

        private PooledList()
        {
        }

        /// <inheritdoc />
        public int Count => this.inner.Count;

        /// <inheritdoc />
        public bool IsReadOnly => ((IList<T>)this.inner).IsReadOnly;

        /// <inheritdoc />
        public T this[int index]
        {
            get => this.inner[index];
            set => this.inner[index] = value;
        }

        /// <summary>
        /// Borrow a dictionary, dispose returns it.
        /// </summary>
        /// <returns>A <see cref="PooledList{T}"/>.</returns>
        public static PooledList<T> Borrow()
        {
            if (Cache.TryDequeue(out var dictionary))
            {
                return dictionary;
            }

            return new PooledList<T>();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            this.inner.Clear();
            Cache.Enqueue(this);
        }

        /// <inheritdoc />
        public IEnumerator<T> GetEnumerator() => this.inner.GetEnumerator();

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        /// <inheritdoc />
        public void Add(T item) => this.inner.Add(item);

        /// <inheritdoc />
        public void Clear() => this.inner.Clear();

        /// <inheritdoc />
        public bool Contains(T item) => this.inner.Contains(item);

        /// <inheritdoc />
        public void CopyTo(T[] array, int arrayIndex) => this.inner.CopyTo(array, arrayIndex);

        /// <inheritdoc />
        public bool Remove(T item) => this.inner.Remove(item);

        /// <inheritdoc />
        public int IndexOf(T item) => this.inner.IndexOf(item);

        /// <inheritdoc />
        public void Insert(int index, T item) => this.inner.Insert(index, item);

        /// <inheritdoc />
        public void RemoveAt(int index) => this.inner.RemoveAt(index);
    }
}

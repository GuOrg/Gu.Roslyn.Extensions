namespace Gu.Roslyn.AnalyzerExtensions
{
    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    /// <summary>
    /// A <see cref="Dictionary{TKey,TValue}"/> for re-use.
    /// </summary>
    /// <typeparam name="TKey">The type of keys.</typeparam>
    /// <typeparam name="TValue">The type of values.</typeparam>
    public sealed class PooledDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IDisposable
        where TKey : notnull
    {
        private static readonly ConcurrentQueue<PooledDictionary<TKey, TValue>> Cache = new ConcurrentQueue<PooledDictionary<TKey, TValue>>();

        private readonly Dictionary<TKey, TValue> inner = new Dictionary<TKey, TValue>(GetComparer());

        private PooledDictionary()
        {
        }

        /// <inheritdoc />
        public int Count => this.inner.Count;

        /// <inheritdoc />
        public bool IsReadOnly => ((IDictionary<TKey, TValue>)this.inner).IsReadOnly;

        /// <inheritdoc />
        public ICollection<TKey> Keys => this.inner.Keys;

        /// <inheritdoc />
        public ICollection<TValue> Values => this.inner.Values;

        /// <inheritdoc />
        public TValue this[TKey key]
        {
            get => this.inner[key];
            set => this.inner[key] = value;
        }

        /// <summary>
        /// Borrow a dictionary, dispose returns it.
        /// </summary>
        /// <returns>A <see cref="PooledDictionary{TKey,TValue}"/>.</returns>
        public static PooledDictionary<TKey, TValue> Borrow()
        {
            if (Cache.TryDequeue(out var dictionary))
            {
                return dictionary;
            }

            return new PooledDictionary<TKey, TValue>();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            this.inner.Clear();
            Cache.Enqueue(this);
        }

        /// <inheritdoc />
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => this.inner.GetEnumerator();

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        /// <inheritdoc />
        public void Add(KeyValuePair<TKey, TValue> item) => ((IDictionary<TKey, TValue>)this.inner).Add(item);

        /// <inheritdoc />
        public void Clear() => this.inner.Clear();

        /// <inheritdoc />
        public bool Contains(KeyValuePair<TKey, TValue> item) => this.inner.Contains(item);

        /// <inheritdoc />
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) => ((IDictionary<TKey, TValue>)this.inner).CopyTo(array, arrayIndex);

        /// <inheritdoc />
        public bool Remove(KeyValuePair<TKey, TValue> item) => ((IDictionary<TKey, TValue>)this.inner).Remove(item);

        /// <inheritdoc />
        public void Add(TKey key, TValue value) => this.inner.Add(key, value);

        /// <inheritdoc />
        public bool ContainsKey(TKey key) => this.inner.ContainsKey(key);

        /// <inheritdoc />
        public bool Remove(TKey key) => this.inner.Remove(key);

        /// <inheritdoc />
        public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value) => this.inner.TryGetValue(key, out value);

        private static IEqualityComparer<TKey> GetComparer()
        {
            return PooledSet.SymbolComparers.OfType<IEqualityComparer<TKey>>().FirstOrDefault() ??
                   EqualityComparer<TKey>.Default;
        }
    }
}

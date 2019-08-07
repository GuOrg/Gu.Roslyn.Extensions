#pragma warning disable CA1000 // Do not declare static members on generic types
namespace Gu.Roslyn.AnalyzerExtensions
{
    using System;
    using System.Collections.Concurrent;

    /// <summary> A cache. </summary>
    /// <typeparam name="TKey">The key type.</typeparam>
    /// <typeparam name="TValue">The value type.</typeparam>
#pragma warning disable CA1724
    public static class Cache<TKey, TValue>
#pragma warning restore CA1724
    {
        private static readonly ConcurrentDictionary<TKey, TValue> Inner = new ConcurrentDictionary<TKey, TValue>();

        /// <summary>
        /// Start a cache transaction.
        /// </summary>
        public static Transaction Begin()
        {
            Inner.Clear();
            return new Transaction();
        }

        /// <summary>
        /// End a cache transaction, the cache is purged when ref count is zero.
        /// </summary>
        [Obsolete("Dispose the transaction returned by Begin()")]
        public static void End()
        {
            Inner.Clear();
        }

        /// <summary>
        /// Get an item from cache or create and add and return.
        /// </summary>
        /// <param name="key">The cache key.</param>
        /// <param name="valueFactory">The factory for new items.</param>
        /// <returns>The cached value.</returns>
        public static TValue GetOrAdd(TKey key, Func<TKey, TValue> valueFactory)
        {
            if (valueFactory == null)
            {
                throw new ArgumentNullException(nameof(valueFactory));
            }

            return Inner.GetOrAdd(key, valueFactory);
        }

        /// <summary>
        /// A transaction that decrements ref count when disposed.
        /// </summary>
        // ReSharper disable once InconsistentNaming
#pragma warning disable CA1034, CA1707, CA1815
        public sealed class Transaction : IDisposable
#pragma warning restore CA1034, CA1707, CA1815
        {
            ~Transaction()
            {
#pragma warning disable IDISP023 // Don't use reference types in finalizer context.
                Inner.Clear();
#pragma warning restore IDISP023 // Don't use reference types in finalizer context.
            }

            /// <inheritdoc />
            public void Dispose()
            {
                Inner.Clear();
                GC.SuppressFinalize(this);
            }
        }
    }
}

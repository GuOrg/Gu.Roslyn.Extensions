namespace Gu.Roslyn.AnalyzerExtensions
{
    using System;
    using System.Collections.Concurrent;
    using System.Diagnostics;
    using System.Threading;

    /// <summary> A cache </summary>
    /// <typeparam name="TKey">The key type.</typeparam>
    /// <typeparam name="TValue">The value type.</typeparam>
    public static class Cache<TKey, TValue>
    {
        private static readonly ConcurrentDictionary<TKey, TValue> Inner = new ConcurrentDictionary<TKey, TValue>();
        //// ReSharper disable once StaticMemberInGenericType
        private static int refCount;

        /// <summary>
        /// Start a cache transaction.
        /// </summary>
        public static void Begin()
        {
#pragma warning disable GU0011 // Don't ignore the return value.
            Interlocked.Increment(ref refCount);
#pragma warning restore GU0011 // Don't ignore the return value.
        }

        /// <summary>
        /// End a cache transaction, the cache is purged when ref count is zero.
        /// </summary>
        public static void End()
        {
#pragma warning disable GU0011 // Don't ignore the return value.
            Interlocked.Exchange(ref refCount, 0);
#pragma warning restore GU0011 // Don't ignore the return value.
            Inner.Clear();
        }

        /// <summary>
        /// Start a cache transaction and end it when disposing.
        /// </summary>
        /// <returns>A <see cref="Transaction_"/></returns>
        public static Transaction_ Transaction()
        {
#pragma warning disable GU0011 // Don't ignore the return value.
            Interlocked.Increment(ref refCount);
#pragma warning restore GU0011 // Don't ignore the return value.
            Debug.Assert(refCount > 0, "refCount > 0");
            return default(Transaction_);
        }

        /// <summary>
        /// Get an item from cache or create and add and return.
        /// </summary>
        /// <param name="key">The cache key.</param>
        /// <param name="valueFactory">The factory for new items.</param>
        /// <returns>The cached value.</returns>
        public static TValue GetOrAdd(TKey key, Func<TKey, TValue> valueFactory)
        {
            return refCount == 0 ? valueFactory(key) : Inner.GetOrAdd(key, valueFactory);
        }

        /// <summary>
        /// A transaction that decrements ref count when disposed.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public struct Transaction_ : IDisposable
        {
            /// <inheritdoc />
            public void Dispose()
            {
                if (Interlocked.Decrement(ref refCount) <= 0)
                {
                    Inner.Clear();
                }
            }
        }
    }
}

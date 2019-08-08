#pragma warning disable CA1000 // Do not declare static members on generic types
namespace Gu.Roslyn.AnalyzerExtensions
{
    using System;
    using System.Collections.Concurrent;
    using System.Runtime.InteropServices.ComTypes;
    using System.Threading;
    using Microsoft.CodeAnalysis;

    /// <summary> A cache. </summary>
    /// <typeparam name="TValue">The value type.</typeparam>
#pragma warning disable CA1724
    public static class Cache<TValue>
#pragma warning restore CA1724
    {
        private static readonly ConcurrentDictionary<SyntaxTree, TValue> Inner = new ConcurrentDictionary<SyntaxTree, TValue>();

        /// <summary>
        /// Start a cache transaction.
        /// </summary>
        /// <param name="compilation">The <see cref="Compilation"/>.</param>
        /// <returns>A <see cref="Transaction"/> that clears the cache when disposed.</returns>
        public static IDisposable Begin(Compilation compilation)
        {
            return new Transaction(compilation);
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
        public static TValue GetOrAdd(SyntaxTree key, Func<SyntaxTree, TValue> valueFactory)
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
        private sealed class Transaction : IDisposable
        {
            private static int refCount;
            private readonly object gate = new object();
            private Compilation compilation;

            /// <summary>
            /// Initializes a new instance of the <see cref="Transaction"/> class.
            /// </summary>
            /// <param name="compilation">The <see cref="Compilation"/>.</param>
            internal Transaction(Compilation compilation)
            {
                this.compilation = compilation;
                _ = Interlocked.Increment(ref refCount);
            }

            /// <summary>
            /// Finalizes an instance of the <see cref="Transaction"/> class.
            /// </summary>
            ~Transaction()
            {
#pragma warning disable IDISP023 // Don't use reference types in finalizer context.
                this.Purge();
#pragma warning restore IDISP023 // Don't use reference types in finalizer context.
            }

            /// <inheritdoc />
            public void Dispose()
            {
                this.Purge();
                GC.SuppressFinalize(this);
            }

            private void Purge()
            {
                if (this.compilation == null)
                {
                    return;
                }

                lock (this.gate)
                {
                    if (this.compilation == null)
                    {
                        return;
                    }

                    if (Interlocked.Decrement(ref refCount) > 0)
                    {
                        foreach (var tree in this.compilation.SyntaxTrees)
                        {
                            Inner.TryRemove(tree, out _);
                        }
                    }
                    else
                    {
                        Inner.Clear();
                    }

                    this.compilation = null;
                }
            }
        }
    }
}

﻿#pragma warning disable CA1000 // Do not declare static members on generic types
namespace Gu.Roslyn.AnalyzerExtensions;

using System;
using System.Collections.Concurrent;
using System.Threading;
using Microsoft.CodeAnalysis;

/// <summary> A cache. </summary>
/// <typeparam name="TValue">The value type.</typeparam>
public static class SyntaxTreeCache<TValue>
{
    private static readonly ConcurrentDictionary<SyntaxTree, TValue> Inner = new();

    /// <summary>
    /// Start a cache transaction.
    /// </summary>
    /// <param name="compilation">The <see cref="Compilation"/>.</param>
    /// <returns>A <see cref="Transaction"/> that clears the cache when disposed.</returns>
    public static IDisposable Begin(Compilation? compilation)
    {
        return new Transaction(compilation);
    }

    /// <summary>
    /// End a cache transaction, the cache is purged when ref count is zero.
    /// </summary>
    [Obsolete("Dispose the transaction returned by Begin() instead.")]
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
        if (key is null)
        {
            throw new ArgumentNullException(nameof(key));
        }

        if (valueFactory is null)
        {
            throw new ArgumentNullException(nameof(valueFactory));
        }

        if (Transaction.RefCount == 0)
        {
            return valueFactory(key);
        }

        return Inner.GetOrAdd(key, valueFactory);
    }

    /// <summary>
    /// A transaction that decrements ref count when disposed.
    /// </summary>
    private sealed class Transaction : IDisposable
    {
#pragma warning disable SA1401 // Fields should be private
        internal static int RefCount;
#pragma warning restore SA1401 // Fields should be private
        private readonly object gate = new();
        private Compilation? compilation;

        /// <summary>
        /// Initializes a new instance of the <see cref="Transaction"/> class.
        /// </summary>
        /// <param name="compilation">The <see cref="Compilation"/>.</param>
        internal Transaction(Compilation? compilation)
        {
            this.compilation = compilation;
            _ = Interlocked.Increment(ref RefCount);
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
            if (this.compilation is null)
            {
                if (RefCount == 0)
                {
                    Inner.Clear();
                }

                return;
            }

            lock (this.gate)
            {
                if (this.compilation is null)
                {
                    return;
                }

                if (Interlocked.Decrement(ref RefCount) > 0)
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

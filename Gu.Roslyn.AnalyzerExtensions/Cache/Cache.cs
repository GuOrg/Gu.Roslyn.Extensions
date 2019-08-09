namespace Gu.Roslyn.AnalyzerExtensions
{
    using System;
    using Microsoft.CodeAnalysis;

    /// <summary>
    /// Helper for using the cache.
    /// </summary>
#pragma warning disable CA1724 // Type names should not match namespaces
    public static class Cache
#pragma warning restore CA1724 // Type names should not match namespaces
    {
        /// <summary>
        /// Get an item from cache or create and add and return.
        /// </summary>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="key">The cache key.</param>
        /// <param name="valueFactory">The factory for new items.</param>
        /// <returns>The cached value.</returns>
        public static TValue GetOrAdd<TValue>(SyntaxTree key, Func<SyntaxTree, TValue> valueFactory) => SyntaxTreeCache<TValue>.GetOrAdd(key, valueFactory);
    }
}

namespace Gu.Roslyn.AnalyzerExtensions
{
    using System;
    using Microsoft.CodeAnalysis.Diagnostics;

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
        /// <typeparam name="TKey">The key type.</typeparam>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="key">The cache key.</param>
        /// <param name="valueFactory">The factory for new items.</param>
        /// <returns>The cached value.</returns>
        public static TValue GetOrAdd<TKey, TValue>(TKey key, Func<TKey, TValue> valueFactory) => Cache<TKey, TValue>.GetOrAdd(key, valueFactory);

        /// <summary>
        /// Controls if Semantic models should be cached for syntax trees.
        /// This can speed up analysis significantly but means Visual Studio uses more memory during compilation.
        /// </summary>
        /// <typeparam name="TKey">The key type.</typeparam>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="context">The <see cref="AnalysisContext"/>.</param>
        public static void CacheToCompilationEnd<TKey, TValue>(this AnalysisContext context)
        {
            context.RegisterCompilationStartAction(x => Cache<TKey, TValue>.Begin());
        }
    }
}
